using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;
using IARA.Mobile.Pub.Application.Helpers;
using IARA.Mobile.Pub.Domain.Enums;
using IARA.Mobile.Pub.ViewModels.Controls.PersonView;
using IARA.Mobile.Shared;
using IARA.Mobile.Shared.Helpers;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages.FishingTicket
{
    public class Under14SubTicketViewModel : BaseTicketViewModel
    {
        private TLFileResult birthCertificateFile;

        public Under14SubTicketViewModel()
        {
            UploadBirthCertificateCommand = CommandBuilder.CreateFrom(UploadBirthCertificate);
            OnSaveCommand = CommandBuilder.CreateFrom(OnSave);
            Person = new PersonViewModel()
            {
                IsEditEnabled = true,
                DateOfBirthRequired = true,
            };
            Person.IsUnder14.Value = true;
            this.AddValidation(new Dictionary<string, Func<bool>>()
            {
                { Group.ACCEPT_AGREEMENT, () =>  false},
            }, others: new IValidatableViewModel[] { Person });
        }

        public TLFileResult BirthCertificateFile
        {
            get => birthCertificateFile;
            set => SetProperty(ref birthCertificateFile, value);
        }

        public ICommand UploadBirthCertificateCommand { get; }

        private async Task UploadBirthCertificate()
        {
            TLFileResult file = await TLMediaPicker.PickPhotoAsync();
            if (file == null)
            {
                return;
            }

            BirthCertificateFile = file.ImageResize(ImageResizeConstants.MAX_WIDTH, ImageResizeConstants.MAX_HEIGTH, ImageResizeConstants.COMPRESSION_RATE);
        }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] {
                GroupResourceEnum.Under14Ticket,
                GroupResourceEnum.FishingTicket,
                GroupResourceEnum.TicketPerson,
                GroupResourceEnum.Validation,
                GroupResourceEnum.Common
            };
        }

        public override Task Initialize(object sender)
        {
            return Task.CompletedTask;
        }

        protected override async Task<bool> CustomValidation()
        {
            if (Photo == null)
            {
                await DisplayAttentionAlert(TranslateExtension.Translator[nameof(GroupResourceEnum.TicketPerson) + "/ShouldUploadPhoto"]);
                return true;
            }

            if (YearsCalculator.YearsDifferenceFromNow(Person.DateOfBirth.Value.Value) >= 14)
            {
                await DisplayAttentionAlert(TranslateExtension.Translator[nameof(GroupResourceEnum.Under14Ticket) + "/Under14Only"]);
                return true;
            }

            if (BirthCertificateFile == null)
            {
                await DisplayAttentionAlert(TranslateExtension.Translator[nameof(GroupResourceEnum.TicketPerson) + "/BirthCertificateIsRequired"]);
                return true;
            }

            if (await FishingTicketsTransaction.IsDuplicateTicket(GetTicketValidationData()))
            {
                await DisplayAttentionAlert(TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/DuplicateTicket"] + "(" + TicketMetadata.Name + ").");
                return true;
            }

            return false;
        }
        protected override FishingTicketRequestDto GetFishingTicketData()
        {
            int fileTypeId = NomenclaturesTransaction.GetActiveFileTypeIdByCode(nameof(FileTypeEnum.BIRTHCERTIFICATE));
            return new FishingTicketRequestDto()
            {
                Id = TicketMetadata.Id,
                TypeId = TicketMetadata.TypeId,
                PeriodId = TicketMetadata.PeriodId,
                Person = Person.MapToApiPerson(),
                PersonAddressRegistrations = Person.MapToApiAddresses(),
                ValidFrom = DateTime.Now,
                HasUserConfirmed = true,
                DeliveryTerritoryUnitId = TerritorialUnit.Value.Id,
                PersonPhoto = photoFullPath != null ? new FileModel()
                {
                    FullPath = photoFullPath,
                    ContentType = PhotoContentType,
                    UploadedOn = DateTime.Now
                } : null,
                Files = new List<FileModel>()
                {
                    new FileModel()
                    {
                        FullPath = BirthCertificateFile.FullPath,
                        ContentType = BirthCertificateFile.ContentType,
                        UploadedOn = DateTime.Now,
                        FileTypeId = fileTypeId
                    }
                }
            };
        }

        protected override TicketValidationDTO GetTicketValidationData()
        {
            return new TicketValidationDTO()
            {
                EgnLnch = Person.EgnLnc.Value,
                PeriodCode = nameof(TicketPeriodEnum.UNTIL14),
                TypeCode = nameof(TicketTypeEnum.UNDER14),
                ChildDateOfBirth = Person.DateOfBirth.Value
            };
        }
    }
}

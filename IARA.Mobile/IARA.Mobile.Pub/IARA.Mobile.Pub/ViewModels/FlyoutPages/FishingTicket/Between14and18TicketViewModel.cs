using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;
using IARA.Mobile.Pub.Application.Helpers;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Domain.Enums;
using IARA.Mobile.Pub.ViewModels.Controls.PersonView;
using IARA.Mobile.Shared.Menu;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages.FishingTicket
{
    public class Between14and18TicketViewModel : BaseTicketViewModel
    {
        private DateTime _ticketValidFrom;
        private string _ticketValidTo;

        public Between14and18TicketViewModel(IFishingTicketsSettings fishingTicketsSettings)
           : base(fishingTicketsSettings)
        {
            OnSaveCommand = CommandBuilder.CreateFrom(OnSave);
            ValidFromChanged = CommandBuilder.CreateFrom<DateTime>(OnValidFromChanged);
            TicketValidFrom = DateTime.Now;

        }

        public DateTime TicketValidFrom
        {
            get => _ticketValidFrom;
            set => SetProperty(ref _ticketValidFrom, value);
        }

        public string TicketValidTo
        {
            get => _ticketValidTo;
            set => SetProperty(ref _ticketValidTo, value);
        }

        public DateTime Today => DateTime.Today;

        public ICommand ValidFromChanged { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] {
                GroupResourceEnum.FishingTicket,
                GroupResourceEnum.TicketPerson,
                GroupResourceEnum.Validation,
                GroupResourceEnum.Common,
                GroupResourceEnum.Under14Ticket
            };
        }

        public override async Task Initialize(object sender)
        {
            await base.Initialize(sender);
            await TLLoadingHelper.ShowFullLoadingScreen();
            Person = new PersonViewModel()
            {
                DateOfBirthRequired = true,
            };
            this.AddValidation(new Dictionary<string, Func<bool>>()
            {
                { Group.ACCEPT_AGREEMENT, () =>  true},
            }, others: new[] { Person });
            if (TicketMetadata.Action == TicketAction.Create)
            {
                await OnValidFromChanged(TicketValidFrom);
                await LoadInitialDataFromProfile();

            }
            else if (TicketMetadata.Action == TicketAction.Renew || TicketMetadata.Action == TicketAction.Update || TicketMetadata.Action == TicketAction.View)
            {
                FishingTicketResponseDto ticket = await LoadTicketBaseData(TicketMetadata.Id);

                if (TicketMetadata.Action == TicketAction.Update || TicketMetadata.Action == TicketAction.View)
                {
                    AllowAdditionalUnder14Tickets = false;

                    if (ticket == null)
                    {
                        await MainNavigator.Current.PopPageAsync();
                    }
                    else
                    {
                        TicketValidFrom = ticket.ValidFrom;
                        await OnValidFromChanged(TicketValidFrom);
                    }
                }
            }
            await TLLoadingHelper.HideFullLoadingScreen();
        }

        protected override FishingTicketRequestDto GetFishingTicketData()
        {
            return new FishingTicketRequestDto()
            {
                Id = TicketMetadata.Id,
                TypeId = TicketMetadata.TypeId,
                PeriodId = TicketMetadata.PeriodId,
                Price = TicketMetadata.Price,
                ValidFrom = TicketValidFrom.Date == DateTime.Now.Date ? DateTime.Now : TicketValidFrom,
                Person = Person.MapToApiPerson(),
                PersonAddressRegistrations = Person.MapToApiAddresses(),
                HasUserConfirmed = AcceptAgreement,
                PersonPhoto = photoFullPath != null ? new FileModel()
                {
                    FullPath = photoFullPath,
                    ContentType = PhotoContentType,
                    UploadedOn = DateTime.Now
                } : null,
            };
        }
        protected override TicketValidationDTO GetTicketValidationData()
        {
            return new TicketValidationDTO()
            {
                EgnLnch = Person.EgnLnc.Value,
                PeriodCode = TicketMetadata.PeriodCode,
                TypeCode = nameof(TicketTypeEnum.BETWEEN14AND18),
                ValidFrom = TicketValidFrom.Date == DateTime.Now.Date ? DateTime.Now : TicketValidFrom,
            };
        }

        protected override async Task<bool> CustomValidation()
        {
            int personYears = YearsCalculator.YearsDifferenceFromNow(Person.DateOfBirth.Value.Value);
            if (personYears < 14 || personYears >= 18)
            {
                await DisplayAttentionAlert(TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/Between14and18Only"]);
                return true;
            }
            return false;
        }

        private async Task OnValidFromChanged(DateTime validFrom)
        {
            TicketValidToCalculationParamsDto parameters = new TicketValidToCalculationParamsDto
            {
                TypeId = TicketMetadata.TypeId,
                PeriodId = TicketMetadata.PeriodId,
                ValidFrom = validFrom,
            };

            DateTime? result = await FishingTicketsTransaction.CalculateTicketValidToDate(parameters);

            TicketValidTo = result.HasValue ? result.GetValueOrDefault().ToString(ValidToDateTimeFormat) : null;
        }
    }
}


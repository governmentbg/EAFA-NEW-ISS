using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.DTObjects.CatchRecords;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;
using IARA.Mobile.Pub.Helpers;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Pub.ViewModels.Controls;
using IARA.Mobile.Pub.ViewModels.Models;
using IARA.Mobile.Shared.Helpers;
using IARA.Mobile.Shared.Menu;
using IARA.Mobile.Shared.Popups;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages.CatchRecords
{
    public class AddCatchRecordViewModel : PageViewModel
    {
        private readonly List<CatchImageModel> _pictures;
        private Position? _location;
        private bool _ticketChosen;
        private List<NomenclatureDto> _fishTypes;
        private List<MenuOption> _addPictureChoices;
        private readonly List<CatchFileLocallyAdded> _locallyUploadedFiles;
        public AddCatchRecordViewModel()
        {
            _pictures = new List<CatchImageModel>();

            Catches = new TLObservableCollection<CFCatchViewModel>();
            Pictures = new TLObservableCollection<CatchImageModel>();
            UserTickets = new TLObservableCollection<UserTicketShortDto>();

            AddCatch = CommandBuilder.CreateFrom(OnAddCatch);
            RemoveCatch = CommandBuilder.CreateFrom<CFCatchViewModel>(OnRemoveCatch);
            AddPicture = CommandBuilder.CreateFrom<MenuResult>(OnAddPicture);
            OpenPicture = CommandBuilder.CreateFrom<CatchImageModel>(OnOpenPicture);
            RemovePicture = CommandBuilder.CreateFrom<CatchImageModel>(OnRemovePicture);
            TicketSelected = CommandBuilder.CreateFrom<UserTicketShortDto>(OnTicketSelected);
            Save = CommandBuilder.CreateFrom(OnSave);
            _locallyUploadedFiles = new List<CatchFileLocallyAdded>();
            this.AddValidation();

            CatchDate.Value = DateTime.Now;
        }

        public bool CanEdit { get; set; }
        public bool IsAdd { get; set; }
        public CatchRecordDto CatchRecord { get; set; }

        [Required]
        [StringLength(500)]
        public ValidState WaterArea { get; set; }

        [Required]
        public ValidStateDateTime CatchDate { get; set; }

        [StringLength(4000)]
        public ValidState Description { get; set; }

        [Required]
        public ValidStateSelect<UserTicketShortDto> Ticket { get; set; }

        public Position? Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public bool TicketChosen
        {
            get => _ticketChosen;
            set => SetProperty(ref _ticketChosen, value);
        }

        public List<MenuOption> AddPictureChoices
        {
            get => _addPictureChoices;
            set => SetProperty(ref _addPictureChoices, value);
        }

        public TLObservableCollection<UserTicketShortDto> UserTickets { get; }
        public TLObservableCollection<CFCatchViewModel> Catches { get; }
        public TLObservableCollection<CatchImageModel> Pictures { get; }

        public ICommand AddCatch { get; }
        public ICommand RemoveCatch { get; }
        public ICommand AddPicture { get; }
        public ICommand OpenPicture { get; }
        public ICommand RemovePicture { get; }
        public ICommand TicketSelected { get; }
        public ICommand Save { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] { GroupResourceEnum.AddCatchRecord, GroupResourceEnum.Validation };
        }

        public override Task Initialize(object sender)
        {
            AddPictureChoices = new List<MenuOption>
            {
                new MenuOption
                {
                    Icon = IconFont.FileImage,
                    Text = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/PickPhoto"],
                    Option = "Pick"
                },
                new MenuOption
                {
                    Icon = IconFont.Camera,
                    Text = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/CapturePhoto"],
                    Option = "Take"
                },
            };

            _fishTypes = NomenclaturesTransaction.GetFishTypes();

            if (CatchRecord != null)
            {
                CatchRecordInfoDto info = CatchRecordsTransaction.GetCatchRecord(CatchRecord.Id);

                if (info == null)
                {
                    return Task.CompletedTask;
                }

                WaterArea.Value = info.WaterArea;
                CatchDate.Value = info.CatchDate;
                Description.Value = info.Description;
                Location = info.Location == null
                    ? null
                    : new Position?(new Position(DMSType.Parse(info.Location.DMSLatitude).ToDecimal(), DMSType.Parse(info.Location.DMSLongitude).ToDecimal()));
                Ticket.Value = info.Ticket;
                TicketChosen = true;

                if (info.Fishes?.Count > 0)
                {
                    foreach (CatchRecordFishDto fish in info.Fishes)
                    {
                        CFCatchViewModel catchViewModel = new CFCatchViewModel(_fishTypes);

                        catchViewModel.Count.Value = fish.Count.ToString();
                        catchViewModel.Quantity.Value = fish.Quantity.ToString();
                        catchViewModel.FishType.Value = fish.FishType;

                        Catches.Add(catchViewModel);
                        Validation.OtherValidations.Add(catchViewModel.Validation);
                    }
                }
                else
                {
                    OnAddCatch();
                }

                if (info.Files?.Count > 0)
                {
                    foreach (CatchRecordFileDto file in info.Files)
                    {
                        CatchImageModel model = new CatchImageModel
                        {
                            Id = file.Id,
                            IsFileSavedLocally = file.IsLocal,
                            ContentType = file.ContentType,
                            Name = file.Name,
                            Size = file.Size,
                            FullPath = file.FullPath,
                            UploadedOn = file.UploadedOn,
                        };

                        if (string.IsNullOrEmpty(file.FullPath) && file.Id.HasValue)
                        {
                            model.Image = ImageSource.FromStream(
                                async (_) =>
                                {
                                    FileResponse fileResult = await CatchRecordsTransaction.GetGalleryPhoto(file.Id.Value);

                                    return fileResult != null
                                        ? new MemoryStream(fileResult.File)
                                        : null;
                                }
                            );
                        }
                        else
                        {
                            model.Image = file.FullPath;
                        }

                        _pictures.Add(model);

                        if (!file.Deleted)
                        {
                            Pictures.Add(model);
                        }
                    }
                }
            }
            else
            {
                OnAddCatch();

                List<UserTicketShortDto> tickets = FishingTicketsTransaction.GetListOfActiveTickets();

                if (tickets?.Count > 0)
                {
                    UserTickets.AddRange(tickets);
                }
            }

            return Task.CompletedTask;
        }

        private void OnAddCatch()
        {
            CFCatchViewModel catchViewModel = new CFCatchViewModel(_fishTypes);

            Catches.Add(catchViewModel);
            Validation.OtherValidations.Add(catchViewModel.Validation);
        }

        private void OnRemoveCatch(CFCatchViewModel catchViewModel)
        {
            Catches.Remove(catchViewModel);
            Validation.OtherValidations.Remove(catchViewModel.Validation);

            if (Catches.Count == 0)
            {
                OnAddCatch();
            }
        }

        private void OnTicketSelected(UserTicketShortDto ticket)
        {
            TicketChosen = true;
        }

        private async Task OnAddPicture(MenuResult menuResult)
        {
            if (Pictures.Count >= 10)
            {
                await App.Current.MainPage.DisplayAlert(null,
                    TranslateExtension.Translator[nameof(GroupResourceEnum.AddCatchRecord) + "/CannotAddMorePictures"],
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Ok"]
                );
                return;
            }

            TLFileResult result;

            switch ((string)menuResult.Option)
            {
                case "Pick":
                    result = await TLMediaPicker.PickPhotoAsync();
                    break;
                case "Take":
                    result = await TLMediaPicker.CapturePhotoAsync();
                    break;
                default:
                    result = null;
                    break;
            }

            if (result != null)
            {
                if (_locallyUploadedFiles.Any(x => x.FileName == result.FileName && x.Size == result.FileSize))
                {
                    //prevent uploading same file twice in the current session
                    return;
                }

                _locallyUploadedFiles.Add(new CatchFileLocallyAdded { FileName = result.FileName, Size = result.FileSize });

                result = XamImageHelper.ImageResize(result, 1920, 1920, 75);

                CatchImageModel picture = new CatchImageModel
                {
                    WasAddedNow = true,
                    Image = result.FullPath,
                    ContentType = result.ContentType,
                    FullPath = result.FullPath,
                    Name = result.FileName,
                    UploadedOn = DateTime.Now,
                    Size = result.FileSize,
                };
                Pictures.Add(picture);
                _pictures.Add(picture);
            }
        }

        private async Task OnOpenPicture(CatchImageModel picture)
        {
            ImageSource source;

            if (string.IsNullOrEmpty(picture.FullPath) && picture.Id.HasValue)
            {
                FileResponse fileResult = await CatchRecordsTransaction.GetPhoto(picture.Id.Value);
                source = ImageSource.FromStream(() => new MemoryStream(fileResult.File));
            }
            else
            {
                source = picture.FullPath;
            }

            PopupPage dialog = null;

            Task CloseDialog()
            {
                return PopupNavigation.Instance.RemovePageAsync(dialog);
            }

            dialog = await TLDialogHelper.ShowDialog(
                new TLFillLayout
                {
                    MeasureMethod = TLFillLayout.MeasureMethodEnum.BasedOnParent,
                    Children =
                    {
                        new Image
                        {
                            Source = source
                        },
                        new StackLayout
                        {
                            VerticalOptions = LayoutOptions.End,
                            HorizontalOptions = LayoutOptions.End,
                            Orientation = StackOrientation.Horizontal,
                            Padding = 5,
                            Children =
                            {
                                new ImageButton
                                {
                                    IsVisible = CanEdit || IsAdd,
                                    Source = new FontImageSource
                                    {
                                        Color = Color.White,
                                        FontFamily = "FA",
                                        Size = 24,
                                        Glyph = IconFont.Trash
                                    },
                                    Padding = 10,
                                    BackgroundColor = App.GetResource<Color>("ErrorColor"),
                                    Command = CommandBuilder.CreateFrom(async () =>
                                    {
                                        await OnRemovePicture(picture);
                                        await CloseDialog();
                                    })
                                },
                                new ImageButton
                                {
                                    IsVisible = !picture.WasAddedNow && !picture.IsFileSavedLocally,
                                    Source = new FontImageSource
                                    {
                                        Color = Color.White,
                                        FontFamily = "FA",
                                        Size = 24,
                                        Glyph = IconFont.Download
                                    },
                                    Padding = 10,
                                    Command = CommandBuilder.CreateFrom(async () =>
                                    {
                                        await OnDownloadPicture(picture);
                                        await CloseDialog();
                                    })
                                }
                            }
                        }
                    }
                },
                picture.Name.Substring(0, Math.Min(20, picture.Name.Length)) + (picture.Name.Length > 20 ? "..." : null),
                Color.White,
                App.GetResource<Color>("Primary"),
                Color.White,
                false,
                false
            );
        }

        private async Task OnRemovePicture(CatchImageModel picture)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.AddCatchRecord) + "/DeleteImage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
            );

            if (result)
            {
                if (picture.WasAddedNow)
                {
                    _pictures.Remove(picture);
                }
                else
                {
                    CatchImageModel pic = _pictures.Where(x => x.Id == picture.Id).FirstOrDefault();
                    if (pic != null)
                    {
                        pic.WasDeleted = true;
                    }
                }

                Pictures.Remove(picture);
            }
        }

        private async Task OnDownloadPicture(CatchImageModel picture)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.AddCatchRecord) + "/DownloadPhotoMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
            );

            if (result)
            {
                IDownloader downloader = DependencyService.Resolve<IDownloader>();
                await downloader.DownloadFile(picture.Name, picture.ContentType, "CatchRecords/Photo", new { id = picture.Id.Value });
            }
        }

        private async Task OnSave()
        {
            Validation.Force();

            if (!Validation.IsValid || Catches.Count == 0)
            {
                return;
            }

            string identifier = CatchRecord?.Identifier ?? Guid.NewGuid().ToString();

            List<FileModel> files = await CatchRecordFilesHelper.HandleAllFiles(_pictures, identifier);

            CreateCatchRecordDto dto = new CreateCatchRecordDto
            {
                Id = CatchRecord?.Id,
                Identifier = identifier,
                CatchDate = CatchDate,
                Description = Description,
                WaterArea = WaterArea,
                Fishes = Catches.Select(f => new CreateCatchRecordFishDto
                {
                    Id = f.Catch?.Id,
                    CatchRecordId = f.Catch?.RecordCatchId,
                    FishTypeId = f.FishType.Value.Value,
                    Count = int.Parse(f.Count),
                    Quantity = double.Parse(f.Quantity)
                }).ToList(),
                Location = Location.HasValue
                    ? new LocationDto
                    {
                        DMSLatitude = DMSType.FromDouble(Location.Value.Latitude).ToString(),
                        DMSLongitude = DMSType.FromDouble(Location.Value.Longitude).ToString()
                    } : null,
                TicketId = Ticket.Value.Id,
                Files = files
            };

            if (CatchRecord != null)
            {
                await CatchRecordsTransaction.EditCatchRecord(dto);
            }
            else
            {
                await CatchRecordsTransaction.CreateCatchRecord(dto);
            }

            await MainNavigator.Current.PopPageAsync();
        }
    }
}

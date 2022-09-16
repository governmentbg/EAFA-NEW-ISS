using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.ViewModels.Models;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class InspectionFilesViewModel : ViewModel
    {
        private List<SelectNomenclatureDto> _fileTypes;

        public InspectionFilesViewModel(InspectionPageViewModel inspection)
        {
            Inspection = inspection;
            AddFile = CommandBuilder.CreateFrom(OnAddFile);
            RemoveFile = CommandBuilder.CreateFrom<InspectionFileViewModel>(OnRemoveFile);
            DownloadFile = CommandBuilder.CreateFrom<InspectionFileViewModel>(OnDownloadFile);

            ListOfFiles = new List<InspectionFileViewModel>();
        }

        public InspectionPageViewModel Inspection { get; }

        public List<InspectionFileViewModel> ListOfFiles { get; }

        public List<SelectNomenclatureDto> FileTypes
        {
            get => _fileTypes;
            private set => SetProperty(ref _fileTypes, value);
        }

        public ValidStateValidatableTable<InspectionFileViewModel> Files { get; set; }

        public ICommand AddFile { get; }
        public ICommand RemoveFile { get; }
        public ICommand DownloadFile { get; }

        public void Init(List<SelectNomenclatureDto> fileTypes)
        {
            FileTypes = fileTypes;
        }

        public void OnEdit(InspectionEditDto dto)
        {
            if (dto.Files == null || dto.Files.Count == 0)
            {
                return;
            }

            List<InspectionFileViewModel> fileViewModels = new List<InspectionFileViewModel>();

            foreach (FileModel file in dto.Files.FindAll(f => FileTypes.Any(s => s.Id == f.FileTypeId)))
            {
                FileResultModel fileResultModel = new FileResultModel
                {
                    IsUploaded = true,
                    ContentType = file.ContentType,
                    FileName = file.Name,
                    FileSize = file.Size,
                };

                InspectionFileViewModel viewModel = new InspectionFileViewModel(fileResultModel, FileTypes, file.FileTypeId)
                {
                    Id = file.Id,
                    IsFileSavedLocally = dto.IsOfflineOnly
                };
                viewModel.Description.Value = file.Description ?? string.Empty;
                viewModel.FileType.Value = FileTypes.Find(s => s.Id == file.FileTypeId);

                ListOfFiles.Add(viewModel);

                if (!file.Deleted)
                {
                    fileViewModels.Add(viewModel);
                }
            }

            Files.Value.AddRange(fileViewModels);
        }

        private async Task OnAddFile()
        {
            List<TLFileResult> files = await TLFilePicker.PickMultipleAsync();

            if (files == null)
            {
                return;
            }

            foreach (TLFileResult file in files)
            {
                if (!Files.Value.Any(f => f.File.FileName == file.FileName))
                {
                    FileResultModel fileModel = new FileResultModel
                    {
                        IsUploaded = false,
                        ContentType = file.ContentType,
                        FileName = file.FileName,
                        FileSize = file.FileSize,
                        FullPath = file.FullPath,
                    };

                    InspectionFileViewModel fileViewModel = new InspectionFileViewModel(fileModel, FileTypes)
                    {
                        WasAddedNow = true
                    };

                    Files.Value.Add(fileViewModel);

                    if (!ListOfFiles.Any(f => f.File.FileName == file.FileName))
                    {
                        ListOfFiles.Add(fileViewModel);
                    }
                }
            }
        }

        private async Task OnRemoveFile(InspectionFileViewModel file)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DeleteMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
            );

            if (result)
            {
                if (file.WasAddedNow)
                {
                    File.Delete(file.File.FullPath);
                    ListOfFiles.Remove(file);
                }
                else
                {
                    file.WasDeleted = true;
                }

                Files.Value.Remove(file);
            }
        }

        private async Task OnDownloadFile(InspectionFileViewModel fileViewModel)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DownloadFileMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
            );

            if (result)
            {
                FileResultModel file = fileViewModel.File;
                IDownloader downloader = DependencyService.Resolve<IDownloader>();

                await downloader.DownloadFile(file.FileName, file.ContentType, "Inspections/DownloadFile", new { id = fileViewModel.Id });
            }
        }
    }
}

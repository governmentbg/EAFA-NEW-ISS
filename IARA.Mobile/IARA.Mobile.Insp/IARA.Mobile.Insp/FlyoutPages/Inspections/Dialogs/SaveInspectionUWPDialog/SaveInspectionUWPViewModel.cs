using System;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Domain.Models;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FloppyDiskInspectionUWPDialog
{
    public class SaveInspectionUWPViewModel : TLBaseDialogViewModel<FileModel>
    {
        private bool _needsSignature;
        private TLFileResult _fileResult;

        public SaveInspectionUWPViewModel()
        {
            PickFile = CommandBuilder.CreateFrom(OnPickFile);
            Save = CommandBuilder.CreateFrom(OnSave);
        }

        public TLFileResult FileResult
        {
            get => _fileResult;
            set => SetProperty(ref _fileResult, value);
        }

        public bool NeedsSignature
        {
            get => _needsSignature;
            set => SetProperty(ref _needsSignature, value);
        }

        public ICommand PickFile { get; }
        public ICommand Save { get; }

        public override Task Initialize(object sender)
        {
            return Task.CompletedTask;
        }

        private async Task OnPickFile()
        {
            TLFileResult file = await TLFilePicker.PickAsync();

            if (file != null)
            {
                FileResult = file;
            }
        }

        private Task OnSave()
        {
            TLFileResult file = FileResult;

            if (file == null)
            {
                NeedsSignature = true;
                return Task.CompletedTask;
            }

            return HideDialog(new FileModel
            {
                ContentType = file.ContentType,
                FullPath = file.FullPath,
                Name = file.FileName,
                Size = file.FileSize,
                UploadedOn = DateTime.Now,
            });
        }
    }
}

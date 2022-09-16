using IARA.Mobile.Insp.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.ViewModels.Base;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FloppyDiskInspectionDialog
{
    public class SaveInspectionViewModel : TLBaseDialogViewModel<bool>
    {
        private bool _needsSignatures;

        public SaveInspectionViewModel()
        {
            Save = CommandBuilder.CreateFrom(OnSave);
            SignatureBytes = new List<Func<Task<byte[]>>>();
        }

        public bool NeedsSignatures
        {
            get => _needsSignatures;
            set => SetProperty(ref _needsSignatures, value);
        }

        public List<SignatureSaveModel> Signatures { get; set; }

        public List<Func<Task<byte[]>>> SignatureBytes { get; }

        public override Task Initialize(object sender)
        {
            return Task.CompletedTask;
        }

        public ICommand Save { get; }

        private async Task OnSave()
        {
            for (int i = 0; i < SignatureBytes.Count; i++)
            {
                byte[] bytes = await SignatureBytes[i]();

                if (bytes == null)
                {
                    NeedsSignatures = true;
                    return;
                }

                Signatures[i].Signature = bytes;
            }

            await HideDialog(true);
        }
    }
}

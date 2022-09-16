using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.ViewModels.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class SignaturesViewModel : ViewModel
    {
        private bool _hasSignature;

        public SignaturesViewModel(InspectionPageViewModel inspection, bool hasInspectedPerson = true)
        {
            Inspection = inspection;
            HasInspectedPerson = hasInspectedPerson;

            Inspector = new SignatureModel(inspection);

            if (hasInspectedPerson)
            {
                InspectedPerson = new SignatureModel(inspection);
            }
        }

        public InspectionPageViewModel Inspection { get; }

        public bool HasInspectedPerson { get; }

        public SignatureModel Inspector { get; }
        public SignatureModel InspectedPerson { get; }

        public bool HasSignature
        {
            get => _hasSignature;
            set => SetProperty(ref _hasSignature, value);
        }

        public void OnEdit(List<FileModel> files, List<SelectNomenclatureDto> fileTypes)
        {
            FileModel inspectorSignatureFile = files?.Find(f => fileTypes.Any(s => s.Id == f.FileTypeId && s.Code == Constants.InspectorSignature));

            if (inspectorSignatureFile != null)
            {
                OnEditSingle(inspectorSignatureFile, Inspector);
                HasSignature = true;
            }

            if (InspectedPerson != null)
            {
                FileModel inspectedPersonSignatureFile = files?.Find(f => fileTypes.Any(s => s.Id == f.FileTypeId && s.Code == Constants.InspectedPersonSignature));

                if (inspectedPersonSignatureFile != null)
                {
                    OnEditSingle(inspectedPersonSignatureFile, InspectedPerson);
                }
            }
        }

        private void OnEditSingle(FileModel inspectorSignatureFile, SignatureModel signature)
        {
            if (inspectorSignatureFile.FullPath != null)
            {
                signature.Image = ImageSource.FromFile(inspectorSignatureFile.FullPath);
            }
            else if (CommonGlobalVariables.InternetStatus == InternetStatus.Connected && inspectorSignatureFile.Id.HasValue)
            {
                signature.Image = ImageSource.FromStream(async (_) =>
                {
                    FileResponse file = await InspectionsTransaction.GetFile(inspectorSignatureFile.Id.Value);

                    if (file == null)
                    {
                        return null;
                    }

                    return new MemoryStream(file.File);
                });
            }
        }
    }
}

using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.ViewModels.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
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
            Inspectors = new TLObservableCollection<SignatureModel>();
            if (hasInspectedPerson)
            {
                InspectedPerson = new SignatureModel(inspection)
                {
                    SignatureType = TranslateExtension.Translator[nameof(GroupResourceEnum.GeneralInfo) + "/InspectedPersonSignature"]
                };
            }
        }

        public InspectionPageViewModel Inspection { get; }

        public bool HasInspectedPerson { get; }

        public TLObservableCollection<SignatureModel> Inspectors { get; }
        public SignatureModel InspectedPerson { get; }

        public bool HasSignature
        {
            get => _hasSignature;
            set => SetProperty(ref _hasSignature, value);
        }

        public void OnEdit(List<FileModel> files, List<SelectNomenclatureDto> fileTypes)
        {
            HasSignature = true;
            List<SignatureModel> inspectorSignatures = new List<SignatureModel>();
            foreach (FileModel inspectorSignature in files?.Where(f => fileTypes.Any(s => s.Id == f.FileTypeId && s.Code == Constants.InspectorSignature)))
            {
                SignatureModel signature = new SignatureModel(Inspection);
                OnEditSingle(inspectorSignature, signature);
                inspectorSignatures.Add(signature);
            }
            Inspectors.AddRange(inspectorSignatures);


            if (InspectedPerson != null)
            {
                FileModel inspectedPersonSignatureFile = files?.Find(f => fileTypes.Any(s => s.Id == f.FileTypeId && s.Code == Constants.InspectedPersonSignature));

                if (inspectedPersonSignatureFile != null)
                {
                    OnEditSingle(inspectedPersonSignatureFile, InspectedPerson);
                    Inspectors.Add(InspectedPerson);
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

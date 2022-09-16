using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using IARA.Common;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;
using IARA.EntityModels.Entities;
using IARA.EntityModels.Interfaces;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Models;
using IARA.Infrastructure.FSM.Utils;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TL.Signer;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = Enums.TransitionCodesEnum.GenSignedPDF)]
    internal class GenerateSignedPdfTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id, pageCode and statusReason as parameters.";

        public GenerateSignedPdfTransition(TransitionContext context)
            : base(context)
        { }

        public override bool CanTransition(int id, bool isTriggeredManually, string statusReason)
        {
            if (!isTriggeredManually || this.HasRegisterEntry(id, RecordTypesEnum.Application) == false)
            {
                return false;
            }

            DateTime now = DateTime.Now;

            PageCodeEnum pageCode = this.GetApplicationPageCode(id);

            List<int> requiredFilesTypeIds = (from reqFiles in db.NrequiredFileTypes
                                              where reqFiles.PageCode == pageCode.ToString() && reqFiles.IsMandatory
                                                  && reqFiles.ValidFrom >= now && reqFiles.ValidTo < now
                                              select reqFiles.FileTypeId).ToList();

            if (requiredFilesTypeIds.Count == 0)
            {
                return true;
            }

            var filesEntityType = FSMUtils.GetApplicationFilesEntityType(pageCode);
            var filesDbSet = typeof(IARADbContext).GetMethod("Set").MakeGenericMethod(filesEntityType)
                                                                   .Invoke(db, new object[] { }) as IQueryable<IFileEntity>;
            List<int> dbFileIds = filesDbSet.Select(x => x.FileTypeId).ToList();

            List<int> missingRequiredFileIds = requiredFilesTypeIds.Except(dbFileIds).ToList();

            return missingRequiredFileIds.Count == 0;
        }

        public override ApplicationStatusesEnum Action(int id, string statusReason = "")
        {
            using (TransactionScope scope = new TransactionScope())
            {
                ApplicationStatusesEnum newStatus = base.Action(id, statusReason);
                db.SaveChanges();

                PageCodeEnum pageCode = this.GetApplicationPageCode(id);

                var now = DateTime.Now;

                var submiterPerson = (from appl in db.Applications
                                      join applHistory in db.ApplicationChangeHistories on appl.Id equals applHistory.ApplicationId
                                      join person in db.Persons on appl.SubmittedByPersonId equals person.Id
                                      where appl.Id == id && applHistory.ValidFrom <= now && applHistory.ValidTo >= now
                                      select new
                                      {
                                          person.EgnLnc,
                                          person.FirstName,
                                          person.MiddleName,
                                          person.LastName,
                                          ApplicationHistoryId = applHistory.Id
                                      }).First();

                PdfDocumentMetadata documentMetadata = new PdfDocumentMetadata(id, submiterPerson.ApplicationHistoryId, submiterPerson.EgnLnc);

                int fileTypeId = db.NfileTypes
                                   .Where(x => x.Code == FileTypeEnum.APPLICATION_PDF.ToString())
                                   .Select(x => x.Id)
                                   .First();

                IJasperReportExecutionService jasperReportsService = serviceProvider.GetRequiredService<IJasperReportExecutionService>();

                switch (pageCode)
                {
                    case PageCodeEnum.SciFi:
                        {
                            string fileName = FormatFileName(ApplicationNumbers.SciFi, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                            GenerateApplicationPdf(db.ScientificPermitRegisters, id, fileTypeId, fileName, (id) =>
                            {
                                return jasperReportsService.GetScientificFishingApplication(id);
                            }, documentMetadata, x => x.ScientificPermitRegisterFiles);
                        }
                        break;
                    case PageCodeEnum.LE:
                        {
                            string fileName = FormatFileName(ApplicationNumbers.LegalAccess, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                            GenerateApplicationNullablePdf(db.Legals, id, fileTypeId, fileName, (id) =>
                            {
                                return jasperReportsService.GetLegalApplication(id);
                            }, documentMetadata, x => x.LegalFiles);
                        }
                        break;
                    case PageCodeEnum.PoundnetCommFish:
                    case PageCodeEnum.RightToFishThirdCountry:
                    case PageCodeEnum.CommFish:
                        {
                            string fileName = "";

                            if (pageCode == PageCodeEnum.PoundnetCommFish)
                            {
                                fileName = FormatFileName(ApplicationNumbers.PoundnetCommFish, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);
                            }
                            else if (pageCode == PageCodeEnum.RightToFishThirdCountry)
                            {
                                fileName = FormatFileName(ApplicationNumbers.RightToFishThirdCountry, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);
                            }
                            else if (pageCode == PageCodeEnum.CommFish)
                            {
                                fileName = FormatFileName(ApplicationNumbers.CommFish, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);
                            }

                            GenerateApplicationPdf(db.CommercialFishingPermitRegisters, id, fileTypeId, fileName, (id) =>
                            {
                                return jasperReportsService.GetCommercialFishingApplication(id);
                            }, documentMetadata, x => x.PermitRegisterFiles);
                        }
                        break;
                    case PageCodeEnum.AquaFarmReg:
                        {
                            string fileName = FormatFileName(ApplicationNumbers.AquacultureFacilityReg, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                            GenerateApplicationPdf(db.AquacultureFacilitiesRegister, id, fileTypeId, fileName, (id) =>
                            {
                                return jasperReportsService.GetAquacultureFacilityApplication(id);
                            }, documentMetadata, x => x.AquacultureFacilityRegisterFiles);
                        }
                        break;
                    case PageCodeEnum.PoundnetCommFishLic:
                    case PageCodeEnum.RightToFishResource:
                    case PageCodeEnum.CatchQuataSpecies:
                        {
                            string fileName = "";

                            if (pageCode == PageCodeEnum.PoundnetCommFishLic)
                            {
                                fileName = FormatFileName(ApplicationNumbers.PoundnetCommFishLic, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);
                            }
                            else if (pageCode == PageCodeEnum.RightToFishResource)
                            {
                                fileName = FormatFileName(ApplicationNumbers.RightToFishResource, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);
                            }
                            else if (pageCode == PageCodeEnum.CatchQuataSpecies)
                            {
                                fileName = FormatFileName(ApplicationNumbers.CatchQuataSpecies, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);
                            }

                            GenerateApplicationPdf(db.CommercialFishingPermitLicensesRegisters, id, fileTypeId, fileName, (id) =>
                            {
                                return jasperReportsService.GetCommercialFishingApplication(id);
                            }, documentMetadata, x => x.PermitLicensesRegisterFiles);
                        }
                        break;
                    case PageCodeEnum.TransferFishCap:
                    case PageCodeEnum.ReduceFishCap:
                    case PageCodeEnum.IncreaseFishCap:
                    case PageCodeEnum.CapacityCertDup:
                        {
                            string fileName = "";
                            Func<int, Task<byte[]>> getApplicationPdf = null;

                            if (pageCode == PageCodeEnum.TransferFishCap)
                            {
                                fileName = FormatFileName(ApplicationNumbers.TransferFishCap, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                                getApplicationPdf = (id) =>
                                {
                                    return jasperReportsService.GetCapacityApplicationTransfer(id);
                                };
                            }
                            else if (pageCode == PageCodeEnum.ReduceFishCap)
                            {
                                fileName = FormatFileName(ApplicationNumbers.ReduceFishCap, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                                getApplicationPdf = (id) =>
                                {
                                    return jasperReportsService.GetCapacityApplicationReduce(id);
                                };
                            }
                            else if (pageCode == PageCodeEnum.IncreaseFishCap)
                            {
                                fileName = FormatFileName(ApplicationNumbers.IncreaseFishCap, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                                getApplicationPdf = (id) =>
                                {
                                    return jasperReportsService.GetCapacityApplicationIncrease(id);
                                };
                            }
                            else if (pageCode == PageCodeEnum.CapacityCertDup)
                            {
                                fileName = FormatFileName(ApplicationNumbers.DuplicateFishCap, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                                getApplicationPdf = (id) =>
                                {
                                    return jasperReportsService.GetCapacityApplicationDuplicate(id);
                                };
                            }

                            GenerateApplicationPdf(id, fileTypeId, fileName, getApplicationPdf, documentMetadata);
                        }
                        break;
                    case PageCodeEnum.RegVessel:
                        {
                            string fileName = FormatFileName(ApplicationNumbers.RegVessel, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                            GenerateApplicationNullablePdf(db.ShipsRegister, id, fileTypeId, fileName, (id) =>
                            {
                                return jasperReportsService.GetFishingVesselsApplication(id);
                            }, documentMetadata, x => x.ShipRegisterFiles);
                        }
                        break;
                    case PageCodeEnum.CommFishLicense:
                        {
                            string fileName = FormatFileName(ApplicationNumbers.CommFishLicense, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                            GenerateApplicationNullablePdf(db.FishermenRegisters, id, fileTypeId, fileName, (id) =>
                            {
                                return jasperReportsService.GetFishermanApplication(id);
                            }, documentMetadata, x => x.FishermenRegisterFiles);
                        }
                        break;
                    case PageCodeEnum.RegFirstSaleCenter:
                    case PageCodeEnum.RegFirstSaleBuyer:
                        {
                            string fileName = "";

                            Func<int, Task<byte[]>> getApplicationPdf = null;

                            if (pageCode == PageCodeEnum.RegFirstSaleCenter)
                            {
                                fileName = FormatFileName(ApplicationNumbers.RegFirstSaleCenter, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                                getApplicationPdf = (id) =>
                                {
                                    return jasperReportsService.GetSalesCentersApplication(id);
                                };
                            }
                            else if (pageCode == PageCodeEnum.RegFirstSaleBuyer)
                            {
                                fileName = FormatFileName(ApplicationNumbers.RegFirstSaleBuyer, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                                getApplicationPdf = (id) =>
                                {
                                    return jasperReportsService.GetBuyersApplication(id);
                                };
                            }

                            GenerateApplicationPdf(db.BuyerRegisters, id, fileTypeId, fileName, getApplicationPdf, documentMetadata, x => x.BuyerRegisterFiles);
                        }
                        break;
                    case PageCodeEnum.ChangeFirstSaleBuyer:
                    case PageCodeEnum.ChangeFirstSaleCenter:
                    case PageCodeEnum.TermFirstSaleCenter:
                    case PageCodeEnum.TermFirstSaleBuyer:
                        {
                            string fileName = "";

                            Func<int, Task<byte[]>> getApplicationPdf = null;

                            if (pageCode == PageCodeEnum.TermFirstSaleCenter)
                            {
                                fileName = FormatFileName(ApplicationNumbers.TermFirstSaleCenter, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                                getApplicationPdf = (id) =>
                                {
                                    return jasperReportsService.GetFirstSaleCenterDeregistrationApplication(id);
                                };
                            }
                            else if (pageCode == PageCodeEnum.TermFirstSaleBuyer)
                            {
                                fileName = FormatFileName(ApplicationNumbers.TermFirstSaleBuyer, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                                getApplicationPdf = (id) =>
                                {
                                    return jasperReportsService.GetBuyerDeregistrationApplication(id);
                                };
                            }
                            else if (pageCode == PageCodeEnum.ChangeFirstSaleCenter)
                            {
                                fileName = FormatFileName(ApplicationNumbers.ChangeFirstSaleCenter, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                                getApplicationPdf = (id) =>
                                {
                                    return jasperReportsService.GetFirstSaleCenterChangeApplication(id);
                                };
                            }
                            else if (pageCode == PageCodeEnum.ChangeFirstSaleBuyer)
                            {
                                fileName = FormatFileName(ApplicationNumbers.ChangeFirstSaleBuyer, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                                getApplicationPdf = (id) =>
                                {
                                    return jasperReportsService.GetBuyerChangeApplication(id);
                                };
                            }

                            GenerateApplicationPdf(id, fileTypeId, fileName, getApplicationPdf, documentMetadata);
                        }
                        break;
                    case PageCodeEnum.ShipRegChange:
                    case PageCodeEnum.AquaFarmChange:
                    case PageCodeEnum.AquaFarmDereg:
                    case PageCodeEnum.DeregShip:
                        {
                            string fileName = "";

                            Func<int, Task<byte[]>> getApplicationPdf = null;

                            if (pageCode == PageCodeEnum.ShipRegChange)
                            {
                                fileName = FormatFileName(ApplicationNumbers.ShipRegChange, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                                getApplicationPdf = (id) =>
                                {
                                    return jasperReportsService.GetFishingVesselsChangeApplication(id);
                                };
                            }
                            else if (pageCode == PageCodeEnum.AquaFarmChange)
                            {
                                fileName = FormatFileName(ApplicationNumbers.AquaculutreFacilityChange, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                                getApplicationPdf = (id) =>
                                {
                                    return jasperReportsService.GetAquacultureFacilityChangeApplication(id);
                                };
                            }
                            else if (pageCode == PageCodeEnum.AquaFarmDereg)
                            {
                                fileName = FormatFileName(ApplicationNumbers.AquacultureFacilityDereg, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                                getApplicationPdf = (id) =>
                                {
                                    return jasperReportsService.GetAquacultureFacilityDeregApplication(id);
                                };
                            }
                            else if (pageCode == PageCodeEnum.DeregShip)
                            {
                                fileName = FormatFileName(ApplicationNumbers.DeregShip, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                                getApplicationPdf = (id) =>
                                {
                                    return jasperReportsService.GetFishingVesselsDestroyApplication(id);
                                };
                            }

                            GenerateApplicationPdf(id, fileTypeId, fileName, getApplicationPdf, documentMetadata);
                        }
                        break;
                    case PageCodeEnum.StatFormAquaFarm:
                        {
                            string fileName = FormatFileName(ApplicationNumbers.StatFormAquaculture, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                            GenerateApplicationPdf(db.StatisticalFormsRegister, id, fileTypeId, fileName, (id) =>
                            {
                                return jasperReportsService.GetStatisticalFormAquacultureApplication(id);
                            }, documentMetadata, x => x.StatisticalFormsRegisterFiles);
                        }
                        break;
                    case PageCodeEnum.StatFormFishVessel:
                        {
                            string fileName = FormatFileName(ApplicationNumbers.StatFormFishingVessel, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                            GenerateApplicationPdf(db.StatisticalFormsRegister, id, fileTypeId, fileName, (id) =>
                            {
                                return jasperReportsService.GetStatisticalFormFishingVesselApplication(id);
                            }, documentMetadata, x => x.StatisticalFormsRegisterFiles);
                        }
                        break;
                    case PageCodeEnum.StatFormRework:
                        {
                            string fileName = FormatFileName(ApplicationNumbers.StatFormRework, submiterPerson.FirstName, submiterPerson.MiddleName, submiterPerson.LastName);

                            GenerateApplicationPdf(db.StatisticalFormsRegister, id, fileTypeId, fileName, (id) =>
                            {
                                return jasperReportsService.GetStatisticalFormReworkApplication(id);
                            }, documentMetadata, x => x.StatisticalFormsRegisterFiles);
                        }
                        break;
                    default:
                        break;
                }

                scope.Complete();
                return newStatus;
            }
        }

        private string FormatFileName(string application, string firstName, string middleName, string lastName)
        {
            return $"Заявление {application} - {firstName} {middleName} {lastName}.pdf".Replace("  ", " ");
        }

        private void GenerateApplicationPdf<TMeta>(int id, int fileTypeId, string fileName, Func<int, Task<byte[]>> getApplicationPdf, TMeta documentMetadata)
            where TMeta : class
        {
            IPdfSigner pdfSignerService = serviceProvider.GetRequiredService<IPdfSigner>();

            byte[] applicationFileBytes = getApplicationPdf(id).Result;

            applicationFileBytes = pdfSignerService.SignPdf(applicationFileBytes, "IARA", documentMetadata);

            Application application = db.Applications
                                        .Include(x => x.ApplicationFiles)
                                        .Where(x => x.Id == id)
                                        .First();

            var applicationFiles = application.ApplicationFiles.Where(x => x.FileTypeId == fileTypeId);

            if (applicationFiles.Any())
            {
                foreach (var file in applicationFiles)
                {
                    file.IsActive = false;
                }
            }

            AddOrEditFile(fileTypeId, application, fileName, applicationFileBytes, x => x.ApplicationFiles);
        }

        private void GenerateApplicationPdf<TMeta, TEntity, TFileEntity>(DbSet<TEntity> registerDbSet,
            int id,
            int fileTypeId,
            string fileName,
            Func<int, Task<byte[]>> getApplicationPdf,
            TMeta documentMetadata,
            Expression<Func<TEntity, ICollection<TFileEntity>>> filesProperty)
            where TEntity : class, IApplicationIdentifier, IBaseApplicationRegisterEntity
            where TFileEntity : class, ISoftDeletable, IFileEntity<TEntity>, new()
            where TMeta : class
        {
            IPdfSigner pdfSignerService = serviceProvider.GetRequiredService<IPdfSigner>();

            byte[] applicationFileBytes = getApplicationPdf(id).Result;

            applicationFileBytes = pdfSignerService.SignPdf(applicationFileBytes, "IARA", documentMetadata);

            TEntity application = registerDbSet.Include(filesProperty)
                                               .Where(x => x.ApplicationId == id
                                                   && x.RecordType == RecordTypesEnum.Application.ToString())
                                               .First();

            var applicationFiles = filesProperty.Compile().Invoke(application).Where(x => x.FileTypeId == fileTypeId);

            if (applicationFiles.Any())
            {
                foreach (var file in applicationFiles)
                {
                    file.IsActive = false;
                }
            }

            AddOrEditFile(fileTypeId, application, fileName, applicationFileBytes, filesProperty);
        }

        private void GenerateApplicationNullablePdf<TEntity, TFileEntity>(DbSet<TEntity> registerDbSet,
            int id,
            int fileTypeId,
            string fileName,
            Func<int, Task<byte[]>> getApplicationPdf,
            PdfDocumentMetadata documentMetadata,
            Expression<Func<TEntity, ICollection<TFileEntity>>> filesProperty)
            where TEntity : class, IApplicationNullableIdentifier, IBaseApplicationRegisterEntity
            where TFileEntity : class, ISoftDeletable, IFileEntity<TEntity>, new()
        {
            IPdfSigner pdfSignerService = serviceProvider.GetRequiredService<IPdfSigner>();

            byte[] applicationFileBytes = getApplicationPdf(id).Result;

            applicationFileBytes = pdfSignerService.SignPdf(applicationFileBytes, "IARA", documentMetadata);

            TEntity application = registerDbSet.Include(filesProperty)
                                               .Where(x => x.ApplicationId == id
                                                   && x.RecordType == RecordTypesEnum.Application.ToString())
                                               .First();


            var applicationFiles = filesProperty.Compile().Invoke(application).Where(x => x.FileTypeId == fileTypeId);

            if (applicationFiles.Any())
            {
                foreach (var file in applicationFiles)
                {
                    file.IsActive = false;
                }
            }

            AddOrEditFile(fileTypeId, application, fileName, applicationFileBytes, filesProperty);
        }

        private void AddOrEditFile<TEntity, TFileEntity>(int fileTypeId,
                                                         TEntity application,
                                                         string fileName,
                                                         byte[] applicationFileBytes,
                                                         Expression<Func<TEntity, ICollection<TFileEntity>>> filesProperty)
            where TEntity : class
            where TFileEntity : class, ISoftDeletable, IFileEntity<TEntity>, new()
        {
            int? fileId = filesProperty.Compile().Invoke(application)
                                                 .Where(x => x.FileTypeId == fileTypeId && x.IsActive)
                                                 .Select(x => new int?(x.Id))
                                                 .FirstOrDefault();

            if (fileId.HasValue)
            {
                db.DeleteFile<TFileEntity>(fileId.Value);
            }

            var fileInfo = new FileInfoDTO(new LocalFormFile(applicationFileBytes, fileName, MimeTypes.PDF), fileTypeId);

            db.AddOrEditFile(application, filesProperty.Compile().Invoke(application), fileInfo);
        }
    }
}

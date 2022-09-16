using System;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.EntityModels.Entities;
using IARA.EntityModels.Interfaces;
using IARA.Infrastructure.Services.Internal;

namespace IARA.Infrastructure.Helpers
{
    internal static class UsageDocumentsHelper
    {
        public static UsageDocument AddUsageDocument<TApplication>(this IARADbContext db, UsageDocumentDTO document, TApplication application)
            where TApplication : IApplicationIdentifier, IBaseApplicationRegisterEntity
        {
            UsageDocument entry = new UsageDocument
            {
                DocumentTypeId = document.DocumentTypeId.Value,
                DocumentNum = document.DocumentNum,
                DocumentValidFrom = document.DocumentValidFrom.Value,
                DocumentValidTo = document.DocumentValidTo,
                IsLessorPerson = document.IsLessorPerson,
                Comments = document.Comments
            };

            if (document.IsLessorPerson != null)
            {
                if (document.IsLessorPerson.Value)
                {
                    entry.LessorPerson = db.AddOrEditPerson(document.LessorPerson, document.LessorAddresses);
                }
                else
                {
                    entry.LessorLegal = db.AddOrEditLegal(new ApplicationRegisterDataDTO
                    {
                        ApplicationId = application.ApplicationId,
                        RecordType = Enum.Parse<RecordTypesEnum>(application.RecordType)
                    }, document.LessorLegal, document.LessorAddresses);
                }
            }

            db.UsageDocuments.Add(entry);

            return entry;
        }

        public static UsageDocument EditUsageDocument<TApplication>(this IARADbContext db, UsageDocumentDTO document, TApplication application)
            where TApplication : IApplicationIdentifier, IBaseApplicationRegisterEntity
        {
            if (document == null)
            {
                return null;
            }

            UsageDocument dbEntry = (from doc in db.UsageDocuments
                                     where doc.Id == document.Id.Value
                                     select doc).Single();

            dbEntry.DocumentTypeId = document.DocumentTypeId.Value;
            dbEntry.DocumentNum = document.DocumentNum;
            dbEntry.DocumentValidFrom = document.DocumentValidFrom.Value;

            if (!document.IsDocumentIndefinite.Value)
            {
                dbEntry.DocumentValidTo = document.DocumentValidTo.Value;
            }
            else
            {
                dbEntry.DocumentValidTo = null;
            }

            dbEntry.IsLessorPerson = document.IsLessorPerson;
            dbEntry.Comments = document.Comments;

            if (document.IsLessorPerson != null)
            {
                if (document.IsLessorPerson.Value)
                {
                    dbEntry.LessorLegalId = null;
                    dbEntry.LessorPerson = db.AddOrEditPerson(document.LessorPerson, document.LessorAddresses, dbEntry.LessorPersonId);
                }
                else
                {
                    dbEntry.LessorPersonId = null;
                    dbEntry.LessorLegal = db.AddOrEditLegal(new ApplicationRegisterDataDTO
                    {
                        ApplicationId = application.ApplicationId,
                        RecordType = Enum.Parse<RecordTypesEnum>(application.RecordType),
                    }, document.LessorLegal, document.LessorAddresses, dbEntry.LessorLegalId);
                }
            }
            else
            {
                dbEntry.LessorPersonId = null;
                dbEntry.LessorLegalId = null;
            }

            return dbEntry;
        }

        public static UsageDocument EditUsageDocument<TApplication>(this IARADbContext db, UsageDocumentRegixDataDTO document, TApplication application)
            where TApplication : IApplicationIdentifier, IBaseApplicationRegisterEntity
        {
            if (document == null)
            {
                return null;
            }

            UsageDocument dbEntry = (from doc in db.UsageDocuments
                                     where doc.Id == document.Id
                                     select doc).Single();

            if (dbEntry.IsLessorPerson != null)
            {
                if (dbEntry.IsLessorPerson.Value)
                {
                    dbEntry.LessorPerson = db.AddOrEditPerson(document.LessorPerson, document.LessorAddresses, dbEntry.LessorPersonId);
                }
                else
                {
                    dbEntry.LessorLegal = db.AddOrEditLegal(new ApplicationRegisterDataDTO
                    {
                        ApplicationId = application.ApplicationId,
                        RecordType = Enum.Parse<RecordTypesEnum>(application.RecordType),
                    }, document.LessorLegal, document.LessorAddresses, dbEntry.LessorLegalId);
                }
            }

            return dbEntry;
        }
    }
}

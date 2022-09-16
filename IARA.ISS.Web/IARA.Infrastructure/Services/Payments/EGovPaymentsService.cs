using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DataAccess;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces.FSM;
using IARA.Logging.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using TL.EGovPayments.ControllerModels;
using TL.EGovPayments.Interfaces;
using TL.EGovPayments.JsonEnums;
using TL.EGovPayments.JsonModels;

namespace IARA.Infrastructure.Services.Payments
{
    public class EGovPaymentsService : BasePaymentService, IEGovIntegrationService
    {
        public EGovPaymentsService(IARADbContext dbContext, IApplicationStateMachine stateMachine, IExtendedLogger logger)
            : base(dbContext, stateMachine, logger)
        { }

        public bool ChangePaymentStatusCallback(PaymentStatus paymentStatus)
        {
            try
            {
                int applicationId = GetApplicationIdByPaymentRef(paymentStatus.Id);
                var statuses = GetApplicationStatus(applicationId);

                if ((paymentStatus.Status == PaymentRequestStatuses.PAID
                    || paymentStatus.Status == PaymentRequestStatuses.AUTHORIZED
                    || paymentStatus.Status == PaymentRequestStatuses.ORDERED)
                   && statuses.Item1 != PaymentStatusesEnum.PaidOK)
                {
                    MarkPaymentAsPaid(applicationId, paymentStatus.Id, paymentStatus.Status.ToString(), paymentStatus.ChangeTime);
                }
                else if ((paymentStatus.Status == PaymentRequestStatuses.EXPIRED
                            || paymentStatus.Status == PaymentRequestStatuses.CANCELED
                            || paymentStatus.Status == PaymentRequestStatuses.SUSPENDED)
                         && statuses.Item2 != ApplicationStatusesEnum.PAYMENT_ANNUL)
                {
                    MarkPaymentAsAnnuled(applicationId, paymentStatus.Status.ToString());
                }
                else // for PENDING payment status
                {
                    string newStatusReason = string.Format(AppResources.statusInPayEGovBG, paymentStatus.Status.ToString());
                    this.UpdateApplicationStatusReason(applicationId, newStatusReason);
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return false;
            }
        }

        public string GeneratePaymentNumber(PaymentRequest paymentRequest)
        {
            return paymentRequest.PaymentReferenceNumber.PadLeft(10, '0');
        }

        public int GetApplicationIdByPaymentRef(string paymentRefNumber)
        {
            return (from application in Db.Applications
                    join payment in Db.ApplicationPayments on application.Id equals payment.ApplicationId
                    where payment.PaymentRefNum == paymentRefNumber
                    select application.Id).FirstOrDefault();
        }

        public EGovPaymentRequestModel GetPaymentData(string paymentRefNumber)
        {
            DateTime now = DateTime.Now;

            var userInfo = GetCurrentUserInfo();

            int applicationId = int.Parse(paymentRefNumber);
            var paymentDetails = GetPaymentDetails(applicationId);

            EGovPaymentRequestModel result = new EGovPaymentRequestModel
            {
                ApplicantIdentifier = userInfo.EgnLnch,
                ApplicantName = userInfo.Name,
                PaymentAmount = (float)paymentDetails.Amount,
                PaymentReason = paymentDetails.Description,
                PaymentRefDate = now,
                PaymentRefNumber = paymentRefNumber
            };

            if (!string.IsNullOrEmpty(result.PaymentReason) && result.PaymentReason.Length > 200)
            {
                result.PaymentReason = result.PaymentReason.Substring(0, 200);
            }

            if (userInfo.IdentifierType == IdentifierTypeEnum.EGN)
            {
                result.ApplicantType = ApplicantTypes.EGN;
            }
            else if (userInfo.IdentifierType == IdentifierTypeEnum.LNC)
            {
                result.ApplicantType = ApplicantTypes.LNCH;
            }
            else
            {
                throw new ArgumentException("Unknown applicant type", nameof(result.ApplicantType));
            }

            return result;
        }

        public void PaymentResponseCallback(VPOSPaymentResponse vPOSPaymentResponse)
        {
            int applicationId = GetApplicationIdByPaymentRef(vPOSPaymentResponse.RequestId);
            logger.LogWarning($"{nameof(PaymentResponseCallback)} ApplicationId: {applicationId} PaymentStatus: {vPOSPaymentResponse.Status}");

            //MovePaymentStatus(applicationId, vPOSPaymentResponse.Status != VPOSPaymentStatus.SUCCESS);
        }

        public void SavePaymentId(string paymentRefNumber, string paymentId)
        {
            int applicationId = int.Parse(paymentRefNumber);
            ApplicationPayment applicationPayment = (from appl in Db.Applications
                                                     join applPayment in Db.ApplicationPayments on appl.Id equals applPayment.ApplicationId
                                                     where appl.Id == applicationId
                                                     select applPayment).Single();

            applicationPayment.PaymentRefNum = paymentId;

            Db.SaveChanges();
        }

        private void UpdateApplicationStatusReason(int applicationId, string statusReason)
        {
            DateTime now = DateTime.Now;
            Application application = (from appl in Db.Applications.AsSplitQuery().Include(x => x.ApplicationChangeHistories)
                                       where appl.Id == applicationId
                                       select appl).Single();

            application.StatusReason = statusReason;

            Db.AddApplicationChangeHistory(application, now);
            Db.SaveChanges();
        }
    }
}

using System;
using System.Linq;
using System.Transactions;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.FSM;
using IARA.DomainModels.DTOModels.StatisticalForms;
using IARA.DomainModels.DTOModels.StatisticalForms.AquaFarms;
using IARA.DomainModels.DTOModels.StatisticalForms.Reworks;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Enums;
using IARA.Infrastructure.FSM.Models;
using IARA.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.Infrastructure.FSM.Transitions
{
    [Connection(TransitionCode = TransitionCodesEnum.AutoFillAdmAct)]
    internal class AutoFillAdministrativeActTransition : Transition
    {
        protected override string ActionErrorMessage => "Use the overload with id and statusReason as parameters.";

        public AutoFillAdministrativeActTransition(TransitionContext context)
            : base(context)
        { }

        public override bool CanTransition(int id, bool isTriggeredManually, string statusReason)
        {
            bool canTransition = false;

            PageCodeEnum applPageCode = GetApplicationPageCode(id);

            switch (applPageCode)
            {
                case PageCodeEnum.StatFormAquaFarm:
                case PageCodeEnum.StatFormFishVessel:
                case PageCodeEnum.StatFormRework:
                    {
                        bool hasRecordTypeApplicationEntry = (from statisticalForm in db.StatisticalFormsRegister
                                                              where statisticalForm.ApplicationId == id
                                                                    && statisticalForm.RecordType == nameof(RecordTypesEnum.Application)
                                                                    && statisticalForm.IsActive
                                                              select 1).Any();

                        bool hasRecordTypeRegisterEntry = (from statisticalForm in db.StatisticalFormsRegister
                                                           where statisticalForm.ApplicationId == id
                                                                 && statisticalForm.RecordType == nameof(RecordTypesEnum.Register)
                                                                 && statisticalForm.IsActive
                                                           select 1).Any();

                        // Трябва да има RecordType = Application запис за заявлението, но НЕ и RecordType = Register запис все още
                        canTransition = hasRecordTypeApplicationEntry && !hasRecordTypeRegisterEntry;
                    }
                    break;
                default: throw new ArgumentException($"Unhandled case for auto creating administrative act for short paper application process for applicationId: {id} with page code: {applPageCode}");
            }

            return canTransition;
        }

        public override ApplicationStatusesEnum Action(int id, string statusReason = "")
        {
            ApplicationStatusesEnum newStatus;
            PageCodeEnum applPageCode = GetApplicationPageCode(id);

            using (TransactionScope scope = db.CreateTransaction())
            {
                switch (applPageCode)
                {
                    case PageCodeEnum.StatFormAquaFarm:
                        {
                            IStatisticalFormsService statisticalFormsService = serviceProvider.GetService<IStatisticalFormsService>();
                            StatisticalFormAquaFarmEditDTO dto = statisticalFormsService.GetApplicationAquaFarmDataForRegister(id);
                            statisticalFormsService.AddStatisticalFormAquaFarm(dto);
                        }
                        break;
                    case PageCodeEnum.StatFormFishVessel:
                        {
                            IStatisticalFormsService statisticalFormsService = serviceProvider.GetService<IStatisticalFormsService>();
                            StatisticalFormFishVesselEditDTO dto = statisticalFormsService.GetApplicationFishVesselDataForRegister(id);
                            statisticalFormsService.AddStatisticalFormFishVessel(dto);
                        }
                        break;
                    case PageCodeEnum.StatFormRework:
                        {
                            IStatisticalFormsService statisticalFormsService = serviceProvider.GetService<IStatisticalFormsService>();
                            StatisticalFormReworkEditDTO dto = statisticalFormsService.GetApplicationReworkDataForRegister(id);
                            statisticalFormsService.AddStatisticalFormRework(dto);
                        }
                        break;
                    default: throw new ArgumentException($"Unhandled case for auto creating administrative act for short paper application process for applicationId: {id} with page code: {applPageCode}");
                }

                newStatus = base.Action(id, statusReason);

                scope.Complete();
            }

            return newStatus;
        }
    }
}

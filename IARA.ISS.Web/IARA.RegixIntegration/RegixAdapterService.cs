using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Common;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.RegixAbstractions.Enums;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using IARA.RegixIntegration.CheckServices;
using TL.RegiXClient.Base;
using TL.RegiXClient.Extended.Models.ActualState;
using TL.RegiXClient.Extended.Models.ForeignPersonIdentity;
using TL.RegiXClient.Extended.Models.NelkEisme;
using TL.RegiXClient.Extended.Models.PermanentAddressSearch;
using TL.RegiXClient.Extended.Models.PersonalIdentity;
using TL.RegiXClient.Extended.Models.PersonDataSearch;
using TL.RegiXClient.Extended.Models.RelationsSearch;

namespace IARA.RegixIntegration
{
    public class RegixAdapterService : IRegixAdapterService
    {
        private IRegiXClientService regixClient;

        private BaseActualStateCheckService actualStateCheckService;
        private BaseForeignPersonCheckService foreignPersonCheckService;
        private BaseLastExpertDecisionCheckService expertDecisionCheckService;
        private BasePermanentAddressCheckService permanentAddressCheckService;
        private BasePersonDataCheckService personDataCheckService;
        private BasePersonIdentityCheckService personIdentityCheckService;
        private BaseRelationsCheckService relationsCheckService;
        private BaseVesselCheckService vesselCheckService;
        private bool disposedValue;

        public RegixAdapterService(Dictionary<RegixCheckTypes, object> checkServices, IRegiXClientService regixClient)
        {
            this.regixClient = regixClient;
            this.expertDecisionCheckService = checkServices[RegixCheckTypes.LastExpertDecision] as BaseLastExpertDecisionCheckService;
            this.actualStateCheckService = checkServices[RegixCheckTypes.ActualState] as BaseActualStateCheckService;
            this.foreignPersonCheckService = checkServices[RegixCheckTypes.ForeignPerson] as BaseForeignPersonCheckService;
            this.personIdentityCheckService = checkServices[RegixCheckTypes.PersonIdentity] as BasePersonIdentityCheckService;
            this.relationsCheckService = checkServices[RegixCheckTypes.Relations] as BaseRelationsCheckService;
            this.personDataCheckService = checkServices[RegixCheckTypes.PersonData] as BasePersonDataCheckService;
            this.permanentAddressCheckService = checkServices[RegixCheckTypes.PermanentAddress] as BasePermanentAddressCheckService;
            this.vesselCheckService = checkServices[RegixCheckTypes.Vessel] as BaseVesselCheckService;
        }

        public Task<bool> EnqueueVesselCheck(RegixContextData<VesselRequest, VesselContext> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null)
        {
            return vesselCheckService.Enqueue(request, priority, timeToDelay);
        }

        public void AddVesselCheck(RegixContextData<VesselRequest, VesselContext> request, IScopedServiceProvider scopedServiceProvider)
        {
            vesselCheckService.AddRegixCheck(request, scopedServiceProvider);
        }

        public Task<bool> EnqueueActualStateCheck(RegixContextData<ActualStateRequestType, RegixLegalContext> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null)
        {
            return actualStateCheckService.Enqueue(request, priority, timeToDelay);
        }

        public void AddActualStateCheck(RegixContextData<ActualStateRequestType, RegixLegalContext> request, IScopedServiceProvider scopedServiceProvider)
        {
            actualStateCheckService.AddRegixCheck(request, scopedServiceProvider);
        }

        public Task<bool> EnqueueExpertCheck(RegixContextData<GetLastExpertDecisionByIdentifierRequest, RecreationalFishingTelkDTO> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null)
        {
            return expertDecisionCheckService.Enqueue(request, priority, timeToDelay);
        }

        public void AddExpertCheck(RegixContextData<GetLastExpertDecisionByIdentifierRequest, RecreationalFishingTelkDTO> request, IScopedServiceProvider scopedServiceProvider)
        {
            expertDecisionCheckService.AddRegixCheck(request, scopedServiceProvider);
        }

        public Task<bool> EnqueueForeignPersonCheck(RegixContextData<ForeignIdentityInfoRequestType, RegixPersonContext> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null)
        {
            return foreignPersonCheckService.Enqueue(request, priority, timeToDelay);
        }

        public void AddForeignPersonCheck(RegixContextData<ForeignIdentityInfoRequestType, RegixPersonContext> request, IScopedServiceProvider scopedServiceProvider)
        {
            foreignPersonCheckService.AddRegixCheck(request, scopedServiceProvider);
        }

        public Task<bool> EnqueuePersonIdentityCheck(RegixContextData<PersonalIdentityInfoRequestType, RegixPersonContext> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null)
        {
            return personIdentityCheckService.Enqueue(request, priority, timeToDelay);
        }

        public void AddPersonIdentityCheck(RegixContextData<PersonalIdentityInfoRequestType, RegixPersonContext> request, IScopedServiceProvider scopedServiceProvider)
        {
            personIdentityCheckService.AddRegixCheck(request, scopedServiceProvider);
        }

        public Task<bool> EnqueuePersonDataCheck(RegixContextData<PersonDataRequestType, RegixPersonContext> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null)
        {
            return personDataCheckService.Enqueue(request, priority, timeToDelay);
        }

        public void AddPersonDataCheck(RegixContextData<PersonDataRequestType, RegixPersonContext> request, IScopedServiceProvider scopedServiceProvider)
        {
            personDataCheckService.AddRegixCheck(request, scopedServiceProvider);
        }

        public Task<bool> EnqueuePermanentAddressCheck(RegixContextData<PermanentAddressRequestType, RegixPersonContext> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null)
        {
            return permanentAddressCheckService.Enqueue(request, priority, timeToDelay);
        }

        public void AddPermanentAddressCheck(RegixContextData<PermanentAddressRequestType, RegixPersonContext> request, IScopedServiceProvider scopedServiceProvider)
        {
            permanentAddressCheckService.AddRegixCheck(request, scopedServiceProvider);
        }

        public Task<bool> EnqueueRelationsCheck(RegixContextData<RelationsRequestType, List<RegixPersonDataDTO>> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null)
        {
            return relationsCheckService.Enqueue(request, priority, timeToDelay);
        }

        public void AddRelationsChecks(RegixContextData<RelationsRequestType, List<RegixPersonDataDTO>> request, IScopedServiceProvider scopedServiceProvider)
        {
            relationsCheckService.AddRegixCheck(request, scopedServiceProvider);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    actualStateCheckService.Dispose();
                    expertDecisionCheckService.Dispose();
                    foreignPersonCheckService.Dispose();
                    personIdentityCheckService.Dispose();
                    personDataCheckService.Dispose();
                    relationsCheckService.Dispose();
                    regixClient.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

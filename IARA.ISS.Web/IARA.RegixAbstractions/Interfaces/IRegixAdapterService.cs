using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Common;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.RegixAbstractions.Models;
using TL.RegiXClient.Extended.Models.ActualState;
using TL.RegiXClient.Extended.Models.ForeignPersonIdentity;
using TL.RegiXClient.Extended.Models.NelkEisme;
using TL.RegiXClient.Extended.Models.PermanentAddressSearch;
using TL.RegiXClient.Extended.Models.PersonalIdentity;
using TL.RegiXClient.Extended.Models.PersonDataSearch;
using TL.RegiXClient.Extended.Models.RelationsSearch;

namespace IARA.RegixAbstractions.Interfaces
{
    public interface IRegixAdapterService : IDisposable
    {
        Task<bool> EnqueueActualStateCheck(RegixContextData<ActualStateRequestType, RegixLegalContext> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null);
        Task<bool> EnqueueExpertCheck(RegixContextData<GetLastExpertDecisionByIdentifierRequest, RecreationalFishingTelkDTO> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null);
        Task<bool> EnqueueForeignPersonCheck(RegixContextData<ForeignIdentityInfoRequestType, RegixPersonContext> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null);
        Task<bool> EnqueuePersonIdentityCheck(RegixContextData<PersonalIdentityInfoRequestType, RegixPersonContext> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null);
        Task<bool> EnqueuePersonDataCheck(RegixContextData<PersonDataRequestType, RegixPersonContext> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null);
        Task<bool> EnqueueRelationsCheck(RegixContextData<RelationsRequestType, List<RegixPersonDataDTO>> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null);
        Task<bool> EnqueuePermanentAddressCheck(RegixContextData<PermanentAddressRequestType, RegixPersonContext> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null);
        Task<bool> EnqueueVesselCheck(RegixContextData<VesselRequest, VesselContext> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null);
        void AddActualStateCheck(RegixContextData<ActualStateRequestType, RegixLegalContext> request, IScopedServiceProvider scopedServiceProvider);
        void AddExpertCheck(RegixContextData<GetLastExpertDecisionByIdentifierRequest, RecreationalFishingTelkDTO> request, IScopedServiceProvider scopedServiceProvider);
        void AddForeignPersonCheck(RegixContextData<ForeignIdentityInfoRequestType, RegixPersonContext> request, IScopedServiceProvider scopedServiceProvider);
        void AddPersonIdentityCheck(RegixContextData<PersonalIdentityInfoRequestType, RegixPersonContext> request, IScopedServiceProvider scopedServiceProvider);
        void AddPermanentAddressCheck(RegixContextData<PermanentAddressRequestType, RegixPersonContext> request, IScopedServiceProvider scopedServiceProvider);
        void AddRelationsChecks(RegixContextData<RelationsRequestType, List<RegixPersonDataDTO>> request, IScopedServiceProvider scopedServiceProvider);
        void AddPersonDataCheck(RegixContextData<PersonDataRequestType, RegixPersonContext> request, IScopedServiceProvider scopedServiceProvider);
        void AddVesselCheck(RegixContextData<VesselRequest, VesselContext> request, IScopedServiceProvider scopedServiceProvider);
    }
}

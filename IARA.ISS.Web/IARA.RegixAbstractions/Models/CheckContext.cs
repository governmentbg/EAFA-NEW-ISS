using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.RegixAbstractions.Interfaces;
using TL.RegiXClient.Extended.Models.ActualState;
using TL.RegiXClient.Extended.Models.ForeignPersonIdentity;
using TL.RegiXClient.Extended.Models.NelkEisme;
using TL.RegiXClient.Extended.Models.PermanentAddressSearch;
using TL.RegiXClient.Extended.Models.PersonalIdentity;
using TL.RegiXClient.Extended.Models.PersonDataSearch;

namespace IARA.RegixAbstractions.Models
{
    public class CheckContext : IDisposable
    {
        public CheckContext(int applicationId,
                            int applicationHistoryId,
                            string serviceType,
                            string serviceUri,
                            string egn,
                            string employeeNames,
                            string employeeIdentifier,
                            IScopedServiceProvider scopedServiceProvider,
                            IRegixAdapterService regixAdapterService,
                            int queueCapacities = 10)
        {
            this.ServiceType = serviceType;
            this.ServiceURI = serviceUri;
            this.EGN = egn;
            this.EmployeeNames = employeeNames;
            this.EmployeeIdentifier = employeeIdentifier;

            this.ApplicationId = applicationId;
            this.ApplicationHistoryId = applicationHistoryId;
            this.regixAdapterService = regixAdapterService;
            this.ScopedServiceProvider = scopedServiceProvider;

            this.personDataChecks = new Queue<RegixContextData<PersonDataRequestType, RegixPersonContext>>(queueCapacities);
            this.actualStateChecks = new Queue<RegixContextData<ActualStateRequestType, RegixLegalContext>>(queueCapacities);

            this.foreignPersonChecks = new Queue<RegixContextData<ForeignIdentityInfoRequestType, RegixPersonContext>>(queueCapacities);
            this.permanentAddressChecks = new Queue<RegixContextData<PermanentAddressRequestType, RegixPersonContext>>(queueCapacities);
            this.personIdentityChecks = new Queue<RegixContextData<PersonalIdentityInfoRequestType, RegixPersonContext>>(queueCapacities);
            this.lastExpertDecisions = new Queue<RegixContextData<GetLastExpertDecisionByIdentifierRequest, RecreationalFishingTelkDTO>>(queueCapacities);
            this.vesselChecks = new Queue<RegixContextData<VesselRequest, VesselContext>>(queueCapacities);
        }

        public string ServiceType { get; set; }
        public string ServiceURI { get; set; }

        public string EmployeeIdentifier { get; set; }
        public string EGN { get; set; }
        public string EmployeeNames { get; set; }
        public int ApplicationId { get; set; }
        public int ApplicationHistoryId { get; set; }

        public IScopedServiceProvider ScopedServiceProvider { get; private set; }
        private IRegixAdapterService regixAdapterService;
        private Queue<RegixContextData<PersonalIdentityInfoRequestType, RegixPersonContext>> personIdentityChecks;
        private Queue<RegixContextData<PersonDataRequestType, RegixPersonContext>> personDataChecks;
        private Queue<RegixContextData<PermanentAddressRequestType, RegixPersonContext>> permanentAddressChecks;
        private Queue<RegixContextData<ForeignIdentityInfoRequestType, RegixPersonContext>> foreignPersonChecks;
        private Queue<RegixContextData<ActualStateRequestType, RegixLegalContext>> actualStateChecks;
        private Queue<RegixContextData<GetLastExpertDecisionByIdentifierRequest, RecreationalFishingTelkDTO>> lastExpertDecisions;
        private Queue<RegixContextData<VesselRequest, VesselContext>> vesselChecks;


        private bool disposedValue;

        public void AddVesselCheck(RegixContextData<VesselRequest, VesselContext> request)
        {
            this.vesselChecks.Enqueue(request);
            this.regixAdapterService.AddVesselCheck(request, ScopedServiceProvider);
        }

        public void AddPersonDataCheck(RegixContextData<PersonDataRequestType, RegixPersonContext> request)
        {
            this.personDataChecks.Enqueue(request);
            this.regixAdapterService.AddPersonDataCheck(request, ScopedServiceProvider);
        }

        public void AddPersonIdentityCheck(RegixContextData<PersonalIdentityInfoRequestType, RegixPersonContext> request)
        {
            this.personIdentityChecks.Enqueue(request);
            this.regixAdapterService.AddPersonIdentityCheck(request, ScopedServiceProvider);
        }

        public void AddPersonPermanentAddressCheck(RegixContextData<PermanentAddressRequestType, RegixPersonContext> request)
        {
            this.permanentAddressChecks.Enqueue(request);
            this.regixAdapterService.AddPermanentAddressCheck(request, ScopedServiceProvider);
        }

        public void AddForeignPersonCheck(RegixContextData<ForeignIdentityInfoRequestType, RegixPersonContext> request)
        {
            this.foreignPersonChecks.Enqueue(request);
            this.regixAdapterService.AddForeignPersonCheck(request, ScopedServiceProvider);
        }

        public void AddActualStateCheck(RegixContextData<ActualStateRequestType, RegixLegalContext> request)
        {
            this.actualStateChecks.Enqueue(request);
            this.regixAdapterService.AddActualStateCheck(request, ScopedServiceProvider);
        }

        public void AddLastExpertDecisionCheck(RegixContextData<GetLastExpertDecisionByIdentifierRequest, RecreationalFishingTelkDTO> request)
        {
            this.lastExpertDecisions.Enqueue(request);
            this.regixAdapterService.AddExpertCheck(request, ScopedServiceProvider);
        }

        public Task<bool> EnqueueAndFlushAll()
        {
            List<Task<bool>> enqueueTasks = new List<Task<bool>>();

            //LEGAL CHECKS
            while (this.actualStateChecks.Count > 0)
            {
                var request = this.actualStateChecks.Dequeue();
                var task = this.regixAdapterService.EnqueueActualStateCheck(request);
                enqueueTasks.Add(task);
            }

            //PERMANENT ADDRESS CHECKS
            while (this.permanentAddressChecks.Count > 0)
            {
                var request = this.permanentAddressChecks.Dequeue();
                var task = this.regixAdapterService.EnqueuePermanentAddressCheck(request);
                enqueueTasks.Add(task);
            }

            //PERSON DATA CHECKS
            while (this.personDataChecks.Count > 0)
            {
                var request = this.personDataChecks.Dequeue();
                var task = this.regixAdapterService.EnqueuePersonDataCheck(request);
                enqueueTasks.Add(task);
            }

            //PERSON IDENTITY CHECKS
            while (this.personIdentityChecks.Count > 0)
            {
                var request = this.personIdentityChecks.Dequeue();
                var task = this.regixAdapterService.EnqueuePersonIdentityCheck(request);
                enqueueTasks.Add(task);
            }

            //LAST EXPERT CHECKS
            while (this.lastExpertDecisions.Count > 0)
            {
                var request = this.lastExpertDecisions.Dequeue();
                var task = this.regixAdapterService.EnqueueExpertCheck(request);
                enqueueTasks.Add(task);
            }

            //FOREIGN PERSON CHECKS
            while (this.foreignPersonChecks.Count > 0)
            {
                var request = this.foreignPersonChecks.Dequeue();
                var task = this.regixAdapterService.EnqueueForeignPersonCheck(request);
                enqueueTasks.Add(task);
            }

            //VESSEL CHECKS
            while (this.vesselChecks.Count > 0)
            {
                var request = this.vesselChecks.Dequeue();
                var task = this.regixAdapterService.EnqueueVesselCheck(request);
                enqueueTasks.Add(task);
            }

            Task.WaitAll(enqueueTasks.ToArray());

            return Task.FromResult(enqueueTasks.All(x => x.Result));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.regixAdapterService = null;
                    this.ScopedServiceProvider?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

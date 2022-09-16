using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.FSM;
using IARA.Infrastructure.FSM.Attributes;
using IARA.Infrastructure.FSM.Enums;
using IARA.Infrastructure.FSM.Models;
using IARA.Interfaces.FSM;

namespace IARA.Infrastructure.FSM
{
    public class ApplicationStateMachine : IApplicationStateMachine
    {
        private static Dictionary<ApplicationHierarchyTypesEnum, Dictionary<string, List<TransitionModel>>> transitions;
        private IARADbContext db;
        private IServiceProvider scopedServiceProvider;
        private bool disposedValue;

        public ApplicationStateMachine(IARADbContext db, IServiceProvider scopedServiceProvider)
        {
            this.db = db;
            this.scopedServiceProvider = scopedServiceProvider;
        }

        public State CurrentState { get; private set; }

        private Dictionary<ApplicationHierarchyTypesEnum, Dictionary<string, List<TransitionModel>>> Transitions
        {
            get
            {
                if (transitions == null)
                {
                    transitions = GetTransitionDTOs();
                }

                return transitions;
            }
        }

        public ApplicationStatusesEnum Act(int id, ApplicationStatusesEnum? toState = null, string statusReason = null)
        {
            this.GetCurrentState(id);
            return this.CurrentState.Next(id, toState, statusReason);
        }

        public ApplicationStatusesEnum Act(int id, FileInfoDTO uploadedFile, ApplicationStatusesEnum? toState = null, string statusReason = null)
        {
            this.GetCurrentState(id);
            return this.CurrentState.Next(id, uploadedFile, toState, statusReason); // 1
        }

        public ApplicationStatusesEnum Act(int id, EventisDataDTO eventisData, ApplicationStatusesEnum? toState = null, string statusReason = null)
        {
            this.GetCurrentState(id);
            return this.CurrentState.Next(id, eventisData, toState, statusReason);
        }

        public ApplicationStatusesEnum Act(int id,
                                           PaymentDataDTO paymentData,
                                           ApplicationStatusesEnum? toState = null,
                                           string statusReason = null)
        {
            this.GetCurrentState(id);
            return this.CurrentState.Next(id, paymentData, toState, statusReason);
        }

        public ApplicationStatusesEnum Act(int id, string draftContent, List<FileInfoDTO> files, ApplicationStatusesEnum? toState = null, string statusReason = null)
        {
            this.GetCurrentState(id);
            return this.CurrentState.Next(id, draftContent, files, toState, statusReason);
        }

        private State GetCurrentState(int id)
        {
            string value = (from appl in db.Applications
                            join statusHierarchy in db.NapplicationStatusHierarchyTypes on appl.ApplicationStatusHierTypeId equals statusHierarchy.Id
                            where appl.Id == id
                            select statusHierarchy.Code).Single();

            ApplicationHierarchyTypesEnum hierarchyType = Enum.Parse<ApplicationHierarchyTypesEnum>(value);

            string dbStatusCode = (from appl in db.Applications
                                   join applStatus in db.NapplicationStatuses on appl.ApplicationStatusId equals applStatus.Id
                                   where appl.Id == id
                                   select applStatus.Code).Single();

            List<TransitionModel> transitionDTOs = Transitions[hierarchyType][dbStatusCode];

            List<Transition> transitions = new List<Transition>();

            foreach (TransitionModel transitionModel in transitionDTOs)
            {
                ConstructorInfo constructorInfo = transitionModel.TransitionType.GetConstructor(new Type[] { typeof(TransitionContext) });
                var context = new TransitionContext
                {
                    Db = db,
                    ToState = transitionModel.ToState,
                    ServiceProvider = this.scopedServiceProvider,
                    StateMachine = this
                };

                Transition transition = constructorInfo.Invoke(new object[] { context }) as Transition;

                transitions.Add(transition);
            }

            this.CurrentState = new State(dbStatusCode, db, transitions);
            return this.CurrentState;
        }

        private List<TransitionModel> GetApplicationStatusHierarchy()
        {
            DateTime now = DateTime.Now;
            var result = from statusHierarchy in db.NapplicationStatusHierarchies
                         join childStatus in db.NapplicationStatuses on statusHierarchy.ChildStatusId equals childStatus.Id
                         join parentStatus in db.NapplicationStatuses on statusHierarchy.ParentStatusId equals parentStatus.Id
                         join hierarchyType in db.NapplicationStatusHierarchyTypes on statusHierarchy.ApplicationStatusHierTypeId equals hierarchyType.Id
                         where statusHierarchy.ValidFrom <= now && statusHierarchy.ValidTo > now
                               && childStatus.ValidFrom <= now && childStatus.ValidTo > now
                               && parentStatus.ValidFrom <= now && parentStatus.ValidTo > now
                               && hierarchyType.ValidFrom <= now && hierarchyType.ValidTo > now
                         select new TransitionModel
                         {
                             FromState = parentStatus.Code,
                             ToState = childStatus.Code,
                             IsManual = statusHierarchy.IsManual,
                             Code = Enum.Parse<TransitionCodesEnum>(statusHierarchy.ExecutionCodeIdentifier),
                             HierarchyType = Enum.Parse<ApplicationHierarchyTypesEnum>(hierarchyType.Code)
                         };

            return result.ToList();
        }

        private Dictionary<ApplicationHierarchyTypesEnum, Dictionary<string, List<TransitionModel>>> GetTransitionDTOs()
        {
            List<TransitionModel> applicationStatusHierarchy = GetApplicationStatusHierarchy();
            List<Type> transitionTypes = typeof(ApplicationStateMachine).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(Transition))).ToList();

            foreach (var type in transitionTypes)
            {
                var connectionAttr = type.GetCustomAttributes(typeof(ConnectionAttribute), false).Cast<ConnectionAttribute>().First();

                IEnumerable<TransitionModel> transitions = applicationStatusHierarchy.Where(x => x.Code == connectionAttr.TransitionCode);
                foreach (var transition in transitions)
                {
                    transition.TransitionType = type;
                }
            }

            return (from statusHierarchy in applicationStatusHierarchy
                    group statusHierarchy by statusHierarchy.HierarchyType into grouped
                    select new
                    {
                        Key = grouped.Key,
                        Transitions = grouped.ToList()
                    }).ToDictionary(s => s.Key, s => (from transition in s.Transitions
                                                      group transition by transition.FromState into groupedTransitions
                                                      select new
                                                      {
                                                          Key = groupedTransitions.Key,
                                                          Transitions = groupedTransitions.ToList()
                                                      }).ToDictionary(t => t.Key, t => t.Transitions));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    db?.Dispose();
                    //scopedServiceProvider?.Dispose();
                }

                transitions = null;

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

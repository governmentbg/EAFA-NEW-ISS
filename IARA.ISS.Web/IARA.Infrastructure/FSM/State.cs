using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.Infrastructure.FSM
{
    public class State
    {
        public ApplicationStatusesEnum Code { get; private set; }

        private List<Transition> transitions;
        private IARADbContext db;

        internal State(string name, IARADbContext db, List<Transition> transitions)
        {
            this.db = db;
            this.Code = Enum.Parse<ApplicationStatusesEnum>(name);
            this.transitions = transitions;
        }

        public ApplicationStatusesEnum Next(int id, ApplicationStatusesEnum? toState = null, string statusReason = null)
        {
            List<Transition> toStateTransitions = this.GetToStateTransitions(toState);
            bool isTriggeredManually = toState != null;

            foreach (var transition in toStateTransitions)
            {
                if (transition.CanTransition(id, isTriggeredManually, statusReason))
                {
                    transition.PreAction(id, this.Code);
                    db.SaveChanges();
                    this.Code = transition.Action(id, statusReason);
                    db.SaveChanges();
                    this.Code = transition.PostAction(id, this.Code);
                    db.SaveChanges();

                    return this.Code;
                }
            }

            throw new InvalidOperationException("Cannot transition to next state. Either there is no valid transition for toState, " +
                                                "or there is no valid transition at all");
        }

        public ApplicationStatusesEnum Next(int id,
                                            FileInfoDTO uploadedFile,
                                            ApplicationStatusesEnum? toState = null,
                                            string statusReason = null)
        {
            List<Transition> toStateTransitions = this.GetToStateTransitions(toState);

            foreach (var transition in toStateTransitions)
            {
                if (transition.CanTransition(id, uploadedFile))
                {
                    transition.PreAction(id, this.Code);
                    db.SaveChanges();
                    this.Code = transition.Action(id, uploadedFile, statusReason);
                    db.SaveChanges();
                    transition.PostAction(id, this.Code);
                    db.SaveChanges();

                    return this.Code;
                }
            }

            throw new InvalidOperationException("Cannot transition to next state. Either there is no valid transition for toState, " +
                                                "or there is no valid transition at all");
        }

        public ApplicationStatusesEnum Next(int id,
                                            EventisDataDTO eventisData,
                                            ApplicationStatusesEnum? toState = null,
                                            string statusReason = null)
        {
            List<Transition> toStateTransitions = this.GetToStateTransitions(toState);

            foreach (var transition in toStateTransitions)
            {
                if (transition.CanTransition(id, eventisData))
                {
                    transition.PreAction(id, this.Code);
                    db.SaveChanges();
                    this.Code = transition.Action(id, eventisData, statusReason);
                    db.SaveChanges();
                    transition.PostAction(id, this.Code);
                    db.SaveChanges();

                    return this.Code;
                }
            }

            throw new InvalidOperationException("Cannot transition to next state. Either there is no valid transition for toState, " +
                                                "or there is no valid transition at all");
        }

        public ApplicationStatusesEnum Next(int id,
                                            PaymentDataDTO paymentData,
                                            ApplicationStatusesEnum? toState = null,
                                            string statusReason = null)
        {
            List<Transition> toStateTransitions = this.GetToStateTransitions(toState);

            foreach (var transition in toStateTransitions)
            {
                if (transition.CanTransition(id, paymentData))
                {
                    transition.PreAction(id, this.Code);
                    db.SaveChanges();
                    this.Code = transition.Action(id, paymentData, statusReason);
                    db.SaveChanges();
                    transition.PostAction(id, this.Code);
                    db.SaveChanges();

                    return this.Code;
                }
            }

            throw new InvalidOperationException("Cannot transition to next state. Either there is no valid transition for toState, " +
                                                "or there is no valid transition at all");
        }

        public ApplicationStatusesEnum Next(int id,
                                            string draftContent,
                                            List<FileInfoDTO> files,
                                            ApplicationStatusesEnum? toState = null,
                                            string statusReason = null)
        {
            List<Transition> toStateTransitions = this.GetToStateTransitions(toState);
            bool isTriggeredManually = toState != null;

            foreach (var transition in toStateTransitions)
            {
                if (transition.CanTransition(id, isTriggeredManually, statusReason))
                {
                    transition.PreAction(id, this.Code);
                    db.SaveChanges();
                    this.Code = transition.Action(id, draftContent, files, statusReason);
                    db.SaveChanges();
                    transition.PostAction(id, this.Code);
                    db.SaveChanges();

                    return this.Code;
                }
            }

            throw new InvalidOperationException("Cannot transition to next state. Either there is no valid transition for toState, " +
                                                "or there is no valid transition at all");
        }

        private List<Transition> GetToStateTransitions(ApplicationStatusesEnum? toState = null)
        {
            IEnumerable<Transition> toStateTransitions = null;

            if (toState != null)
            {
                toStateTransitions = transitions.Where(x => x.NextStatus == toState);
            }
            else
            {
                toStateTransitions = transitions;
            }

            return toStateTransitions.ToList();
        }
    }
}

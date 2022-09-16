using System;
using IARA.Common.Enums;
using IARA.Infrastructure.FSM.Enums;

namespace IARA.DomainModels.DTOModels.FSM
{
    public class TransitionModel
    {
        public TransitionCodesEnum Code { get; set; }

        public string FromState { get; set; }

        public string ToState { get; set; }

        public bool IsManual { get; set; }

        public ApplicationHierarchyTypesEnum HierarchyType { get; set; }

        public Type TransitionType { get; set; }
    }
}

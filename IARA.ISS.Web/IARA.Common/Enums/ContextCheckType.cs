namespace IARA.Common.Enums
{
    public enum ContextCheckType : int
    {
        AgentPerson = 1 << 1,
        AgentAddress = AgentPerson | 1,

        ChangeLegal = 1 << 2,
        ChangePerson = 1 << 3,
        ChangePersonAddress = ChangePerson | 1,
        ChangeLegalAddress = ChangeLegal | 1,

        HolderLegal = 1 << 4,
        HolderPerson = 1 << 5,
        HolderPersonAddress = HolderPerson | 1,
        HolderLegalAddress = HolderLegal | 1,

        Legal = 1 << 6,
        LegalAuthorizedPerson = 1 << 7,

        LessorLegal = 1 << 8,
        LessorPerson = 1 << 9,
        LessorLegalAddress = LessorLegal | 1,
        LessorPersonAddress = LessorPerson | 1,

        OrganizerPerson = 1 << 10,
        OrganizerAddress = OrganizerPerson | 1,

        OwnerLegal = 1 << 11,
        OwnerPerson = 1 << 12,
        OwnerLegalAddress = OwnerLegal | 1,
        OwnerPersonAddress = OwnerPerson | 1,

        ReceiverPerson = 1 << 13,
        ReceiverAddress = ReceiverPerson | 1,

        RepresentativePerson = 1 << 14,
        RepresentativeAddress = RepresentativePerson | 1,

        RequesterPerson = 1 << 15,
        RequesterAddress = RequesterPerson | 1,

        Ship = 1 << 16,

        SubmittedByPerson = 1 << 17,
        SubmittedByAddress = SubmittedByPerson | 1,

        SubmittedForLegal = 1 << 18,
        SubmittedForPerson = 1 << 19,
        SubmittedForPersonAddress = SubmittedForPerson | 1,
        SubmittedForLegalAddress = SubmittedForLegal | 1,

        DisabledPerson = 1 << 20,
        Vessel = 1 << 21
    }
}

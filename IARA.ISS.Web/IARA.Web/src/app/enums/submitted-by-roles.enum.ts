export enum SubmittedByRolesEnum {
    Personal = 1 << 0,
    PersonalRepresentative = 1 << 1,
    LegalOwner = 1 << 2,
    LegalRepresentative = 1 << 3,

    PersonalRole = Personal | PersonalRepresentative,
    LegalRole = LegalOwner | LegalRepresentative,
    RepresentativeRole = PersonalRepresentative | LegalRepresentative
}

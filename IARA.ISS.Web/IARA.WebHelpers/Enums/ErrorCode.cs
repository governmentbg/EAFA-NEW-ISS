namespace IARA.WebHelpers.Enums
{
    public enum ErrorCode
    {
        InvalidEmail = 0,
        InvalidEgnLnch = 1,
        WrongPassword = 2,
        InvalidSqlQuery = 3,
        AlreadySubmitted = 4,
        NoEDeliveryRegistration = 5,
        BuyerDoesNotExist = 6,
        InvalidLogBookPagesRange = 7,
        PageNumberNotInLogbook = 8,
        LogBookPageAlreadySubmitted = 9,
        InvalidData = 10,
        InspectorAlreadyExists = 11,
        InvalidStateMachineTransitionOperation = 12,
        QualifiedFisherAlreadyExists = 13,
        InvalidOriginDeclarationNumber = 14,
        InvalidTransportationDocNumber = 15,
        InvalidAdmissionDocNumber = 16,
        LogBookNotFound = 17,
        PageNumberNotInLogBookLicense = 18,
        InvalidLogBookLicensePagesRange = 19,
        NoMaximumFishingCapacityToDate = 20,
        PermitDoesNotExist = 21,
        PermitLicenceDoesNotExist = 22,
        QualifiedFisherDoesNotExist = 23,
        NotInspector = 24,
        NoPermitRegisterForPermitLicense = 25
    }
}

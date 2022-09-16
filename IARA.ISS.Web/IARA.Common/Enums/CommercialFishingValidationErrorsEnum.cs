namespace IARA.Common.Enums
{
    public enum CommercialFishingValidationErrorsEnum
    {
        PermitSubmittedForNotShipOwner,
        CaptainNotQualifiedFisherCheck,
        NoEDeliveryRegistration,
        InvalidPermitRegistrationNumber,

        ShipIsThirdCountry,
        ShipIsNotThirdCountry,

        ShipAlreadyHasValidBlackSeaPermit,
        ShipAlreadyHasValidDanubePermit,

        ShipHasNoValidBlackSeaPermit,
        ShipHasNoValidDanubePermit,

        ShipAlreadyHasValidPoundNetPermit,
        ShipHasNoPoundNetPermit
    }
}

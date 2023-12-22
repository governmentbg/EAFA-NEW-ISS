export enum CommercialFishingValidationErrorsEnum {
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

    ShipHasNoPoundNetPermit,

    InvalidPermitLisenseRegistrationNumber
}
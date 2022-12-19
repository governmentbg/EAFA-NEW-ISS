export class ErrorModel {
    public id: number = 0;
    public messages: string[] = [];
    public type: ErrorType = ErrorType.Unhandled;
    public code: ErrorCode | undefined;

    public constructor(obj?: Partial<ErrorModel>) {
        Object.assign(this, obj);
    }
}

export enum ErrorType {
    Unhandled = 0,
    Handled = 1,
    Validation = 2
}

export enum ErrorCode {
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
    NoPermitRegisterForPermitLicense = 25,
    DuplicatedMarksNumbers = 26,
    DuplicatedPingersNumbers = 27,
    ShipEventExistsOnSameDate = 28,
    UsernameExists = 29,
    EmailExists = 30,
    SendFLUXSalesFailed = 31,
    AuanNumAlreadyExists = 32,
    LogBookHasSubmittedPages = 33,
    ApplicationFileInvalid = 34,
    InvalidPermitNumber = 35,
    PrintConfigurationAlreadyExists = 36,
    LogBookPageAlreadySubmittedOtherLogBook = 37
}
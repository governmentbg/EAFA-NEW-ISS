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
    LogBookPageAlreadySubmittedOtherLogBook = 37,
    MaxNumberMissingPagesExceeded = 38,
    MoreThanOneActiveOnlineLogBook = 39,
    CannotDeletePermitWithLicense = 40,
    CannotDeleteLicenseWithLogBooks = 41,
    PermitSuspensionValidToExists = 42,
    PermitLicenseSuspensionValidToExists = 43,
    CannotAddEditPageForShipUnder10M = 44,
    CannotAddEditPageForShip10M12M = 45,
    CannotAddEditPageForShipOver12M = 46,
    CannotAddEditFirstSalePageAboveLimitTurnover = 47,
    CannotAddEditFirstSalePageBelowLimitTurnover = 48,
    CannotAddEditLockedAdmissionPage = 49,
    CannotAddEditLockedAquaculturePage = 50,
    ReportCodeAlreadyExists = 51,
    PoundNetNumAlreadyExists = 52,
    PatrolVehicleAlreadyExists = 53,
    InspectionReportNumAlreadyExists = 54,
    InvalidPermitLicenseNumber = 55,
    InvalidPassword = 56,
    OldPasswordFound = 57,
    MoreThanOneActiveShipLogBook = 58,
    CannotCancelAuanWithPenalDecrees = 59,
    MissingPageWithOldNumber = 60,
    CannotDeleteInspectionWithAuans = 61,
    CannotDeleteAuanWithDecrees = 62,
    CannotDeleteDecreeWithPenalPoints = 63,
    InvalidInspectionType = 64,
    FishingAssociationAlreadyExists = 65,
    SendFLUXISRFailed = 66,
    InspectionNotSigned = 67,
    PenalDecreeNumAlreadyExists = 68,
    ReportGroupNameAlreadyExists = 69
}
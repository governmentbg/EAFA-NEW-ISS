export enum LoginResultTypes {
    Fail = 0,
    Success = 1,
    Locked = 2,
    EmailNotConfirmed = 4,
    Blocked = 8,
    OtherSessionExists = 16,
    ForbiddenIP = 32,
    LoginTypeForbidden = 64,
    UserMissingInDB = 128,
    PasswordExpired = 256,
}
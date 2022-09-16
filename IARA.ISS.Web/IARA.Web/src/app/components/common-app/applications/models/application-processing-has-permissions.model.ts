export class ApplicationProcessingHasPermissions {
    public canAddRecords: boolean = false;
    public canEditRecords: boolean = false;
    public canDeleteRecords: boolean = false;
    public canRestoreRecords: boolean = false;
    public canEnterEventisNumber: boolean = false;
    public canCancelRecords: boolean = false;
    public canInspectAndCorrectRecords: boolean = false;
    public canProcessPaymentData: boolean = false;
    public canConfirmDataRegularity: boolean = false;
    public canAddAdministrativeActRecords: boolean = false;
    public canDownloadOnlineApplications: boolean = false;
    public canUploadOnlineApplications: boolean = false;
    public canViewAdministrativeActRecords: boolean = false;

    constructor(obj?: Partial<ApplicationProcessingHasPermissions>) {
        Object.assign(this, obj);
    }
}
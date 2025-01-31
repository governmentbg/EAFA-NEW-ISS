export class PagesPermissions {
    public canEditAdmissionLogBookRecords: boolean = false;
    public canEditTransportationLogBookRecords: boolean = false;
    public canEditFirstSaleLogBookRecords: boolean = false;
    public canCancelShipLogBookRecords: boolean = false;
    public canAddAdmissionLogBookRecords: boolean = false;
    public canAddTransportationLogBookRecords: boolean = false;
    public canAddFirstSaleLogBookRecords: boolean = false;
    public canEditShipLogBookRecords: boolean = false;
    public canEditNumberShipLogBookRecords: boolean = false;
    public canAddEditFilesShipLogBookRecords: boolean = false;
    public canEditOnlineShipLogBookRecords: boolean = false;

    public constructor(obj?: Partial<PagesPermissions>) {
        Object.assign(this, obj);
    }
}
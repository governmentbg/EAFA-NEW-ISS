import { PermitLicenseTableModel } from '../../../models/permit-license-table.model';

export class PermitLicensesTableParams {
    public readOnly: boolean = false;
    public isEdit: boolean = false;
    public model: PermitLicenseTableModel | undefined;

    public constructor(params?: Partial<PermitLicensesTableParams>) {
        Object.assign(this, params);
    }
}
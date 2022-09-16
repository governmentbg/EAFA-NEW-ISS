import { InspectionPermitDTO } from '@app/models/generated/dtos/InspectionPermitDTO';

export class PermitLicenseTableModel extends InspectionPermitDTO {
    public institution: string | undefined;
    public isCurrentUser: boolean = false;

    public constructor(params?: Partial<PermitLicenseTableModel>) {
        super(params);
        Object.assign(this, params);
    }
}
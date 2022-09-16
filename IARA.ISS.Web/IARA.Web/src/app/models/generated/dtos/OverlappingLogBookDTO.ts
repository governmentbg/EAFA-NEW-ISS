

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class OverlappingLogBookDTO { 
    public constructor(obj?: Partial<OverlappingLogBookDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public number?: string;

    @StrictlyTyped(Number)
    public statusId?: number;

    @StrictlyTyped(Date)
    public issueDate?: Date;

    @StrictlyTyped(Date)
    public finishDate?: Date;

    @StrictlyTyped(Number)
    public startPage?: number;

    @StrictlyTyped(Number)
    public endPage?: number;

    @StrictlyTyped(String)
    public ownerName?: string;

    @StrictlyTyped(Number)
    public logBookPermitLicenseId?: number;

    @StrictlyTyped(Number)
    public logBookPermitLicenseStartPage?: number;

    @StrictlyTyped(Number)
    public logBookPermitLicenseEndPage?: number;

    @StrictlyTyped(Date)
    public logBookPermitLicenseValidFrom?: Date;

    @StrictlyTyped(Date)
    public logBookPermitLicenseValidTo?: Date;

    @StrictlyTyped(String)
    public logBookPermitLicenseNumber?: string;
}
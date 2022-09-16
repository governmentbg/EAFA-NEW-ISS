

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class CapacityCertificateHistoryApplDTO { 
    public constructor(obj?: Partial<CapacityCertificateHistoryApplDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Date)
    public applicationDate?: Date;

    @StrictlyTyped(String)
    public reason?: string;

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(String)
    public shipName?: string;

    @StrictlyTyped(String)
    public transferredCapacityCertificate?: string;

    @StrictlyTyped(Number)
    public duplicateCapacityCertificateId?: number;

    @StrictlyTyped(String)
    public duplicateCapacityCertificate?: string;
}
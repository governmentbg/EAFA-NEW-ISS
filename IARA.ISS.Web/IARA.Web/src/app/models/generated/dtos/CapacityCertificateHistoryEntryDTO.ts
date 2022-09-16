

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class CapacityCertificateHistoryEntryDTO { 
    public constructor(obj?: Partial<CapacityCertificateHistoryEntryDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Date)
    public dateOfEvent?: Date;

    @StrictlyTyped(String)
    public reason?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
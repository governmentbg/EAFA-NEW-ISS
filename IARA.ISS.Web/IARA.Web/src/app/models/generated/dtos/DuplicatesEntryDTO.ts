

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class DuplicatesEntryDTO { 
    public constructor(obj?: Partial<DuplicatesEntryDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Date)
    public date?: Date;

    @StrictlyTyped(String)
    public submittedBy?: string;

    @StrictlyTyped(Number)
    public price?: number;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(Number)
    public deliveryId?: number;
}
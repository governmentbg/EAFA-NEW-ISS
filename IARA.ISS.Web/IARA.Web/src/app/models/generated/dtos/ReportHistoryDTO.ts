

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class ReportHistoryDTO { 
    public constructor(obj?: Partial<ReportHistoryDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public egn?: string;

    @StrictlyTyped(String)
    public documentsName?: string;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public validTo?: Date;

    @StrictlyTyped(Boolean)
    public isPerson?: boolean;
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FishingActivityReportItemDTO { 
    public constructor(obj?: Partial<FishingActivityReportItemDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public requestId?: number;

    @StrictlyTyped(String)
    public uuid?: string;

    @StrictlyTyped(String)
    public purpose?: string;

    @StrictlyTyped(String)
    public reportType?: string;

    @StrictlyTyped(String)
    public faType?: string;

    @StrictlyTyped(Date)
    public date?: Date;

    @StrictlyTyped(String)
    public status?: string;

    @StrictlyTyped(String)
    public errorMessage?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
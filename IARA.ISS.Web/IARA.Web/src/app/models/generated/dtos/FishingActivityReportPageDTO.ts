

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FishingActivityReportPageDTO { 
    public constructor(obj?: Partial<FishingActivityReportPageDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public reportId?: number;

    @StrictlyTyped(String)
    public pageNumber?: string;

    @StrictlyTyped(String)
    public pageStatus?: string;

    @StrictlyTyped(String)
    public gearName?: string;

    @StrictlyTyped(String)
    public unloadPort?: string;

    @StrictlyTyped(String)
    public unloadedFish?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
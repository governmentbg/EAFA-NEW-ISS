

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class AuanInspectionDTO { 
    public constructor(obj?: Partial<AuanInspectionDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public userId?: number;

    @StrictlyTyped(Date)
    public startDate?: Date;

    @StrictlyTyped(Number)
    public territoryUnitId?: number;
}
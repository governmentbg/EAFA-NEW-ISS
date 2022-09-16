
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class RecreationalFishingReportRequestDTO {
    public constructor(obj?: Partial<RecreationalFishingReportRequestDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public territoryUnitId?: number;

    @StrictlyTyped(Number)
    public associationId?: number;

    @StrictlyTyped(Date)
    public dateFrom?: Date;

    @StrictlyTyped(Date)
    public dateTo?: Date;
}
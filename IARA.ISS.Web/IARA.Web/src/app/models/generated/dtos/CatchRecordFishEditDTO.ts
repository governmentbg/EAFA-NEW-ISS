
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class CatchRecordFishEditDTO {
    public constructor(obj?: Partial<CatchRecordFishEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public catchRecordId?: number;

    @StrictlyTyped(Number)
    public fishTypeId?: number;

    @StrictlyTyped(Number)
    public count?: number;

    @StrictlyTyped(Number)
    public quantity?: number;
}
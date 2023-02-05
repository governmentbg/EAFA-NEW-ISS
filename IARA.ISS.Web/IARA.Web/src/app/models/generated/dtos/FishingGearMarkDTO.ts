

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FishingGearMarkStatusesEnum } from '@app/enums/fishing-gear-mark-statuses.enum';

export class FishingGearMarkDTO { 
    public constructor(obj?: Partial<FishingGearMarkDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public number?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(Number)
    public statusId?: number;

    @StrictlyTyped(Date)
    public createdOn?: Date;

    @StrictlyTyped(Number)
    public selectedStatus?: FishingGearMarkStatusesEnum;
}
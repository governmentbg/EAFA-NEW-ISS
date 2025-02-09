﻿

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FishingGearPingerStatusesEnum } from '@app/enums/fishing-gear-pinger-statuses.enum';

export class FishingGearPingerDTO { 
    public constructor(obj?: Partial<FishingGearPingerDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public number?: string;

    @StrictlyTyped(Number)
    public statusId?: number;

    @StrictlyTyped(Number)
    public selectedStatus?: FishingGearPingerStatusesEnum;

    @StrictlyTyped(String)
    public brand?: string;

    @StrictlyTyped(String)
    public model?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class ShipFishingCapacityHistoryDTO { 
    public constructor(obj?: Partial<ShipFishingCapacityHistoryDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(Number)
    public shipUID?: number;

    @StrictlyTyped(String)
    public shipCfr?: string;

    @StrictlyTyped(Number)
    public grossTonnage?: number;

    @StrictlyTyped(Number)
    public power?: number;

    @StrictlyTyped(Number)
    public grossTonnageChange?: number;

    @StrictlyTyped(Number)
    public powerChange?: number;

    @StrictlyTyped(Date)
    public dateFrom?: Date;

    @StrictlyTyped(Date)
    public dateTo?: Date;

    @StrictlyTyped(String)
    public reason?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
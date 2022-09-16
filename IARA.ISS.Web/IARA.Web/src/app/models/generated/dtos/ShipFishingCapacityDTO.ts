

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ShipFishingCapacityHistoryDTO } from './ShipFishingCapacityHistoryDTO';

export class ShipFishingCapacityDTO { 
    public constructor(obj?: Partial<ShipFishingCapacityDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(String)
    public shipCfr?: string;

    @StrictlyTyped(String)
    public shipName?: string;

    @StrictlyTyped(Number)
    public grossTonnage?: number;

    @StrictlyTyped(Number)
    public power?: number;

    @StrictlyTyped(Date)
    public dateOfChange?: Date;

    @StrictlyTyped(ShipFishingCapacityHistoryDTO)
    public history?: ShipFishingCapacityHistoryDTO[];

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
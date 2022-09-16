

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ShipRegisterEditDTO } from './ShipRegisterEditDTO';

export class ShipRegisterReduceCapacityDTO { 
    public constructor(obj?: Partial<ShipRegisterReduceCapacityDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(ShipRegisterEditDTO)
    public ships?: ShipRegisterEditDTO[];
}
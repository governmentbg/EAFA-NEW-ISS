

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ShipRegisterEditDTO } from './ShipRegisterEditDTO';

export class ShipRegisterDeregistrationDTO { 
    public constructor(obj?: Partial<ShipRegisterDeregistrationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(ShipRegisterEditDTO)
    public ships?: ShipRegisterEditDTO[];
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ShipRegisterEditDTO } from './ShipRegisterEditDTO';

export class ShipRegisterChangeOfCircumstancesDTO { 
    public constructor(obj?: Partial<ShipRegisterChangeOfCircumstancesDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(ShipRegisterEditDTO)
    public ships?: ShipRegisterEditDTO[];
}
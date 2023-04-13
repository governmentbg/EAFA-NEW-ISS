

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { StatisticalFormShipSeaDaysDTO } from './StatisticalFormShipSeaDaysDTO';
import { StatisticalFormsSeaDaysDTO } from './StatisticalFormsSeaDaysDTO';

export class StatisticalFormShipDTO { 
    public constructor(obj?: Partial<StatisticalFormShipDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(Number)
    public shipYears?: number;

    @StrictlyTyped(Number)
    public shipLenghtId?: number;

    @StrictlyTyped(Number)
    public grossTonnageId?: number;

    @StrictlyTyped(Boolean)
    public hasEngine?: boolean;

    @StrictlyTyped(Number)
    public fuelTypeId?: number;

    @StrictlyTyped(Number)
    public fuelConsumption?: number;

    @StrictlyTyped(StatisticalFormShipSeaDaysDTO)
    public shipSeaDays?: StatisticalFormShipSeaDaysDTO[];

    @StrictlyTyped(StatisticalFormsSeaDaysDTO)
    public fishingGearSeaDays?: StatisticalFormsSeaDaysDTO[];
}
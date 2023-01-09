

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

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
}
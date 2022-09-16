

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ShipRegisterCatchHistoryDTO { 
    public constructor(obj?: Partial<ShipRegisterCatchHistoryDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Date)
    public dateOfCatch?: Date;

    @StrictlyTyped(Number)
    public quantityKg?: number;

    @StrictlyTyped(String)
    public placeOfCatch?: string;

    @StrictlyTyped(String)
    public logbookPage?: string;
}
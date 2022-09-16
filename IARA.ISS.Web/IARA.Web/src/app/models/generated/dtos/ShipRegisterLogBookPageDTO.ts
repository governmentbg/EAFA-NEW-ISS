

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ShipRegisterLogBookPageDTO { 
    public constructor(obj?: Partial<ShipRegisterLogBookPageDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public logBookNum?: string;

    @StrictlyTyped(String)
    public pageNum?: string;

    @StrictlyTyped(String)
    public fishingGear?: string;

    @StrictlyTyped(Date)
    public outingStartDate?: Date;

    @StrictlyTyped(String)
    public portOfUnloading?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
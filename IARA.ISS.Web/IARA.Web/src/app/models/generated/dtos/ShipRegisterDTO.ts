

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ShipEventTypeEnum } from '@app/enums/ship-event-type.enum';

export class ShipRegisterDTO { 
    public constructor(obj?: Partial<ShipRegisterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public cfr?: string;

    @StrictlyTyped(String)
    public externalMark?: string;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public owners?: string;

    @StrictlyTyped(Date)
    public lastChangeDate?: Date;

    @StrictlyTyped(String)
    public lastChangeStatus?: string;

    @StrictlyTyped(Number)
    public eventType?: ShipEventTypeEnum;

    @StrictlyTyped(Boolean)
    public isThirdPartyShip?: boolean;
}
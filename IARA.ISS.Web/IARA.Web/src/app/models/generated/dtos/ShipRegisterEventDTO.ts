

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ShipRegisterEventDTO { 
    public constructor(obj?: Partial<ShipRegisterEventDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public no?: number;

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(String)
    public type?: string;

    @StrictlyTyped(Date)
    public date?: Date;

    @StrictlyTyped(Boolean)
    public usrRsr?: boolean;

    @StrictlyTyped(Boolean)
    public isTemporary?: boolean;
}
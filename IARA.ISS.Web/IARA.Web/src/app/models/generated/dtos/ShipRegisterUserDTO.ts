

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ShipRegisterUserDTO { 
    public constructor(obj?: Partial<ShipRegisterUserDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public egnLncEik?: string;

    @StrictlyTyped(String)
    public permitLicenceType?: string;

    @StrictlyTyped(String)
    public groundsForUse?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
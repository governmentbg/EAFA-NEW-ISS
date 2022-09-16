

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ShipRegisterUsedCertificateDTO { 
    public constructor(obj?: Partial<ShipRegisterUsedCertificateDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Date)
    public date?: Date;

    @StrictlyTyped(Number)
    public num?: number;

    @StrictlyTyped(Number)
    public enginePower?: number;

    @StrictlyTyped(Number)
    public grossTonnage?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
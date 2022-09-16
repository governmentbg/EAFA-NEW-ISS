

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class HolderGroundForUseDTO { 
    public constructor(obj?: Partial<HolderGroundForUseDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public typeId?: number;

    @StrictlyTyped(String)
    public number?: string;

    @StrictlyTyped(Date)
    public groundForUseValidFrom?: Date;

    @StrictlyTyped(Date)
    public groundForUseValidTo?: Date;

    @StrictlyTyped(Boolean)
    public isGroundForUseUnlimited?: boolean;
}
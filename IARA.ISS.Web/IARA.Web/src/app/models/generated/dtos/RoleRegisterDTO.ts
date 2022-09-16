

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class RoleRegisterDTO { 
    public constructor(obj?: Partial<RoleRegisterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public code?: string;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public validTo?: Date;

    @StrictlyTyped(Number)
    public usersCount?: number;

    @StrictlyTyped(Boolean)
    public hasInternalAccess?: boolean;

    @StrictlyTyped(Boolean)
    public hasPublicAccess?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
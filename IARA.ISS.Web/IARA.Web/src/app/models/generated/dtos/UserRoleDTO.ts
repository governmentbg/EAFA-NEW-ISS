

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class UserRoleDTO { 
    public constructor(obj?: Partial<UserRoleDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public roleId?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(Date)
    public accessValidFrom?: Date;

    @StrictlyTyped(Date)
    public accessValidTo?: Date;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class UserRoleRegisterDTO { 
    public constructor(obj?: Partial<UserRoleRegisterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public roleId?: number;

    @StrictlyTyped(Number)
    public userId?: number;

    @StrictlyTyped(Date)
    public accessValidFrom?: Date;

    @StrictlyTyped(Date)
    public accessValidTo?: Date;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
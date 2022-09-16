

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class RoleDTO {
    public constructor(obj?: Partial<RoleDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public userRoleId?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(Date)
    public accessValidFrom?: Date;

    @StrictlyTyped(Date)
    public accessValidTo?: Date;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
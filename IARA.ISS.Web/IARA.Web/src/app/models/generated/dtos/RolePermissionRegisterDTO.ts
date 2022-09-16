

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class RolePermissionRegisterDTO { 
    public constructor(obj?: Partial<RolePermissionRegisterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public permissionId?: number;

    @StrictlyTyped(Number)
    public roleId?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
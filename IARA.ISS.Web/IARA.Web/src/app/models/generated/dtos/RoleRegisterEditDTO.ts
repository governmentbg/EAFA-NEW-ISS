

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { UserRoleRegisterDTO } from './UserRoleRegisterDTO';

export class RoleRegisterEditDTO { 
    public constructor(obj?: Partial<RoleRegisterEditDTO>) {
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

    @StrictlyTyped(Boolean)
    public hasInternalAccess?: boolean;

    @StrictlyTyped(Boolean)
    public hasPublicAccess?: boolean;

    @StrictlyTyped(UserRoleRegisterDTO)
    public users?: UserRoleRegisterDTO[];

    @StrictlyTyped(Number)
    public permissionIds?: number[];

    @StrictlyTyped(Number)
    public reportPermissionIds?: number[];
}
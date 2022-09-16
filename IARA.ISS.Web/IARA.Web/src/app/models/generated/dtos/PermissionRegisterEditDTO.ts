

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RolePermissionRegisterDTO } from './RolePermissionRegisterDTO';

export class PermissionRegisterEditDTO { 
    public constructor(obj?: Partial<PermissionRegisterEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(Number)
    public groupId?: number;

    @StrictlyTyped(Number)
    public typeId?: number;

    @StrictlyTyped(RolePermissionRegisterDTO)
    public roles?: RolePermissionRegisterDTO[];
}
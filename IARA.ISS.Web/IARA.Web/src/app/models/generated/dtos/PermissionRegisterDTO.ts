

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PermissionRegisterDTO { 
    public constructor(obj?: Partial<PermissionRegisterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(String)
    public group?: string;

    @StrictlyTyped(String)
    public type?: string;

    @StrictlyTyped(Number)
    public rolesCount?: number;
}
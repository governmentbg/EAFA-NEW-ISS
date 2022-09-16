

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { EgnLncDTO } from './EgnLncDTO';
import { RoleDTO } from './RoleDTO';

export class UserEditDTO { 
    public constructor(obj?: Partial<UserEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public username?: string;

    @StrictlyTyped(String)
    public firstName?: string;

    @StrictlyTyped(String)
    public lastName?: string;

    @StrictlyTyped(String)
    public middleName?: string;

    @StrictlyTyped(String)
    public email?: string;

    @StrictlyTyped(EgnLncDTO)
    public egnLnc?: EgnLncDTO;

    @StrictlyTyped(String)
    public phone?: string;

    @StrictlyTyped(String)
    public position?: string;

    @StrictlyTyped(Number)
    public personId?: number;

    @StrictlyTyped(RoleDTO)
    public userRoles?: RoleDTO[];

    @StrictlyTyped(Boolean)
    public userMustChangePassword?: boolean;

    @StrictlyTyped(Boolean)
    public isLocked?: boolean;

    @StrictlyTyped(Number)
    public departmentId?: number;

    @StrictlyTyped(Number)
    public sectorId?: number;

    @StrictlyTyped(Number)
    public territoryUnitId?: number;
}
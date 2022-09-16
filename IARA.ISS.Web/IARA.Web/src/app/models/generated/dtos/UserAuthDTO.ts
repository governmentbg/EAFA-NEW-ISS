

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { EgnLncDTO } from './EgnLncDTO';
import { LoginTypesEnum } from '@app/enums/login-types.enum';

export class UserAuthDTO { 
    public constructor(obj?: Partial<UserAuthDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public personId?: number;

    @StrictlyTyped(EgnLncDTO)
    public egnLnc?: EgnLncDTO;

    @StrictlyTyped(String)
    public firstName?: string;

    @StrictlyTyped(String)
    public username?: string;

    @StrictlyTyped(String)
    public middleName?: string;

    @StrictlyTyped(String)
    public lastName?: string;

    @StrictlyTyped(Boolean)
    public hasUserPassLogin?: boolean;

    @StrictlyTyped(Boolean)
    public hasEAuthLogin?: boolean;

    @StrictlyTyped(Boolean)
    public userMustChangePassword?: boolean;

    @StrictlyTyped(Boolean)
    public userMustChangeData?: boolean;

    @StrictlyTyped(Number)
    public currentLoginType?: LoginTypesEnum;

    @StrictlyTyped(Boolean)
    public isInternalUser?: boolean;

    @StrictlyTyped(String)
    public permissions?: string[];
}
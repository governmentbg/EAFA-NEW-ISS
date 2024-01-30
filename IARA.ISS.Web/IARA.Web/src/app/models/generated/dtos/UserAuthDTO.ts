

import { User } from '@app/components/common-app/auth/models/auth/user.model';
import { LoginTypesEnum } from '@app/enums/login-types.enum';
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { EgnLncDTO } from './EgnLncDTO';

export class UserAuthDTO extends User<string> {
    public constructor(obj?: Partial<UserAuthDTO>) {
        if (obj != undefined) {
            super(obj as User<string>);
            Object.assign(this, obj);
        }
        else {
            super();
        }
    }

    @StrictlyTyped(Number)
    public userId!: number;

    @StrictlyTyped(Number)
    public personId?: number;

    @StrictlyTyped(EgnLncDTO)
    public egnLnc?: EgnLncDTO;

    @StrictlyTyped(String)
    public firstName?: string;

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
}
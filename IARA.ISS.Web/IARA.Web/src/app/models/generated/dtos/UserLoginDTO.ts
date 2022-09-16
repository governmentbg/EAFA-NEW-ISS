

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class UserLoginDTO { 
    public constructor(obj?: Partial<UserLoginDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public email?: string;

    @StrictlyTyped(String)
    public password?: string;

    @StrictlyTyped(String)
    public firstName?: string;

    @StrictlyTyped(String)
    public lastName?: string;

    @StrictlyTyped(String)
    public middleName?: string;
}
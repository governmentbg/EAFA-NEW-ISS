

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class UserDTO { 
    public constructor(obj?: Partial<UserDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public firstName?: string;

    @StrictlyTyped(String)
    public lastName?: string;

    @StrictlyTyped(String)
    public middleName?: string;

    @StrictlyTyped(String)
    public email?: string;

    @StrictlyTyped(Date)
    public registrationDate?: Date;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(String)
    public userRoles?: string;

    @StrictlyTyped(String)
    public mobileDevices?: string;
}
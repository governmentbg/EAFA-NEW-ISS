
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class UserChangePasswordDTO {
    public constructor(obj?: Partial<UserChangePasswordDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public token?: string;

    @StrictlyTyped(String)
    public password?: string;
}
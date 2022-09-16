
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class UserPasswordDTO {
    public constructor(obj?: Partial<UserPasswordDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public personId?: number;

    @StrictlyTyped(String)
    public oldPassword?: string;

    @StrictlyTyped(String)
    public newPassword?: string;
}
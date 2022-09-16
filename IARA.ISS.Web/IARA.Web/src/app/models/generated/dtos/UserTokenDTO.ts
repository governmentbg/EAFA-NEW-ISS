
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class UserTokenDTO {
    public constructor(obj?: Partial<UserTokenDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public token?: string;
}
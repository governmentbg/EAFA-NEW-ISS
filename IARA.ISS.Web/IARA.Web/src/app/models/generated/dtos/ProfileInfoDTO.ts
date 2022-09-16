
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ProfileInfoDTO {
    public constructor(obj?: Partial<ProfileInfoDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public firstName?: string;

    @StrictlyTyped(String)
    public lastName?: string;

    @StrictlyTyped(String)
    public username?: string;

    @StrictlyTyped(String)
    public email?: string;
}
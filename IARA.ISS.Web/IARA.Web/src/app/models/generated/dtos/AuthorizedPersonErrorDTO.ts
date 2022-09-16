
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class AuthorizedPersonErrorDTO {
    public constructor(obj?: Partial<AuthorizedPersonErrorDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public egnLnc?: string;

    @StrictlyTyped(String)
    public email?: string;

    @StrictlyTyped(Boolean)
    public egnAndEmailDontMatch?: boolean;
}

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AuthenticationOptionEnum } from '../../enums/authentication-option.enum';

export class TFAuthenticationModel {
    public constructor(obj?: Partial<TFAuthenticationModel>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Boolean)
    public hasTFAuthenticator!: boolean;

    @StrictlyTyped(Boolean)
    public isInternalUser!: boolean;

    @StrictlyTyped(String)
    public phone?: string;

    @StrictlyTyped(String)
    public email?: string;

    @StrictlyTyped(Number)
    public twoFactorMethod?: AuthenticationOptionEnum;
}
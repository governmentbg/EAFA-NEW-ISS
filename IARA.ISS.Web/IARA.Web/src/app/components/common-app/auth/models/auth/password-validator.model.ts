import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PasswordValidatorModel {
    public constructor(obj?: Partial<PasswordValidatorModel>) {
        if (obj != undefined) {
            Object.assign(this, obj);
        }
    }

    @StrictlyTyped(Boolean)
    public hasDigits!: boolean;

    @StrictlyTyped(Boolean)
    public hasLowerCase!: boolean;

    @StrictlyTyped(Boolean)
    public hasCapitalLetters!: boolean;

    @StrictlyTyped(Boolean)
    public hasSpecialSymbols!: boolean;

    @StrictlyTyped(Number)
    public minLength?: number;
}

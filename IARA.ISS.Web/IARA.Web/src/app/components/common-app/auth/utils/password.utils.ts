import { ValidatorFn, Validators } from '@angular/forms';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { PasswordValidatorModel } from '../models/auth/password-validator.model';

export class PasswordUtils {
    public static buildPasswordValidator(validator: PasswordValidatorModel, isRequired: boolean = true): ValidatorFn[] {
        const passwordValidators: ValidatorFn[] = [Validators.maxLength(30)];

        if (isRequired) {
            passwordValidators.push(Validators.required);
        }

        if (validator.hasCapitalLetters === true) {
            passwordValidators.push(TLValidators.capitalLetters);
        }

        if (validator.hasLowerCase === true) {
            passwordValidators.push(TLValidators.lowercaseLetters);
        }

        if (validator.hasDigits === true) {
            passwordValidators.push(TLValidators.digits);
        }

        if (validator.hasSpecialSymbols === true) {
            passwordValidators.push(TLValidators.specialSymbol);
        }

        if (validator.minLength) {
            passwordValidators.push(Validators.minLength(+validator.minLength));
        }

        return passwordValidators;
    }
}
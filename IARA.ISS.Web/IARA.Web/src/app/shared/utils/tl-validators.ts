import { AbstractControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CommonUtils } from './common.utils';
import { EgnUtils } from './egn.utils';
import { EikUtils } from './eik.utils';
import { PnfUtils } from './pnf.utils';

export class TLValidators {

    /**
     * Includes: 
     * - At least 8 characters in length both cyrilic or latin;
     * - Lowercase letters or Uppercase letters;
     * - Numbers;
     * - Special characters;
     * */
    public static COMPLEXITY_PATTERN = `(?=.*[a-zA-Z\u0401\u0451\u0410-\u044f])(?=.*[0-9])(?=.*[$@!%*?&]).{8,}`;

    public static number(min?: number, max?: number, fractionDigits?: number): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control.value !== undefined && control.value !== null) {
                const str: string = typeof control.value !== 'string' ? control.value.toString() : control.value;

                // trim, replace all commas with dot, remove all non-numeric symbols
                let newStr: string = str.trim().replace(',', '.').replace(/[^0-9.,-]/g, '');

                if (fractionDigits === 0) {
                    newStr = newStr.replace('.', '');
                }

                if (str !== newStr) {
                    control.setValue(newStr);
                }

                if (newStr.length !== 0) {
                    if (isNaN(Number(newStr))) {
                        control.setValue(undefined);
                        return { number: false };
                    }
                    else {
                        const num: number = Number(newStr);
                        if (num === 0 && fractionDigits === 0 && newStr.length !== 1) {
                            control.setValue(0);
                        }

                        if (min !== undefined) {
                            if (num < min) {
                                return {
                                    min: { min: min, actual: num }
                                };
                            }
                        }
                        if (max !== undefined) {
                            if (num > max) {
                                return {
                                    max: { max: max, actual: num }
                                };
                            }
                        }
                        if (fractionDigits !== undefined) {
                            const parts: string[] = newStr.split('.');
                            if (parts.length === 2) {
                                if (parts[1].length !== fractionDigits) {
                                    return { fractiondigits: fractionDigits };
                                }
                            }
                            else if (fractionDigits > 0) {
                                return { fractiondigits: fractionDigits };
                            }
                        }
                    }
                }
            }
            return null;
        }
    }

    public static requiredIf(predicate: () => boolean): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (predicate() === true) {
                return Validators.required(control);
            }
            return null;
        }
    }

    public static egn: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
        const value: string = control.value;
        if (value !== undefined && value !== null) {
            if (EgnUtils.isEgnValid(value)) {
                return null;
            }
        }
        return { egn: true };
    };

    public static pnf: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
        const value: string = control.value;
        if (value !== undefined && value !== null) {
            if (PnfUtils.isPnfValid(value)) {
                return null;
            }
        }
        return { pnf: true };
    };

    public static eik: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
        const value: string = control.value;
        if (value !== undefined && value !== null) {
            if (EikUtils.isEikValid(value)) {
                return null;
            }
        }
        return { eik: true };
    };

    public static cfr: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
        const value: string = control.value;
        if (value !== undefined && value !== null) {
            const trimmed: string = value.replace(' ', '');

            if (value !== trimmed) {
                control.setValue(trimmed, { emitEvent: false });
            }

            if (trimmed.length !== 12) {
                return { cfr: true };
            }

            const country: string = trimmed.substr(0, 3);
            if (!/^[A-Z]/.test(country)) {
                return { cfr: true };
            }

            const numbers: string = trimmed.substr(3, 9);
            if (!/^\d/.test(numbers)) {
                return { cfr: true };
            }

            return null;
        }
        return { cfr: true };
    };

    public static exactLength(length: number): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const value: string | undefined | null = control.value;
            if (value !== null && value !== undefined && value.length > 0) {
                if (value.length !== length) {
                    return { exactLength: length };
                }
            }
            return null;
        };
    }

    /**
     * Confirm password validator
     *
     * @param {AbstractControl} control
     * @returns {ValidationErrors | null}
     */
    public static confirmPasswordValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
        if (!control.parent || !control) {
            return null;
        }

        const password = control.parent.get('password');
        const passwordConfirmation = control.parent.get('passwordConfirmation');

        if (!password || !passwordConfirmation) {
            return null;
        }

        if ((password.value === null || password.value === undefined)
            && (passwordConfirmation.value === null || passwordConfirmation.value == undefined)
        ) {
            return null;
        }

        if (password.value === '' && passwordConfirmation.value === '') {
            return null;
        }

        if (password.value === passwordConfirmation.value) {
            return null;
        }

        return { passwordsNotMatching: true };
    };

    public static expectedValueMatch(expectedValue: string | number | boolean | Date | undefined): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (CommonUtils.isNullOrEmpty(expectedValue)) {
                return null;
            }

            const controlValue = control.value;

            if (controlValue instanceof NomenclatureDTO) {
                if (controlValue.displayName === expectedValue as string) {
                    return null;
                }
            }
            else if (expectedValue instanceof Date) {
                const controlValueDate: Date = controlValue as Date;
                if (controlValueDate !== undefined && controlValueDate !== null) {
                    if (controlValueDate.getDate() === expectedValue.getDate()
                        && controlValueDate.getMonth() === expectedValue.getMonth()
                        && controlValueDate.getFullYear() === expectedValue.getFullYear()) {
                        return null;
                    }
                }
            }
            else { // the type should be simple: string, number, boolean, undefined
                switch (typeof expectedValue) {
                    case 'number':
                        if (Number(controlValue) === expectedValue) {
                            return null;
                        }
                        break;
                    case 'string':
                        if (String(controlValue) === expectedValue) {
                            return null;
                        }
                        break;
                    case 'boolean': {
                        const controlBooleanValue: boolean = controlValue.toString() === 'true';
                        if (controlBooleanValue === expectedValue) {
                            return null;
                        }
                    } break;
                    case 'undefined':
                        return null;
                }
            }

            return { expectedValueNotMatching: expectedValue };
        }
    }

    public static passwordComplexityValidator(pattern: string = TLValidators.COMPLEXITY_PATTERN): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const value: string = control.value;
            if (value === null || value === undefined || value.match(pattern) || value.length === 0) {
                return null;
            }

            return { passwordcomplexity: true };
        }
    }
}

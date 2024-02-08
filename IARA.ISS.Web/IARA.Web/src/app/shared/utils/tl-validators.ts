import { AbstractControl, FormControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PaymentSummaryDTO } from '@app/models/generated/dtos/PaymentSummaryDTO';
import { Moment } from 'moment';
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
    public static COMPLEXITY_PATTERN = `(?=.*[a-zA-Z\u0401\u0451\u0410-\u044f])(?=.*[0-9])(?=.*[$@!%*?&])(?=.*[^a-zA-Z0-9\u0401\u0451\u0410-\u044f]).{8,}`;

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
        if (value === undefined || value === null || value === '' || EgnUtils.isEgnValid(value)) {
            return null;
        }
        return { egn: true };
    };

    public static pnf: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
        const value: string = control.value;
        if (value === undefined || value === null || value === '' || PnfUtils.isPnfValid(value)) {
            return null;
        }
        return { pnf: true };
    };

    public static eik: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
        const value: string = control.value;
        if (value === undefined || value === null || value === '' || EikUtils.isEikValid(value)) {
            return null;
        }
        return { eik: true };
    };

    public static cfr: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
        const value: string = control.value;
        if (value === undefined || value === null || value === '') {
            return null;
        }

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

    public static expectedValueMatch(expectedValue: string | number | boolean | Date | undefined, toLower: boolean = true): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {

            if (CommonUtils.isNullOrEmpty(expectedValue)) {
                return null;
            }
            const controlValue = control.value;

            if (controlValue instanceof NomenclatureDTO) {
                if (toLower) {
                    if (controlValue.displayName?.toLowerCase()?.replace(' ', '') === (expectedValue as string)?.toLowerCase()?.replace(' ', '')) {
                        return null;
                    }
                }
                else {
                    if (controlValue.displayName?.replace(' ', '') === (expectedValue as string)?.replace(' ', '')) {
                        return null;
                    }
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
                    case 'number': {
                        if (Number(controlValue) === expectedValue) {
                            return null;
                        }
                    }
                        break;
                    case 'string': {
                        if (toLower) {
                            if (String(controlValue).toLowerCase() === expectedValue.toLowerCase()) {
                                return null;
                            }
                        }
                        else {
                            if (String(controlValue) === expectedValue) {
                                return null;
                            }
                        }
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

    public static minDate(minDateControl?: AbstractControl, minDate?: Date): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control?.value !== null && control?.value !== undefined) {
                let minDateToCompare: Date | undefined;

                if (minDateControl !== null && minDateControl !== undefined) {
                    const minDateControlValue: Date | Moment | null | undefined = minDateControl.value;

                    if (minDateControlValue !== null && minDateControlValue !== undefined) {
                        if (minDateControlValue instanceof Date) {
                            minDateToCompare = minDateControlValue;
                        }
                        else {
                            minDateToCompare = minDateControlValue.toDate();
                        }

                    }
                }
                else if (minDate !== null && minDate !== undefined) {
                    minDateToCompare = minDate;
                }

                let date: Date | undefined = undefined;

                if (control.value instanceof Date) {
                    date = control.value;
                }
                else {
                    date = control.value.toDate();
                }

                if (minDateToCompare !== null && minDateToCompare !== undefined) {
                    if (date !== null && date !== undefined && date.getTime() < minDateToCompare.getTime()) {
                        return {
                            mindate: {
                                min: minDateToCompare,
                                actual: date
                            }
                        };
                    }
                }
            }

            return null;
        }
    }

    public static maxDate(maxDateControl?: AbstractControl, maxDate?: Date): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control?.value !== null && control?.value !== undefined) {
                let maxDateToCompare: Date | undefined;

                if (maxDateControl !== null && maxDateControl !== undefined) {
                    const maxDateControlValue: Date | Moment | null | undefined = maxDateControl.value;

                    if (maxDateControlValue !== null && maxDateControlValue !== undefined) {
                        if (maxDateControlValue instanceof Date) {
                            maxDateToCompare = maxDateControlValue;
                        }
                        else {
                            maxDateToCompare = maxDateControlValue.toDate();
                        }
                    }
                }
                else if (maxDate !== null && maxDate !== undefined) {
                    maxDateToCompare = maxDate;
                }

                let date: Date | undefined = undefined;

                if (control.value instanceof Date) {
                    date = control.value;
                }
                else {
                    date = control.value.toDate();
                }

                if (maxDateToCompare !== null && maxDateToCompare !== undefined) {
                    if (date !== null && date !== undefined && date.getTime() > maxDateToCompare.getTime()) {
                        return {
                            maxdate: {
                                max: maxDateToCompare,
                                actual: date
                            }
                        };
                    }
                }
            }

            return null;
        }
    }

    public static sameTotalPriceAndPaidPriceValidator(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            if (form === null || form === undefined) {
                return null;
            }

            const totalPaidPriceControl: FormControl | undefined = form.get('totalPaidPriceControl') as FormControl;

            if (totalPaidPriceControl === null || totalPaidPriceControl === undefined) {
                return null;
            }

            const totalPaidPrice: number = Number(totalPaidPriceControl!.value);

            if (totalPaidPrice === null || totalPaidPrice === undefined || isNaN(totalPaidPrice)) {
                return null;
            }

            const paymentSummaryControl: FormControl | undefined = form.get('paymentSummaryControl') as FormControl;

            if (paymentSummaryControl === null || paymentSummaryControl === undefined) {
                return null;
            }

            const totalPrice: number = Number((paymentSummaryControl!.value as PaymentSummaryDTO)?.totalPrice);

            if (totalPrice === null || totalPrice === undefined || isNaN(totalPaidPrice)) {
                return null;
            }

            if (totalPrice !== totalPaidPrice) {
                return { 'totalPriceNotEqualToPaid': Math.abs(totalPrice - totalPaidPrice) };
            }

            return null;
        }
    }


    public static specialSymbol: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
        return TLValidators.regexValidation(/^(?=.*[$@!%*?&.])/, control.value) ? { specialsymbol: true } : null;
    };

    public static lowercaseLetters: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
        return TLValidators.regexValidation(/^(?=.*[a-z])/, control.value) ? { lowercaseletters: true } : null;
    };

    public static capitalLetters: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
        return TLValidators.regexValidation(/^(?=.*[A-Z])/, control.value) ? { capitalletters: true } : null;
    };

    public static digits: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
        return TLValidators.regexValidation(/^(?=.*[0-9])/, control.value) ? { digits: true } : null;
    };

    public static digitsOnly: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
        return TLValidators.regexValidation(/(?=^[0-9]*$)/, control.value) ? { digitsonly: true } : null;
    };

    private static regexValidation(pattern: RegExp, value: string): boolean {
        if (value === undefined || value === null || value.length === 0) {
            return false;
        }

        if (pattern.test(value)) {
            return false;
        }
        return true;
    };

    private static isEmptyInputValue(value: any): boolean {
        return value == null || value.length === 0;
    }

    /**
     * Must match validator
     *
     * @param controlPath A dot-delimited string values that define the path to the control.
     * @param matchingControlPath A dot-delimited string values that define the path to the matching control.
     */
    public static mustMatch(controlPath: string, matchingControlPath: string): ValidatorFn {
        return (formGroup: AbstractControl): ValidationErrors | null => {

            // Get the control and matching control
            const control = formGroup.get(controlPath);
            const matchingControl = formGroup.get(matchingControlPath);

            // Return if control or matching control doesn't exist
            if (!control || !matchingControl) {
                return null;
            }

            // Delete the mustMatch error to reset the error on the matching control
            if (matchingControl.hasError('mustMatch')) {
                delete matchingControl.errors?.mustMatch;
                matchingControl.updateValueAndValidity();
            }

            // Don't validate empty values on the matching control
            // Don't validate if values are matching
            if (TLValidators.isEmptyInputValue(matchingControl.value) || control.value === matchingControl.value) {
                return null;
            }

            // Prepare the validation errors
            const errors = { mustMatch: true };

            // Set the validation error on the matching control
            matchingControl.setErrors(errors);

            // Return the errors
            return errors;
        };
    }
}

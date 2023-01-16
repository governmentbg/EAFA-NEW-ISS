import { AbstractControl, ValidationErrors, Validator, ValidatorFn } from '@angular/forms';
import { TLTranslatePipe } from '@app/shared/pipes/tl-translate.pipe';
import { DateUtils } from '@app/shared/utils/date.utils';
import { GetControlErrorLabelTextCallback } from './base-tl-control';
import { ITranslateService } from './interfaces/translate-service.interface';
import { TLError } from './models/tl-error.model';

const NO_VALIDATION_TRANSLATION: string = `No ITranslateService instance is present. In order to translate validation errors one must pass getControlErrorLabelText with all error translations or tlTranslationService with all codes in "validation" section.`

export class TLUtils {
    public static hasControlRequiredValidator(validator: ValidatorFn | null | undefined): boolean {
        let hasRequiredValidator = false;
        if (validator) {
            const validation = validator({} as AbstractControl);
            if (validation?.required) {
                hasRequiredValidator = true;
            }
            else {
                hasRequiredValidator = false;
            }
        }
        return hasRequiredValidator;
    }

    public static getFormControlName(control: AbstractControl): string {
        const parent = control.parent;
        let controlName: string = '';
        if (parent !== null && parent !== undefined) {
            for (const name of Object.keys(parent.controls)) {
                if (control === parent.get(name)) {
                    controlName = name;
                }
            }
        }

        return controlName;
    }

    public static buildErrorsCollection(
        control: AbstractControl | null,
        getControlErrorLabelText: GetControlErrorLabelTextCallback | undefined,
        tlTranslateService: ITranslateService | undefined,
        tlTranslatePipe: TLTranslatePipe
    ): TLError[] {
        const errors: TLError[] = [];

        if (control !== null && control !== undefined) {
            const controlErrors: ValidationErrors | null = control?.errors;

            if (controlErrors !== undefined && controlErrors !== null) {
                for (const key of Object.keys(controlErrors)) {
                    if (key.endsWith('Control')) {
                        const innerControlErrors = (controlErrors as ValidationErrors)[key as keyof typeof controlErrors];

                        if (innerControlErrors !== undefined && innerControlErrors !== null) {
                            for (const innerKey of Object.keys(innerControlErrors)) {
                                errors.push(...this.buildErrors(key, innerControlErrors, innerKey, getControlErrorLabelText, tlTranslateService, tlTranslatePipe));
                            }
                        }
                    }
                    else {
                        if (controlErrors !== undefined && controlErrors !== null) {
                            errors.push(...this.buildErrors(TLUtils.getFormControlName(control), controlErrors, key, getControlErrorLabelText, tlTranslateService, tlTranslatePipe));
                        }
                    }
                }
            }
        }

        return errors;
    }

    private static buildErrors(
        controlName: string,
        controlErrors: ValidationErrors,
        key: string,
        getControlErrorLabelText: GetControlErrorLabelTextCallback | undefined,
        tlTranslateService: ITranslateService | undefined,
        tlTranslatePipe: TLTranslatePipe
    ): TLError[] {
        const errors: TLError[] = [];

        if (getControlErrorLabelText !== null && getControlErrorLabelText !== undefined) {
            const errorInfo: TLError | undefined = getControlErrorLabelText(controlName, controlErrors[key], key);
            if (errorInfo !== undefined && errorInfo !== null) {
                errors.push(errorInfo);
            }
            else {
                const errorMessage = this.getDefaultTranslatedErrorMessage(controlErrors[key], key, tlTranslateService, tlTranslatePipe);
                this.splitMultilineError(errorMessage as string, errors);
            }
        }
        else {
            const errorMessage = this.getDefaultTranslatedErrorMessage(controlErrors[key], key, tlTranslateService, tlTranslatePipe);
            this.splitMultilineError(errorMessage as string, errors);
        }
        return errors;
    }

    private static splitMultilineError(errorMessage: string, errors: TLError[]): TLError[] {
        const messages = errorMessage.split('\n');
        if (messages != undefined) {
            for (const message of messages) {
                errors.push(new TLError({ text: message as string }));
            }
        } else {
            errors.push(new TLError({ text: errorMessage as string }));
        }

        return errors;
    }

    private static getDefaultTranslatedErrorMessage(
        error: Record<string, unknown>,
        errorCode: string,
        tlTranslationService: ITranslateService | undefined, // not needed
        tlTranslatePipe: TLTranslatePipe
    ): string {
        let message: string = '';
        const validationTranslation: string = tlTranslatePipe.transform('validation.' + errorCode, 'cap');

        switch (errorCode) {
            case 'required':
            case 'requiredtrue':
            case 'email':
            case 'selectedvaluefromdropdown':
            case 'novaluesindropdown':
            case 'passwordcomplexity': {
                message = validationTranslation;
            } break;
            case 'min': {
                if (error !== null && error !== undefined) {
                    message = `${validationTranslation}: ${error.min}`;
                }
            } break;
            case 'max': {
                if (error !== null && error !== undefined) {
                    message = `${validationTranslation}: ${error.max}`;
                }
            } break;
            case 'fractiondigits': {
                if (error !== null && error !== undefined) {
                    message = `${validationTranslation}: ${error}`;
                }
            } break;
            case 'minlength': {
                if (error.requiredLength !== null && error.requiredLength !== undefined) {
                    message = `${validationTranslation}: ${error.requiredLength}`;
                }
            } break;
            case 'maxlength': {
                if (error.requiredLength !== null && error.requiredLength !== undefined) {
                    message = `${validationTranslation}: ${error.requiredLength}`;
                }
            } break;
            case 'pattern': {
                if (error.requiredPattern !== null && error.requiredPattern !== undefined) {
                    message = `${validationTranslation}: ${error.requiredPattern}`;
                }
            } break;
            case 'exactLength': {
                if (error !== null && error !== undefined) {
                    message = `${validationTranslation}: ${error}`;
                }
            } break;
            case 'matDatepickerMax': {
                if (error.max !== null && error.max !== undefined) {
                    message = `${validationTranslation}: ${DateUtils.ToDisplayDateString(error.max as Date)}`;
                }
            } break;
            case 'matDatepickerMin': {
                if (error.min !== null && error.min !== undefined) {
                    message = `${validationTranslation}: ${DateUtils.ToDisplayDateString(error.min as Date)}`;
                }
            } break;
            default: {
                message = tlTranslatePipe.transform('validation.default', 'cap');
            } break;
        }

        if (message !== null && message !== undefined && !message.startsWith('validation.')) {
            return message;
        }
        else {
            throw new Error(NO_VALIDATION_TRANSLATION);
        }
    }
}
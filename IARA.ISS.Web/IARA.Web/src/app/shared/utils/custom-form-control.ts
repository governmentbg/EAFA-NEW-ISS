import { AbstractControl, ControlValueAccessor, FormArray, FormControl, FormGroup, NgControl, ValidationErrors, Validator } from '@angular/forms';
import { Subject } from 'rxjs';
import { ValidityChecker } from '../directives/validity-checker/validity-checker.abstract';
import { ValidityCheckerComponent } from './validity-checker.component';

export abstract class CustomFormControl<ValueType> extends ValidityCheckerComponent implements ControlValueAccessor, Validator {
    public get control(): FormControl {
        return this.abstractControl as FormControl;
    }

    public get form(): FormGroup {
        return this.abstractControl as FormGroup;
    }

    public get formArray(): FormArray {
        return this.abstractControl as FormArray;
    }

    public isDisabled: boolean = false;

    protected onMarkAsTouched: Subject<void> = new Subject<void>();
    protected onMarkAsUntouched: Subject<void> = new Subject<void>();
    protected onMarkAsDirty: Subject<void> = new Subject<void>();
    protected onMarkAsPristine: Subject<void> = new Subject<void>();

    protected readonly ngControl: NgControl | null;
    protected onChanged: (value: ValueType) => void;
    protected onTouched: (value: ValueType) => void;

    private abstractControl!: AbstractControl;

    public constructor(
        ngControl: NgControl | null,
        registerValueChanges: boolean = true,
        validityChecker: ValidityChecker | undefined = undefined
    ) {
        super(validityChecker);

        this.ngControl = ngControl ?? null;

        this.onChanged = (value: ValueType) => { return; };
        this.onTouched = (value: ValueType) => { return; };

        this.abstractControl = this.buildForm();

        if (this.ngControl) {
            this.ngControl.valueAccessor = this;

            if (registerValueChanges && this.abstractControl !== undefined && this.abstractControl !== null) {
                this.abstractControl.valueChanges.subscribe({
                    next: () => {
                        this.onChanged(this.getValue());
                    }
                });
            }
        }
    }

    public abstract writeValue(value: ValueType): void;

    public registerOnChange(fn: (value: ValueType) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: ValueType) => void): void {
        this.onTouched = fn;
    }

    public setDisabledState(isDisabled: boolean): void {
        this.isDisabled = isDisabled;

        if (this.isDisabled) {
            this.abstractControl.disable();
        }
        else {
            this.abstractControl.enable();
        }
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        return this.getValidationErrors(this.abstractControl);
    }

    protected abstract getValue(): ValueType;
    protected abstract buildForm(): AbstractControl;

    protected initCustomFormControl(): void {
        if (this.ngControl && this.ngControl.control) {
            this.ngControl.control.validator = this.validate.bind(this);

            const self = this;

            const markAsTouched = this.ngControl.control.markAsTouched;
            this.ngControl.control.markAsTouched = function (opts?: any) {
                markAsTouched.apply(this, opts);
                self.abstractControl.markAllAsTouched();
                self.onMarkAsTouched.next();
            };

            const markAsUntouched = this.ngControl.control.markAsUntouched;
            this.ngControl.control.markAsUntouched = function (opts?: any) {
                markAsUntouched.apply(this, opts);
                self.abstractControl.markAsUntouched();
                self.onMarkAsUntouched.next();
            };

            const markAsDirty = this.ngControl.control.markAsDirty;
            this.ngControl.control.markAsDirty = function (opts?: any) {
                markAsDirty.apply(this, opts);
                self.abstractControl.markAsDirty();
                self.onMarkAsDirty.next();
            };

            const markAsPristine = this.ngControl.control.markAsPristine;
            this.ngControl.control.markAsPristine = function (opts?: any) {
                markAsPristine.apply(this, opts);
                self.abstractControl.markAsPristine();
                self.onMarkAsPristine.next();
            };
        }

        this.initValidityChecker();
    }

    private getValidationErrors(control: AbstractControl): ValidationErrors | null {
        if (control instanceof FormControl) {
            return control.errors;
        }
        else if (control instanceof FormArray) {
            const errors: ValidationErrors = control.errors ?? {};
            
            for (const cntrl of control.controls) {
                const controlErrors: ValidationErrors | null = this.getValidationErrors(cntrl);
                if (controlErrors !== null) {
                    for (const error of Object.keys(controlErrors)) {
                        errors[error] = controlErrors[error];
                    }
                }
            }

            return Object.keys(errors).length === 0 ? null : errors;
        }
        else if (control instanceof FormGroup) {
            const errors: ValidationErrors = control.errors ?? {};
            
            for (const key of Object.keys(control.controls)) {
                const controlErrors: ValidationErrors | null = this.getValidationErrors(control.controls[key]);
                if (controlErrors !== null) {
                    errors[key] = controlErrors;
                }
            }

            return Object.keys(errors).length === 0 ? null : errors;
        }

        return null;
    }
}
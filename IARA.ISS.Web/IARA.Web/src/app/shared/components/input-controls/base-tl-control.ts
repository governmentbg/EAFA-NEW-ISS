import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormControl, NgControl, ValidatorFn } from '@angular/forms';
import { FloatLabelType, MatFormFieldAppearance } from '@angular/material/form-field';
import { TLTranslatePipe } from '@app/shared/pipes/tl-translate.pipe';
import { TLError } from './models/tl-error.model';
import { TLUtils } from './utils';

export const NOOP_VALUE_ACCESSOR: ControlValueAccessor = {
    writeValue(): void { },
    registerOnChange(): void { },
    registerOnTouched(): void { }
};

export type GetControlErrorLabelTextCallback = (controlName: string, error: unknown, errorCode: string) => TLError | undefined;

@Component({ template: '' })
export abstract class BaseTLControl implements OnInit {

    @Input()
    public label: string = '';

    @Input() public set showLabel(value: boolean) {
        this._showLabel = value;
        if (!this._showLabel) {
            this.label = '';
        }
    }

    @Input()
    public disableTooltip: boolean = false;

    @Output()
    public focusout: EventEmitter<FocusEvent> = new EventEmitter<FocusEvent>();

    public readonly TOOLTIP_SHOW_DELAY_MS: number = 1000;
    public readonly TOOLTIP_POSITION: string = 'above';

    @Input()
    public appearance: MatFormFieldAppearance = 'standard';

    @Input()
    public multilineError: boolean = false;

    @Input()
    public hint: string = '';

    @Input()
    public showHint: boolean = false;

    @Input()
    public getControlErrorLabelText: GetControlErrorLabelTextCallback | undefined;

    @Input()
    public readonly: boolean = false;

    @Input()
    public tooltipResourceName: string = '';

    @Input()
    public showTooltip: string = '';

    @Input()
    public floatLabel: FloatLabelType = 'auto';

    @Input()
    public spellcheck: boolean = false;

    @Input()
    public autocorrect: string = 'off';

    @Input()
    public autocomplete: string = 'off';

    @Input()
    public autocapitalize: string = 'off';

    public set ngControl(value: NgControl) {
        this._ngControl = value;
        this.ngControlInitialized.emit();
    }

    public get ngControl(): NgControl {
        return this._ngControl as NgControl;
    }

    public _showLabel: boolean = true;
    public fieldIsRequired: boolean = false;
    public errors: TLError[] = new Array<TLError>();
    public _ngControl: NgControl | undefined;

    protected formControl: FormControl | undefined;
    protected ngControlInitialized: EventEmitter<void>;
    protected translatePipe: TLTranslatePipe;

    public constructor(ngControl: NgControl, translatePipe: TLTranslatePipe) {
        this.ngControlInitialized = new EventEmitter<void>();
        this._ngControl = ngControl;

        this.translatePipe = translatePipe;

        if (this._ngControl) {
            this.formControl = ngControl.control as FormControl;
            // Note: we provide the value accessor through here, instead of
            // the `providers` to avoid running into a circular import.
            // And we use NOOP_VALUE_ACCESSOR so WrappedInput don't do anything with NgControl
            this.ngControl.valueAccessor = NOOP_VALUE_ACCESSOR;
        }
    }

    public ngOnInit(): void {
        if (this._ngControl) {
            const validator = this._ngControl?.control?.validator;
            this.formControl = this._ngControl.control as FormControl;

            this.fieldIsRequired = this.hasControlRequiredValidator(validator);

            // eslint-disable-next-line @typescript-eslint/no-this-alias
            const self = this;
            const originalMethod = this._ngControl?.control?.markAsTouched;
            (this._ngControl.control as AbstractControl).markAsTouched = function (a: any) {
                originalMethod?.apply(this, a);
                self.buildErrorsCollection();
            };

            const originalPendingMethod = this._ngControl?.control?.markAsPending;
            (this._ngControl.control as AbstractControl).markAsPending = function (a: any) {
                originalPendingMethod?.apply(this, a);
                self.fieldIsRequired = self.hasControlRequiredValidator(self._ngControl?.control?.validator);
                self.formControl?.updateValueAndValidity(a);
            };

            (this._ngControl.control as AbstractControl).statusChanges.subscribe({
                next: (status: string) => {
                    if (status === 'INVALID') {
                        this.buildErrorsCollection();
                    }
                }
            });
        }
    }

    protected buildErrorsCollection(): void {
        this.errors = TLUtils.buildErrorsCollection(this.ngControl?.control, this.getControlErrorLabelText, this.translatePipe);
    }

    private hasControlRequiredValidator(validator: ValidatorFn | null | undefined): boolean {
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

    public onFocusOut(event: FocusEvent): void {
        this.focusout.emit(event);
    }
}

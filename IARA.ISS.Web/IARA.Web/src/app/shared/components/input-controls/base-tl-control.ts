import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormControl, NgControl } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLTranslatePipe } from '../../pipes/tl-translate.pipe';
import { ITranslateService } from './interfaces/translate-service.interface';
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
    public disableTooltip: boolean = false;

    @Output()
    public focusout: EventEmitter<FocusEvent> = new EventEmitter<FocusEvent>();

    public readonly TOOLTIP_SHOW_DELAY_MS: number = 1000;
    public readonly TOOLTIP_POSITION: string = 'above';

    public set ngControl(value: NgControl) {
        this._ngControl = value;
        this.ngControlInitialized.emit();
    }

    public get ngControl(): NgControl {
        return this._ngControl as NgControl;
    }

    public tlTranslatePipe: TLTranslatePipe;
    public _showLabel: boolean = true;
    public tlTranslationService: ITranslateService | undefined;
    public fieldIsRequired: boolean = false;
    public errors: TLError[] = new Array<TLError>();
    public _ngControl: NgControl | undefined;

    protected formControl: FormControl | undefined;
    protected ngControlInitialized: EventEmitter<void>;

    constructor(ngControl: NgControl, fuseTranslateionService: FuseTranslationLoaderService, tlTranslatePipe: TLTranslatePipe) {
        this.ngControlInitialized = new EventEmitter<void>();
        this._ngControl = ngControl;

        this.tlTranslationService = fuseTranslateionService;
        this.tlTranslatePipe = tlTranslatePipe;

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

            this.fieldIsRequired = TLUtils.hasControlRequiredValidator(validator);

            const self = this;
            const originalMethod = this._ngControl?.control?.markAsTouched;
            (this._ngControl.control as AbstractControl).markAsTouched = function (a: any) {
                originalMethod?.apply(this, a);
                self.buildErrorsCollection();
            };

            const originalPendingMethod = this._ngControl?.control?.markAsPending;
            (this._ngControl.control as AbstractControl).markAsPending = function (a: any) {
                originalPendingMethod?.apply(this, a);
                self.fieldIsRequired = TLUtils.hasControlRequiredValidator(self._ngControl?.control?.validator);
                self.formControl?.updateValueAndValidity(a);
            };
        }
    }

    public onFocusOut(event: FocusEvent): void {
        this.focusout.emit(event);
    }

    protected buildErrorsCollection(): void {
        this.errors = TLUtils.buildErrorsCollection(this.ngControl?.control, this.getControlErrorLabelText, this.tlTranslationService, this.tlTranslatePipe);
    }
}
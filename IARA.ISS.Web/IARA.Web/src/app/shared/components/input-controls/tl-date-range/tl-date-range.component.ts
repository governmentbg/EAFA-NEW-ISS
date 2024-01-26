import { Component, DoCheck, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLTranslatePipe } from '@app/shared/pipes/tl-translate.pipe';
import { DateUtils } from '@app/shared/utils/date.utils';
import { GetControlErrorLabelTextCallback } from '../base-tl-control';
import { TLError } from '../models/tl-error.model';
import { TLUtils } from '../utils';

export class DateRangeData {
    public start: Date | undefined;
    public end: Date | undefined;

    public constructor(data?: Partial<DateRangeData>) {
        Object.assign(this, data);
    }
}

@Component({
    selector: 'tl-date-range',
    templateUrl: './tl-date-range.component.html',
})
export class TLDateRangeComponent implements OnInit, DoCheck, ControlValueAccessor {
    @Input()
    public label: string = '';

    @Input()
    public getControlErrorLabelText: GetControlErrorLabelTextCallback | undefined;

    @Input()
    public readonly: boolean = false;

    @Input()
    public tooltipResourceName: string = '';

    @Input()
    public bothDatesRequired: boolean = false;

    @Input()
    public min: Date = DateUtils.MIN_DATE;

    @Input()
    public max: Date = DateUtils.MAX_DATE;

    @Input()
    public disableTooltip: boolean = false;

    public readonly TOOLTIP_SHOW_DELAY_MS: number = 1000;
    public readonly TOOLTIP_POSITION: string = 'above';

    public form: FormGroup;
    public errors: TLError[] = [];

    public required: boolean = false;

    private onChanged: (value: DateRangeData | undefined) => void;
    private onTouched: (value: DateRangeData | undefined) => void;
    private ngControl: NgControl;

    private translate: FuseTranslationLoaderService;
    private translatePipe: TLTranslatePipe;

    public constructor(@Self() ngControl: NgControl, translate: FuseTranslationLoaderService, translatePipe: TLTranslatePipe) {
        this.ngControl = ngControl;
        this.translate = translate;
        this.translatePipe = translatePipe;

        this.onChanged = (value: DateRangeData | undefined) => { return; };
        this.onTouched = (value: DateRangeData | undefined) => { return; };

        this.ngControl.valueAccessor = this;

        this.form = new FormGroup({
            startDateControl: new FormControl(null),
            endDateControl: new FormControl(null)
        });

        this.form.valueChanges.subscribe({
            next: () => {
                this.onChanged(this.getValue());
                this.buildErrorsCollection();
            }
        });
    }

    public ngOnInit(): void {
        if (this.ngControl.control) {
            this.setValidators(this);

            this.overrideUpdateValueAndValidity();
            this.overrideMarkAsTouched();
        }
    }

    public ngDoCheck(): void {
        if (this.ngControl?.control?.touched) {
            this.form.markAllAsTouched();
        }
    }

    public writeValue(value: DateRangeData): void {
        if (value !== null && value !== undefined) {
            this.form.get('startDateControl')!.setValue(value.start);
            this.form.get('endDateControl')!.setValue(value.end);
        }
        else {
            this.form.reset();
        }
    }

    public registerOnChange(fn: (value: DateRangeData | undefined) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: DateRangeData | undefined) => void): void {
        this.onTouched = fn;
    }

    public setDisabledState(isDisabled: boolean): void {
        if (isDisabled) {
            this.form.disable();
        }
        else {
            this.form.enable();
        }
    }

    private getValue(): DateRangeData | undefined {
        const result: DateRangeData = new DateRangeData({
            start: this.form.get('startDateControl')!.value ?? undefined,
            end: this.form.get('endDateControl')!.value ?? undefined
        });

        if (this.bothDatesRequired) {
            // ако поне една дата не е попълнена
            if (result.start === undefined || result.end === undefined) {
                return undefined;
            }
        }
        else {
            // ако и двете дати не са попълнени
            if (result.start === undefined && result.end === undefined) {
                return undefined;
            }
        }

        return result;
    }

    private buildErrorsCollection(): void {
        this.errors = TLUtils.buildErrorsCollection(this.ngControl?.control, this.getControlErrorLabelText, this.translatePipe);
    }

    private overrideUpdateValueAndValidity(): void {
        // eslint-disable-next-line @typescript-eslint/no-this-alias
        const self = this;
        const originalMethod = this.ngControl.control!.updateValueAndValidity;
        this.ngControl.control!.updateValueAndValidity = function (a: any) {
            originalMethod.apply(this, a);
            self.setValidators(self);
        };
    }

    private overrideMarkAsTouched(): void {
        // eslint-disable-next-line @typescript-eslint/no-this-alias
        const self = this;
        const originalMethod = this.ngControl.control!.markAsTouched;
        this.ngControl.control!.markAsTouched = function (a: any) {
            originalMethod.apply(this, a);
            self.buildErrorsCollection();
        };
    }

    private setValidators(self: TLDateRangeComponent): void {
        self.required = TLUtils.hasControlRequiredValidator(self.ngControl!.control!.validator);

        if (self.required === true) {
            self.form.setValidators(Validators.required);
            self.form.get('startDateControl')!.setValidators(Validators.required);
            self.form.get('endDateControl')!.setValidators(Validators.required);
        }
        else {
            self.form.clearValidators();
            self.form.get('startDateControl')!.clearValidators();
            self.form.get('endDateControl')!.clearValidators();
        }
    }
}
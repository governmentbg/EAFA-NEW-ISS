import { AfterViewInit, Component, Input, OnInit, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { CustomFormControl } from '../../utils/custom-form-control';
import { DateRangeData } from '../input-controls/tl-date-range/tl-date-range.component';

export class DateRangeIndefiniteData {
    public range: DateRangeData | undefined;
    public indefinite: boolean = false;

    public constructor(data?: Partial<DateRangeIndefiniteData>) {
        Object.assign(this, data);
    }
}

@Component({
    selector: 'date-range-indefinite',
    templateUrl: './date-range-indefinite.component.html'
})
export class DateRangeIndefiniteComponent extends CustomFormControl<DateRangeIndefiniteData | undefined> implements OnInit, AfterViewInit {
    @Input()
    public rangeLabel: string = '';

    @Input()
    public dateLabel: string = '';

    @Input()
    public checkboxLabel: string = '';

    public constructor(@Self() ngControl: NgControl) {
        super(ngControl);
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public ngAfterViewInit(): void {
        this.form.get('isIndefiniteControl')!.valueChanges.subscribe({
            next: (value: boolean | undefined) => {
                if (value === true) {
                    this.form.get('rangeControl')!.setValue(undefined);
                    this.form.get('rangeControl')!.clearValidators();
                    this.form.get('rangeControl')!.setErrors(null);

                    this.form.get('dateControl')!.setValidators(Validators.required);
                    this.form.get('dateControl')!.markAsPending({ emitEvent: false });
                    this.form.get('dateControl')!.updateValueAndValidity({ emitEvent: false });
                }
                else {
                    this.form.get('dateControl')!.setValue(undefined);
                    this.form.get('dateControl')!.clearValidators();
                    this.form.get('dateControl')!.setErrors(null);

                    this.form.get('rangeControl')!.setValidators(Validators.required);
                    this.form.get('rangeControl')!.updateValueAndValidity({ emitEvent: false });
                }
            }
        });
    }

    public writeValue(value: DateRangeIndefiniteData): void {
        if (value !== null && value !== undefined) {
            this.form.get('isIndefiniteControl')!.setValue(value.indefinite ?? false);

            if (value.indefinite === true) {
                this.form.get('dateControl')!.setValue(value.range?.start);
            }
            else {
                this.form.get('rangeControl')!.setValue(value.range);
            }
        }
        else {
            this.form.reset();
        }
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            rangeControl: new FormControl(null, Validators.required),
            dateControl: new FormControl(null),
            isIndefiniteControl: new FormControl(false)
        });
    }

    protected getValue(): DateRangeIndefiniteData | undefined {
        const result: DateRangeIndefiniteData = new DateRangeIndefiniteData({
            indefinite: this.form.get('isIndefiniteControl')!.value ?? false
        });

        if (result.indefinite) {
            result.range = new DateRangeData({
                start: this.form.get('dateControl')!.value ?? undefined,
                end: undefined
            });
        }
        else {
            result.range = this.form.get('rangeControl')!.value ?? undefined;
        }

        return result;
    }
}
import { Component, Input, OnInit, Self } from '@angular/core';
import { AbstractControl, FormControl, NgControl } from '@angular/forms';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';

@Component({
    selector: 'tl-input-stepper',
    templateUrl: './tl-input-stepper.component.html',
    styleUrls: ['./tl-input-stepper.component.scss']
})
export class TLInputStepperComponent extends CustomFormControl<number> implements OnInit {
    @Input()
    public initial: number = 1;

    @Input()
    public step: number = 1;

    @Input()
    public min: number = 0;

    @Input()
    public max?: number;

    public constructor(@Self() ngControl: NgControl) {
        super(ngControl, false);
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
        this.control.setValue(this.initial);

        this.control.valueChanges.subscribe({
            next: (value: number) => {
                if (value < this.min) {
                    value = this.min;
                }
                if (this.max !== undefined && value > this.max) {
                    value = this.max;
                }
                this.control.setValue(value, { onlySelf: true, emitEvent: false });
                this.onChanged(value);
            }
        });
    }

    public writeValue(value: number): void {
        if (value !== null && value !== undefined) {
            this.control.setValue(value);
        }
    }

    public registerOnChange(fn: (value: number) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: number) => void): void {
        this.onTouched = fn;
    }

    public minus(): void {
        if (!(this.control.value - this.step < this.min)) {
            this.control.setValue(this.control.value - this.step);
        }
    }

    public plus(): void {
        if (!(this.max !== undefined && this.control.value + this.step > this.max)) {
            this.control.setValue(this.control.value + this.step);
        }
    }

    protected buildForm(): AbstractControl {
        return new FormControl();
    }

    protected getValue(): number {
        return this.control.value;
    }
}

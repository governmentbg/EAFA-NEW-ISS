import { Component, EventEmitter, Input, OnChanges, OnInit, Output, Self, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, NgControl, ValidatorFn, Validators } from '@angular/forms';
import { FloatLabelType, MatFormFieldAppearance } from '@angular/material/form-field';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { PrefixInputDTO } from '@app/models/generated/dtos/PrefixInputDTO';
import { GetControlErrorLabelTextCallback } from '../base-tl-control';
import { HTMLInputTypes } from '../tl-input/tl-input.component';
import { TLValidators } from '@app/shared/utils/tl-validators';

@Component({
    selector: 'tl-prefix-input',
    templateUrl: './tl-prefix-input.component.html'
})
export class TLPrefixInputComponent extends CustomFormControl<PrefixInputDTO | undefined> implements OnInit, OnChanges {
    @Input()
    public appearance: MatFormFieldAppearance = 'legacy';

    @Input()
    public type: HTMLInputTypes = 'text';

    @Input()
    public showLabel: boolean = true;

    @Input()
    public label: string = '';

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

    @Input()
    public floatLabel: FloatLabelType = 'auto';

    @Input()
    public validators: ValidatorFn[] | undefined;

    @Output()
    public focusout: EventEmitter<FocusEvent> = new EventEmitter<FocusEvent>();

    public prefix: string | undefined;

    public constructor(@Self() ngControl: NgControl) {
        super(ngControl);
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if ('validators' in changes) {
            if (this.validators !== null && this.validators !== undefined) {
                this.control.setValidators(this.validators);
            }
            else {
                this.control.clearValidators();
            }

            this.control.markAsPending();
        }
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(value: PrefixInputDTO | undefined): void {
        if (value !== null && value !== undefined) {
            this.prefix = value.prefix;
            this.control.setValue(value.inputValue);
        }
        else {
            this.prefix = undefined;
            this.control.setValue(undefined);
        }
    }

    protected getValue(): PrefixInputDTO | undefined {
        if (this.control.value !== null && this.control.value !== undefined) {
            return new PrefixInputDTO({
                prefix: this.prefix,
                inputValue: this.control.value
            });
        }
        else {
            return undefined;
        }
    }

    protected buildForm(): AbstractControl {
        return new FormControl(undefined, [Validators.required, TLValidators.number(0)]);
    }

    public onFocusOut(event: FocusEvent): void {
        this.focusout.emit(event);
    }
}
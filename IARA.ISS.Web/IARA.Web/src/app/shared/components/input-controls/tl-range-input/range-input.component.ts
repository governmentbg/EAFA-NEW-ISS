import { FocusMonitor, FocusOrigin } from '@angular/cdk/a11y';
import { AfterViewInit, Component, DoCheck, ElementRef, Inject, Input, OnChanges, OnDestroy, OnInit, Optional, Self, ViewChild } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormControl, FormGroup, NgControl, ValidationErrors, Validator, ValidatorFn, Validators } from '@angular/forms';
import { MatFormField, MatFormFieldControl, MAT_FORM_FIELD } from '@angular/material/form-field';
import { Subject } from 'rxjs';
import { TLTranslatePipe } from '../../../pipes/tl-translate.pipe';

export class RangeInputData {
    public start: number | undefined;
    public end: number | undefined;

    public constructor(range?: Partial<RangeInputData>) {
        Object.assign(this, range);
    }
}

@Component({
    selector: 'range-input',
    templateUrl: './range-input.component.html',
    styleUrls: ['./range-input.component.scss'],
    providers: [{ provide: MatFormFieldControl, useExisting: RangeInputComponent }],
    host: {
        '[class.floating]': 'shouldLabelFloat',
        '[id]': 'id',
        '[attr.aria-describedby]': 'describedBy'
    }
})
export class RangeInputComponent implements OnInit, AfterViewInit, DoCheck, OnChanges, OnDestroy, MatFormFieldControl<RangeInputData>, ControlValueAccessor, Validator {
    @Input() public fromLabel!: string;
    @Input() public toLabel!: string;

    @Input() public separator!: string;

    @Input() public min: number | undefined;
    @Input() public max: number | undefined;

    @Input() public readonly: boolean = false;

    @Input() public startRequired: boolean = true;
    @Input() public endRequired: boolean = true;

    @Input()
    public set required(value: boolean) {
        this.isRequired = value ?? false;
        this.stateChanges.next();
    }

    public get required(): boolean {
        return this.isRequired;
    }

    public form!: FormGroup;
    public formField: MatFormField;
    public ngControl: NgControl;

    public id: string;
    public stateChanges: Subject<void> = new Subject<void>();
    public focused: boolean = false;
    public errorState: boolean = false;
    public controlType: string = 'range-input';
    public describedBy: string = '';

    public set value(value: RangeInputData | null) {
        this.writeValue(value);
        this.onChanged(value);
        this.stateChanges.next();
    }

    public get placeholder(): string {
        return this.placeHolder;
    }

    public set placeholder(value: string) {
        this.placeHolder = value;
        this.stateChanges.next();
    }

    public get disabled(): boolean {
        return this.isDisabled;
    }

    public get empty(): boolean {
        const value: RangeInputData = this.getValue();
        return (value.start === undefined || value.start === null)
            && (value.end === undefined || value.end === null);
    }

    public get shouldLabelFloat(): boolean {
        return this.focused || !this.empty;
    }

    @ViewChild('startValue')
    private startValueElement!: ElementRef;

    @ViewChild('endValue')
    private endValueElement!: ElementRef;

    private translate: TLTranslatePipe;
    private onChanged: (value: RangeInputData | null) => void;
    private onTouched: (value: RangeInputData | null) => void;

    private focusMonitor: FocusMonitor;
    private elementRef: ElementRef;

    private isDisabled: boolean = false;
    private isRequired: boolean = false;
    private placeHolder: string = '';

    private static nextId: number = 0;

    public constructor(
        @Self() ngControl: NgControl,
        @Inject(MAT_FORM_FIELD) formField: MatFormField,
        focusMonitor: FocusMonitor,
        elementRef: ElementRef,
        translate: TLTranslatePipe
    ) {
        this.ngControl = ngControl;
        this.formField = formField;
        this.focusMonitor = focusMonitor;
        this.elementRef = elementRef;
        this.translate = translate;

        this.onChanged = (value: RangeInputData | null) => { return; };
        this.onTouched = (value: RangeInputData | null) => { return; };

        this.ngControl.valueAccessor = this;

        this.id = `tl-range-input-${RangeInputComponent.nextId++}`;

        this.buildForm();
    }

    public ngOnInit(): void {
        if (this.ngControl.control) {
            this.ngControl.control.validator = this.validate.bind(this);
        }

        if (this.fromLabel === undefined || this.fromLabel === null) {
            this.fromLabel = this.translate.transform('common.from', 'cap');
        }
        if (this.toLabel === undefined || this.toLabel === null) {
            this.toLabel = this.translate.transform('common.to', 'cap');
        }
        if (this.separator === undefined || this.separator === null) {
            this.separator = '–';
        }

        this.setupValidators();
    }

    public ngAfterViewInit(): void {
        this.recalculateWidths();

        this.form.valueChanges.subscribe({
            next: () => {
                this.recalculateWidths();
                this.setupValidators();
                this.onChanged(this.getValue());
            }
        });

        this.focusMonitor.monitor(this.elementRef.nativeElement, true).subscribe({
            next: (origin: FocusOrigin) => {
                const previous: boolean = this.focused;

                this.focused = origin !== undefined && origin !== null;
                this.onTouched(this.getValue());

                this.stateChanges.next();
                if (previous && this.focused) {
                    this.ngControl.control?.updateValueAndValidity({ emitEvent: false });
                }
            }
        });
    }

    public ngDoCheck(): void {
        if (this.ngControl?.control?.touched) {
            this.form.markAllAsTouched();
        }
    }

    public ngOnChanges(): void {
        this.stateChanges.next();
    }

    public ngOnDestroy(): void {
        this.stateChanges.complete();
        this.focusMonitor.stopMonitoring(this.elementRef.nativeElement);
    }

    public setDescribedByIds(ids: string[]): void {
        this.describedBy = ids.join(' ');
    }

    public onContainerClick(event: MouseEvent): void {
        if ((event.target as Element).tagName.toLowerCase() !== 'input') {
            this.elementRef.nativeElement.querySelector('input').focus();
        }
    }

    public writeValue(value: RangeInputData | null): void {
        if (value !== undefined && value !== null) {
            this.form.get('startValueControl')!.setValue(value.start);
            this.form.get('endValueControl')!.setValue(value.end);
        }
        else {
            this.form.get('startValueControl')!.setValue(undefined);
            this.form.get('endValueControl')!.setValue(undefined);
        }
    }

    public registerOnChange(fn: (value: RangeInputData | null) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: RangeInputData | null) => void): void {
        this.onTouched = fn;
    }

    public setDisabledState(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
        if (this.isDisabled) {
            this.form.disable();
        }
        else {
            this.form.enable();
        }
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        const errors: ValidationErrors = {};
        for (const key of Object.keys(this.form.controls)) {
            const controlErrors: ValidationErrors | null = this.form.controls[key].errors;
            if (controlErrors !== null) {
                errors[key] = controlErrors;
            }
        }

        if (Object.keys(errors).length === 0) {
            this.errorState = false;
            return null;
        }
        this.errorState = true;
        return errors;
    }

    private recalculateWidths(): void {
        const value: RangeInputData = this.getValue();
        const startLength: number = value.start !== undefined && value.start !== null ? value.start.toString().length : this.fromLabel.length;
        const endLength: number = value.end !== undefined && value.end !== null ? value.end.toString().length : this.toLabel.length;

        this.startValueElement.nativeElement.style.width = `${startLength}ch`;
        this.endValueElement.nativeElement.style.width = `${endLength}ch`;
    }

    private buildForm(): void {
        this.form = new FormGroup({
            startValueControl: new FormControl(),
            endValueControl: new FormControl()
        });
    }

    private getValue(): RangeInputData {
        return new RangeInputData({
            start: this.form.get('startValueControl')!.value ?? undefined,
            end: this.form.get('endValueControl')!.value ?? undefined
        });
    }

    private setupValidators(): void {
        const value: RangeInputData = this.getValue();

        const startValidators: ValidatorFn[] = [];
        const endValidators: ValidatorFn[] = [];

        if (this.min !== undefined) {
            startValidators.push(Validators.min(this.min));

            if (value.end !== undefined && !(value.end < this.min)) {
                startValidators.push(Validators.max(value.end));
            }
        }
        else {
            if (value.end !== undefined) {
                startValidators.push(Validators.max(value.end));
            }
        }

        if (this.max !== undefined) {
            endValidators.push(Validators.max(this.max));

            if (value.start !== undefined && !(value.start > this.max)) {
                endValidators.push(Validators.min(value.start));
            }
        }
        else {
            if (value.start !== undefined) {
                endValidators.push(Validators.min(value.start));
            }
        }

        if (this.required) {
            if (this.startRequired) {
                startValidators.push(Validators.required);
            }
            if (this.endRequired) {
                endValidators.push(Validators.required);
            }
        }

        this.form.get('startValueControl')!.setValidators(startValidators);
        this.form.get('endValueControl')!.setValidators(endValidators);

        this.form.updateValueAndValidity({ emitEvent: false, onlySelf: false });
    }
}
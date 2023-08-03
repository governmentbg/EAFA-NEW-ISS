import { FocusMonitor } from '@angular/cdk/a11y';
import { BooleanInput, coerceBooleanProperty } from '@angular/cdk/coercion';
import {
    AfterViewInit,
    Component,
    ElementRef,
    Inject,
    Input,
    OnDestroy,
    OnInit,
    Optional,
    Self,
    ViewChild
} from '@angular/core';
import {
    AbstractControl,
    ControlValueAccessor,
    FormControl,
    FormGroup,
    NgControl,
    Validators
} from '@angular/forms';
import { MatFormField, MatFormFieldControl, MAT_FORM_FIELD } from '@angular/material/form-field';
import { Subject } from 'rxjs';
import { CommonUtils } from '../../../utils/common.utils';
import { Coordinates } from './coordinates.model';



@Component({
    selector: 'coordinates-input',
    templateUrl: './coordinates-input.component.html',
    styleUrls: ['coordinates-input.component.scss'],
    providers: [{ provide: MatFormFieldControl, useExisting: CoordinatesInputComponent }],
    host: {
        '[class.example-floating]': 'shouldLabelFloat',
        '[id]': 'id',
    }
})
export class CoordinatesInputComponent implements ControlValueAccessor, MatFormFieldControl<string>, OnDestroy, OnInit, AfterViewInit {
    private static nextId = 0;
    private static readonly ASCII_COMMA = 44;
    private static readonly ASCII_PERIOD = 46;

    constructor(focusMonitor: FocusMonitor, elementRef: ElementRef<HTMLElement>,
        @Optional() @Inject(MAT_FORM_FIELD) formField: MatFormField,
        @Optional() @Self() ngControl: NgControl) {
        this._focusMonitor = focusMonitor;
        this._elementRef = elementRef;
        this._formField = formField;
        this.ngControl = ngControl;

        this.parts = new FormGroup({
            degreesControl: new FormControl('', [/*Validators.required, */Validators.minLength(1), Validators.maxLength(2)]),
            minutesControl: new FormControl('', [/*Validators.required, */Validators.minLength(2), Validators.maxLength(2)]),
            secondsControl: new FormControl('', [/*Validators.required, */Validators.minLength(1), Validators.maxLength(6)])
        });


        this._focusMonitor.monitor(this._elementRef, true).subscribe(origin => {
            if (this.focused && !origin) {
                this.onTouched();
            }
            this.focused = !!origin;
            this.stateChanges.next();
        });

        if (this.ngControl != null) {
            this.ngControl.valueAccessor = this;
        }
    }

    ngOnInit(): void {
        if (this.ngControl && this.ngControl.control) {
            const self = this;

            const markAsTouched = this.ngControl.control.markAsTouched;
            this.ngControl.control.markAsTouched = function (opts?: any) {
                markAsTouched.apply(this, opts);
                self.parts.markAllAsTouched();
            };

            const markAsUntouched = this.ngControl.control.markAsUntouched;
            this.ngControl.control.markAsUntouched = function (opts?: any) {
                markAsUntouched.apply(this, opts);
                self.parts.markAsUntouched();
            };

            const markAsDirty = this.ngControl.control.markAsDirty;
            this.ngControl.control.markAsDirty = function (opts?: any) {
                markAsDirty.apply(this, opts);
                self.parts.markAsDirty();
            };

            const markAsPristine = this.ngControl.control.markAsPristine;
            this.ngControl.control.markAsPristine = function (opts?: any) {
                markAsPristine.apply(this, opts);
                self.parts.markAsPristine();
            };
        }

    }

    ngAfterViewInit(): void {
        if (this._required) {
            this.parts.controls.degreesControl.setValidators([
                Validators.required,
                this.parts.controls.degreesControl.validator!
            ]);
            this.parts.controls.minutesControl.setValidators([
                Validators.required,
                this.parts.controls.minutesControl.validator!
            ]);
            this.parts.controls.secondsControl.setValidators([
                Validators.required,
                this.parts.controls.secondsControl.validator!
            ]);
            this.parts.controls.degreesControl.updateValueAndValidity();
            this.parts.controls.minutesControl.updateValueAndValidity();
            this.parts.controls.secondsControl.updateValueAndValidity();
        }
    }

    public autofilled?: boolean | undefined;

    @ViewChild('degrees') public degreesInput: ElementRef<HTMLInputElement> | undefined;
    @ViewChild('minutes') public minutesInput: ElementRef<HTMLInputElement> | undefined;
    @ViewChild('seconds') public secondsInput: ElementRef<HTMLInputElement> | undefined;

    private _focusMonitor: FocusMonitor;
    private _elementRef: ElementRef<HTMLElement>;
    private coordinates: Coordinates | undefined;
    public _formField: MatFormField;
    public ngControl: NgControl;
    public parts: FormGroup;
    public stateChanges = new Subject<void>();
    public focused = false;
    public controlType = 'coordinates-input';
    public id = `coordinates-input-${CoordinatesInputComponent.nextId++}`;
    public onChange = (_: any) => { /**/ };
    public onTouched = () => { /**/ };

    public get empty(): any {
        return this.coordinates == undefined || this.coordinates.isEmpty();
    }

    public get shouldLabelFloat(): any {
        return this.focused || !this.empty;
    }

    @Input('aria-describedby') userAriaDescribedBy: string | undefined;


    public get placeholder(): string {
        return this._placeholder as string;
    }

    @Input() public set placeholder(value: string) {
        this._placeholder = value;
        this.stateChanges.next();
    }

    private _placeholder: string | undefined;

    public get required(): boolean {
        return this._required;
    }

    @Input() public set required(value: boolean) {
        this._required = coerceBooleanProperty(value);
        this.stateChanges.next();
    }

    private _required = false;

    public get disabled(): boolean {
        return this._disabled;
    }

    @Input() public set disabled(value: boolean) {
        this._disabled = coerceBooleanProperty(value);
        this._disabled ? this.parts.disable() : this.parts.enable();
        this.stateChanges.next();
    }

    private _disabled = false;


    public get value(): string | null {
        if (this.parts.valid) {
            return `${this.parts.controls.degreesControl.value} ${this.parts.controls.minutesControl.value} ${this.parts.controls.secondsControl.value}`;
        }

        return null;
    }

    @Input() public set value(_value: string | null) {
        const coordinates = this.convertToCoordinates(_value);
        if (!coordinates.equals(this.coordinates)) {
            this.coordinates = coordinates;
            this.parts.setValue({
                degreesControl: this.coordinates.degrees,
                minutesControl: this.coordinates.minutes,
                secondsControl: !CommonUtils.isNullOrEmpty(this.coordinates.seconds)
                    ? Number(parseFloat(this.coordinates.seconds).toFixed(2))
                    : this.coordinates.seconds
            });
            this.stateChanges.next();
        }
    }

    public get errorState(): boolean {
        return this.parts.touched && this._required && this.parts.invalid;
    }

    public autoFocusNext(control: AbstractControl, element: HTMLInputElement | undefined, nextElement: ElementRef<HTMLInputElement> | undefined): void {
        if (!control.errors && nextElement != undefined && element != undefined && element.value?.length === Number(element.getAttribute('maxlength'))) {
            this._focusMonitor.focusVia(nextElement, 'program');
        }
    }

    public autoFocusPrev(control: AbstractControl, prevElement: ElementRef<HTMLInputElement> | undefined): void {
        if (control.value.length < 1 && prevElement != undefined) {
            this._focusMonitor.focusVia(prevElement, 'program');
        }
    }

    public ngOnDestroy(): void {
        this.stateChanges.complete();
        this._focusMonitor.stopMonitoring(this._elementRef);
    }

    public setDescribedByIds(ids: string[]): void {
        const controlElement = this._elementRef.nativeElement
            .querySelector('.input-container')!;
        controlElement.setAttribute('aria-describedby', ids.join(' '));
    }

    public onContainerClick(): void {
        //if (this.parts.controls.degreesControl.valid) {
        //    this._focusMonitor.focusVia(this.minutesInput as HTMLInputElement, 'program');
        //} else if (this.parts.controls.minutesControl.valid) {
        //    this._focusMonitor.focusVia(this.secondsInput as HTMLInputElement, 'program');
        //} else if (this.parts.controls.secondsControl.valid) {
        //    this._focusMonitor.focusVia(this.minutesInput as HTMLInputElement, 'program');
        //}
    }

    public onIntegerKeyPress(event: any): boolean {
        const charCode = (event.query) ? event.query : event.keyCode;
        //Allow only ASCII codes for number inputs
        if ((charCode >= 0 && charCode <= 31)
            || (charCode >= 48 && charCode <= 57)) {
            return true;
        } else {
            return false;
        }
    }

    public onFloatKeyPress(event: any): boolean {
        const charCode = (event.query) ? event.query : event.keyCode;
        //Allow only ASCII codes for number inputs
        if ((charCode >= 0 && charCode <= 31)
            || (charCode >= 48 && charCode <= 57)) {
            return true;
        } else if (charCode == CoordinatesInputComponent.ASCII_PERIOD
            || charCode == CoordinatesInputComponent.ASCII_COMMA) {
            const value = (this.parts.controls.secondsControl.value as string);

            if (value == undefined || value == '') {
                return false;
            } else {
                const periodIndex = (this.parts.controls.secondsControl.value as string).indexOf('.');
                const commaIndex = (this.parts.controls.secondsControl.value as string).indexOf(',');
                return periodIndex == -1 && commaIndex == -1 ? true : false;
            }
        }
        else {
            return false;
        }
    }

    public writeValue(_value: string | null): void {
        this.value = _value;
    }

    public registerOnChange(fn: any): void {
        this.onChange = fn;
    }

    public registerOnTouched(fn: any): void {
        this.onTouched = fn;
    }

    public setDisabledState(isDisabled: boolean): void {
        this.disabled = isDisabled;

        if (this.disabled) {
            this.parts.disable();
        }
        else {
            this.parts.enable();
        }
    }

    public _handleInput(control: AbstractControl, element: ElementRef<HTMLInputElement> | undefined, nextElement?: ElementRef<HTMLInputElement> | undefined): void {
        if (nextElement != undefined) {
            this.autoFocusNext(control, element?.nativeElement, nextElement);
        }
        this.onChange(this.value);
    }

    private convertToCoordinates(value: string | undefined | null): Coordinates {

        if (value != undefined && value != null && value != '') {
            value = value.trim();
            if (value != '') {
                const values: string[] = value.split(' ');
                return new Coordinates(values[0], values[1], values[2]);
            } else {
                return new Coordinates('', '', '');
            }
        } else {
            return new Coordinates('', '', '');
        }
    }

    static ngAcceptInputType_disabled: BooleanInput;
    static ngAcceptInputType_required: BooleanInput;
}

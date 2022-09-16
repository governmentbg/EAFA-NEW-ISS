import { AfterViewInit, Component, DoCheck, Input, OnInit, Self, ViewChild } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormControl, FormGroup, NgControl, ValidationErrors, Validator, ValidatorFn } from '@angular/forms';
import { StatisticalFormEmployeeInfoDTO } from '@app/models/generated/dtos/StatisticalFormEmployeeInfoDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLValidators } from '@app/shared/utils/tl-validators';

@Component({
    selector: 'statistical-forms-employees-info-table',
    templateUrl: './statistical-forms-employees-info-table.component.html'
})
export class StatisticalFormsEmployeesInfoTableComponent implements OnInit, AfterViewInit, DoCheck, ControlValueAccessor, Validator {
    @Input()
    public cardLabel: string | undefined;

    @Input()
    public infoTypeColumnLabel: string | undefined;

    public employeeInfo: StatisticalFormEmployeeInfoDTO[] = [];

    public formGroup: FormGroup;
    public isDisabled: boolean = false;

    public translate: FuseTranslationLoaderService;

    private ngControl: NgControl;
    private onChanged: (value: StatisticalFormEmployeeInfoDTO[]) => void = (value: StatisticalFormEmployeeInfoDTO[]) => { return; };
    private onTouched: (value: StatisticalFormEmployeeInfoDTO[]) => void = (value: StatisticalFormEmployeeInfoDTO[]) => { return; };

    public constructor(
        @Self() ngControl: NgControl,

        translate: FuseTranslationLoaderService
    ) {
        this.ngControl = ngControl;
        this.ngControl.valueAccessor = this;

        this.translate = translate;

        this.formGroup = new FormGroup({});
    }

    public ngOnInit(): void {
        if (this.ngControl.control) {
            this.ngControl.control.validator = this.validate.bind(this);
        }
    }

    public ngAfterViewInit(): void {
        this.formGroup.valueChanges.subscribe({
            next: (value: any) => {
                if (value !== null && value !== undefined) {
                    for (const count of this.employeeInfo) {
                        const menWithPayControlName: string = count.code! + 'menWithPayControl';
                        const menWithoutPayControlName: string = count.code! + 'menWithoutPayControl';
                        const womenWithPayControlName: string = count.code! + 'womenWithPayControl';
                        const womenWithoutPayControlName: string = count.code! + 'womenWithoutPayControl';

                        if (this.formGroup.controls[menWithPayControlName] !== undefined && value[menWithPayControlName] !== null) {
                            count.menWithPay = value[menWithPayControlName] !== '' ? value[menWithPayControlName] : null;
                        }
                        if (this.formGroup.controls[menWithoutPayControlName] !== undefined && value[menWithoutPayControlName] !== null) {
                            count.menWithoutPay = value[menWithoutPayControlName] !== '' ? value[menWithoutPayControlName] : null;
                        }
                        if (this.formGroup.controls[womenWithPayControlName] !== undefined && value[womenWithPayControlName] !== null) {
                            count.womenWithPay = value[womenWithPayControlName] !== '' ? value[womenWithPayControlName] : null;
                        }
                        if (this.formGroup.controls[womenWithoutPayControlName] !== undefined && value[womenWithoutPayControlName] !== null) {
                            count.womenWithoutPay = value[womenWithoutPayControlName] !== '' ? value[womenWithoutPayControlName] : null;
                        }
                    }
                }
                else {
                    for (const count of this.employeeInfo) {
                        count.menWithPay = undefined;
                        count.menWithoutPay = undefined;
                        count.womenWithPay = undefined;
                        count.womenWithoutPay = undefined;
                    }
                }
                this.formGroup.updateValueAndValidity({ emitEvent: false });
                this.onChanged(this.employeeInfo);
            }
        });
    }

    public writeValue(value: StatisticalFormEmployeeInfoDTO[]): void {
        if (value !== null && value !== undefined) {
            this.employeeInfo = value;
            this.buildForm(this.employeeInfo);
            this.setDisabledState(this.isDisabled);
            this.onChanged(value);
        }
    }

    public registerOnChange(fn: (value: StatisticalFormEmployeeInfoDTO[]) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: StatisticalFormEmployeeInfoDTO[]) => void): void {
        this.onTouched = fn;
    }

    public ngDoCheck(): void {
        if (this.ngControl?.control?.touched) {
            this.formGroup.markAllAsTouched();
        }
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        let errors: ValidationErrors = {};
        if (this.formGroup.errors !== null && this.formGroup.errors !== undefined) {
            errors = this.formGroup.errors;
        }

        Object.keys(this.formGroup.controls).forEach((key: string) => {
            const controlErrors: ValidationErrors | null = this.formGroup.controls[key].errors;
            if (controlErrors !== null) {
                errors[key] = controlErrors;
            }
        });

        return Object.keys(errors).length > 0 ? errors : null;
    }

    public setDisabledState(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
        if (this.isDisabled) {
            this.formGroup.disable();
        }
        else {
            this.formGroup.enable();
        }
    }

    private payCountValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            for (const row of this.employeeInfo) {
                if (row.menWithoutPay !== undefined && row.menWithoutPay !== null) {
                    return null;
                }
                if (row.menWithPay !== undefined && row.menWithPay !== null) {
                    return null;
                }
                if (row.womenWithoutPay !== undefined && row.womenWithoutPay !== null) {
                    return null;
                }
                if (row.womenWithPay !== undefined && row.womenWithPay !== null) {
                    return null;
                }
            }
            return { 'payCountValidationError': true };
        };
    }

    private buildForm(employeeInfo: StatisticalFormEmployeeInfoDTO[]): void {
        for (const row of employeeInfo) {
            const menWithPayControlName: string = row.code! + 'menWithPayControl';
            const menWithoutPayControlName: string = row.code! + 'menWithoutPayControl';
            const womenWithPayControlName: string = row.code! + 'womenWithPayControl';
            const womenWithoutPayControlName: string = row.code! + 'womenWithoutPayControl';
            this.formGroup.addControl(menWithPayControlName, new FormControl(row.menWithPay, TLValidators.number(0)));
            this.formGroup.addControl(menWithoutPayControlName, new FormControl(row.menWithoutPay, TLValidators.number(0)));
            this.formGroup.addControl(womenWithPayControlName, new FormControl(row.womenWithPay, TLValidators.number(0)));
            this.formGroup.addControl(womenWithoutPayControlName, new FormControl(row.womenWithoutPay, TLValidators.number(0)));
        }

        this.formGroup.setValidators(this.payCountValidator());
    }
}
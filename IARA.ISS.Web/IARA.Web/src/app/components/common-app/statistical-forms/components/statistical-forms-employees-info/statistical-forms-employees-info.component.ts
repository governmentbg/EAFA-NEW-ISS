import { Component, DoCheck, OnInit, Self } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormControl, FormGroup, NgControl, ValidationErrors, Validator } from '@angular/forms';
import { StatisticalFormEmployeeInfoDTO } from '@app/models/generated/dtos/StatisticalFormEmployeeInfoDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { StatisticalFormEmployeeInfoGroupDTO } from '@app/models/generated/dtos/StatisticalFormEmployeeInfoGroupDTO';

@Component({
    selector: 'statistical-forms-employees-info',
    templateUrl: './statistical-forms-employees-info.component.html'
})
export class StatisticalFormsEmployeesInfoComponent implements ControlValueAccessor, OnInit, DoCheck, Validator {
    public form!: FormGroup;
    public employeesWorkDays: StatisticalFormEmployeeInfoDTO[] = [];
    public employeeAge: StatisticalFormEmployeeInfoDTO[] = [];
    public employeesEducation: StatisticalFormEmployeeInfoDTO[] = [];
    public employeesNationality: StatisticalFormEmployeeInfoDTO[] = [];

    public isDisabled: boolean = false;
    public translate: FuseTranslationLoaderService;
    public employeeInfoGroups: StatisticalFormEmployeeInfoGroupDTO[] = [];

    private onChanged: (value: StatisticalFormEmployeeInfoGroupDTO[]) => void = (value: StatisticalFormEmployeeInfoGroupDTO[]) => { return; };
    private onTouched: (value: StatisticalFormEmployeeInfoGroupDTO[]) => void = (value: StatisticalFormEmployeeInfoGroupDTO[]) => { return; };
    private ngControl: NgControl;

    public constructor(
        @Self() ngControl: NgControl,

        translate: FuseTranslationLoaderService
    ) {
        this.ngControl = ngControl;
        this.ngControl.valueAccessor = this;

        this.translate = translate;

        this.buildForm();
    }

    public ngOnInit(): void {
        if (this.ngControl.control) {
            this.ngControl.control.validator = this.validate.bind(this);
        }

        this.form.valueChanges.subscribe({
            next: (employeeInfoGroups: StatisticalFormEmployeeInfoGroupDTO[]) => {
                this.employeeInfoGroups = employeeInfoGroups;
                this.onChanged(employeeInfoGroups);
            }
        });
    }

    public ngDoCheck(): void {
        if (this.ngControl?.control?.touched) {
            this.form.markAllAsTouched();
        }
    }

    public writeValue(value: StatisticalFormEmployeeInfoGroupDTO[]): void {
        if (value !== null && value !== undefined) {
            this.employeeInfoGroups = value;
            this.setEmployeeInfo();
            this.fillForm();

            this.onChanged(value);
        }
    }

    public registerOnChange(fn: (value: StatisticalFormEmployeeInfoGroupDTO[]) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: StatisticalFormEmployeeInfoGroupDTO[]) => void): void {
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
        let errors: ValidationErrors = {};

        Object.keys(this.form.controls).forEach((key: string) => {
            const controlErrors: ValidationErrors | null = this.form.controls[key].errors;
            if (controlErrors !== null) {
                errors[key] = controlErrors;
            }
        });

        return Object.keys(errors).length > 0 ? errors : null;
    }


    private buildForm(): void {
        this.form = new FormGroup({
            workDaysControl: new FormControl(),
            ageControl: new FormControl(),
            educationControl: new FormControl(),
            nationalityControl: new FormControl()
        });
    }

    private setEmployeeInfo(): void {
        this.employeesWorkDays = [];
        this.employeesNationality = [];
        this.employeeAge = [];
        this.employeesEducation = [];
        for (const workDay of this.employeeInfoGroups![0].employeeTypes!) {
            this.employeesWorkDays.push(workDay);
        }
        for (const nationality of this.employeeInfoGroups![1].employeeTypes!) {
            this.employeesNationality.push(nationality);
        }
        for (const age of this.employeeInfoGroups![2].employeeTypes!) {
            this.employeeAge.push(age);
        }
        for (const education of this.employeeInfoGroups![3].employeeTypes!) {
            this.employeesEducation.push(education);
        }
    }

    private getValue(): StatisticalFormEmployeeInfoGroupDTO[] {
        this.employeeInfoGroups[0].employeeTypes = this.form.get('workDaysControl')!.value;


        return this.employeeInfoGroups;
    }

    private fillForm() {
        this.form.get('workDaysControl')!.setValue(this.employeesWorkDays);
        this.form.get('ageControl')!.setValue(this.employeeAge);
        this.form.get('nationalityControl')!.setValue(this.employeesNationality);
        this.form.get('educationControl')!.setValue(this.employeesEducation);
    }
}
import { Component, Input, OnInit, Optional, Self } from '@angular/core';
import { AbstractControl, FormArray, FormControl, NgControl } from '@angular/forms';
import { InspectedPersonTypeEnum } from '@app/enums/inspected-person-type.enum';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { InspectionSubjectPersonnelDTO } from '@app/models/generated/dtos/InspectionSubjectPersonnelDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';

@Component({
    selector: 'inspected-persons-array',
    templateUrl: './inspected-persons-array.component.html'
})
export class InspectedPersonsArrayComponent extends CustomFormControl<InspectionSubjectPersonnelDTO[]> implements OnInit {
    @Input()
    public viewMode: boolean = false;

    @Input()
    public title!: string;

    @Input()
    public personType!: InspectedPersonTypeEnum;

    @Input()
    public countries: NomenclatureDTO<number>[] = [];

    public constructor(
        @Self() ngControl: NgControl,
        @Self() @Optional() validityChecker: ValidityCheckerDirective
    ) {
        super(ngControl, true, validityChecker);
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(values: InspectionSubjectPersonnelDTO[]): void {
        this.fillForm(values);
    }

    public addPersonControl(): void {
        const inspectedPerson: InspectionSubjectPersonnelDTO = new InspectionSubjectPersonnelDTO({
            isActive: true
        });

        const control: FormControl = new FormControl(inspectedPerson);
        this.formArray.push(control);

        this.onChanged(this.form.value);
    }

    public deletePersonControl(personControl: FormControl, index: number): void {
        this.formArray.removeAt(index);
        this.onChanged(this.form.value);

        if (this.formArray.value === null || this.formArray.value === undefined || this.formArray.value.length === 0) {
            this.buildFormArray();
        }
    }

    protected getValue(): InspectionSubjectPersonnelDTO[] {
        return this.formArray.value;
    }

    protected buildForm(): AbstractControl {
        return new FormArray([]);
    }

    private fillForm(values: InspectionSubjectPersonnelDTO[]): void {
        if (values === null || values === undefined || values.length === 0) {
            this.reset();
        }
        else {
            this.formArray.clear();
        }

        if (values) {
            this.fillControls(values);
            this.setDisabledState(this.isDisabled);
        }
    }

    private reset(): void {
        this.formArray.clear();
        this.buildFormArray();
    }

    private buildFormArray(): void {
        const control: FormControl = new FormControl(
            new InspectionSubjectPersonnelDTO({
                isActive: true
            })
        );

        this.formArray.push(control);

        this.onChanged(this.getValue());
    }

    private fillControls(values: InspectionSubjectPersonnelDTO[]): void {
        for (const value of values) {
            const newControl: FormControl = new FormControl(value);
            this.formArray.push(newControl);
        }

        this.onChanged(this.getValue());
    }
}
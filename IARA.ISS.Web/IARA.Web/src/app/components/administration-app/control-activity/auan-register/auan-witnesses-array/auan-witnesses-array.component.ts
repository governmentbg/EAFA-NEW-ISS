import { Component, Input, OnInit, Self } from '@angular/core';
import { AbstractControl, FormArray, FormControl, NgControl } from '@angular/forms';
import { AuanWitnessDTO } from '@app/models/generated/dtos/AuanWitnessDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';

@Component({
    selector: 'auan-witnesses-array',
    templateUrl: './auan-witnesses-array.component.html'
})
export class AuanWitnessesArrayComponent extends CustomFormControl<AuanWitnessDTO[]> implements OnInit {
    @Input() public viewMode!: boolean;

    public isDisabled: boolean = false;

    public constructor(
        @Self() ngControl: NgControl
    ) {
        super(ngControl);
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(values: AuanWitnessDTO[]): void {
        this.fillForm(values);
    }

    public addWitnessControl(): void {
        const catchAquaticOrganismType: AuanWitnessDTO = new AuanWitnessDTO({
            isActive: true
        });

        const control: FormControl = new FormControl(catchAquaticOrganismType);
        this.formArray.push(control);

        this.onChanged(this.form.value);
    }

    public deleteWitnessControl(witnessControl: FormControl, index: number): void {
        this.formArray.removeAt(index);
        this.onChanged(this.form.value);

        if (this.formArray.value === null || this.formArray.value === undefined || this.formArray.value.length === 0) {
            this.buildFormArray();
        }
    }

    protected getValue(): AuanWitnessDTO[] {
        return this.formArray.value;
    }

    protected buildForm(): AbstractControl {
        return new FormArray([]);
    }

    private fillForm(values: AuanWitnessDTO[]): void {
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
            new AuanWitnessDTO({
                isActive: true
            })
        );

        this.formArray.push(control);

        this.onChanged(this.getValue());
    }

    private fillControls(witnesses: AuanWitnessDTO[]): void {
        for (const witness of witnesses) {
            const newControl: FormControl = new FormControl(witness);
            this.formArray.push(newControl);
        }

        this.onChanged(this.getValue());
    }
}
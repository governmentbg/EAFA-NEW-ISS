import { Component, Input, OnInit, Optional, Self } from '@angular/core';
import { AbstractControl, FormArray, FormControl, NgControl } from '@angular/forms';
import { InspectedDeclarationCatchDTO } from '@app/models/generated/dtos/InspectedDeclarationCatchDTO';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { FishNomenclatureDTO } from '@app/models/generated/dtos/FishNomenclatureDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

@Component({
    selector: 'inspected-catches-array',
    templateUrl: './inspected-catches-array.component.html'
})
export class InspectedCatchesArrayComponent extends CustomFormControl<InspectedDeclarationCatchDTO[]> implements OnInit {
    @Input()
    public viewMode: boolean = false;

    @Input()
    public fishes: FishNomenclatureDTO[] = []; 

    @Input()
    public catchTypes: NomenclatureDTO<number>[] = [];

    @Input()
    public presentations: NomenclatureDTO<number>[] = [];

    @Input()
    public turbotSizeGroups: NomenclatureDTO<number>[] = [];

    @Input()
    public hasCatchType: boolean = true;

    @Input()
    public hasUndersizedCheck: boolean = false;

    @Input()
    public hasDeclaration: boolean = false;

    public constructor(
        @Self() ngControl: NgControl,
        @Self() @Optional() validityChecker: ValidityCheckerDirective
    ) {
        super(ngControl, true, validityChecker);
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(values: InspectedDeclarationCatchDTO[]): void {
        this.fillForm(values);
    }

    public addCatchControl(): void {
        const inspectedCatch: InspectedDeclarationCatchDTO = new InspectedDeclarationCatchDTO({
            //   isActive: true
        });

        const control: FormControl = new FormControl(inspectedCatch);
        this.formArray.push(control);

        this.onChanged(this.form.value);
    }

    public deleteCatchControl(catchControl: InspectedDeclarationCatchDTO, index: number): void {
        this.formArray.removeAt(index);
        this.onChanged(this.form.value);

        if (this.formArray.value === null || this.formArray.value === undefined || this.formArray.value.length === 0) {
            this.buildFormArray();
        }
    }

    protected getValue(): InspectedDeclarationCatchDTO[] {
        return this.formArray.value;
    }

    protected buildForm(): AbstractControl {
        return new FormArray([]);
    }

    private fillForm(values: InspectedDeclarationCatchDTO[]): void {
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
            new InspectedDeclarationCatchDTO({
                //TODO isActive?
            })
        );

        this.formArray.push(control);

        this.onChanged(this.getValue());
    }

    private fillControls(values: InspectedDeclarationCatchDTO[]): void {
        for (const value of values) {
            const newControl: FormControl = new FormControl(value);
            this.formArray.push(newControl);
        }

        this.onChanged(this.getValue());
    }
}
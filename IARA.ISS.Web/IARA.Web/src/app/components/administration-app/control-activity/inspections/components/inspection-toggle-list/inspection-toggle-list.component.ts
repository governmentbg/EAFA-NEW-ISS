import { Component, Input, OnChanges, OnInit, Self, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { InspectionToggleTypesEnum } from '@app/enums/inspection-toggle-types.enum';
import { InspectionCheckDTO } from '@app/models/generated/dtos/InspectionCheckDTO';
import { InspectionCheckModel } from '../../models/inspection-check.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { InspectionCheckTypesEnum } from '@app/enums/inspection-check-types.enum';
import { InspectionUtils } from '@app/shared/utils/inspection.utils';

@Component({
    selector: 'inspection-toggle-list',
    templateUrl: './inspection-toggle-list.component.html'
})
export class InspectionToggleListComponent extends CustomFormControl<InspectionCheckDTO[] | undefined> implements OnInit, OnChanges {

    @Input()
    public toggles: InspectionCheckModel[] = [];

    public readonly boolOptions: NomenclatureDTO<InspectionToggleTypesEnum>[] = [];
    public readonly tripleOptions: NomenclatureDTO<InspectionToggleTypesEnum>[] = [];

    public readonly inspectionCheckTypesEnum: typeof InspectionCheckTypesEnum = InspectionCheckTypesEnum;

    private values: InspectionCheckDTO[] = [];

    public constructor(@Self() ngControl: NgControl, translate: FuseTranslationLoaderService) {
        super(ngControl);

        this.boolOptions = InspectionUtils.getToggleBoolOptions(translate);
        this.tripleOptions = InspectionUtils.getToggleTripleOptions(translate);

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.control.updateValueAndValidity();
            }
        });

        this.control.valueChanges.subscribe({
            next: () => {
                this.onChanged(this.getValue());
            }
        });
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const toggles = changes['toggles'];

        if (toggles !== null && toggles !== undefined) {
            const keys = Object.keys(this.form.controls);

            for (let i = 0; i < keys.length; i++) {
                this.form.removeControl(keys[i]);
            }

            const currentToggles = toggles.currentValue as InspectionCheckModel[];
            const length = currentToggles.length;

            for (let i = 0; i < length; i++) {
                const found = this.values.find(f => f.checkTypeId === currentToggles[i].value);

                this.form.addControl(i.toString(), new FormControl({
                    value: found,
                    disabled: this.isDisabled
                }));
            }
        }
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(value: InspectionCheckDTO[] | undefined): void {
        if (value !== null && value !== undefined) {
            this.values = value;
            const foundIndexex: number[] = [];

            for (const check of value) {
                const index = this.toggles.findIndex(f => f.value === check.checkTypeId);

                if (index >= 0 && this.form.controls[index.toString()] !== undefined) {
                    this.form.controls[index.toString()].setValue(check);
                    foundIndexex.push(index);
                }
            }

            for (let i = 0; i < this.toggles.length; i++) {
                if (!foundIndexex.includes(i) && this.form.controls[i.toString()] !== undefined) {
                    this.form.controls[i.toString()].setValue(undefined);
                }
            }
        }
        else {
            this.values = [];
            const keys = Object.keys(this.form.controls);

            for (let i = 0; i < keys.length; i++) {
                this.form.controls[keys[i]].setValue(undefined);
            }
        }
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({});
    }

    protected getValue(): InspectionCheckDTO[] | undefined {
        const values: InspectionCheckDTO[] = [];

        const keys = Object.keys(this.form.controls);

        for (let i = 0; i < keys.length; i++) {
            const value: InspectionCheckDTO | undefined = this.form.controls[keys[i]].value;

            if (value !== null && value !== undefined) {
                values.push(value);
            }
        }

        return values;
    }
}
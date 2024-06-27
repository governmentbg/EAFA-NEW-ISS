import { Component, Input, OnChanges, OnInit, Self, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { InspectionToggleTypesEnum } from '@app/enums/inspection-toggle-types.enum';
import { InspectionCheckDTO } from '@app/models/generated/dtos/InspectionCheckDTO';
import { InspectionCheckModel } from '../../models/inspection-check.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

@Component({
    selector: 'inspection-toggle',
    templateUrl: './inspection-toggle.component.html'
})
export class InspectionToggleComponent extends CustomFormControl<InspectionCheckDTO | undefined> implements OnInit, OnChanges {

    @Input()
    public toggle!: InspectionCheckModel;

    @Input()
    public options!: NomenclatureDTO<InspectionToggleTypesEnum>[];

    public fullOptions!: NomenclatureDTO<InspectionToggleTypesEnum>[];
    public errors: string[] = [];

    public model: InspectionCheckDTO | undefined;

    public readonly inspectionToggleTypesEnum: typeof InspectionToggleTypesEnum = InspectionToggleTypesEnum;

    public constructor(@Self() ngControl: NgControl) {
        super(ngControl);

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
        const options = changes['options'];
        const toggle = changes['toggle'];

        if (options !== null && options !== undefined) {
            this.fullOptions = Object.create(options.currentValue);
            this.fullOptions.push(new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.R,
                displayName: 'X'
            }));
        }

        if (toggle !== null && toggle !== undefined && toggle.currentValue !== null && toggle.currentValue !== undefined) {
            if ((toggle.currentValue as InspectionCheckModel).isMandatory === true) {
                this.form.get('togglesControl')!.setValidators([Validators.required]);
                this.form.get('togglesControl')!.updateValueAndValidity({ emitEvent: false });
            }
        }
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(value: InspectionCheckDTO | undefined): void {
        this.model = value;

        if (value !== null && value !== undefined) {
            this.form.get('togglesControl')!.setValue(this.options.find(f => f.value === value.checkValue)?.value);
            this.form.get('toggleTextControl')!.setValue(value.description);
        }
        else {
            this.form.get('togglesControl')!.setValue(null);
            this.form.get('toggleTextControl')!.setValue(null);
        }

        this.onChanged(this.getValue());
    }

    protected buildForm(): AbstractControl {
        const form = new FormGroup({
            togglesControl: new FormControl(undefined),
            toggleTextControl: new FormControl({ value: undefined, disabled: true }),
        });

        form.get('togglesControl')!.valueChanges.subscribe({
            next: this.onTogglesChanged.bind(this)
        });

        return form;
    }

    protected getValue(): InspectionCheckDTO | undefined {
        const checkValue: InspectionToggleTypesEnum = this.form.get('togglesControl')!.value;

        if (checkValue === null || checkValue === undefined) {
            return undefined;
        }

        const check = new InspectionCheckDTO({
            id: this.model?.id,
            checkTypeId: this.toggle.value,
            checkValue: checkValue,
            description: this.form.get('toggleTextControl')!.value,
        });

        return check;
    }

    private onTogglesChanged(value?: InspectionToggleTypesEnum): void {
        if (value !== null && value !== undefined) {
            if (value === InspectionToggleTypesEnum.R) {
                this.form.get('togglesControl')!.setValue(null);
                this.form.get('toggleTextControl')!.disable();
            }
            else if (value === InspectionToggleTypesEnum.Y) {
                this.form.get('toggleTextControl')!.enable();
            }
            else {
                this.form.get('toggleTextControl')!.disable();
            }

            if (this.isDisabled) {
                this.form.get('toggleTextControl')!.disable();
            }
        }
    }
}
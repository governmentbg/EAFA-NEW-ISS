import { Component, Input, OnChanges, OnInit, Self, SimpleChange, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';

import { ChangeOfCircumstancesDataTypeEnum } from '@app/enums/change-of-circumstances-data-type.enum';
import { ChangeOfCircumstancesDTO } from '@app/models/generated/dtos/ChangeOfCircumstancesDTO';
import { ChangeOfCircumstancesTypeDTO } from '@app/models/generated/dtos/ChangeOfCircumstancesTypeDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';

@Component({
    selector: 'single-change-of-circumstances',
    templateUrl: './single-change-of-circumstances.component.html'
})
export class SingleChangeOfCircumstancesComponent extends CustomFormControl<ChangeOfCircumstancesDTO> implements OnInit, OnChanges {
    @Input()
    public changeTypes: ChangeOfCircumstancesTypeDTO[] = [];

    @Input()
    public showOnlyRegiXData: boolean = false;

    public readonly dataTypes: typeof ChangeOfCircumstancesDataTypeEnum = ChangeOfCircumstancesDataTypeEnum;

    private id: number | undefined;

    public constructor(@Self() ngControl: NgControl) {
        super(ngControl);

        this.form.get('changeTypeControl')!.valueChanges.subscribe({
            next: (type: ChangeOfCircumstancesTypeDTO | undefined) => {
                this.resetControls(type);
            }
        });
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        if (this.showOnlyRegiXData === true) {
            this.form.get('changeTypeControl')!.disable({ emitEvent: false });
            this.form.get('descriptionControl')!.disable({ emitEvent: false });
        }

        this.form.valueChanges.subscribe({
            next: () => {
                this.onChanged(this.getValue());
            }
        });
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const showOnlyRegiXData: SimpleChange | undefined = changes['showOnlyRegiXData'];

        if (showOnlyRegiXData !== undefined && showOnlyRegiXData !== null) {
            if (this.showOnlyRegiXData === true) {
                this.form.get('changeTypeControl')!.disable({ emitEvent: false });
                this.form.get('descriptionControl')!.disable({ emitEvent: false });
            }
        }
    }

    public writeValue(value: ChangeOfCircumstancesDTO): void {
        if (value !== undefined && value !== null) {
            this.id = value.id;
            this.form.get('changeTypeControl')!.setValue(this.changeTypes.find(x => x.value === value.typeId));
            this.form.get('descriptionControl')!.setValue(value.description);
            this.form.get('personControl')!.setValue(value.person);
            this.form.get('legalControl')!.setValue(value.legal);
            this.form.get('addressControl')!.setValue(value.address);
        }

        setTimeout(() => {
            this.onChanged(this.getValue());
        });
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            changeTypeControl: new FormControl(null, Validators.required),
            descriptionControl: new FormControl(null),
            personControl: new FormControl(null),
            legalControl: new FormControl(null),
            addressControl: new FormControl(null)
        });
    }

    protected getValue(): ChangeOfCircumstancesDTO {
        const result = new ChangeOfCircumstancesDTO({
            id: this.id,
            typeId: this.form.get('changeTypeControl')!.value?.value,
            dataType: this.form.get('changeTypeControl')!.value?.dataType,
            description: this.form.get('descriptionControl')!.value,
        });

        switch (result.dataType) {
            case ChangeOfCircumstancesDataTypeEnum.Person:
                result.person = this.form.get('personControl')!.value;
                break;
            case ChangeOfCircumstancesDataTypeEnum.Legal:
                result.legal = this.form.get('legalControl')!.value;
                break;
            case ChangeOfCircumstancesDataTypeEnum.Address:
                result.address = this.form.get('addressControl')!.value;
                break;
        }

        return result;
    }

    private resetControls(type: ChangeOfCircumstancesTypeDTO | undefined): void {
        if (type !== undefined && type !== null) {
            if (type.dataType === ChangeOfCircumstancesDataTypeEnum.FreeText) {
                this.form.get('descriptionControl')!.setValidators(Validators.required);
                this.form.get('descriptionControl')!.markAsPending();
            }
            else {
                this.form.get('descriptionControl')!.clearValidators();
            }
        }
        else {
            this.form.get('descriptionControl')!.clearValidators();
        }

        this.form.get('descriptionControl')!.updateValueAndValidity();
    }
}
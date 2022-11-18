import { Component, Input, OnInit, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { PortVisitDTO } from '@app/models/generated/dtos/PortVisitDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';

@Component({
    selector: 'inspected-port',
    templateUrl: './inspected-port.component.html'
})
export class InspectedPortComponent extends CustomFormControl<PortVisitDTO | undefined> implements OnInit {

    @Input()
    public ports: NomenclatureDTO<number>[] = [];

    @Input()
    public countries: NomenclatureDTO<number>[] = [];

    public isFromRegister: boolean = true;

    public constructor(@Self() ngControl: NgControl) {
        super(ngControl);

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.control.updateValueAndValidity();
            }
        });
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(value: PortVisitDTO | undefined): void {
        if (value !== undefined && value !== null) {
            this.isFromRegister = value.portId !== undefined && value.portId !== null;
            this.form.get('portRegisteredControl')!.setValue(this.isFromRegister);

            if (this.isFromRegister) {
                this.form.get('portControl')!.setValue(this.ports.find(f => f.value === value.portId));
            }
            else {
                this.form.get('nameControl')!.setValue(value.portName);
                this.form.get('countryControl')!.setValue(this.countries.find(f => f.value === value.portCountryId));
            }
        }
        else {
            this.form.get('countryControl')!.setValue(this.countries.find(f => f.code === CommonUtils.COUNTRIES_BG));
        }
    }

    protected buildForm(): AbstractControl {
        const form = new FormGroup({
            portRegisteredControl: new FormControl(true),
            portControl: new FormControl(undefined, Validators.required),
            nameControl: new FormControl(undefined),
            countryControl: new FormControl(undefined),
        });

        form.get('portRegisteredControl')!.valueChanges.subscribe({
            next: this.onPortRegisteredChanged.bind(this)
        });

        return form;
    }

    protected getValue(): PortVisitDTO | undefined {
        if (this.isFromRegister) {
            const portControl = this.form.get('portControl')!;

            if (portControl.value !== undefined && portControl.value !== null) {
                return new PortVisitDTO({
                    portId: portControl.value.value,
                });
            }
        }
        else {
            return new PortVisitDTO({
                portCountryId: this.form.get('countryControl')!.value?.value,
                portName: this.form.get('nameControl')!.value,
            });
        }

        return undefined;
    }

    private onPortRegisteredChanged(value: boolean): void {
        this.isFromRegister = value;

        if (value) {
            this.form.get('portControl')!.setValidators(Validators.required);
            this.form.get('nameControl')!.clearValidators();
            this.form.get('countryControl')!.clearValidators();
        } else {
            this.form.get('portControl')!.clearValidators();
            this.form.get('nameControl')!.setValidators([Validators.required, Validators.maxLength(500)]);
            this.form.get('countryControl')!.setValidators([Validators.required]);
        }

        this.form.get('portControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('nameControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('countryControl')!.updateValueAndValidity({ emitEvent: false });
    }
}
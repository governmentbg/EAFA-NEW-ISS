import { AfterViewInit, Component, Input, OnInit, Self } from '@angular/core';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { AuanInspectedEntityDTO } from '@app/models/generated/dtos/AuanInspectedEntityDTO';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';

@Component({
    selector: 'inspected-entity-basic-info',
    templateUrl: './inspected-entity-basic-info.component.html'
})
export class InspectedEntityBasicInfoComponent extends CustomFormControl<AuanInspectedEntityDTO> implements OnInit, AfterViewInit {
    @Input()
    public viewMode: boolean = false;

    public inspectedEntity: AuanInspectedEntityDTO | undefined;

    public constructor(
        @Self() ngControl: NgControl
    ) {
        super(ngControl);
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public ngAfterViewInit(): void {
        if (this.inspectedEntity !== null && this.inspectedEntity !== undefined) {
            if (this.inspectedEntity.isPerson === true) {
                if (this.inspectedEntity.person !== undefined && this.inspectedEntity.person !== null) {
                    this.form.get('personControl')!.setValue(this.inspectedEntity.person);
                }
                else {
                    this.form.get('personControl')!.setValue(this.inspectedEntity.unregisteredPerson);
                }
                this.form.get('personAddressesControl')!.setValue(this.inspectedEntity.addresses);
            }
            else if (this.inspectedEntity.isPerson === false) {
                this.form.get('legalControl')!.setValue(this.inspectedEntity.legal);
                this.form.get('legalAddressesControl')!.setValue(this.inspectedEntity.addresses);
            }
        }
    }

    public writeValue(value: AuanInspectedEntityDTO): void {
        if (value !== undefined && value !== null) {
            this.inspectedEntity = value;
            if (value.isPerson !== undefined && value.isPerson !== null) {

                this.inspectedEntity.isPerson = value.isPerson;
                if (value.isPerson === true) {
                    this.form.get('personControl')!.setValue(value.person);
                    this.form.get('personAddressesControl')!.setValue(value.addresses);
                    this.form.get('personWorkPlaceControl')!.setValue(value.personWorkPlace);
                    this.form.get('personWorkPositionControl')!.setValue(value.personWorkPosition);
                }
                else if (value.isPerson === false) {
                    this.form.get('legalControl')!.setValue(value.legal);
                    this.form.get('legalAddressesControl')!.setValue(value.addresses);
                }
            }
        }
    }

    protected getValue(): AuanInspectedEntityDTO {
        if (this.inspectedEntity !== undefined && this.inspectedEntity !== null) {
            const result: AuanInspectedEntityDTO = new AuanInspectedEntityDTO({
                isUnregisteredPerson: false,
                isPerson: this.inspectedEntity!.isPerson === true
            });

            if (result.isPerson === true) {
                result.person = this.form.get('personControl')!.value;
                result.addresses = this.form.get('personAddressesControl')!.value;
                result.personWorkPlace = this.form.get('personWorkPlaceControl')!.value;
                result.personWorkPosition = this.form.get('personWorkPositionControl')!.value;
            }
            else if (result.isPerson === false) {
                result.legal = this.form.get('legalControl')!.value;
                result.addresses = this.form.get('legalAddressesControl')!.value;
            }

            return result;
        }

        return new AuanInspectedEntityDTO();
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            personControl: new FormControl(null),
            personAddressesControl: new FormControl(null),
            personWorkPlaceControl: new FormControl(null, Validators.maxLength(100)),
            personWorkPositionControl: new FormControl(null, Validators.maxLength(100)),
            legalControl: new FormControl(null),
            legalAddressesControl: new FormControl(null)
        });
    }
}
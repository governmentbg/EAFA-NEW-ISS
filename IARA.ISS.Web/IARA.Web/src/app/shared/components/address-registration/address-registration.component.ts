import { Component, Input, OnChanges, OnInit, Optional, Self, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ValidationErrors, Validators } from '@angular/forms';

import { AddressTypesEnum } from '@app/enums/address-types.enum';
import { AddressRegistrationDTO } from '@app/models/generated/dtos/AddressRegistrationDTO';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { NotifierDirective } from '@app/shared/directives/notifier/notifier.directive';
import { NotifyingCustomFormControl } from '@app/shared/utils/notifying-custom-form-control';

@Component({
    selector: 'address-registration',
    templateUrl: './address-registration.component.html'
})
export class AddressRegistrationComponent extends NotifyingCustomFormControl<AddressRegistrationDTO[]> implements OnInit, OnChanges {
    @Input()
    public addressLabel!: string;

    @Input()
    public secondAddressLabel!: string;

    @Input()
    public checkboxLabel: string | undefined;

    @Input()
    public addressType: AddressTypesEnum = AddressTypesEnum.PERMANENT;

    @Input()
    public secondAddressType: AddressTypesEnum = AddressTypesEnum.CORRESPONDENCE;

    @Input()
    public expectedResults: AddressRegistrationDTO[] = [];

    @Input()
    public readonly: boolean = false;

    public notifierGroup: Notifier = new Notifier();

    public expectedAddressResult?: AddressRegistrationDTO;
    public expectedSecondAddressResult?: AddressRegistrationDTO;

    public constructor(
        @Self() ngControl: NgControl,
        @Optional() @Self() validityChecker: ValidityCheckerDirective,
        @Optional() @Self() notifier: NotifierDirective
    ) {
        super(ngControl, true, validityChecker, notifier);
    }

    public ngOnInit(): void {
        this.initNotifyingCustomFormControl(this.notifierGroup, () => { this.notify(); });

        this.form.get('checkboxControl')?.valueChanges.subscribe((checked: boolean) => {
            if (checked) {
                this.form.get('secondAddressControl')!.setValidators(null);
            }
            else {
                this.form.get('secondAddressControl')!.setValidators(Validators.required);
            }
            this.form.get('secondAddressControl')!.updateValueAndValidity();
        });
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const expectedResults: AddressRegistrationDTO[] | undefined = changes['expectedResults']?.currentValue;

        if (expectedResults !== null && expectedResults !== undefined) {
            this.expectedAddressResult = expectedResults.find(x => x.addressType === this.addressType);
            this.expectedSecondAddressResult = expectedResults.find(x => x.addressType === this.secondAddressType);
        }
    }

    public writeValue(values: AddressRegistrationDTO[]): void {
        setTimeout(() => {
            if (values !== null && values !== undefined && values.length !== 0) {
                this.form.get('firstAddressControl')!.setValue(values[0]);

                const hasSecondAddress: boolean = values[1] !== null && values[1] !== undefined;

                this.form.get('checkboxControl')?.setValue(!hasSecondAddress);
                if (hasSecondAddress) {
                    this.form.get('secondAddressControl')?.setValue(values[1]);
                }
            }
            else {
                this.form.get('firstAddressControl')!.setValue(null);
                this.form.get('checkboxControl')?.setValue(true);
                this.form.get('secondAddressControl')?.setValue(null);
            }
        });
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        const errors: ValidationErrors = {};

        for (const key of Object.keys(this.form.controls)) {
            if (this.form.controls[key].errors !== null && this.form.controls[key].errors !== undefined) {
                for (const error in this.form.controls[key].errors) {
                    if (!['expectedValueNotMatching'].includes(error)) {
                        errors[error] = this.form.controls[key].errors![error];
                    }
                }
            }
        }

        return Object.keys(errors).length === 0 ? null : errors;
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            firstAddressControl: new FormControl(null, Validators.required),
            checkboxControl: new FormControl(true),
            secondAddressControl: new FormControl(null)
        });
    }

    protected getValue(): AddressRegistrationDTO[] {
        const first: AddressRegistrationDTO | undefined = this.form.get('firstAddressControl')?.value ?? undefined;

        const result: AddressRegistrationDTO[] = [];
        if (first) {
            result.push(first);
        }

        if (this.form.get('checkboxControl')?.value !== true) {
            const second: AddressRegistrationDTO | undefined = this.form.get('secondAddressControl')?.value ?? undefined;
            if (second) {
                result.push(second);
            }
        }
        return result;
    }
}

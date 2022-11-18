import { Component, Input, OnChanges, OnInit, Optional, Self, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl } from '@angular/forms';

import { PaymentTariffDTO } from '@app/models/generated/dtos/PaymentTariffDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';

const CHECKBOX_DEFAULT_FLEX: number = 5;
const NAME_DEFAULT_FLEX: number = 35;
const QUANTITY_DEFAULT_FLEX: number = 20;
const UNIT_PRICE_DEFAULT_FLEX: number = 20;
const PRICE_DEFAULT_FLEX: number = 20;

@Component({
    selector: 'payment-tariff',
    templateUrl: './payment-tariff.component.html'
})
export class PaymentTariffComponent extends CustomFormControl<PaymentTariffDTO | undefined> implements OnInit, OnChanges {
    @Input()
    public showIsChecked: boolean = false;

    @Input()
    public isCheckboxReadonly: boolean = true;

    @Input()
    public set paymentTariff(value: PaymentTariffDTO | undefined) {
        this.tariff = value;
        
        if (this.tariff === null || this.tariff === undefined) {
            this.hasDescription = false;
            this.hasBasedOnPlea = false;
        }
        else {
            if (this.tariff.tariffDescription !== null && this.tariff.tariffDescription !== undefined && this.tariff.tariffDescription.length > 0) {
                this.hasDescription = true;
            }
            else {
                this.hasDescription = false;
            }

            if (this.tariff!.tariffBasedOnPlea !== null && this.tariff!.tariffBasedOnPlea !== undefined && this.tariff!.tariffBasedOnPlea.length > 0) {
                this.hasBasedOnPlea = true;
            }
            else {
                this.hasBasedOnPlea = false;
            }
        }
    }

    /**
     * percent betweeen 0 and 100, default is 5
     * */
    @Input()
    public set tariffCheckboxFlex(value: number | undefined) {
        if (value !== null && value !== undefined) {
            this.checkboxFlex = value;
        }
        else {
            this.checkboxFlex = CHECKBOX_DEFAULT_FLEX;
        }
    }

    /**
     * percent between 0 and 100, default is 35 (40 when there is no checkbox)
     * */
    @Input()
    public set tariffNameFlex(value: number | undefined) {
        if (value !== null && value !== undefined) {
            this.nameFlex = value;
        }
        else {
            this.nameFlex = NAME_DEFAULT_FLEX;
        }
    }

    /**
     * percent between 0 and 100, default is 20
     * */
    @Input()
    public set tariffQuantityFlex(value: number | undefined) {
        if (value !== null && value !== undefined) {
            this.quantityFlex = value;
        }
        else {
            this.quantityFlex = QUANTITY_DEFAULT_FLEX;
        }
    }

    /**
     * percent between 0 and 100, default is 20
     * */
    @Input()
    public set tariffUnitPriceFlex(value: number | undefined) {
        if (value !== null && value !== undefined) {
            this.unitPriceFlex = value;
        }
        else {
            this.unitPriceFlex = UNIT_PRICE_DEFAULT_FLEX;
        }
    }

    /**
     * percent between 0 and 100, default is 20
     * */
    @Input()
    public set tariffPriceFlex(value: number | undefined) {
        if (value !== null && value !== undefined) {
            this.priceFlex = value;
        }
        else {
            this.priceFlex = PRICE_DEFAULT_FLEX;
        }
    }

    public checkboxFlex: number = CHECKBOX_DEFAULT_FLEX;
    public nameFlex: number = NAME_DEFAULT_FLEX;
    public quantityFlex: number = QUANTITY_DEFAULT_FLEX;
    public unitPriceFlex: number = UNIT_PRICE_DEFAULT_FLEX;
    public priceFlex: number = PRICE_DEFAULT_FLEX;

    public tariff: PaymentTariffDTO | undefined;

    public hasDescription: boolean = false;
    public hasBasedOnPlea: boolean = false;

    public constructor(
        @Self() @Optional() ngControl: NgControl,
        @Optional() @Self() validityChecker: ValidityCheckerDirective
    ) {
        super(ngControl, false, validityChecker);

        this.control.valueChanges.subscribe({
            next: (value: boolean) => {
                if (this.tariff !== null && this.tariff !== undefined) {
                    this.tariff.isChecked = value;
                }

                this.onChanged(this.getValue());
            }
        });
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if ('showIsChecked' in changes) {
            if (!changes['showIsChecked'].currentValue) {
                this.nameFlex = NAME_DEFAULT_FLEX + CHECKBOX_DEFAULT_FLEX;
            }
        }
    }

    public writeValue(value: PaymentTariffDTO | undefined): void {
        this.paymentTariff = value;
        this.control.setValue(this.tariff?.isChecked ?? false);

        this.setDisabledCheckboxes();
    }

    protected getValue(): PaymentTariffDTO | undefined {
        return this.tariff;
    }

    protected buildForm(): AbstractControl {
        return new FormControl(false);
    }

    private setDisabledCheckboxes(): void {
        if (!this.tariff?.isCalculated) {
            this.control.disable();
        }
        else {
            this.control.enable();
        }
    }
}
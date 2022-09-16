import { Component, Input } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { PaymentTariffDTO } from '@app/models/generated/dtos/PaymentTariffDTO';


const NAME_DEFAULT_FLEX: number = 40;
const QUANTITY_DEFAULT_FLEX: number = 20;
const UNIT_PRICE_DEFAULT_FLEX: number = 20;
const PRICE_DEFAULT_FLEX: number = 20;

@Component({
    selector: 'payment-tariff',
    templateUrl: './payment-tariff.component.html'
})
export class PaymentTariffComponent {
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
     * percent between 0 and 100, default is 40
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

    public nameFlex: number = NAME_DEFAULT_FLEX;
    public quantityFlex: number = QUANTITY_DEFAULT_FLEX;
    public unitPriceFlex: number = UNIT_PRICE_DEFAULT_FLEX;
    public priceFlex: number = PRICE_DEFAULT_FLEX;

    public tariff: PaymentTariffDTO | undefined;

    public hasDescription: boolean = false;
    public hasBasedOnPlea: boolean = false;

}
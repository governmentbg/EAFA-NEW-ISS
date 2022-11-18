import { Component, Input, OnInit, Optional, Self, SimpleChanges } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup, NgControl } from '@angular/forms';
import { Subscription } from 'rxjs';

import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { PaymentSummaryDTO } from '@app/models/generated/dtos/PaymentSummaryDTO';
import { TariffNomenclatureDTO } from '@app/models/generated/dtos/TariffNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PaymentTariffDTO } from '@app/models/generated/dtos/PaymentTariffDTO';

const CHECKBOX_LABEL_DEFAULT_FLEX: number = 5;
const NAME_LABEL_DEFAULT_FLEX: number = 35;
const QUANTITY_LABEL_DEFAULT_FLEX: number = 20;
const UNIT_PRICE_LABEL_DEFAULT_FLEX: number = 20;
const PRICE_LABEL_DEFAULT_FLEX: number = 20;

@Component({
    selector: 'payment-tariffs',
    templateUrl: './payment-tariffs.component.html'
})
export class PaymentTariffsComponent extends CustomFormControl<PaymentSummaryDTO | undefined> implements OnInit {
    @Input()
    public showIsChecked: boolean = false;

    public paymentSummary: PaymentSummaryDTO = new PaymentSummaryDTO({ tariffs: [], totalPrice: 0 });

    public checkboxLabelFlex: number = CHECKBOX_LABEL_DEFAULT_FLEX;
    public nameLabelFlex: number = NAME_LABEL_DEFAULT_FLEX;
    public nameLabelNoCheckboxFlex: number = NAME_LABEL_DEFAULT_FLEX + CHECKBOX_LABEL_DEFAULT_FLEX;
    public quantityLabelFlex: number = QUANTITY_LABEL_DEFAULT_FLEX;
    public unitPriceLabelFlex: number = UNIT_PRICE_LABEL_DEFAULT_FLEX;
    public priceLabelFlex: number = PRICE_LABEL_DEFAULT_FLEX;

    private paymentTariffs: TariffNomenclatureDTO[] = [];
    private readonly nomenclatures: CommonNomenclatures;
    private readonly loader: FormControlDataLoader;

    public constructor(@Self() ngControl: NgControl, nomenclatures: CommonNomenclatures) {
        super(ngControl, false);

        this.nomenclatures = nomenclatures;
        this.loader = new FormControlDataLoader(this.getPaymentTariffs.bind(this));

        this.formArray.valueChanges.subscribe({
            next: (tariffs: PaymentTariffDTO[] | undefined) => {
                if (tariffs !== null && tariffs !== undefined) {
                    this.paymentSummary.totalPrice = tariffs.filter(x => x.isChecked).map(x => x.price!).reduce((x: number, y: number) => {
                        return x + y;
                    }, 0);
                }

                this.onChanged(this.getValue());
            }
        });
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
        this.loader.load();
    }

    public writeValue(value: PaymentSummaryDTO | undefined): void {
        if (value !== null && value !== undefined) {
            this.loader.load(() => {
                if (value.tariffs !== null && value.tariffs !== undefined) {
                    for (const paymentTariff of value.tariffs) {
                        const tariff: TariffNomenclatureDTO = this.paymentTariffs.find(x => x.value === paymentTariff.tariffId)!;
                        paymentTariff.tariffName = tariff.displayName;
                        paymentTariff.tariffDescription = tariff.description;
                        paymentTariff.tariffBasedOnPlea = tariff.basedOnPlea;
                    }
                }

                this.paymentSummary = value;

                this.fillFormArray();
            });
        }
        else {
            this.paymentSummary = new PaymentSummaryDTO({ tariffs: [], totalPrice: 0 });
            this.formArray.reset();
        }
    }

    protected buildForm(): AbstractControl {
        return new FormArray([]);
    }

    protected getValue(): PaymentSummaryDTO {
        const paymentSummary: PaymentSummaryDTO = new PaymentSummaryDTO({
            tariffs: [],
            totalPrice: this.paymentSummary.totalPrice
        });

        for (const control of this.formArray.controls) {
            paymentSummary.tariffs!.push(control.value);
        }

        return paymentSummary;
    }

    private fillFormArray(): void {
        this.formArray.clear();

        if (this.paymentSummary.tariffs !== null && this.paymentSummary.tariffs !== undefined) {
            for (const tariff of this.paymentSummary.tariffs) {
                this.formArray.push(new FormControl(tariff));
            }
        }
    }

    private getPaymentTariffs(): Subscription {
        return NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.PaymentTariffs, this.nomenclatures.getPaymentTariffs.bind(this.nomenclatures), false
        ).subscribe({
            next: (types: TariffNomenclatureDTO[]) => {
                this.paymentTariffs = types;
                this.loader.complete();
            }
        });
    }
}
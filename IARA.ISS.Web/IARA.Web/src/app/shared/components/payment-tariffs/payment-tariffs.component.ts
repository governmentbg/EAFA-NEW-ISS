import { Component, OnInit, Optional, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl } from '@angular/forms';
import { Subscription } from 'rxjs';

import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { PaymentSummaryDTO } from '@app/models/generated/dtos/PaymentSummaryDTO';
import { TariffNomenclatureDTO } from '@app/models/generated/dtos/TariffNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PaymentTariffDTO } from '@app/models/generated/dtos/PaymentTariffDTO';


@Component({
    selector: 'payment-tariffs',
    templateUrl: './payment-tariffs.component.html'
})
export class PaymentTariffsComponent extends CustomFormControl<PaymentSummaryDTO> implements OnInit {
    public tariffs: TariffNomenclatureDTO[] = [];
    public paymentSummary: PaymentSummaryDTO = new PaymentSummaryDTO({ tariffs: [], totalPrice: 0 });

    private nomenclatures: CommonNomenclatures;
    private readonly loader: FormControlDataLoader;

    public constructor(@Optional() @Self() ngControl: NgControl, nomenclatures: CommonNomenclatures) {
        super(ngControl);
        this.nomenclatures = nomenclatures;

        this.loader = new FormControlDataLoader(this.getPaymentTariffs.bind(this));
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
        this.loader.load();
    }

    public writeValue(value: PaymentSummaryDTO): void {
        if (value !== null && value !== undefined) {
            this.loader.load(() => {
                if (value.tariffs !== null && value.tariffs !== undefined) {
                    for (const paymentTariff of value.tariffs) {
                        const tariff: TariffNomenclatureDTO = this.tariffs.find(x => x.value === paymentTariff.tariffId)!;
                        paymentTariff.tariffName = tariff.displayName;
                        paymentTariff.tariffDescription = tariff.description;
                        paymentTariff.tariffBasedOnPlea = tariff.basedOnPlea;
                    }
                }
                this.paymentSummary = value;
            });
        }
        else {
            this.paymentSummary = new PaymentSummaryDTO({ tariffs: [], totalPrice: 0 });
            this.form.reset();
        }
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({ paymentSummary: new FormControl() });
    }

    protected getValue(): PaymentSummaryDTO {
        return this.paymentSummary;
    }

    private getPaymentTariffs(): Subscription {
        return NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.PaymentTariffs, this.nomenclatures.getPaymentTariffs.bind(this.nomenclatures), false
        ).subscribe({
            next: (types: TariffNomenclatureDTO[]) => {
                this.tariffs = types;
                this.loader.complete();
            }
        });
    }
}
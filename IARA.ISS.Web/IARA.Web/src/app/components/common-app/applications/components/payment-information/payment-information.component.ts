import { CurrencyPipe } from "@angular/common";
import { Component, Input, OnInit, Optional, Self } from "@angular/core";
import { AbstractControl, FormControl, FormGroup, NgControl, ValidationErrors, Validators } from "@angular/forms";
import { forkJoin, Observable, Subscription } from 'rxjs';

import { ApplicationPaymentInformationDTO } from '@app/models/generated/dtos/ApplicationPaymentInformationDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { PaymentSummaryDTO } from '@app/models/generated/dtos/PaymentSummaryDTO';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';

@Component({
    selector: 'payment-information',
    templateUrl: './payment-information.component.html'
})
export class PaymentInformationComponent extends CustomFormControl<ApplicationPaymentInformationDTO> implements OnInit {
    @Input()
    public set paymentData(value: ApplicationPaymentInformationDTO) {
        this.setModelData(value);
    }

    @Input()
    public hideBasicInfo: boolean = false;

    @Input()
    public isOnlineApplication: boolean = false;

    public paymentTypes: NomenclatureDTO<number>[] = [];
    public paymentStatuses: NomenclatureDTO<number>[] = [];

    private id: number | undefined;

    private readonly currencyPipe: CurrencyPipe;
    private readonly nomenclatures: CommonNomenclatures;
    private readonly loader: FormControlDataLoader;
    private readonly isPublicApp!: boolean;

    public constructor(
        @Optional() @Self() ngControl: NgControl,
        currencyPipe: CurrencyPipe,
        @Optional() @Self() validityChecker: ValidityCheckerDirective,
        nomenclatures: CommonNomenclatures
    ) {
        super(ngControl, true, validityChecker);

        this.currencyPipe = currencyPipe;
        this.nomenclatures = nomenclatures;
        this.isPublicApp = IS_PUBLIC_APP;

        this.loader = new FormControlDataLoader(this.getPaymentTypes.bind(this));
    }

    public ngOnInit(): void {
        if (this.isPublicApp !== true) {
            this.form.setValidators([TLValidators.sameTotalPriceAndPaidPriceValidator()]);
            this.form.updateValueAndValidity({ emitEvent: false });
        }

        this.initCustomFormControl();

        this.form.get('totalPaidPriceControl')!.valueChanges.subscribe({
            next: (price: number | undefined) => {
                const paymentSummary: PaymentSummaryDTO = this.form.get('paymentSummaryControl')!.value;

                if (price !== undefined && price !== null) {
                    price = Number(price);
                }

                paymentSummary.totalPaidPrice = price;

                this.form.get('paymentSummaryControl')!.setValue(paymentSummary);
            }
        });
    }

    public writeValue(value: ApplicationPaymentInformationDTO): void {
        this.setModelData(value);
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        const errors: ValidationErrors = {};

        if (this.form.errors !== null) {
            for (const key of Object.keys(this.form.errors)) {
                if (key !== 'totalPriceNotEqualToPaid') {
                    errors[key] = this.form.errors[key];
                }
            }
        }

        for (const key of Object.keys(this.form.controls)) {
            for (const error in this.form.controls[key].errors) {
                errors[error] = this.form.controls[key].errors![error];
            }
        }

        return Object.keys(errors).length === 0 ? null : errors;
    }

    protected getValue(): ApplicationPaymentInformationDTO {
        const model: ApplicationPaymentInformationDTO = new ApplicationPaymentInformationDTO({
            id: this.id
        });

        model.lastUpdateDate = this.form.get('lastUpdateDateControl')!.value;
        model.paymentDate = this.form.get('paymentDateControl')!.value;
        model.totalPaidPrice = this.form.get('totalPaidPriceControl')!.value;
        model.paymentStatus = this.form.get('paymentStatusControl')!.value?.code;
        model.referenceNumber = this.form.get('referenceNumberControl')!.value;
        model.paymentType = this.form.get('paymentTypeControl')!.value?.code;
        model.paymentSummary = this.form.get('paymentSummaryControl')!.value;

        return model;
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            paymentSummaryControl: new FormControl(),
            paymentTypeControl: new FormControl(undefined),
            paymentDateControl: new FormControl(undefined),
            totalPaidPriceControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),
            paymentStatusControl: new FormControl(),
            referenceNumberControl: new FormControl(undefined, Validators.maxLength(50)),
            lastUpdateDateControl: new FormControl()
        });
    }

    private setModelData(value: ApplicationPaymentInformationDTO | undefined): void {
        if (value !== null && value !== undefined) {
            this.mapModelToForm(value);
        }
        else {
            this.form.reset();
        }
    }

    private mapModelToForm(model: ApplicationPaymentInformationDTO): void {
        this.loader.load(() => {
            this.id = model.id;

            this.form.get('paymentSummaryControl')!.setValue(model.paymentSummary);
            this.form.get('paymentTypeControl')!.setValue(this.paymentTypes.find(x => x.code === model.paymentType));
            this.form.get('paymentStatusControl')!.setValue(this.paymentStatuses.find(x => x.code === model.paymentStatus));

            this.form.get('paymentDateControl')!.setValue(model.paymentDate);
            this.form.get('totalPaidPriceControl')!.setValue(model.totalPaidPrice);
            this.form.get('referenceNumberControl')!.setValue(model.referenceNumber);
            this.form.get('lastUpdateDateControl')!.setValue(model.lastUpdateDate);
        });
    }

    private getPaymentTypes(): Subscription {
        let paymentTypesObservable: Observable<NomenclatureDTO<number>[]>;

        if (this.isOnlineApplication) {
            paymentTypesObservable = NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.OnlinePaymentTypes, this.nomenclatures.getOnlinePaymentTypes.bind(this.nomenclatures), false
            );
        }
        else {
            paymentTypesObservable = NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.OfflinePaymentTypes, this.nomenclatures.getOfflinePaymentTypes.bind(this.nomenclatures), false
            )
        }

        const paymentStatusesObservable = NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.PaymentStatuses, this.nomenclatures.getPaymentStatuses.bind(this.nomenclatures), false
        );

        return forkJoin([paymentTypesObservable, paymentStatusesObservable])
            .subscribe({
                next: (nomenclatures: NomenclatureDTO<number>[][]) => {
                    this.paymentTypes = nomenclatures[0];
                    this.paymentStatuses = nomenclatures[1];

                    this.loader.complete();
                }
            });
    }
}

import { Component, Input, OnChanges, OnInit, Optional, Self, SimpleChange, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ValidationErrors, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';

import { ApplicationStatusesEnum } from '@app/enums/application-statuses.enum';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PaymentTypesEnum } from '@app/enums/payment-types.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PaymentDataDTO } from '@app/models/generated/dtos/PaymentDataDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { PaymentDataInfo } from './models/payment-data-info.model';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { PaymentSummaryDTO } from '@app/models/generated/dtos/PaymentSummaryDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';

@Component({
    selector: 'payment-data',
    templateUrl: './payment-data.component.html'
})
export class PaymentDataComponent extends CustomFormControl<PaymentDataDTO> implements OnInit, OnChanges, IDialogComponent {
    @Input()
    public paymentDateMin: Date | undefined;

    @Input()
    public paymentDateMax: Date | undefined;

    @Input()
    public defaultDate: Date | undefined;

    @Input()
    public applicationsService: IApplicationsService | undefined;

    @Input()
    public paymentSummary: PaymentSummaryDTO | undefined;

    public showAppliedTariffsPanel: boolean = false;

    public paymentTypes: NomenclatureDTO<number>[] = [];

    private applicationId: number | undefined;

    private readonly nomenclatures: CommonNomenclatures;
    private readonly loader: FormControlDataLoader;

    public constructor(@Optional() @Self() ngControl: NgControl, nomenclatures: CommonNomenclatures) {
        super(ngControl);
        this.nomenclatures = nomenclatures;

        this.loader = new FormControlDataLoader(this.getPaymentTypes.bind(this));
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        this.form.get('totalPaidPriceControl')!.valueChanges.subscribe({
            next: (price: number | undefined) => {
                if (this.paymentSummary !== null && this.paymentSummary !== undefined) {
                    this.paymentSummary = this.form.get('paymentSummaryControl')!.value;

                    if (price !== null && price !== undefined) {
                        price = Number(price);
                    }

                    this.paymentSummary!.totalPaidPrice = price;

                    this.form.get('paymentSummaryControl')!.setValue(this.paymentSummary);
                }
            }
        });

        this.resetForm();

        if (this.applicationsService !== null && this.applicationsService !== undefined && this.applicationId !== null && this.applicationId !== undefined) {
            this.applicationsService.getApplicationPaymentSummary(this.applicationId).subscribe({
                next: (result: PaymentSummaryDTO) => {
                    this.paymentSummary = result;
                    this.form.get('paymentSummaryControl')!.setValue(this.paymentSummary);

                    this.setAppliedTariffsFlag();
                }
            });
        }
        else {
            this.form.get('paymentSummaryControl')!.setValue(this.paymentSummary);
        }

        this.setAppliedTariffsFlag();
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const paymentSummary: SimpleChange | undefined = changes['paymentSummary'];
        if (paymentSummary !== undefined) {
            this.form.get('paymentSummaryControl')!.setValue(this.paymentSummary);
        }

        this.setAppliedTariffsFlag();
    }

    public setData(data: PaymentDataInfo, buttons: DialogWrapperData): void {
        this.paymentTypes = data.paymentTypes;
        this.applicationsService = data.service;
        this.applicationId = data.applicationId;
        this.paymentDateMin = data.paymentDateMin;
        this.paymentDateMax = data.paymentDateMax;

        if (data.viewMode) {
            this.form.disable();
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();

        if (this.isFormValid()) {
            const paymentData: PaymentDataDTO = this.getValue();
            this.applicationsService!.enterPaymentData(this.applicationId!, paymentData).subscribe((value: ApplicationStatusesEnum) => {
                dialogClose(paymentData);
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public writeValue(value: PaymentDataDTO): void {
        if (value !== null && value !== undefined) {
            this.loader.load(() => {
                this.form.get('paymentTypeControl')!.setValue(this.paymentTypes.find(x => x.code === PaymentTypesEnum[value.paymentType!]));
            });

            this.form.get('paymentRefControl')!.setValue(value.paymentRefNumber);
            this.form.get('paymentDateControl')!.setValue(value.paymentDateTime);
            this.form.get('totalPaidPriceControl')!.setValue(value.totalPaidPrice);
        }
        else {
            this.resetForm();
        }
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

    protected buildForm(): AbstractControl {
        return new FormGroup({
            paymentSummaryControl: new FormControl(),
            paymentTypeControl: new FormControl(undefined, Validators.required),
            paymentRefControl: new FormControl(undefined, Validators.maxLength(50)),
            totalPaidPriceControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),
            paymentDateControl: new FormControl(undefined, Validators.required)
        }, [
            TLValidators.sameTotalPriceAndPaidPriceValidator()
        ]);
    }

    protected getValue(): PaymentDataDTO {
        return new PaymentDataDTO({
            paymentType: PaymentTypesEnum[this.form.get('paymentTypeControl')?.value?.code as keyof typeof PaymentTypesEnum],
            paymentRefNumber: this.form.get('paymentRefControl')?.value ?? undefined,
            paymentDateTime: this.form.get('paymentDateControl')?.value ?? undefined,
            totalPaidPrice: this.form.get('totalPaidPriceControl')?.value ?? undefined
        });
    }

    private resetForm(): void {
        this.form.get('paymentSummaryControl')!.setValue(undefined);

        this.loader.load(() => {
            this.form.get('paymentTypeControl')!.setValue(this.paymentTypes.find(x => x.code === PaymentTypesEnum[PaymentTypesEnum.CASH]));
        });

        this.form.get('paymentRefControl')!.setValue(undefined);
        this.form.get('paymentDateControl')!.setValue(this.defaultDate);
        this.form.get('totalPaidPriceControl')!.setValue(undefined);
    }

    private getPaymentTypes(): Subscription {
        return NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.OfflinePaymentTypes, this.nomenclatures.getOfflinePaymentTypes.bind(this.nomenclatures), false
        ).subscribe({
            next: (types: NomenclatureDTO<number>[]) => {
                this.paymentTypes = types;

                this.loader.complete();
            }
        });
    }

    private setAppliedTariffsFlag(): void {
        this.showAppliedTariffsPanel = this.paymentSummary !== undefined && this.paymentSummary !== null && !this.paymentSummary.hasCalculatedTariffs;
    }

    private isFormValid(): boolean {
        if (this.form.valid) {
            return true;
        }
        else {
            const errors: ValidationErrors | null = this.validate(this.form);

            return errors === null;
        }
    }
}
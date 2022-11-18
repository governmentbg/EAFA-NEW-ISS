import { CurrencyPipe } from "@angular/common";
import { Component, Input, Optional, Self } from "@angular/core";
import { AbstractControl, FormControl, FormGroup, NgControl } from "@angular/forms";
import { ApplicationPaymentInformationDTO } from '@app/models/generated/dtos/ApplicationPaymentInformationDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';


@Component({
    selector: 'payment-information',
    templateUrl: './payment-information.component.html'
})
export class PaymentInformationComponent extends CustomFormControl<ApplicationPaymentInformationDTO> {
    @Input()
    public set paymentData(value: ApplicationPaymentInformationDTO) {
        this.setModelData(value);
    }

    @Input()
    public hideBasicInfo: boolean = false;

    private currencyPipe: CurrencyPipe;

    public constructor(@Optional() @Self() ngControl: NgControl, currencyPipe: CurrencyPipe) {
        super(ngControl);

        this.currencyPipe = currencyPipe;
    }

    public writeValue(value: ApplicationPaymentInformationDTO): void {
        this.setModelData(value);
    }

    protected getValue(): ApplicationPaymentInformationDTO {
        const model: ApplicationPaymentInformationDTO = new ApplicationPaymentInformationDTO();

        model.lastUpdateDate = this.form.get('lastUpdateDateControl')!.value;
        model.paymentDate = this.form.get('paymentDateControl')!.value;
        model.paymentStatus = this.form.get('paymentStatusControl')!.value;
        model.referenceNumber = this.form.get('referenceNumberControl')!.value;
        model.paymentType = this.form.get('paymentTypeControl')!.value;
        model.paymentSummary = this.form.get('paymentSummaryControl')!.value;

        return model;
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            paymentSummaryControl: new FormControl(),
            paymentTypeControl: new FormControl(),
            paymentDateControl: new FormControl(),
            paymentStatusControl: new FormControl(),
            referenceNumberControl: new FormControl(),
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
        this.form.get('paymentSummaryControl')!.setValue(model.paymentSummary);
        this.form.get('paymentTypeControl')!.setValue(model.paymentType);
        this.form.get('paymentDateControl')!.setValue(model.paymentDate);
        this.form.get('paymentStatusControl')!.setValue(model.paymentStatus);
        this.form.get('referenceNumberControl')!.setValue(model.referenceNumber);
        this.form.get('lastUpdateDateControl')!.setValue(model.lastUpdateDate);
    }
}

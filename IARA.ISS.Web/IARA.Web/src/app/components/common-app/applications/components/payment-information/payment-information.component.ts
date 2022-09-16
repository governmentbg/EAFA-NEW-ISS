import { CurrencyPipe } from "@angular/common";
import { Component, Input } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { ApplicationPaymentInformationDTO } from "@app/models/generated/dtos/ApplicationPaymentInformationDTO";


@Component({
    selector: 'payment-information',
    templateUrl: './payment-information.component.html'
})
export class PaymentInformationComponent {
    @Input()
    public set paymentData(value: ApplicationPaymentInformationDTO) {
        if (value !== null && value !== undefined) {
            this.mapModelToForm(value);
        }
        else {
            this.form.reset();
        }
    }
    public form: FormGroup;

    private currencyPipe: CurrencyPipe;

    public constructor(currencyPipe: CurrencyPipe) {
        this.currencyPipe = currencyPipe;
        this.form = new FormGroup({
            paymentSummaryControl: new FormControl(),
            paymentTypeControl: new FormControl(),
            paymentDateControl: new FormControl(null),
            paymentStatusControl: new FormControl(),
            referenceNumberControl: new FormControl(),
            lastUpdateDateControl: new FormControl()
        });
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

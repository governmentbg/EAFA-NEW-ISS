import { FormDataModel } from '@tl/tl-egov-payments';
import { GeneratedPaymentModel } from '@tl/tl-epay-payments';
import { Observable } from 'rxjs';

export interface IEPaymentsService {
    initiateEPayBGPayment(paymentRequestNum: string): Observable<GeneratedPaymentModel>;
    initiateEPayDirectPayment(paymentRequestNum: string): Observable<GeneratedPaymentModel>;
    initiateEGovBankPayment(paymentRequestNum: string): Observable<string>;
    initiateEGovEPayBGPayment(paymentRequestNum: string): Observable<FormDataModel>;
    initiateEGovEPOSPayment(paymentRequestNum: string): Observable<FormDataModel>;
}
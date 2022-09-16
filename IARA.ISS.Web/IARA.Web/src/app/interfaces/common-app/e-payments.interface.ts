import { FormDataModel } from '@tl/tl-egov-payments';
import { GeneratedPaymentModel } from "@tl/tl-epay-payments";
import { Observable } from "rxjs";

export interface IEPaymentsService {
    initiateEPayBGPayment(applicationId: number): Observable<GeneratedPaymentModel>;
    initiateEPayDirectPayment(applicationId: number): Observable<GeneratedPaymentModel>;
    initiateEGovBankPayment(applicationId: number): Observable<string>;
    initiateEGovEPayBGPayment(applicationId: number): Observable<FormDataModel>;
    initiateEGovEPOSPayment(applicationId: number): Observable<FormDataModel>;
}
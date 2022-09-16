import { Observable } from "rxjs";
import { ApplicationHierarchyTypesEnum } from "@app/enums/application-hierarchy-types.enum";

export interface IPaymentsService {
    getApplicationOriginType(paymentId: string, isFromEPay: boolean, isPaymentCanceled: boolean): Observable<ApplicationHierarchyTypesEnum>;
}
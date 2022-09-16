import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ApplicationHierarchyTypesEnum } from '@app/enums/application-hierarchy-types.enum';
import { IPaymentsService } from '../../interfaces/public-app/payments-public.interface';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';

@Injectable({
    providedIn: 'root'
})
export class PaymentsPublicService implements IPaymentsService {
    private readonly area: AreaTypes = AreaTypes.Public;
    private readonly controller: string = 'PaymentsPublic';
    private readonly requestService: RequestService;

    public constructor(requestService: RequestService) {
        this.requestService = requestService;
    }

    public getApplicationOriginType(paymentId: string, isFromEPay: boolean, isPaymentCanceled: boolean): Observable<ApplicationHierarchyTypesEnum> {

        let params = new HttpParams();

        params = params.append('paymentId', paymentId.toString());
        params = params.append('isFromEPay', isFromEPay.toString());
        params = params.append('isPaymentCanceled', isPaymentCanceled.toString());

        return this.requestService.get(this.area, this.controller, 'GetApplicationOriginType', { httpParams: params });
    }
}
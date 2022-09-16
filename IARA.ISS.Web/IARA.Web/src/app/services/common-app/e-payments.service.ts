import { HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IEPaymentsService } from '@app/interfaces/common-app/e-payments.interface';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';
import { FormDataModel, ITLEGovPaymentRequestService, TLEGovPaymentService, VPOSPaymentTypes } from '@tl/tl-egov-payments';
import { EPaymentType, GeneratedPaymentModel, TLEPaymentService, InvoicePaymentRequestModel } from '@tl/tl-epay-payments';
import { from, Observable } from 'rxjs';
import { RequestProperties } from '../../shared/services/request-properties';

@Injectable({
    providedIn: 'root'
})
export class EPaymentsService implements IEPaymentsService {
    private eGovPaymentSevice: TLEGovPaymentService;
    private ePayBgPaymentService: TLEPaymentService;

    public constructor(eGovPaymentSevice: TLEGovPaymentService,
        ePayBgPaymentService: TLEPaymentService,
        requestService: RequestService) {

        this.ePayBgPaymentService = ePayBgPaymentService;
        this.ePayBgPaymentService.RequestService = new EPaymentsRequestService(requestService, 'EPay');
        //TLEPaymentService.setBaseServiceURL(Environment.Instance.apiBaseUrl + '/Integration/EPay');

        this.eGovPaymentSevice = eGovPaymentSevice;
        this.eGovPaymentSevice.RequestService = new EPaymentsRequestService(requestService, 'EGovPayments');
    }

    public initiateEPayBGPayment(applicationId: number): Observable<GeneratedPaymentModel> {

        let paymentRequest = new InvoicePaymentRequestModel(applicationId.toString());
        paymentRequest.paymentType = EPaymentType.WebEPay;
        return from(this.ePayBgPaymentService.generatePaymentRequestById(paymentRequest));
    }

    public initiateEPayDirectPayment(applicationId: number): Observable<GeneratedPaymentModel> {

        let paymentRequest = new InvoicePaymentRequestModel(applicationId.toString());
        paymentRequest.paymentType = EPaymentType.DirectCreditPay;
        return from(this.ePayBgPaymentService.generatePaymentRequestById(paymentRequest));
    }

    public initiateEGovBankPayment(applicationId: number): Observable<string> {
        return from(this.eGovPaymentSevice.registerOfflinePaymentByRefNumber(applicationId.toString()));
    }

    public initiateEGovEPayBGPayment(applicationId: number): Observable<FormDataModel> {
        return from(this.eGovPaymentSevice.createVPOSPaymentByRefNumber(applicationId.toString(), VPOSPaymentTypes.EPAY));
    }

    public initiateEGovEPOSPayment(applicationId: number): Observable<FormDataModel> {
        return from(this.eGovPaymentSevice.createVPOSPaymentByRefNumber(applicationId.toString(), VPOSPaymentTypes.BANK));
    }
}

export class EPaymentsRequestService implements ITLEGovPaymentRequestService {

    private controller;
    private requestService: RequestService;
    private headers?: HttpHeaders;
    public constructor(requestService: RequestService, controller: string) {
        this.controller = controller;
        this.requestService = requestService;
    }

    public get<TResult>(methodName: string, params: HttpParams): Observable<TResult> {
        const requestProperties = RequestProperties.DEFAULT;
        return this.requestService.get<TResult>(AreaTypes.Integration, this.controller, methodName, { httpParams: params, properties: requestProperties, headers: this.headers });
    }

    public getText(methodName: string, params: HttpParams): Observable<string> {
        const requestProperties = RequestProperties.DEFAULT;
        return this.requestService.get(AreaTypes.Integration, this.controller, methodName, { httpParams: params, responseType: 'text', properties: requestProperties, headers: this.headers });
    }

    public post<TRequest, TResult>(methodName: string, request: TRequest): Observable<TResult> {
        const requestProperties = RequestProperties.DEFAULT;
        return this.requestService.post<TResult, TRequest>(AreaTypes.Integration, this.controller, methodName, request, { properties: requestProperties, headers: this.headers });
    }

    public postText<TRequest>(methodName: string, request: TRequest): Observable<string> {
        const requestProperties = RequestProperties.DEFAULT;
        return this.requestService.post(AreaTypes.Integration, this.controller, methodName, request, { properties: requestProperties, headers: this.headers });
    }

    public setAccessToken(token: string) {
        this.headers = new HttpHeaders();
        this.headers = this.headers.append('Authorization', `Bearer ${token}`);
    }
}
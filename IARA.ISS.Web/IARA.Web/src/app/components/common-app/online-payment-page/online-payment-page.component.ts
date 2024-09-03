import { Component, ElementRef, ViewChild } from "@angular/core";
import { EPaymentsRequestService } from '@app/services/common-app/e-payments.service';
import { RequestService } from '@app/shared/services/request.service';
import { FuseConfigService } from "@fuse/services/config.service";
import { FuseTranslationLoaderService } from "@fuse/services/translation-loader.service";
import { FormDataModel, TLEGovPaymentService, VPOSPaymentTypes } from '@tl/tl-egov-payments';
import { EPaymentType, InvoicePaymentRequestModel, TLEPaymentService } from '@tl/tl-epay-payments';

@Component({
    selector: 'online-payment-page',
    templateUrl: './online-payment-page.component.html',
    styleUrls: ['./online-payment-page.component.scss']
})
export class OnlinePaymentPageComponent {
    private translationService: FuseTranslationLoaderService;
    private fuseConfigService: FuseConfigService;

    private eGovPaymentSevice: TLEGovPaymentService;
    private epayPaymentService: TLEPaymentService;

    public buttonText: string = '';
    public paymentFormData?: FormDataModel;

    @ViewChild('btn') public button!: ElementRef;

    public constructor(
        translationService: FuseTranslationLoaderService,
        fuseConfigService: FuseConfigService,
        eGovPaymentSevice: TLEGovPaymentService,
        epayPaymentService: TLEPaymentService,
        requestService: RequestService) {
        this.translationService = translationService;
        this.fuseConfigService = fuseConfigService;
        this.eGovPaymentSevice = eGovPaymentSevice;
        this.eGovPaymentSevice.RequestService = new EPaymentsRequestService(requestService, 'EGovPayments');

        this.epayPaymentService = epayPaymentService;
        this.epayPaymentService.RequestService = new EPaymentsRequestService(requestService, 'EPay');

        // Configure the layout
        this.fuseConfigService.setConfig({
            layout: {
                navbar: {
                    hidden: true
                },
                toolbar: {
                    hidden: true
                },
                footer: {
                    hidden: true
                },
                sidepanel: {
                    hidden: true
                }
            }
        });

        document.onlinePayment = this;
        window.onlinePayment = this;
    }

    public setPaymentParams(paymentRefNumber: string, paymentType: 'EPAY' | 'CARD', token: string, isForeign: boolean = false): void {

        if (!isForeign) {
            (this.eGovPaymentSevice.RequestService as EPaymentsRequestService).setAccessToken(token);
            switch (paymentType) {
                case 'EPAY':
                    {
                        this.buttonText = this.translationService.getValue('online-payment-data.pay-with-epay');
                        this.eGovPaymentSevice.createVPOSPaymentByRefNumber(paymentRefNumber, VPOSPaymentTypes.EPAY, true).then(result => {
                            this.paymentFormData = result;
                        });
                    }
                    break;
                case 'CARD': {
                    this.buttonText = this.translationService.getValue('online-payment-data.pay-with-egov');
                    this.eGovPaymentSevice.createVPOSPaymentByRefNumber(paymentRefNumber, VPOSPaymentTypes.BANK, true).then(result => {
                        this.paymentFormData = result;
                    });
                } break;
                default:
            }
        } else {
            (this.epayPaymentService.RequestService as EPaymentsRequestService).setAccessToken(token);
            switch (paymentType) {
                case 'EPAY':
                    {
                        this.buttonText = this.translationService.getValue('online-payment-data.pay-with-epay');

                        const paymentModel: InvoicePaymentRequestModel = new InvoicePaymentRequestModel(paymentRefNumber);
                        paymentModel.paymentType = EPaymentType.WebEPay;

                        this.epayPaymentService.generatePaymentRequestById(paymentModel).then(result => {
                            //this.paymentFormData = result;
                        });
                    }
                    break;
                case 'CARD': {
                    this.buttonText = this.translationService.getValue('online-payment-data.pay-with-egov');

                    const paymentModel: InvoicePaymentRequestModel = new InvoicePaymentRequestModel(paymentRefNumber);
                    paymentModel.paymentType = EPaymentType.DirectCreditPay;

                    this.epayPaymentService.generatePaymentRequestById(paymentModel).then(result => {
                        //this.paymentFormData = result;
                    });
                } break;
                default:
            }
        }
    }
}

declare global {
    interface Window {
        onlinePayment: OnlinePaymentPageComponent;
    }
    interface Document {
        onlinePayment: OnlinePaymentPageComponent;
    }
}
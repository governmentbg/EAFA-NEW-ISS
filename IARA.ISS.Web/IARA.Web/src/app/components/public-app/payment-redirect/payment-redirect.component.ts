import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Params, Router } from "@angular/router";
import { ApplicationHierarchyTypesEnum } from "@app/enums/application-hierarchy-types.enum";
import { PaymentsPublicService } from "@app/services/public-app/payments-public.service";
import { CommonUtils } from '../../../shared/utils/common.utils';


@Component({
    selector: 'payment-redirect-page',
    template: ''
})
export class PaymentRedirectPageComponent implements OnInit {
    private service: PaymentsPublicService;
    private route: ActivatedRoute;
    private router: Router;

    public constructor(route: ActivatedRoute, router: Router, paymentsService: PaymentsPublicService) {
        this.route = route;
        this.router = router;
        this.service = paymentsService;
    }

    public ngOnInit(): void {
        this.route.queryParams.subscribe({
            next: (params: Params) => {
                const isPaymentCanceled = CommonUtils.toBoolean(params['isPaymentCanceled']);
                const invoiceNumber = params['invoiceNumber'];
                if (invoiceNumber !== null && invoiceNumber !== undefined && invoiceNumber !== '') {
                    this.updatePaymentStatus(invoiceNumber, true, isPaymentCanceled);
                } else {
                    const requestId = params['requestId'];
                    this.updatePaymentStatus(requestId, false, isPaymentCanceled);
                }
            }
        });
    }

    private updatePaymentStatus(paymentId: string, isFromEPay: boolean, isPaymentCanceled: boolean): void {
        this.service.getApplicationOriginType(paymentId, isFromEPay, isPaymentCanceled).subscribe((applicationHierarchyType: ApplicationHierarchyTypesEnum) => {
            if (applicationHierarchyType === ApplicationHierarchyTypesEnum.RecreationalFishingTicket) {
                this.redirectToTicketsPage();
            }
            else {
                this.redirectToApplicationsPage();
            }
        });
    }

    private redirectToTicketsPage(): void {
        this.router.navigateByUrl('/recreational-fishing');
    }

    private redirectToApplicationsPage(): void {
        this.router.navigateByUrl('/submitted-applications');
    }

}
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PageEvent } from '@angular/material/paginator';
import { forkJoin } from 'rxjs';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { ApplicationStatusesEnum } from '@app/enums/application-statuses.enum';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PaymentStatusesEnum } from '@app/enums/payment-statuses.enum';
import { ReasonData } from '@app/models/common/reason-data.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RecreationalFishingTicketCardDTO } from '@app/models/generated/dtos/RecreationalFishingTicketCardDTO';
import { RecreationalFishingTicketDTO } from '@app/models/generated/dtos/RecreationalFishingTicketDTO';
import { RecreationalFishingPublicService } from '@app/services/public-app/recreational-fishing-public.service';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { OnlinePaymentDataDialogParams } from '@app/shared/components/online-payment-data/egov-offline-payment-data/models/online-payment-data-dialog-params.model';
import { OnlinePaymentDataComponent } from '@app/shared/components/online-payment-data/online-payment-data.component';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { EnterReasonComponent } from '@app/components/common-app/applications/components/enter-reason/enter-reason.component';
import { ReasonDialogParams } from '@app/components/common-app/applications/components/enter-reason/models/reason-dialog-params.model';
import { EditTicketDialogParams } from '@app/components/common-app/recreational-fishing/applications/models/edit-ticket-dialog-params.model';
import { RecreationalFishingTicketComponent } from '@app/components/common-app/recreational-fishing/tickets/components/ticket/recreational-fishing-ticket.component';
import { TicketStatusEnum } from '@app/enums/ticket-status.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';

@Component({
    selector: 'recreational-fishing-tickets',
    templateUrl: './recreational-fishing-tickets.component.html',
    styleUrls: ['./recreational-fishing-tickets.component.scss']
})
export class RecreationalFishingTicketsComponent implements OnInit {
    public showBuyTicketScreen: boolean;
    public initialLoadComplete: boolean = false;

    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;
    public readonly ticketStatuses: typeof TicketStatusEnum = TicketStatusEnum;
    public readonly paymentStatuses: typeof PaymentStatusesEnum = PaymentStatusesEnum;
    public readonly applicationStatuses: typeof ApplicationStatusesEnum = ApplicationStatusesEnum;
    public readonly canProcessPaymentData: boolean;

    public readonly defaultPageSize: number = 5;
    public readonly pageSizeOptions: number[] = [2, 3, 4, 5, 10];
    public ticketsLength: number = 0;

    public service: RecreationalFishingPublicService;
    public tickets: RecreationalFishingTicketCardDTO[] = [];

    private types: NomenclatureDTO<number>[] = [];
    private periods: NomenclatureDTO<number>[] = [];

    private allTickets: RecreationalFishingTicketCardDTO[] = [];
    private translate: FuseTranslationLoaderService;
    private router: Router;
    private editDialog: TLMatDialog<RecreationalFishingTicketComponent>;
    private statusReasonDialog: TLMatDialog<EnterReasonComponent>;
    private paymentDialog: TLMatDialog<OnlinePaymentDataComponent>;

    private pageEvent: PageEvent;

    public constructor(
        service: RecreationalFishingPublicService,
        router: Router,
        translate: FuseTranslationLoaderService,
        editDialog: TLMatDialog<RecreationalFishingTicketComponent>,
        statusReasonDialog: TLMatDialog<EnterReasonComponent>,
        paymentDialog: TLMatDialog<OnlinePaymentDataComponent>,
        permissions: PermissionsService,
        route: ActivatedRoute
    ) {
        this.service = service;
        this.router = router;
        this.translate = translate;
        this.editDialog = editDialog;
        this.statusReasonDialog = statusReasonDialog;
        this.paymentDialog = paymentDialog;

        this.pageEvent = new PageEvent();
        this.pageEvent.pageIndex = 0;
        this.pageEvent.pageSize = this.defaultPageSize;
        this.pageEvent.length = 0;

        this.canProcessPaymentData = permissions.has(PermissionsEnum.OnlineSubmittedApplicationsProcessPaymentData);

        const state: { buyTicket: boolean } = window.history.state;
        this.showBuyTicketScreen = state.buyTicket === true || route.snapshot.url.join('/') === 'recreational-fishing/purchase-ticket';
    }

    public ngOnInit(): void {
        forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TicketTypes, this.service.getTicketTypes.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TicketPeriods, this.service.getTicketPeriods.bind(this.service), false)
        ).subscribe({
            next: ([types, periods]) => {
                this.types = types;
                this.periods = periods;

                this.getAllUserTickets();
            }
        });
    }

    public navigateToBuyTicketPage(): void {
        this.router.navigateByUrl('/recreational-fishing/purchase-ticket', {
            state: {
                buyTicket: true
            }
        });
    }

    public editTicket(ticket: RecreationalFishingTicketCardDTO, viewMode: boolean = false): void {
        const params = new EditTicketDialogParams({
            id: ticket.id,
            applicationId: undefined,
            isApplication: false,
            showOnlyRegiXData: false,
            isReadonly: viewMode,
            viewMode: viewMode,
            isAssociation: false,
            isPersonal: true,
            type: this.types.find(x => x.value === ticket.typeId)!,
            period: this.periods.find(x => x.value === ticket.periodId)!,
            service: this.service
        });

        const title: string = viewMode
            ? this.translate.getValue('recreational-fishing.application-view-ticket-dialog-title')
            : this.translate.getValue('recreational-fishing.application-edit-ticket-dialog-title');

        const dialog = this.editDialog.open({
            title: title,
            TCtor: RecreationalFishingTicketComponent,
            headerAuditButton: undefined,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: params,
            translteService: this.translate,
            disableDialogClose: !viewMode,
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: 'common.cancel'
            },
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: 'common.save'
            },
            viewMode: viewMode
        }, '1400px');

        dialog.subscribe({
            next: (ticket?: RecreationalFishingTicketDTO) => {
                if (ticket !== undefined && ticket !== null) {
                    this.getAllUserTickets();
                }
            }
        });
    }

    public renewTicket(ticket: RecreationalFishingTicketCardDTO): void {
        const params = new EditTicketDialogParams({
            id: ticket.id,
            applicationId: undefined,
            isApplication: false,
            showOnlyRegiXData: false,
            isAssociation: false,
            isPersonal: true,
            isRenewal: true,
            type: this.types.find(x => x.value === ticket.typeId)!,
            period: this.periods.find(x => x.value === ticket.periodId)!,
            service: this.service
        });

        const title: string = this.translate.getValue('recreational-fishing.ticket-renewal');

        const dialog = this.editDialog.open({
            title: title,
            TCtor: RecreationalFishingTicketComponent,
            headerAuditButton: undefined,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: params,
            translteService: this.translate,
            disableDialogClose: true,
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: 'common.cancel'
            },
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: 'common.save'
            }
        }, '1400px');

        dialog.subscribe({
            next: (ticket?: RecreationalFishingTicketDTO) => {
                if (ticket !== undefined && ticket !== null) {
                    const card = new RecreationalFishingTicketCardDTO({
                        applicationId: ticket.applicationId,
                        price: ticket.price
                    });
                    this.pay(card);
                }
            }
        });
    }

    public refusePayment(ticket: RecreationalFishingTicketCardDTO): void {
        const dialog = this.statusReasonDialog.openWithTwoButtons({
            title: this.translate.getValue('recreational-fishing.refuse-pay-ticket-dialog-title'),
            TCtor: EnterReasonComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeRefusePaymentDialogBtnClicked.bind(this)
            },
            translteService: this.translate,
            componentData: new ReasonDialogParams({
                isApplication: true,
                id: ticket.id,
                saveReasonServiceMethod: this.service.cancelTicket.bind(this.service)
            }),
            disableDialogClose: true
        }, '800px');

        dialog.subscribe({
            next: (data?: ReasonData) => {
                if (data) {
                    this.getAllUserTickets();
                }
            }
        });
    }

    public pay(ticket: RecreationalFishingTicketCardDTO): void {
        const dialog = this.paymentDialog.open({
            title: this.translate.getValue('recreational-fishing.initiate-online-payment'),
            TCtor: OnlinePaymentDataComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closePaymentDialogBtnClicked.bind(this)
            },
            translteService: this.translate,
            componentData: new OnlinePaymentDataDialogParams({
                applicationId: ticket.applicationId,
                showTariffs: true
            })
        }, '1000px');

        dialog.subscribe({
            next: () => {
                this.getAllUserTickets();
            }
        });
    }

    public switchPage(event: PageEvent): void {
        this.pageEvent = event;

        this.tickets = [...this.allTickets];
        this.ticketsLength = this.tickets.length;

        this.tickets = this.tickets.slice(
            this.pageEvent.pageIndex * this.pageEvent.pageSize,
            this.pageEvent.pageIndex * this.pageEvent.pageSize + this.pageEvent.pageSize
        );
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeRefusePaymentDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closePaymentDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private getAllUserTickets(): void {
        this.service.getAllUserTickets().subscribe({
            next: (tickets: RecreationalFishingTicketCardDTO[]) => {
                for (const ticket of tickets) {
                    ticket.period = this.periods.find(x => x.value === ticket.periodId)!.displayName;
                    ticket.type = this.types.find(x => x.value === ticket.typeId)!.displayName;

                    if (ticket.ticketStatus === TicketStatusEnum.EXPIRED) {
                        ticket.status = this.translate.getValue('recreational-fishing.ticket-card-expired');
                    }
                    else if (ticket.ticketStatus === TicketStatusEnum.CANCELED) {
                        ticket.status = this.translate.getValue('recreational-fishing.ticket-card-canceled');
                    }
                    else if (ticket.applicationStatus === ApplicationStatusesEnum.WAIT_PMT_FROM_USR) {
                        ticket.status = this.translate.getValue('recreational-fishing.ticket-card-awaiting-payment');
                    }
                    else if (ticket.applicationStatus === ApplicationStatusesEnum.PAYMENT_PROCESSING) {
                        ticket.status = this.translate.getValue('recreational-fishing.ticket-card-payment-processing');
                    }
                    else {
                        switch (ticket.paymentStatus) {
                            case PaymentStatusesEnum.NotNeeded:
                            case PaymentStatusesEnum.PaidOK:
                                ticket.status = this.translate.getValue('recreational-fishing.ticket-card-active');
                                break;
                            case PaymentStatusesEnum.Unpaid:
                                ticket.status = this.translate.getValue('recreational-fishing.ticket-card-awaiting-payment');
                                break;
                            case PaymentStatusesEnum.PaymentFail:
                                ticket.status = this.translate.getValue('recreational-fishing.ticket-card-payment-failed');
                                break;
                        }
                    }
                }

                this.allTickets = tickets;
                this.switchPage(this.pageEvent);

                this.initialLoadComplete = true;
            }
        });
    }
}

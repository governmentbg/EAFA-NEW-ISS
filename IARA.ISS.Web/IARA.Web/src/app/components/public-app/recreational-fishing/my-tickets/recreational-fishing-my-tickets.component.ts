import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User } from '@app/components/common-app/auth/models/auth/user.model';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PaymentStatusesEnum } from '@app/enums/payment-statuses.enum';
import { TicketPeriodEnum } from '@app/enums/ticket-period.enum';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RecreationalFishingTicketViewDTO } from '@app/models/generated/dtos/RecreationalFishingTicketViewDTO';
import { UserAuthDTO } from '@app/models/generated/dtos/UserAuthDTO';
import { SecurityService } from '@app/services/common-app/security.service';
import { RecreationalFishingPublicService } from '@app/services/public-app/recreational-fishing-public.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';

@Component({
    selector: 'recreational-fishing-my-tickets',
    templateUrl: './recreational-fishing-my-tickets.component.html',
    styleUrls: ['./recreational-fishing-my-tickets.component.scss']
})
export class RecreationalFishingMyTicketsComponent implements OnInit {
    public tickets: RecreationalFishingTicketViewDTO[] = [];

    public canBuyTickets: boolean = false;
    public showWarningBadge: boolean = false;

    private authService: SecurityService;
    private service: RecreationalFishingPublicService;
    private router: Router;

    public constructor(
        authService: SecurityService,
        service: RecreationalFishingPublicService,
        router: Router,
        permissions: PermissionsService
    ) {
        this.authService = authService;
        this.service = service;
        this.router = router;

        this.canBuyTickets = permissions.has(PermissionsEnum.TicketsPublicAddRecords);
    }

    public ngOnInit(): void {
        this.authService.getUser().subscribe({
            next: (userInfo: UserAuthDTO) => {
                if (userInfo !== null) {
                    this.service.currentUserTickets.subscribe({
                        next: async (tickets: RecreationalFishingTicketViewDTO[]) => {
                            this.tickets = tickets;

                            if (this.tickets.length > 0) {
                                const nomenclatures: NomenclatureDTO<number>[][] = await Promise.all([
                                    NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TicketTypes, this.service.getTicketTypes.bind(this.service), false).toPromise(),
                                    NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TicketPeriods, this.service.getTicketPeriods.bind(this.service), false).toPromise()
                                ]);

                                const types: NomenclatureDTO<number>[] = nomenclatures[0];
                                const periods: NomenclatureDTO<number>[] = nomenclatures[1];

                                const noPeriodTypeId: number = periods.find(x => x.code === TicketPeriodEnum[TicketPeriodEnum.NOPERIOD])!.value!;
                                const today: Date = new Date();

                                for (const ticket of tickets) {
                                    ticket.type = types.find(x => x.value === ticket.typeId)!.displayName;

                                    if (ticket.periodId !== noPeriodTypeId) {
                                        ticket.period = periods.find(x => x.value === ticket.periodId)!.displayName;
                                    }

                                    if (ticket.validTo) {
                                        ticket.daysRemaining = Math.ceil((ticket.validTo.getTime() - today.getTime()) / (1000 * 60 * 60 * 24));
                                    }

                                    if (ticket.daysRemaining! <= 0) {
                                        this.showWarningBadge = true;
                                    }

                                    if (ticket.paymentStatus !== PaymentStatusesEnum.PaidOK && ticket.paymentStatus !== PaymentStatusesEnum.NotNeeded) {
                                        this.showWarningBadge = true;
                                    }
                                }
                            }
                        }
                    });

                    this.service.getUserRequestedOrActiveTickets().subscribe();
                }
            }
        });
    }

    public buyTicket(): void {
        this.router.navigateByUrl('/recreational-fishing/purchase-ticket', {
            state: {
                buyTicket: true
            }
        });
    }

    public showAllTickets(): void {
        this.router.navigateByUrl('/recreational-fishing', {
            state: {
                buyTicket: false
            }
        });
    }

    public trackTicket(index: number, item: RecreationalFishingTicketViewDTO): number {
        return item.id!;
    }
}
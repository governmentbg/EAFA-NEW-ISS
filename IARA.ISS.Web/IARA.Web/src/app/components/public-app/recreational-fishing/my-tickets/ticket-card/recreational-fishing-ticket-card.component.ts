import { Component, Input } from '@angular/core';
import { PaymentStatusesEnum } from '@app/enums/payment-statuses.enum';
import { RecreationalFishingTicketViewDTO } from '@app/models/generated/dtos/RecreationalFishingTicketViewDTO';
import { TicketStatusEnum } from '@app/enums/ticket-status.enum';
import { ApplicationStatusesEnum } from '@app/enums/application-statuses.enum';

@Component({
    selector: 'recreational-fishing-ticket-card',
    templateUrl: './recreational-fishing-ticket-card.component.html'
})
export class RecreationalFishingTicketCardComponent {
    @Input()
    public ticket!: RecreationalFishingTicketViewDTO;

    public statuses: typeof TicketStatusEnum = TicketStatusEnum;
    public paymentStatuses: typeof PaymentStatusesEnum = PaymentStatusesEnum;
    public applicationStatuses: typeof ApplicationStatusesEnum = ApplicationStatusesEnum;

    public constructor() {
        // nothing
    }
}
import { Component, Input } from '@angular/core';
import { PaymentStatusesEnum } from '@app/enums/payment-statuses.enum';
import { RecreationalFishingTicketViewDTO } from '@app/models/generated/dtos/RecreationalFishingTicketViewDTO';

@Component({
    selector: 'recreational-fishing-ticket-card',
    templateUrl: './recreational-fishing-ticket-card.component.html'
})
export class RecreationalFishingTicketCardComponent {
    @Input()
    public ticket!: RecreationalFishingTicketViewDTO;

    public statuses: typeof PaymentStatusesEnum = PaymentStatusesEnum;

    public constructor() {
        // nothing
    }
}
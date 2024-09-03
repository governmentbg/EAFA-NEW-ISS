

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { TicketStatusEnum } from '@app/enums/ticket-status.enum';
import { PaymentStatusesEnum } from '@app/enums/payment-statuses.enum';
import { ApplicationStatusesEnum } from '@app/enums/application-statuses.enum';

export class RecreationalFishingTicketViewDTO { 
    public constructor(obj?: Partial<RecreationalFishingTicketViewDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public typeId?: number;

    @StrictlyTyped(String)
    public type?: string;

    @StrictlyTyped(Number)
    public periodId?: number;

    @StrictlyTyped(String)
    public period?: string;

    @StrictlyTyped(String)
    public person?: string;

    @StrictlyTyped(Date)
    public validTo?: Date;

    @StrictlyTyped(Number)
    public ticketStatus?: TicketStatusEnum;

    @StrictlyTyped(Number)
    public paymentStatus?: PaymentStatusesEnum;

    @StrictlyTyped(Number)
    public applicationStatus?: ApplicationStatusesEnum;

    @StrictlyTyped(String)
    public status?: string;

    @StrictlyTyped(Number)
    public daysRemaining?: number;
}
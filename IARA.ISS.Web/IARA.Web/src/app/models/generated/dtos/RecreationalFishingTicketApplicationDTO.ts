

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { TicketStatusEnum } from '@app/enums/ticket-status.enum';
import { ApplicationStatusesEnum } from '@app/enums/application-statuses.enum';

export class RecreationalFishingTicketApplicationDTO { 
    public constructor(obj?: Partial<RecreationalFishingTicketApplicationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Number)
    public createdByUserId?: number;

    @StrictlyTyped(Boolean)
    public isOnlineTicket?: boolean;

    @StrictlyTyped(String)
    public ticketNum?: string;

    @StrictlyTyped(String)
    public ticketHolderName?: string;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public validTo?: Date;

    @StrictlyTyped(Number)
    public ticketTypeId?: number;

    @StrictlyTyped(String)
    public ticketType?: string;

    @StrictlyTyped(Number)
    public ticketPeriodId?: number;

    @StrictlyTyped(String)
    public ticketPeriod?: string;

    @StrictlyTyped(Number)
    public ticketPrice?: number;

    @StrictlyTyped(String)
    public ticketIssuedBy?: string;

    @StrictlyTyped(String)
    public ticketStatusName?: string;

    @StrictlyTyped(Number)
    public ticketStatus?: TicketStatusEnum;

    @StrictlyTyped(Number)
    public applicationStatus?: ApplicationStatusesEnum;

    @StrictlyTyped(Number)
    public prevStatusCode?: ApplicationStatusesEnum;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
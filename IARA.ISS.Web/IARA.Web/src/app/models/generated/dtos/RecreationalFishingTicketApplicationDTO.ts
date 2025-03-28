﻿

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PaymentStatusesEnum } from '@app/enums/payment-statuses.enum';
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

    @StrictlyTyped(String)
    public validityPeriod?: string;

    @StrictlyTyped(Date)
    public issuedOn?: Date;

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

    @StrictlyTyped(String)
    public applicationStatusName?: string;

    @StrictlyTyped(String)
    public regixErrorDescription?: string;

    @StrictlyTyped(String)
    public paymentStatusName?: string;

    @StrictlyTyped(Number)
    public paymentStatus?: PaymentStatusesEnum;

    @StrictlyTyped(Number)
    public ticketStatus?: TicketStatusEnum;

    @StrictlyTyped(Number)
    public applicationStatus?: ApplicationStatusesEnum;

    @StrictlyTyped(Number)
    public prevStatusCode?: ApplicationStatusesEnum;

    @StrictlyTyped(String)
    public applicationStatusReason?: string;

    @StrictlyTyped(Boolean)
    public isDuplicate?: boolean;

    @StrictlyTyped(Boolean)
    public isExpired?: boolean;

    @StrictlyTyped(Boolean)
    public isSameTerritoryUnit?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
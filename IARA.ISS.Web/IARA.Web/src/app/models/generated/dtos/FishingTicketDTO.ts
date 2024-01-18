

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FishingTicketDTO { 
    public constructor(obj?: Partial<FishingTicketDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public typeId?: number;

    @StrictlyTyped(String)
    public type?: string;

    @StrictlyTyped(String)
    public typeName?: string;

    @StrictlyTyped(Number)
    public periodId?: number;

    @StrictlyTyped(String)
    public periodCode?: string;

    @StrictlyTyped(String)
    public periodName?: string;

    @StrictlyTyped(Number)
    public price?: number;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public validTo?: Date;

    @StrictlyTyped(String)
    public personFullName?: string;

    @StrictlyTyped(String)
    public statusCode?: string;

    @StrictlyTyped(String)
    public statusName?: string;

    @StrictlyTyped(String)
    public paymentStatus?: string;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(String)
    public applicationStatusCode?: string;

    @StrictlyTyped(String)
    public applicationStatusReason?: string;

    @StrictlyTyped(String)
    public ticketNumber?: string;

    @StrictlyTyped(String)
    public paymentRequestNum?: string;
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class RecreationalFishingAddTicketsResultDTO { 
    public constructor(obj?: Partial<RecreationalFishingAddTicketsResultDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public paidTicketApplicationId?: number;

    @StrictlyTyped(String)
    public paidTicketPaymentRequestNum?: string;

    @StrictlyTyped(Number)
    public ticketIds?: number[];

    @StrictlyTyped(Number)
    public childTicketIds?: number[];
}
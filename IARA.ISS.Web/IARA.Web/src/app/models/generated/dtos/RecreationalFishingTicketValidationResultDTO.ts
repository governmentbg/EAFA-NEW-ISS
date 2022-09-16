
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class RecreationalFishingTicketValidationResultDTO {
    public constructor(obj?: Partial<RecreationalFishingTicketValidationResultDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public currentlyActiveUnder14Tickets?: number;

    @StrictlyTyped(Boolean)
    public cannotPurchaseTicket?: boolean;
}
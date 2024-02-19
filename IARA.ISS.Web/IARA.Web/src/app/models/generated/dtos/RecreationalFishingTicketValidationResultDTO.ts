

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RecreationalFishingRepresentativeCountDTO } from './RecreationalFishingRepresentativeCountDTO';

export class RecreationalFishingTicketValidationResultDTO { 
    public constructor(obj?: Partial<RecreationalFishingTicketValidationResultDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Boolean)
    public cannotPurchaseTicket?: boolean;

    @StrictlyTyped(Date)
    public ticketValidFrom?: Date;

    @StrictlyTyped(Date)
    public ticketValidTo?: Date;

    @StrictlyTyped(Boolean)
    public telkisIndefinite?: boolean;

    @StrictlyTyped(RecreationalFishingRepresentativeCountDTO)
    public representativeCount?: RecreationalFishingRepresentativeCountDTO;
}
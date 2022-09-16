

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RecreationalFishingTicketDTO } from './RecreationalFishingTicketDTO';
import { PaymentDataDTO } from './PaymentDataDTO';

export class RecreationalFishingTicketsDTO { 
    public constructor(obj?: Partial<RecreationalFishingTicketsDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(RecreationalFishingTicketDTO)
    public tickets?: RecreationalFishingTicketDTO[];

    @StrictlyTyped(Number)
    public associationId?: number;

    @StrictlyTyped(Boolean)
    public hasPaymentData?: boolean;

    @StrictlyTyped(PaymentDataDTO)
    public paymentData?: PaymentDataDTO;
}
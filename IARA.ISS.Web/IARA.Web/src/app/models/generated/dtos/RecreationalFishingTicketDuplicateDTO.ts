

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PaymentDataDTO } from './PaymentDataDTO';
import { FileInfoDTO } from './FileInfoDTO';

export class RecreationalFishingTicketDuplicateDTO { 
    public constructor(obj?: Partial<RecreationalFishingTicketDuplicateDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public ticketNum?: string;

    @StrictlyTyped(Number)
    public ticketId?: number;

    @StrictlyTyped(Number)
    public price?: number;

    @StrictlyTyped(PaymentDataDTO)
    public paymentData?: PaymentDataDTO;

    @StrictlyTyped(Number)
    public createdByAssociationId?: number;

    @StrictlyTyped(FileInfoDTO)
    public personPhoto?: FileInfoDTO;
}
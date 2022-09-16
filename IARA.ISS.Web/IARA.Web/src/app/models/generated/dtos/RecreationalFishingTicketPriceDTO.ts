
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { TicketTypeEnum } from '@app/enums/ticket-type.enum';
import { TicketPeriodEnum } from '@app/enums/ticket-period.enum';

export class RecreationalFishingTicketPriceDTO {
    public constructor(obj?: Partial<RecreationalFishingTicketPriceDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public ticketType?: TicketTypeEnum;

    @StrictlyTyped(Number)
    public ticketPeriod?: TicketPeriodEnum;

    @StrictlyTyped(Number)
    public price?: number;
}
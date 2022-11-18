

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { EgnLncDTO } from './EgnLncDTO';
import { TicketTypeEnum } from '@app/enums/ticket-type.enum';
import { TicketPeriodEnum } from '@app/enums/ticket-period.enum';

export class RecreationalFishingTicketValidationDTO { 
    public constructor(obj?: Partial<RecreationalFishingTicketValidationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(EgnLncDTO)
    public personEgnLnc?: EgnLncDTO;

    @StrictlyTyped(EgnLncDTO)
    public representativePersonEgnLnc?: EgnLncDTO;

    @StrictlyTyped(Number)
    public ticketType?: TicketTypeEnum;

    @StrictlyTyped(Number)
    public ticketPeriod?: TicketPeriodEnum;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public birthDate?: Date;

    @StrictlyTyped(Date)
    public telkValidTo?: Date;
}
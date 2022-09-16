

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { EgnLncDTO } from './EgnLncDTO';

export class RecreationalFishingTicketValidationDTO { 
    public constructor(obj?: Partial<RecreationalFishingTicketValidationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(EgnLncDTO)
    public personEgnLnc?: EgnLncDTO;

    @StrictlyTyped(Boolean)
    public isRepresentative?: boolean;

    @StrictlyTyped(Number)
    public typeId?: number;

    @StrictlyTyped(Number)
    public periodId?: number;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Number)
    public under14TicketsCount?: number;
}
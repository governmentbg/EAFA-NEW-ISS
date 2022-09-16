

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class RecreationalFishingTicketDuplicateTableDTO { 
    public constructor(obj?: Partial<RecreationalFishingTicketDuplicateTableDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public ticketNum?: string;

    @StrictlyTyped(Date)
    public issueDate?: Date;

    @StrictlyTyped(Number)
    public price?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
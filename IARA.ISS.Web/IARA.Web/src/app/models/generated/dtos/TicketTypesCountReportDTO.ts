

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { TicketTypeEnum } from '@app/enums/ticket-type.enum';

export class TicketTypesCountReportDTO { 
    public constructor(obj?: Partial<TicketTypesCountReportDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public icon?: string;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(Number)
    public ticketTypeCode?: TicketTypeEnum;

    @StrictlyTyped(Number)
    public count?: number;

    @StrictlyTyped(Number)
    public typeId?: number;
}
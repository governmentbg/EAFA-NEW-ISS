

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class TicketValidationDTO { 
    public constructor(obj?: Partial<TicketValidationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public typeCode?: string;

    @StrictlyTyped(String)
    public periodCode?: string;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(String)
    public egnLnch?: string;

    @StrictlyTyped(Date)
    public childDateOfBirth?: Date;

    @StrictlyTyped(Boolean)
    public telkIsIndefinite?: boolean;

    @StrictlyTyped(Date)
    public telkValidTo?: Date;
}
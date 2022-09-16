

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class SystemPropertiesDTO { 
    public constructor(obj?: Partial<SystemPropertiesDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public elderTicketFemaleAge?: number;

    @StrictlyTyped(Number)
    public elderTicketMaleAge?: number;

    @StrictlyTyped(Number)
    public maxNumberOfUnder14Tickets?: number;

    @StrictlyTyped(Number)
    public maxNumberFishingGears?: number;

    @StrictlyTyped(String)
    public acdrUserID?: string;

    @StrictlyTyped(String)
    public acdrUserName?: string;
}
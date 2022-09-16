

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PortVisitDTO { 
    public constructor(obj?: Partial<PortVisitDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public portId?: number;

    @StrictlyTyped(String)
    public portName?: string;

    @StrictlyTyped(Number)
    public portCountryId?: number;

    @StrictlyTyped(Date)
    public visitDate?: Date;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
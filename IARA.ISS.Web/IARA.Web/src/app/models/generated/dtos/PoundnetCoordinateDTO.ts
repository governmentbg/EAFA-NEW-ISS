

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PoundnetCoordinateDTO { 
    public constructor(obj?: Partial<PoundnetCoordinateDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public longitude?: string;

    @StrictlyTyped(String)
    public latitude?: string;

    @StrictlyTyped(Boolean)
    public isConnectPoint?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
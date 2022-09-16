

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class AquacultureCoordinateDTO { 
    public constructor(obj?: Partial<AquacultureCoordinateDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public longitude?: string;

    @StrictlyTyped(String)
    public latitude?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
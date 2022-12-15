

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PrintConfigurationEditDTO { 
    public constructor(obj?: Partial<PrintConfigurationEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationTypeId?: number;

    @StrictlyTyped(Number)
    public territoryUnitId?: number;

    @StrictlyTyped(Number)
    public signUserId?: number;

    @StrictlyTyped(Number)
    public substituteUserId?: number;

    @StrictlyTyped(String)
    public substituteReason?: string;
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PrintConfigurationDTO { 
    public constructor(obj?: Partial<PrintConfigurationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public applicationTypeName?: string;

    @StrictlyTyped(String)
    public territoryUnitName?: string;

    @StrictlyTyped(String)
    public signUserNames?: string;

    @StrictlyTyped(String)
    public substituteUserNames?: string;

    @StrictlyTyped(String)
    public substituteReason?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
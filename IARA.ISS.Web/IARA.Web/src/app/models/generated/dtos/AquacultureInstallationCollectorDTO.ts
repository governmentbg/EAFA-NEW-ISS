

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class AquacultureInstallationCollectorDTO { 
    public constructor(obj?: Partial<AquacultureInstallationCollectorDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public collectorTypeId?: number;

    @StrictlyTyped(Number)
    public totalCount?: number;

    @StrictlyTyped(Number)
    public totalArea?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
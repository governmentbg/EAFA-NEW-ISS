

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class AquacultureInstallationDTO { 
    public constructor(obj?: Partial<AquacultureInstallationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public installationTypeName?: string;

    @StrictlyTyped(Number)
    public totalArea?: number;

    @StrictlyTyped(Number)
    public totalVolume?: number;

    @StrictlyTyped(Number)
    public totalCount?: number;

    @StrictlyTyped(Boolean)
    public hasValidationErrors?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
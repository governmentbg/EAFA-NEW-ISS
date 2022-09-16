

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class AquacultureInstallationBasinDTO { 
    public constructor(obj?: Partial<AquacultureInstallationBasinDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public basinPurposeTypeId?: number;

    @StrictlyTyped(Number)
    public basinMaterialTypeId?: number;

    @StrictlyTyped(Number)
    public count?: number;

    @StrictlyTyped(Number)
    public area?: number;

    @StrictlyTyped(Number)
    public volume?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
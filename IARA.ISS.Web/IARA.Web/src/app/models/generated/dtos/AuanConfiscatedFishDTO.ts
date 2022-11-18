

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class AuanConfiscatedFishDTO { 
    public constructor(obj?: Partial<AuanConfiscatedFishDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public fishTypeId?: number;

    @StrictlyTyped(Number)
    public weight?: number;

    @StrictlyTyped(Number)
    public length?: number;

    @StrictlyTyped(Number)
    public count?: number;

    @StrictlyTyped(Number)
    public confiscationActionId?: number;

    @StrictlyTyped(Number)
    public territoryUnitId?: number;

    @StrictlyTyped(Number)
    public turbotSizeGroupId?: number;

    @StrictlyTyped(Number)
    public applianceId?: number;

    @StrictlyTyped(Number)
    public lawSectionId?: number;

    @StrictlyTyped(String)
    public lawText?: string;

    @StrictlyTyped(String)
    public comments?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FishingGearInspectionNomenclatureDTO { 
    public constructor(obj?: Partial<FishingGearInspectionNomenclatureDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public subjectId?: number;

    @StrictlyTyped(Number)
    public typeId?: number;

    @StrictlyTyped(Number)
    public count?: number;

    @StrictlyTyped(Number)
    public length?: number;

    @StrictlyTyped(Number)
    public height?: number;

    @StrictlyTyped(Number)
    public netEyeSize?: number;

    @StrictlyTyped(Number)
    public hookCount?: number;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(Number)
    public towelLength?: number;

    @StrictlyTyped(Number)
    public houseLength?: number;

    @StrictlyTyped(Number)
    public houseWidth?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
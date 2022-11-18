

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { SturgeonGendersEnum } from '@app/enums/sturgeon-genders.enum';

export class CatchRecordFishDTO { 
    public constructor(obj?: Partial<CatchRecordFishDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public fishId?: number;

    @StrictlyTyped(String)
    public fishName?: string;

    @StrictlyTyped(Number)
    public quantityKg?: number;

    @StrictlyTyped(Number)
    public catchTypeId?: number;

    @StrictlyTyped(Number)
    public catchSizeId?: number;

    @StrictlyTyped(Number)
    public turbotCount?: number;

    @StrictlyTyped(Number)
    public turbotSizeGroupId?: number;

    @StrictlyTyped(Number)
    public sturgeonGender?: SturgeonGendersEnum;

    @StrictlyTyped(Number)
    public sturgeonWeightKg?: number;

    @StrictlyTyped(Number)
    public sturgeonSize?: number;

    @StrictlyTyped(Boolean)
    public isContinentalCatch?: boolean;

    @StrictlyTyped(String)
    public thirdCountryCatchZone?: string;

    @StrictlyTyped(Number)
    public catchQuadrantId?: number;

    @StrictlyTyped(String)
    public catchQuadrant?: string;

    @StrictlyTyped(String)
    public catchZone?: string;

    @StrictlyTyped(Boolean)
    public isDetainedOnBoard?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
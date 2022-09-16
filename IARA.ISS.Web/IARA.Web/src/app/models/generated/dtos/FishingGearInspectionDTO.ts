

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FishingGearInspectionDTO { 
    public constructor(obj?: Partial<FishingGearInspectionDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public subjectId?: number;

    @StrictlyTyped(Number)
    public permitId?: number;

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

    @StrictlyTyped(Number)
    public cordThickness?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FishingGearForChoiceDTO { 
    public constructor(obj?: Partial<FishingGearForChoiceDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public type?: string;

    @StrictlyTyped(Number)
    public count?: number;

    @StrictlyTyped(String)
    public marksNumbers?: string;

    @StrictlyTyped(Number)
    public length?: number;

    @StrictlyTyped(Number)
    public netEyeSize?: number;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(Boolean)
    public isChecked?: boolean;
}
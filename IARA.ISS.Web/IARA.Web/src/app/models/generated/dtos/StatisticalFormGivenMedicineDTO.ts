

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class StatisticalFormGivenMedicineDTO { 
    public constructor(obj?: Partial<StatisticalFormGivenMedicineDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public medicineType?: string;

    @StrictlyTyped(Number)
    public grams?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
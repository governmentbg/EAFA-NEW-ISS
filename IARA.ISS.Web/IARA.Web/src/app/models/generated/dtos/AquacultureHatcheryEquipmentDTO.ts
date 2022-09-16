

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class AquacultureHatcheryEquipmentDTO { 
    public constructor(obj?: Partial<AquacultureHatcheryEquipmentDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public equipmentTypeId?: number;

    @StrictlyTyped(Number)
    public count?: number;

    @StrictlyTyped(Number)
    public volume?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
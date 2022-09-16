

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';

export class FishingGearPingerDTO { 
    public constructor(obj?: Partial<FishingGearPingerDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public number?: string;

    @StrictlyTyped(Number)
    public statusId?: number;

    @StrictlyTyped(NomenclatureDTO)
    public selectedStatus?: NomenclatureDTO<number>;

    @StrictlyTyped(String)
    public brand?: string;

    @StrictlyTyped(String)
    public model?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
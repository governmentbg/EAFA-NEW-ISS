

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';

export class ScientificFishingOutingCatchDTO { 
    public constructor(obj?: Partial<ScientificFishingOutingCatchDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public outingId?: number;

    @StrictlyTyped(Number)
    public fishTypeId?: number;

    @StrictlyTyped(NomenclatureDTO)
    public fishType?: NomenclatureDTO<number>;

    @StrictlyTyped(Number)
    public catchUnder100?: number;

    @StrictlyTyped(Number)
    public catch100To500?: number;

    @StrictlyTyped(Number)
    public catch500To1000?: number;

    @StrictlyTyped(Number)
    public catchOver1000?: number;

    @StrictlyTyped(Number)
    public totalKeptCount?: number;

    @StrictlyTyped(Number)
    public totalCatch?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
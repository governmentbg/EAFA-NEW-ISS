

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { ShipNomenclatureFlags } from '@app/enums/ship-nomenclature-flags.enum';
import { ShipNomenclatureChangeFlags } from '@app/enums/ship-nomenclature-change-flags.enum';

export class ShipNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<ShipNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        }
        else {
            super();
        }
    }

    @StrictlyTyped(String)
    public cfr?: string;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public externalMark?: string;

    @StrictlyTyped(Number)
    public totalLength?: number;

    @StrictlyTyped(Number)
    public grossTonnage?: number;

    @StrictlyTyped(Number)
    public mainEnginePower?: number;

    @StrictlyTyped(Number)
    public flags?: ShipNomenclatureFlags;

    @StrictlyTyped(Number)
    public changeFlags?: ShipNomenclatureChangeFlags;

    @StrictlyTyped(Number)
    public shipIds?: number[];

    public eventData?: { [key: number]: ShipNomenclatureDTO; };
}
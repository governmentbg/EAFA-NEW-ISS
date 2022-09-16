
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';

export class VesselTypeNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<VesselTypeNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        }
        else {
            super();
        }
    }

    @StrictlyTyped(Number)
    public parentVesselTypeId?: number;
}
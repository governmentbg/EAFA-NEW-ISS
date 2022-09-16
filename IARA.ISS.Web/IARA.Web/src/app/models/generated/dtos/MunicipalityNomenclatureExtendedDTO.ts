
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';

export class MunicipalityNomenclatureExtendedDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<MunicipalityNomenclatureExtendedDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        }
        else {
            super();
        }
    }

    @StrictlyTyped(Number)
    public districtId?: number;
}
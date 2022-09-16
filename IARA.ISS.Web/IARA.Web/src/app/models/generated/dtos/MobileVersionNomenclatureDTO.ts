
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';

export class MobileVersionNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<MobileVersionNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        }
        else {
            super();
        }
    }

    @StrictlyTyped(Number)
    public version?: number;
}
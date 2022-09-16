
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';

export class SystemParameterNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<SystemParameterNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        }
        else {
            super();
        }
    }

    @StrictlyTyped(String)
    public paramValue?: string;
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO'; 

export class SailAreaNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<SailAreaNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public maxShoreDistance?: number;

    @StrictlyTyped(Number)
    public maxSeaState?: number;
}
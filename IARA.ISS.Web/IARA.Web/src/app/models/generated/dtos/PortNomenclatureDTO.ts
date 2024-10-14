

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO'; 

export class PortNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<PortNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Boolean)
    public isDanube?: boolean;

    @StrictlyTyped(Boolean)
    public isBlackSea?: boolean;
}
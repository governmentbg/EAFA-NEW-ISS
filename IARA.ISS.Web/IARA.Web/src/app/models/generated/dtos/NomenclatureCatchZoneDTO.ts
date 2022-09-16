

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { LocationDTO } from './LocationDTO'; 

export class NomenclatureCatchZoneDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<NomenclatureCatchZoneDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(LocationDTO)
    public points?: LocationDTO[];
}
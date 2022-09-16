

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { HasDescrNomenclatureDTO } from './HasDescrNomenclatureDTO'; 

export class VesselActivityNomenclatureDTO extends HasDescrNomenclatureDTO {
    public constructor(obj?: Partial<VesselActivityNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as HasDescrNomenclatureDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Boolean)
    public isFishing?: boolean;
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO'; 

export class InspectorUserNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<InspectorUserNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(String)
    public issuerPosition?: string;

    @StrictlyTyped(String)
    public departmentName?: string;

    @StrictlyTyped(String)
    public departmentAddress?: string;
}
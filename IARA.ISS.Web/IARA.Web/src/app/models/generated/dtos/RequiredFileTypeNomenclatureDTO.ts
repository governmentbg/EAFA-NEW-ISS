

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO'; 

export class RequiredFileTypeNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<RequiredFileTypeNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public fileTypeId?: number;

    @StrictlyTyped(Boolean)
    public isMandatory?: boolean;
}
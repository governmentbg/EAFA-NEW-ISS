

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO'; 

export class ScientificFishingReasonNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<ScientificFishingReasonNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Boolean)
    public isLegalReason?: boolean;
}
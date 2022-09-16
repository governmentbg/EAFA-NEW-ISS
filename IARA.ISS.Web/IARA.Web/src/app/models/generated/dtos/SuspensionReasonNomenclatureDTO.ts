

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO'; 

export class SuspensionReasonNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<SuspensionReasonNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public suspensionTypeId?: number;

    @StrictlyTyped(Number)
    public monthsDuration?: number;
}
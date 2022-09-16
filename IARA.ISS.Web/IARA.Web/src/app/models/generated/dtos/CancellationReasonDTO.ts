

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { CancellationReasonGroupEnum } from '@app/enums/cancellation-reason-group.enum'; 

export class CancellationReasonDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<CancellationReasonDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public group?: CancellationReasonGroupEnum;
}
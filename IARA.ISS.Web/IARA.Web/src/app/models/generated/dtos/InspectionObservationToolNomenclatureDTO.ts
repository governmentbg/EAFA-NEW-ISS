

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { ObservationToolOnBoardEnum } from '@app/enums/observation-tool-on-board.enum'; 

export class InspectionObservationToolNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<InspectionObservationToolNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public onBoard?: ObservationToolOnBoardEnum;
}
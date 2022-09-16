

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO'; 

export class PopulatedAreaNomenclatureExtendedDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<PopulatedAreaNomenclatureExtendedDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public municipalityId?: number;

    @StrictlyTyped(String)
    public areaType?: string;
}
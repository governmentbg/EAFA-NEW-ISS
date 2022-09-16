

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO'; 

export class StatisticalFormAquacultureNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<StatisticalFormAquacultureNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(String)
    public urorNum?: string;

    @StrictlyTyped(String)
    public regNum?: string;

    @StrictlyTyped(String)
    public eik?: string;

    @StrictlyTyped(String)
    public legalName?: string;
}
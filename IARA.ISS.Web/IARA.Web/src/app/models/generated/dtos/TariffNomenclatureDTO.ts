

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO'; 

export class TariffNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<TariffNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(String)
    public basedOnPlea?: string;

    @StrictlyTyped(Boolean)
    public isCalculated?: boolean;

    @StrictlyTyped(Number)
    public price?: number;
}
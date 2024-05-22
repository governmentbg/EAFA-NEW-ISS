

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO'; 

export class TerritoryUnitNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<TerritoryUnitNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(String)
    public address?: string;

    @StrictlyTyped(String)
    public phone?: string;

    @StrictlyTyped(String)
    public workingTime?: string;

    @StrictlyTyped(String)
    public deliveryTerritoryUniitMessage?: string;
}
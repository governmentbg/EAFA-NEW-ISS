

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { DeclarationLogBookPageFishDTO } from './DeclarationLogBookPageFishDTO'; 

export class InspectionLogBookPageNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<InspectionLogBookPageNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(String)
    public originDeclarationNum?: string;

    @StrictlyTyped(Date)
    public originDeclarationDate?: Date;

    @StrictlyTyped(Number)
    public logPageNum?: number;

    @StrictlyTyped(Date)
    public logBookPageDate?: Date;

    @StrictlyTyped(DeclarationLogBookPageFishDTO)
    public logBookProducts?: DeclarationLogBookPageFishDTO[];
}
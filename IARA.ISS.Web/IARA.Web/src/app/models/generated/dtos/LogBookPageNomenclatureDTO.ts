

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO'; 

export class LogBookPageNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<LogBookPageNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(String)
    public productInfo?: string;

    @StrictlyTyped(Date)
    public fillDate?: Date;

    @StrictlyTyped(Number)
    public pageNumber?: number;
}
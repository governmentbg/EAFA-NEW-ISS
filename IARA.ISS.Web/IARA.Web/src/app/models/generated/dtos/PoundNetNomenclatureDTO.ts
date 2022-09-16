

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO'; 

export class PoundNetNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<PoundNetNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(String)
    public statusCode?: string;

    @StrictlyTyped(String)
    public statusName?: string;

    @StrictlyTyped(Boolean)
    public hasPoundNetPermit?: boolean;

    @StrictlyTyped(String)
    public depth?: string;
}
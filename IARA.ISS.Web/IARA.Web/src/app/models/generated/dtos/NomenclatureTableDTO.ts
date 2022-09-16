

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO'; 

export class NomenclatureTableDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<NomenclatureTableDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public groupId?: number;

    @StrictlyTyped(String)
    public tableName?: string;

    @StrictlyTyped(Boolean)
    public canInsertRows?: boolean;

    @StrictlyTyped(Boolean)
    public canEditRows?: boolean;

    @StrictlyTyped(Boolean)
    public canDeleteRows?: boolean;
}
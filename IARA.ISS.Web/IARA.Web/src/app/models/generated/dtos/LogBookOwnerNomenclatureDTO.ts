

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum'; 

export class LogBookOwnerNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<LogBookOwnerNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public personId?: number;

    @StrictlyTyped(Number)
    public legalId?: number;

    @StrictlyTyped(Number)
    public buyerId?: number;

    @StrictlyTyped(String)
    public ownerName?: string;

    @StrictlyTyped(String)
    public logBookNumber?: string;

    @StrictlyTyped(Number)
    public logBookType?: LogBookTypesEnum;

    @StrictlyTyped(Number)
    public ownerType?: LogBookPagePersonTypesEnum;
}
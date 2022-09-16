

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ChangeOfCircumstancesDataTypeEnum } from '@app/enums/change-of-circumstances-data-type.enum'; 

export class ChangeOfCircumstancesTypeDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<ChangeOfCircumstancesTypeDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(Number)
    public dataType?: ChangeOfCircumstancesDataTypeEnum;

    @StrictlyTyped(Boolean)
    public isDeletion?: boolean;
}
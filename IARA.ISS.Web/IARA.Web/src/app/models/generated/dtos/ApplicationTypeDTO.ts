

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum'; 

export class ApplicationTypeDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<ApplicationTypeDTO>) {
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
    public groupId?: number;

    @StrictlyTyped(String)
    public groupName?: string;

    @StrictlyTyped(Boolean)
    public isChecked?: boolean;
}
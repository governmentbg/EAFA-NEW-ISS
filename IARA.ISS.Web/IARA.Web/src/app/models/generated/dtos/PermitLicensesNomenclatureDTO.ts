

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PermitNomenclatureDTO } from './PermitNomenclatureDTO'; 

export class PermitLicensesNomenclatureDTO extends PermitNomenclatureDTO {
    public constructor(obj?: Partial<PermitLicensesNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as PermitNomenclatureDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(String)
    public waterTypeCode?: string;

    @StrictlyTyped(String)
    public tariffCodes?: string[];
}
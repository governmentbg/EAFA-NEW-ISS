

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO'; 

export class InspectedPermitLicenseNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<InspectedPermitLicenseNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public year?: number;

    @StrictlyTyped(String)
    public unregisteredPermitLicenseNum?: string;

    @StrictlyTyped(String)
    public inspectionReportNum?: string;
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO'; 

export class InspectionPermitLicenseDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<InspectionPermitLicenseDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(String)
    public permitNumber?: string;

    @StrictlyTyped(String)
    public licenseNumber?: string;

    @StrictlyTyped(String)
    public typeName?: string;

    @StrictlyTyped(Number)
    public typeId?: number;

    @StrictlyTyped(String)
    public waterType?: string;

    @StrictlyTyped(String)
    public captainName?: string;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public validTo?: Date;
}
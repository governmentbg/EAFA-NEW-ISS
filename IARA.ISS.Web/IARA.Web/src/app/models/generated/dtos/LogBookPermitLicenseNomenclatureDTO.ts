

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { WaterTypesEnum } from '@app/enums/water-types.enum'; 

export class LogBookPermitLicenseNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<LogBookPermitLicenseNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public permitLicenseId?: number;

    @StrictlyTyped(Number)
    public logBookId?: number;

    @StrictlyTyped(String)
    public permitLicenseNumber?: string;

    @StrictlyTyped(String)
    public permitLicenseName?: string;

    @StrictlyTyped(Number)
    public permitLicenseWaterType?: WaterTypesEnum;

    @StrictlyTyped(String)
    public permitLicenseWaterTypeName?: string;
}
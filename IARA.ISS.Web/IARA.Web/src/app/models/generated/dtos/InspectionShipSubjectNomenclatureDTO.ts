

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { EgnLncDTO } from './EgnLncDTO';
import { InspectionSubjectAddressDTO } from './InspectionSubjectAddressDTO';
import { InspectedPersonTypeEnum } from '@app/enums/inspected-person-type.enum'; 

export class InspectionShipSubjectNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<InspectionShipSubjectNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public entryId?: number;

    @StrictlyTyped(EgnLncDTO)
    public egnLnc?: EgnLncDTO;

    @StrictlyTyped(String)
    public eik?: string;

    @StrictlyTyped(String)
    public firstName?: string;

    @StrictlyTyped(String)
    public middleName?: string;

    @StrictlyTyped(String)
    public lastName?: string;

    @StrictlyTyped(Boolean)
    public isLegal?: boolean;

    @StrictlyTyped(Number)
    public type?: InspectedPersonTypeEnum;

    @StrictlyTyped(InspectionSubjectAddressDTO)
    public address?: InspectionSubjectAddressDTO;

    @StrictlyTyped(Number)
    public countryId?: number;
}
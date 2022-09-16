

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { EgnLncDTO } from './EgnLncDTO';
import { InspectionSubjectAddressDTO } from './InspectionSubjectAddressDTO';
import { InspectedPersonTypeEnum } from '@app/enums/inspected-person-type.enum'; 

export class InspectedBuyerNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<InspectedBuyerNomenclatureDTO>) {
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

    @StrictlyTyped(Boolean)
    public hasUtility?: boolean;

    @StrictlyTyped(Boolean)
    public hasVehicle?: boolean;

    @StrictlyTyped(String)
    public utilityName?: string;

    @StrictlyTyped(InspectionSubjectAddressDTO)
    public utilityAddress?: InspectionSubjectAddressDTO;

    @StrictlyTyped(String)
    public vehicleNumber?: string;
}
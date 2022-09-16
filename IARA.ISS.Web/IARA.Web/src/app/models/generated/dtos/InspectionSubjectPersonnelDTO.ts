

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { UnregisteredPersonDTO } from './UnregisteredPersonDTO';
import { InspectionSubjectAddressDTO } from './InspectionSubjectAddressDTO';
import { InspectedPersonTypeEnum } from '@app/enums/inspected-person-type.enum'; 

export class InspectionSubjectPersonnelDTO extends UnregisteredPersonDTO {
    public constructor(obj?: Partial<InspectionSubjectPersonnelDTO>) {
        if (obj != undefined) {
            super(obj as UnregisteredPersonDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Boolean)
    public isRegistered?: boolean;

    @StrictlyTyped(Number)
    public entryId?: number;

    @StrictlyTyped(Number)
    public type?: InspectedPersonTypeEnum;

    @StrictlyTyped(InspectionSubjectAddressDTO)
    public registeredAddress?: InspectionSubjectAddressDTO;
}
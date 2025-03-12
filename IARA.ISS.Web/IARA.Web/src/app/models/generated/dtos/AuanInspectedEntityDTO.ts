

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';
import { UnregisteredPersonDTO } from './UnregisteredPersonDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { InspectedEntityControlActivityInfoDTO } from './InspectedEntityControlActivityInfoDTO'; 

export class AuanInspectedEntityDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<AuanInspectedEntityDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Boolean)
    public isUnregisteredPerson?: boolean;

    @StrictlyTyped(Boolean)
    public isPerson?: boolean;

    @StrictlyTyped(RegixPersonDataDTO)
    public person?: RegixPersonDataDTO;

    @StrictlyTyped(RegixLegalDataDTO)
    public legal?: RegixLegalDataDTO;

    @StrictlyTyped(UnregisteredPersonDTO)
    public unregisteredPerson?: UnregisteredPersonDTO;

    @StrictlyTyped(String)
    public personWorkPlace?: string;

    @StrictlyTyped(String)
    public personWorkPosition?: string;

    @StrictlyTyped(AddressRegistrationDTO)
    public addresses?: AddressRegistrationDTO[];

    @StrictlyTyped(InspectedEntityControlActivityInfoDTO)
    public inspectedEntityControlActivityInfo?: InspectedEntityControlActivityInfoDTO;
}
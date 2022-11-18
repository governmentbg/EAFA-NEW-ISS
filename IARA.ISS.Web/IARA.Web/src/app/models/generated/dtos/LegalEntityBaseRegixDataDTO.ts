

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BaseRegixChecksDTO } from './BaseRegixChecksDTO';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { RegixLegalDataDTO } from './RegixLegalDataDTO'; 

export class LegalEntityBaseRegixDataDTO extends BaseRegixChecksDTO {
    public constructor(obj?: Partial<LegalEntityBaseRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as BaseRegixChecksDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(RegixPersonDataDTO)
    public requester?: RegixPersonDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public requesterAddresses?: AddressRegistrationDTO[];

    @StrictlyTyped(RegixLegalDataDTO)
    public legal?: RegixLegalDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public addresses?: AddressRegistrationDTO[];
}
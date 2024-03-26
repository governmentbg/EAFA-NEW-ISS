

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BaseRegixChecksDTO } from './BaseRegixChecksDTO';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { RecreationalFishingTelkDTO } from './RecreationalFishingTelkDTO';
import { FileInfoDTO } from './FileInfoDTO'; 

export class RecreationalFishingTicketBaseRegixDataDTO extends BaseRegixChecksDTO {
    public constructor(obj?: Partial<RecreationalFishingTicketBaseRegixDataDTO>) {
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
    public person?: RegixPersonDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public personAddressRegistrations?: AddressRegistrationDTO[];

    @StrictlyTyped(RegixPersonDataDTO)
    public representativePerson?: RegixPersonDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public representativePersonAddressRegistrations?: AddressRegistrationDTO[];

    @StrictlyTyped(RecreationalFishingTelkDTO)
    public telkData?: RecreationalFishingTelkDTO;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}
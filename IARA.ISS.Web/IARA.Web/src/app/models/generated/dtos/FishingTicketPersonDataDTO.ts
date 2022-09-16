

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';

export class FishingTicketPersonDataDTO { 
    public constructor(obj?: Partial<FishingTicketPersonDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(RegixPersonDataDTO)
    public person?: RegixPersonDataDTO;

    @StrictlyTyped(FileInfoDTO)
    public photo?: FileInfoDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public userAddresses?: AddressRegistrationDTO[];
}
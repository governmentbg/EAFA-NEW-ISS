
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { FileInfoDTO } from './FileInfoDTO';

export class RecreationalFishingUserTicketDataDTO {
    public constructor(obj?: Partial<RecreationalFishingUserTicketDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(RegixPersonDataDTO)
    public person?: RegixPersonDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public addressRegistrations?: AddressRegistrationDTO[];

    @StrictlyTyped(FileInfoDTO)
    public photo?: FileInfoDTO;
}
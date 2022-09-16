
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { FileInfoDTO } from './FileInfoDTO';

export class RecreationalFishingTicketHolderDTO {
    public constructor(obj?: Partial<RecreationalFishingTicketHolderDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(RegixPersonDataDTO)
    public person?: RegixPersonDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public addresses?: AddressRegistrationDTO[];

    @StrictlyTyped(FileInfoDTO)
    public photo?: FileInfoDTO;
}
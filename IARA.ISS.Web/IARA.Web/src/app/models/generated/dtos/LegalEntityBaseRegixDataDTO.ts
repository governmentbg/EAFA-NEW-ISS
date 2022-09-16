
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';

export class LegalEntityBaseRegixDataDTO {
    public constructor(obj?: Partial<LegalEntityBaseRegixDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(RegixPersonDataDTO)
    public requester?: RegixPersonDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public requesterAddresses?: AddressRegistrationDTO[];

    @StrictlyTyped(RegixLegalDataDTO)
    public legal?: RegixLegalDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public addresses?: AddressRegistrationDTO[];

    @StrictlyTyped(String)
    public statusReason?: string;
}
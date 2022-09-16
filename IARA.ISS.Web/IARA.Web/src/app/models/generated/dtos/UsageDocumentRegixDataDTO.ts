

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';

export class UsageDocumentRegixDataDTO { 
    public constructor(obj?: Partial<UsageDocumentRegixDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public documentTypeId?: number;

    @StrictlyTyped(Boolean)
    public isLessorPerson?: boolean;

    @StrictlyTyped(RegixPersonDataDTO)
    public lessorPerson?: RegixPersonDataDTO;

    @StrictlyTyped(RegixLegalDataDTO)
    public lessorLegal?: RegixLegalDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public lessorAddresses?: AddressRegistrationDTO[];

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
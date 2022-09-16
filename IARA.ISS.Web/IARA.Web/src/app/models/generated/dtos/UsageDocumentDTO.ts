

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';

export class UsageDocumentDTO { 
    public constructor(obj?: Partial<UsageDocumentDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public documentTypeId?: number;

    @StrictlyTyped(String)
    public documentNum?: string;

    @StrictlyTyped(Boolean)
    public isDocumentIndefinite?: boolean;

    @StrictlyTyped(Date)
    public documentValidFrom?: Date;

    @StrictlyTyped(Date)
    public documentValidTo?: Date;

    @StrictlyTyped(Boolean)
    public isLessorPerson?: boolean;

    @StrictlyTyped(RegixPersonDataDTO)
    public lessorPerson?: RegixPersonDataDTO;

    @StrictlyTyped(RegixLegalDataDTO)
    public lessorLegal?: RegixLegalDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public lessorAddresses?: AddressRegistrationDTO[];

    @StrictlyTyped(String)
    public comments?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
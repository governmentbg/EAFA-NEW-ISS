

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';

export class LegalFullDataDTO { 
    public constructor(obj?: Partial<LegalFullDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(RegixLegalDataDTO)
    public legal?: RegixLegalDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public addresses?: AddressRegistrationDTO[];
}
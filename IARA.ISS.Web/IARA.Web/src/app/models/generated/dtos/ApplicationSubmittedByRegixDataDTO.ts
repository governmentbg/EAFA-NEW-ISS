

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';

export class ApplicationSubmittedByRegixDataDTO { 
    public constructor(obj?: Partial<ApplicationSubmittedByRegixDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(RegixPersonDataDTO)
    public person?: RegixPersonDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public addresses?: AddressRegistrationDTO[];
}
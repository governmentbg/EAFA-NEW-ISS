

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { SubmittedByRolesEnum } from '@app/enums/submitted-by-roles.enum';

export class ApplicationSubmittedForRegixDataDTO { 
    public constructor(obj?: Partial<ApplicationSubmittedForRegixDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public submittedByRole?: SubmittedByRolesEnum;

    @StrictlyTyped(RegixLegalDataDTO)
    public legal?: RegixLegalDataDTO;

    @StrictlyTyped(RegixPersonDataDTO)
    public person?: RegixPersonDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public addresses?: AddressRegistrationDTO[];
}
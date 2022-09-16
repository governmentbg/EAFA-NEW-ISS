

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';

export class ShipOwnerRegixDataDTO { 
    public constructor(obj?: Partial<ShipOwnerRegixDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public names?: string;

    @StrictlyTyped(String)
    public egnLncEik?: string;

    @StrictlyTyped(Boolean)
    public isOwnerPerson?: boolean;

    @StrictlyTyped(RegixPersonDataDTO)
    public regixPersonData?: RegixPersonDataDTO;

    @StrictlyTyped(RegixLegalDataDTO)
    public regixLegalData?: RegixLegalDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public addressRegistrations?: AddressRegistrationDTO[];

    @StrictlyTyped(Boolean)
    public hasValidationErrors?: boolean;

    @StrictlyTyped(Boolean)
    public hasRegixDataDiscrepancy?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
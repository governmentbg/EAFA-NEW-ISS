

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';

export class ScientificFishingPermitHolderRegixDataDTO { 
    public constructor(obj?: Partial<ScientificFishingPermitHolderRegixDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public ownerId?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public egn?: string;

    @StrictlyTyped(Boolean)
    public hasValidationErrors?: boolean;

    @StrictlyTyped(Boolean)
    public hasRegixDataDiscrepancy?: boolean;

    @StrictlyTyped(RegixPersonDataDTO)
    public regixPersonData?: RegixPersonDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public addressRegistrations?: AddressRegistrationDTO[];

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
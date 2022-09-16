

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';

export class FishingCapacityHolderRegixDataDTO { 
    public constructor(obj?: Partial<FishingCapacityHolderRegixDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Boolean)
    public isHolderPerson?: boolean;

    @StrictlyTyped(RegixPersonDataDTO)
    public person?: RegixPersonDataDTO;

    @StrictlyTyped(RegixLegalDataDTO)
    public legal?: RegixLegalDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public addresses?: AddressRegistrationDTO[];

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public egnEik?: string;

    @StrictlyTyped(Boolean)
    public hasRegixDataDiscrepancy?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
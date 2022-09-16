

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';

export class FishingCapacityCertificateEditDTO { 
    public constructor(obj?: Partial<FishingCapacityCertificateEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public duplicateOfCertificateNum?: string;

    @StrictlyTyped(String)
    public certificateNum?: string;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public validTo?: Date;

    @StrictlyTyped(Number)
    public grossTonnage?: number;

    @StrictlyTyped(Number)
    public power?: number;

    @StrictlyTyped(Boolean)
    public isHolderPerson?: boolean;

    @StrictlyTyped(RegixPersonDataDTO)
    public person?: RegixPersonDataDTO;

    @StrictlyTyped(RegixLegalDataDTO)
    public legal?: RegixLegalDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public addresses?: AddressRegistrationDTO[];

    @StrictlyTyped(String)
    public comments?: string;
}
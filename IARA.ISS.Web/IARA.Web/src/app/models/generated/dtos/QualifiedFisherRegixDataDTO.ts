

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { ApplicationRegiXCheckDTO } from './ApplicationRegiXCheckDTO';
import { SubmittedByRolesEnum } from '@app/enums/submitted-by-roles.enum';

export class QualifiedFisherRegixDataDTO { 
    public constructor(obj?: Partial<QualifiedFisherRegixDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(RegixPersonDataDTO)
    public submittedByRegixData?: RegixPersonDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public submittedByAddresses?: AddressRegistrationDTO[];

    @StrictlyTyped(Number)
    public submittedByRole?: SubmittedByRolesEnum;

    @StrictlyTyped(RegixPersonDataDTO)
    public submittedForRegixData?: RegixPersonDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public submittedForAddresses?: AddressRegistrationDTO[];

    @StrictlyTyped(ApplicationRegiXCheckDTO)
    public applicationRegiXChecks?: ApplicationRegiXCheckDTO[];

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(String)
    public statusReason?: string;
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BaseRegixChecksDTO } from './BaseRegixChecksDTO';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { SubmittedByRolesEnum } from '@app/enums/submitted-by-roles.enum'; 

export class QualifiedFisherRegixDataDTO extends BaseRegixChecksDTO {
    public constructor(obj?: Partial<QualifiedFisherRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as BaseRegixChecksDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
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

    @StrictlyTyped(Number)
    public id?: number;
}
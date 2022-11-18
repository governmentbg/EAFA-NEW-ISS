

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FishingAssociationBaseRegixDataDTO } from './FishingAssociationBaseRegixDataDTO';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';
import { FishingAssociationPersonDTO } from './FishingAssociationPersonDTO'; 

export class FishingAssociationRegixDataDTO extends FishingAssociationBaseRegixDataDTO {
    public constructor(obj?: Partial<FishingAssociationRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as FishingAssociationBaseRegixDataDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(RegixPersonDataDTO)
    public submittedBy?: RegixPersonDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public submittedByAddresses?: AddressRegistrationDTO[];

    @StrictlyTyped(RegixLegalDataDTO)
    public submittedFor?: RegixLegalDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public submittedForAddresses?: AddressRegistrationDTO[];

    @StrictlyTyped(FishingAssociationPersonDTO)
    public persons?: FishingAssociationPersonDTO[];
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FishingAssociationBaseRegixDataDTO } from './FishingAssociationBaseRegixDataDTO';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';
import { FishingAssociationPersonDTO } from './FishingAssociationPersonDTO';
import { ApplicationPaymentInformationDTO } from './ApplicationPaymentInformationDTO';
import { ApplicationBaseDeliveryDTO } from './ApplicationBaseDeliveryDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { FishingAssociationRegixDataDTO } from './FishingAssociationRegixDataDTO'; 

export class FishingAssociationApplicationEditDTO extends FishingAssociationBaseRegixDataDTO {
    public constructor(obj?: Partial<FishingAssociationApplicationEditDTO>) {
        if (obj != undefined) {
            super(obj as FishingAssociationBaseRegixDataDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(Boolean)
    public isPaid?: boolean;

    @StrictlyTyped(Boolean)
    public hasDelivery?: boolean;

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

    @StrictlyTyped(ApplicationPaymentInformationDTO)
    public paymentInformation?: ApplicationPaymentInformationDTO;

    @StrictlyTyped(ApplicationBaseDeliveryDTO)
    public deliveryData?: ApplicationBaseDeliveryDTO;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];

    @StrictlyTyped(FishingAssociationRegixDataDTO)
    public regiXDataModel?: FishingAssociationRegixDataDTO;
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BaseRegixChecksDTO } from './BaseRegixChecksDTO';
import { ApplicationSubmittedByDTO } from './ApplicationSubmittedByDTO';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { ApplicationPaymentInformationDTO } from './ApplicationPaymentInformationDTO';
import { ApplicationBaseDeliveryDTO } from './ApplicationBaseDeliveryDTO';
import { FishingGearDTO } from './FishingGearDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { PermitLicenseFishingGearsRegixDataDTO } from './PermitLicenseFishingGearsRegixDataDTO'; 

export class PermitLicenseFishingGearsApplicationDTO extends BaseRegixChecksDTO {
    public constructor(obj?: Partial<PermitLicenseFishingGearsApplicationDTO>) {
        if (obj != undefined) {
            super(obj as BaseRegixChecksDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(Boolean)
    public hasDelivery?: boolean;

    @StrictlyTyped(Boolean)
    public isPaid?: boolean;

    @StrictlyTyped(Boolean)
    public isDraft?: boolean;

    @StrictlyTyped(Number)
    public permitLicenseId?: number;

    @StrictlyTyped(String)
    public permitLicenseNumber?: string;

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(ApplicationSubmittedByDTO)
    public submittedBy?: ApplicationSubmittedByDTO;

    @StrictlyTyped(ApplicationSubmittedForDTO)
    public submittedFor?: ApplicationSubmittedForDTO;

    @StrictlyTyped(ApplicationPaymentInformationDTO)
    public paymentInformation?: ApplicationPaymentInformationDTO;

    @StrictlyTyped(ApplicationBaseDeliveryDTO)
    public deliveryData?: ApplicationBaseDeliveryDTO;

    @StrictlyTyped(FishingGearDTO)
    public fishingGears?: FishingGearDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];

    @StrictlyTyped(PermitLicenseFishingGearsRegixDataDTO)
    public regiXDataModel?: PermitLicenseFishingGearsRegixDataDTO;
}
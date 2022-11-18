

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { TransferFishingCapacityBaseRegixDataDTO } from './TransferFishingCapacityBaseRegixDataDTO';
import { ApplicationSubmittedByDTO } from './ApplicationSubmittedByDTO';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { FishingCapacityHolderDTO } from './FishingCapacityHolderDTO';
import { ApplicationPaymentInformationDTO } from './ApplicationPaymentInformationDTO';
import { ApplicationBaseDeliveryDTO } from './ApplicationBaseDeliveryDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { TransferFishingCapacityRegixDataDTO } from './TransferFishingCapacityRegixDataDTO'; 

export class TransferFishingCapacityApplicationDTO extends TransferFishingCapacityBaseRegixDataDTO {
    public constructor(obj?: Partial<TransferFishingCapacityApplicationDTO>) {
        if (obj != undefined) {
            super(obj as TransferFishingCapacityBaseRegixDataDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(Boolean)
    public isDraft?: boolean;

    @StrictlyTyped(Boolean)
    public isPaid?: boolean;

    @StrictlyTyped(Boolean)
    public hasDelivery?: boolean;

    @StrictlyTyped(ApplicationSubmittedByDTO)
    public submittedBy?: ApplicationSubmittedByDTO;

    @StrictlyTyped(ApplicationSubmittedForDTO)
    public submittedFor?: ApplicationSubmittedForDTO;

    @StrictlyTyped(Number)
    public capacityCertificateId?: number;

    @StrictlyTyped(FishingCapacityHolderDTO)
    public holders?: FishingCapacityHolderDTO[];

    @StrictlyTyped(ApplicationPaymentInformationDTO)
    public paymentInformation?: ApplicationPaymentInformationDTO;

    @StrictlyTyped(ApplicationBaseDeliveryDTO)
    public deliveryData?: ApplicationBaseDeliveryDTO;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];

    @StrictlyTyped(TransferFishingCapacityRegixDataDTO)
    public regiXDataModel?: TransferFishingCapacityRegixDataDTO;
}
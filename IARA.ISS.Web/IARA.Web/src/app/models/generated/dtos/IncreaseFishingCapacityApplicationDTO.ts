

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { IncreaseFishingCapacityBaseRegixDataDTO } from './IncreaseFishingCapacityBaseRegixDataDTO';
import { ApplicationSubmittedByDTO } from './ApplicationSubmittedByDTO';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { AcquiredFishingCapacityDTO } from './AcquiredFishingCapacityDTO';
import { FishingCapacityFreedActionsDTO } from './FishingCapacityFreedActionsDTO';
import { ApplicationPaymentInformationDTO } from './ApplicationPaymentInformationDTO';
import { ApplicationBaseDeliveryDTO } from './ApplicationBaseDeliveryDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { IncreaseFishingCapacityRegixDataDTO } from './IncreaseFishingCapacityRegixDataDTO'; 

export class IncreaseFishingCapacityApplicationDTO extends IncreaseFishingCapacityBaseRegixDataDTO {
    public constructor(obj?: Partial<IncreaseFishingCapacityApplicationDTO>) {
        if (obj != undefined) {
            super(obj as IncreaseFishingCapacityBaseRegixDataDTO);
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

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(ApplicationSubmittedByDTO)
    public submittedBy?: ApplicationSubmittedByDTO;

    @StrictlyTyped(ApplicationSubmittedForDTO)
    public submittedFor?: ApplicationSubmittedForDTO;

    @StrictlyTyped(Number)
    public increaseGrossTonnageBy?: number;

    @StrictlyTyped(Number)
    public increasePowerBy?: number;

    @StrictlyTyped(AcquiredFishingCapacityDTO)
    public acquiredCapacity?: AcquiredFishingCapacityDTO;

    @StrictlyTyped(FishingCapacityFreedActionsDTO)
    public remainingCapacityAction?: FishingCapacityFreedActionsDTO;

    @StrictlyTyped(ApplicationPaymentInformationDTO)
    public paymentInformation?: ApplicationPaymentInformationDTO;

    @StrictlyTyped(ApplicationBaseDeliveryDTO)
    public deliveryData?: ApplicationBaseDeliveryDTO;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];

    @StrictlyTyped(IncreaseFishingCapacityRegixDataDTO)
    public regiXDataModel?: IncreaseFishingCapacityRegixDataDTO;
}
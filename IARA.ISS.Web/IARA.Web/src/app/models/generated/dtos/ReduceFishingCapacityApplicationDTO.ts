

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ReduceFishingCapacityBaseRegixDataDTO } from './ReduceFishingCapacityBaseRegixDataDTO';
import { ApplicationSubmittedByDTO } from './ApplicationSubmittedByDTO';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { FishingCapacityFreedActionsDTO } from './FishingCapacityFreedActionsDTO';
import { ApplicationPaymentInformationDTO } from './ApplicationPaymentInformationDTO';
import { ApplicationBaseDeliveryDTO } from './ApplicationBaseDeliveryDTO';
import { FileInfoDTO } from './FileInfoDTO'; 

export class ReduceFishingCapacityApplicationDTO extends ReduceFishingCapacityBaseRegixDataDTO {
    public constructor(obj?: Partial<ReduceFishingCapacityApplicationDTO>) {
        if (obj != undefined) {
            super(obj as ReduceFishingCapacityBaseRegixDataDTO);
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
    public reduceGrossTonnageBy?: number;

    @StrictlyTyped(Number)
    public reducePowerBy?: number;

    @StrictlyTyped(FishingCapacityFreedActionsDTO)
    public freedCapacityAction?: FishingCapacityFreedActionsDTO;

    @StrictlyTyped(ApplicationPaymentInformationDTO)
    public paymentInformation?: ApplicationPaymentInformationDTO;

    @StrictlyTyped(ApplicationBaseDeliveryDTO)
    public deliveryData?: ApplicationBaseDeliveryDTO;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}
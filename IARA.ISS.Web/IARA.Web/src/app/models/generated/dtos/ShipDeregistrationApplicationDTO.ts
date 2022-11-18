

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ShipDeregistrationBaseRegixDataDTO } from './ShipDeregistrationBaseRegixDataDTO';
import { ApplicationSubmittedByDTO } from './ApplicationSubmittedByDTO';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { FishingCapacityFreedActionsDTO } from './FishingCapacityFreedActionsDTO';
import { ApplicationPaymentInformationDTO } from './ApplicationPaymentInformationDTO';
import { ApplicationBaseDeliveryDTO } from './ApplicationBaseDeliveryDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { ShipDeregistrationRegixDataDTO } from './ShipDeregistrationRegixDataDTO'; 

export class ShipDeregistrationApplicationDTO extends ShipDeregistrationBaseRegixDataDTO {
    public constructor(obj?: Partial<ShipDeregistrationApplicationDTO>) {
        if (obj != undefined) {
            super(obj as ShipDeregistrationBaseRegixDataDTO);
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

    @StrictlyTyped(String)
    public deregistrationReason?: string;

    @StrictlyTyped(ApplicationSubmittedByDTO)
    public submittedBy?: ApplicationSubmittedByDTO;

    @StrictlyTyped(ApplicationSubmittedForDTO)
    public submittedFor?: ApplicationSubmittedForDTO;

    @StrictlyTyped(FishingCapacityFreedActionsDTO)
    public freedCapacityAction?: FishingCapacityFreedActionsDTO;

    @StrictlyTyped(ApplicationPaymentInformationDTO)
    public paymentInformation?: ApplicationPaymentInformationDTO;

    @StrictlyTyped(ApplicationBaseDeliveryDTO)
    public deliveryData?: ApplicationBaseDeliveryDTO;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];

    @StrictlyTyped(ShipDeregistrationRegixDataDTO)
    public regiXDataModel?: ShipDeregistrationRegixDataDTO;
}
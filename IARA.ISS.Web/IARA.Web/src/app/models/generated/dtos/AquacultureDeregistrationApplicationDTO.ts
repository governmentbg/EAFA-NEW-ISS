

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AquacultureDeregistrationBaseRegixDataDTO } from './AquacultureDeregistrationBaseRegixDataDTO';
import { ApplicationSubmittedByDTO } from './ApplicationSubmittedByDTO';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { ApplicationPaymentInformationDTO } from './ApplicationPaymentInformationDTO';
import { ApplicationBaseDeliveryDTO } from './ApplicationBaseDeliveryDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { AquacultureDeregistrationRegixDataDTO } from './AquacultureDeregistrationRegixDataDTO'; 

export class AquacultureDeregistrationApplicationDTO extends AquacultureDeregistrationBaseRegixDataDTO {
    public constructor(obj?: Partial<AquacultureDeregistrationApplicationDTO>) {
        if (obj != undefined) {
            super(obj as AquacultureDeregistrationBaseRegixDataDTO);
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

    @StrictlyTyped(ApplicationSubmittedByDTO)
    public submittedBy?: ApplicationSubmittedByDTO;

    @StrictlyTyped(ApplicationSubmittedForDTO)
    public submittedFor?: ApplicationSubmittedForDTO;

    @StrictlyTyped(Number)
    public aquacultureFacilityId?: number;

    @StrictlyTyped(String)
    public deregistrationReason?: string;

    @StrictlyTyped(ApplicationPaymentInformationDTO)
    public paymentInformation?: ApplicationPaymentInformationDTO;

    @StrictlyTyped(ApplicationBaseDeliveryDTO)
    public deliveryData?: ApplicationBaseDeliveryDTO;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];

    @StrictlyTyped(AquacultureDeregistrationRegixDataDTO)
    public regiXDataModel?: AquacultureDeregistrationRegixDataDTO;
}
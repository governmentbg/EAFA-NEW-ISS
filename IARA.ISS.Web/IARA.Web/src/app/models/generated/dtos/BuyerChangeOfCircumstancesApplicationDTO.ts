

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BuyerChangeOfCircumstancesBaseRegixDataDTO } from './BuyerChangeOfCircumstancesBaseRegixDataDTO';
import { ApplicationSubmittedByDTO } from './ApplicationSubmittedByDTO';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { ChangeOfCircumstancesDTO } from './ChangeOfCircumstancesDTO';
import { ApplicationPaymentInformationDTO } from './ApplicationPaymentInformationDTO';
import { ApplicationBaseDeliveryDTO } from './ApplicationBaseDeliveryDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { BuyerChangeOfCircumstancesRegixDataDTO } from './BuyerChangeOfCircumstancesRegixDataDTO'; 

export class BuyerChangeOfCircumstancesApplicationDTO extends BuyerChangeOfCircumstancesBaseRegixDataDTO {
    public constructor(obj?: Partial<BuyerChangeOfCircumstancesApplicationDTO>) {
        if (obj != undefined) {
            super(obj as BuyerChangeOfCircumstancesBaseRegixDataDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Boolean)
    public isDraft?: boolean;

    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(ApplicationSubmittedByDTO)
    public submittedBy?: ApplicationSubmittedByDTO;

    @StrictlyTyped(ApplicationSubmittedForDTO)
    public submittedFor?: ApplicationSubmittedForDTO;

    @StrictlyTyped(ChangeOfCircumstancesDTO)
    public changes?: ChangeOfCircumstancesDTO[];

    @StrictlyTyped(Boolean)
    public isPaid?: boolean;

    @StrictlyTyped(ApplicationPaymentInformationDTO)
    public paymentInformation?: ApplicationPaymentInformationDTO;

    @StrictlyTyped(Boolean)
    public hasDelivery?: boolean;

    @StrictlyTyped(ApplicationBaseDeliveryDTO)
    public deliveryData?: ApplicationBaseDeliveryDTO;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];

    @StrictlyTyped(BuyerChangeOfCircumstancesRegixDataDTO)
    public regiXDataModel?: BuyerChangeOfCircumstancesRegixDataDTO;
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ShipChangeOfCircumstancesBaseRegixDataDTO } from './ShipChangeOfCircumstancesBaseRegixDataDTO';
import { ApplicationSubmittedByDTO } from './ApplicationSubmittedByDTO';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { ChangeOfCircumstancesDTO } from './ChangeOfCircumstancesDTO';
import { ApplicationPaymentInformationDTO } from './ApplicationPaymentInformationDTO';
import { ApplicationBaseDeliveryDTO } from './ApplicationBaseDeliveryDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { ShipChangeOfCircumstancesRegixDataDTO } from './ShipChangeOfCircumstancesRegixDataDTO'; 

export class ShipChangeOfCircumstancesApplicationDTO extends ShipChangeOfCircumstancesBaseRegixDataDTO {
    public constructor(obj?: Partial<ShipChangeOfCircumstancesApplicationDTO>) {
        if (obj != undefined) {
            super(obj as ShipChangeOfCircumstancesBaseRegixDataDTO);
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

    @StrictlyTyped(ChangeOfCircumstancesDTO)
    public changes?: ChangeOfCircumstancesDTO[];

    @StrictlyTyped(ApplicationPaymentInformationDTO)
    public paymentInformation?: ApplicationPaymentInformationDTO;

    @StrictlyTyped(ApplicationBaseDeliveryDTO)
    public deliveryData?: ApplicationBaseDeliveryDTO;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];

    @StrictlyTyped(ShipChangeOfCircumstancesRegixDataDTO)
    public regiXDataModel?: ShipChangeOfCircumstancesRegixDataDTO;
}
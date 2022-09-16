

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { DuplicatesApplicationBaseRegixDataDTO } from './DuplicatesApplicationBaseRegixDataDTO';
import { ApplicationSubmittedByDTO } from './ApplicationSubmittedByDTO';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { BuyerDuplicateDataDTO } from './BuyerDuplicateDataDTO';
import { PermitDuplicateDataDTO } from './PermitDuplicateDataDTO';
import { PermitLicenseDuplicateDataDTO } from './PermitLicenseDuplicateDataDTO';
import { QualifiedFisherDuplicateDataDTO } from './QualifiedFisherDuplicateDataDTO';
import { ApplicationPaymentInformationDTO } from './ApplicationPaymentInformationDTO';
import { ApplicationBaseDeliveryDTO } from './ApplicationBaseDeliveryDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum'; 

export class DuplicatesApplicationDTO extends DuplicatesApplicationBaseRegixDataDTO {
    public constructor(obj?: Partial<DuplicatesApplicationDTO>) {
        if (obj != undefined) {
            super(obj as DuplicatesApplicationBaseRegixDataDTO);
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

    @StrictlyTyped(ApplicationSubmittedByDTO)
    public submittedBy?: ApplicationSubmittedByDTO;

    @StrictlyTyped(ApplicationSubmittedForDTO)
    public submittedFor?: ApplicationSubmittedForDTO;

    @StrictlyTyped(String)
    public reason?: string;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(BuyerDuplicateDataDTO)
    public buyer?: BuyerDuplicateDataDTO;

    @StrictlyTyped(PermitDuplicateDataDTO)
    public permit?: PermitDuplicateDataDTO;

    @StrictlyTyped(PermitLicenseDuplicateDataDTO)
    public permitLicence?: PermitLicenseDuplicateDataDTO;

    @StrictlyTyped(QualifiedFisherDuplicateDataDTO)
    public qualifiedFisher?: QualifiedFisherDuplicateDataDTO;

    @StrictlyTyped(ApplicationPaymentInformationDTO)
    public paymentInformation?: ApplicationPaymentInformationDTO;

    @StrictlyTyped(ApplicationBaseDeliveryDTO)
    public deliveryData?: ApplicationBaseDeliveryDTO;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}
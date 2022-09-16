

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ApplicationSubmittedByDTO } from './ApplicationSubmittedByDTO';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { EgnLncDTO } from './EgnLncDTO';
import { HolderGroundForUseDTO } from './HolderGroundForUseDTO';
import { QuotaAquaticOrganismDTO } from './QuotaAquaticOrganismDTO';
import { FishingGearDTO } from './FishingGearDTO';
import { ApplicationPaymentInformationDTO } from './ApplicationPaymentInformationDTO';
import { ApplicationBaseDeliveryDTO } from './ApplicationBaseDeliveryDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class CommercialFishingApplicationEditDTO { 
    public constructor(obj?: Partial<CommercialFishingApplicationEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Number)
    public permitLicensePermitId?: number;

    @StrictlyTyped(String)
    public permitLicensePermitNumber?: string;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(Boolean)
    public isPaid?: boolean;

    @StrictlyTyped(Boolean)
    public hasDelivery?: boolean;

    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(ApplicationSubmittedByDTO)
    public submittedBy?: ApplicationSubmittedByDTO;

    @StrictlyTyped(ApplicationSubmittedForDTO)
    public submittedFor?: ApplicationSubmittedForDTO;

    @StrictlyTyped(Number)
    public qualifiedFisherId?: number;

    @StrictlyTyped(Boolean)
    public qualifiedFisherSameAsSubmittedFor?: boolean;

    @StrictlyTyped(EgnLncDTO)
    public qualifiedFisherIdentifier?: EgnLncDTO;

    @StrictlyTyped(String)
    public qualifiedFisherFirstName?: string;

    @StrictlyTyped(String)
    public qualifiedFisherMiddleName?: string;

    @StrictlyTyped(String)
    public qualifiedFisherLastName?: string;

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(Number)
    public poundNetId?: number;

    @StrictlyTyped(Boolean)
    public isHolderShipOwner?: boolean;

    @StrictlyTyped(HolderGroundForUseDTO)
    public shipGroundForUse?: HolderGroundForUseDTO;

    @StrictlyTyped(HolderGroundForUseDTO)
    public poundNetGroundForUse?: HolderGroundForUseDTO;

    @StrictlyTyped(String)
    public unloaderPhoneNumber?: string;

    @StrictlyTyped(Number)
    public waterTypeId?: number;

    @StrictlyTyped(Number)
    public aquaticOrganismTypeIds?: number[];

    @StrictlyTyped(QuotaAquaticOrganismDTO)
    public quotaAquaticOrganisms?: QuotaAquaticOrganismDTO[];

    @StrictlyTyped(FishingGearDTO)
    public fishingGears?: FishingGearDTO[];

    @StrictlyTyped(ApplicationPaymentInformationDTO)
    public paymentInformation?: ApplicationPaymentInformationDTO;

    @StrictlyTyped(ApplicationBaseDeliveryDTO)
    public deliveryData?: ApplicationBaseDeliveryDTO;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];

    @StrictlyTyped(String)
    public statusReason?: string;
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ApplicationSubmittedByDTO } from './ApplicationSubmittedByDTO';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { UsageDocumentDTO } from './UsageDocumentDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { ApplicationPaymentInformationDTO } from './ApplicationPaymentInformationDTO';
import { ApplicationBaseDeliveryDTO } from './ApplicationBaseDeliveryDTO';
import { CommonDocumentDTO } from './CommonDocumentDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class BuyerApplicationEditDTO { 
    public constructor(obj?: Partial<BuyerApplicationEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(ApplicationSubmittedByDTO)
    public submittedBy?: ApplicationSubmittedByDTO;

    @StrictlyTyped(Number)
    public submittedForLegalId?: number;

    @StrictlyTyped(Number)
    public submittedForPersonId?: number;

    @StrictlyTyped(ApplicationSubmittedForDTO)
    public submittedFor?: ApplicationSubmittedForDTO;

    @StrictlyTyped(Number)
    public organizerPersonId?: number;

    @StrictlyTyped(RegixPersonDataDTO)
    public organizer?: RegixPersonDataDTO;

    @StrictlyTyped(Boolean)
    public organizerSameAsSubmittedBy?: boolean;

    @StrictlyTyped(Number)
    public agentId?: number;

    @StrictlyTyped(RegixPersonDataDTO)
    public agent?: RegixPersonDataDTO;

    @StrictlyTyped(Boolean)
    public isAgentSameAsSubmittedBy?: boolean;

    @StrictlyTyped(Boolean)
    public isAgentSameAsSubmittedForCustodianOfProperty?: boolean;

    @StrictlyTyped(UsageDocumentDTO)
    public premiseUsageDocument?: UsageDocumentDTO;

    @StrictlyTyped(Number)
    public premiseAddressId?: number;

    @StrictlyTyped(AddressRegistrationDTO)
    public premiseAddress?: AddressRegistrationDTO;

    @StrictlyTyped(Boolean)
    public hasUtility?: boolean;

    @StrictlyTyped(String)
    public utilityName?: string;

    @StrictlyTyped(Boolean)
    public hasVehicle?: boolean;

    @StrictlyTyped(String)
    public vehicleNumber?: string;

    @StrictlyTyped(ApplicationPaymentInformationDTO)
    public paymentInformation?: ApplicationPaymentInformationDTO;

    @StrictlyTyped(ApplicationBaseDeliveryDTO)
    public deliveryData?: ApplicationBaseDeliveryDTO;

    @StrictlyTyped(CommonDocumentDTO)
    public babhLawLicenseDocumnet?: CommonDocumentDTO;

    @StrictlyTyped(CommonDocumentDTO)
    public veteniraryVehicleRegLicenseDocument?: CommonDocumentDTO;

    @StrictlyTyped(String)
    public comments?: string;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];

    @StrictlyTyped(String)
    public statusReason?: string;

    @StrictlyTyped(Boolean)
    public hasDelivery?: boolean;

    @StrictlyTyped(Boolean)
    public isPaid?: boolean;

    @StrictlyTyped(Number)
    public annualTurnover?: number;
}
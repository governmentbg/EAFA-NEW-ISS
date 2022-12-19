

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { UsageDocumentDTO } from './UsageDocumentDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { CommonDocumentDTO } from './CommonDocumentDTO';
import { CancellationHistoryEntryDTO } from './CancellationHistoryEntryDTO';
import { DuplicatesEntryDTO } from './DuplicatesEntryDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { BuyerTypesEnum } from '@app/enums/buyer-types.enum';
import { BuyerStatusesEnum } from '@app/enums/buyer-statuses.enum';

export class BuyerEditDTO { 
    public constructor(obj?: Partial<BuyerEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(Number)
    public territoryUnitId?: number;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(Number)
    public buyerType?: BuyerTypesEnum;

    @StrictlyTyped(Number)
    public buyerStatus?: BuyerStatusesEnum;

    @StrictlyTyped(Boolean)
    public hasUtility?: boolean;

    @StrictlyTyped(String)
    public utilityName?: string;

    @StrictlyTyped(Boolean)
    public hasVehicle?: boolean;

    @StrictlyTyped(String)
    public vehicleNumber?: string;

    @StrictlyTyped(String)
    public urorrNumber?: string;

    @StrictlyTyped(String)
    public registrationNumber?: string;

    @StrictlyTyped(Date)
    public registrationDate?: Date;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(String)
    public comments?: string;

    @StrictlyTyped(UsageDocumentDTO)
    public premiseUsageDocuments?: UsageDocumentDTO[];

    @StrictlyTyped(Number)
    public premiseAddressId?: number;

    @StrictlyTyped(AddressRegistrationDTO)
    public premiseAddress?: AddressRegistrationDTO;

    @StrictlyTyped(Number)
    public submittedForLegalId?: number;

    @StrictlyTyped(Number)
    public submittedForPersonId?: number;

    @StrictlyTyped(ApplicationSubmittedForDTO)
    public submittedFor?: ApplicationSubmittedForDTO;

    @StrictlyTyped(RegixPersonDataDTO)
    public organizer?: RegixPersonDataDTO;

    @StrictlyTyped(Number)
    public organizerPersonId?: number;

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

    @StrictlyTyped(Number)
    public annualTurnover?: number;

    @StrictlyTyped(CommonDocumentDTO)
    public babhLawLicenseDocuments?: CommonDocumentDTO[];

    @StrictlyTyped(CommonDocumentDTO)
    public veteniraryVehicleRegLicenseDocuments?: CommonDocumentDTO[];

    @StrictlyTyped(CancellationHistoryEntryDTO)
    public cancellationHistory?: CancellationHistoryEntryDTO[];

    @StrictlyTyped(DuplicatesEntryDTO)
    public duplicateEntries?: DuplicatesEntryDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}
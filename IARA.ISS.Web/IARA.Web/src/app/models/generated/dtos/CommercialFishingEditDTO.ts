

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { SuspensionDataDTO } from './SuspensionDataDTO';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { HolderGroundForUseDTO } from './HolderGroundForUseDTO';
import { EgnLncDTO } from './EgnLncDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { QuotaAquaticOrganismDTO } from './QuotaAquaticOrganismDTO';
import { FishingGearDTO } from './FishingGearDTO';
import { CommercialFishingLogBookEditDTO } from './CommercialFishingLogBookEditDTO';
import { DuplicatesEntryDTO } from './DuplicatesEntryDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { CommercialFishingTypesEnum } from '@app/enums/commercial-fishing-types.enum';

export class CommercialFishingEditDTO { 
    public constructor(obj?: Partial<CommercialFishingEditDTO>) {
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

    @StrictlyTyped(Number)
    public type?: CommercialFishingTypesEnum;

    @StrictlyTyped(Number)
    public permitLicensePermitId?: number;

    @StrictlyTyped(String)
    public permitRegistrationNumber?: string;

    @StrictlyTyped(String)
    public permitLicenseRegistrationNumber?: string;

    @StrictlyTyped(Date)
    public issueDate?: Date;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public validTo?: Date;

    @StrictlyTyped(SuspensionDataDTO)
    public suspensions?: SuspensionDataDTO[];

    @StrictlyTyped(Boolean)
    public isPermitUnlimited?: boolean;

    @StrictlyTyped(ApplicationSubmittedForDTO)
    public submittedFor?: ApplicationSubmittedForDTO;

    @StrictlyTyped(Boolean)
    public isHolderShipOwner?: boolean;

    @StrictlyTyped(HolderGroundForUseDTO)
    public shipGroundForUse?: HolderGroundForUseDTO;

    @StrictlyTyped(HolderGroundForUseDTO)
    public poundNetGroundForUse?: HolderGroundForUseDTO;

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(Number)
    public qualifiedFisherId?: number;

    @StrictlyTyped(EgnLncDTO)
    public qualifiedFisherIdentifier?: EgnLncDTO;

    @StrictlyTyped(String)
    public qualifiedFisherFirstName?: string;

    @StrictlyTyped(String)
    public qualifiedFisherMiddleName?: string;

    @StrictlyTyped(String)
    public qualifiedFisherLastName?: string;

    @StrictlyTyped(Boolean)
    public qualifiedFisherSameAsSubmittedFor?: boolean;

    @StrictlyTyped(FileInfoDTO)
    public qualifiedFisherPhoto?: FileInfoDTO;

    @StrictlyTyped(Number)
    public waterTypeId?: number;

    @StrictlyTyped(Number)
    public aquaticOrganismTypeIds?: number[];

    @StrictlyTyped(QuotaAquaticOrganismDTO)
    public quotaAquaticOrganisms?: QuotaAquaticOrganismDTO[];

    @StrictlyTyped(Number)
    public poundNetId?: number;

    @StrictlyTyped(String)
    public unloaderPhoneNumber?: string;

    @StrictlyTyped(FishingGearDTO)
    public fishingGears?: FishingGearDTO[];

    @StrictlyTyped(CommercialFishingLogBookEditDTO)
    public logBooks?: CommercialFishingLogBookEditDTO[];

    @StrictlyTyped(DuplicatesEntryDTO)
    public duplicateEntries?: DuplicatesEntryDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}
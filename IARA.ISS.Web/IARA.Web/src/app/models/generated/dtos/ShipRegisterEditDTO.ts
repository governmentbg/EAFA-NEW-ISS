

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AcquiredFishingCapacityDTO } from './AcquiredFishingCapacityDTO';
import { FishingCapacityFreedActionsDTO } from './FishingCapacityFreedActionsDTO';
import { CancellationDetailsDTO } from './CancellationDetailsDTO';
import { ShipOwnerDTO } from './ShipOwnerDTO';
import { ShipRegisterUserDTO } from './ShipRegisterUserDTO';
import { ShipRegisterUsedCertificateDTO } from './ShipRegisterUsedCertificateDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { ShipEventTypeEnum } from '@app/enums/ship-event-type.enum';
import { ShipExportTypeEnum } from '@app/enums/ship-export-type.enum';

export class ShipRegisterEditDTO { 
    public constructor(obj?: Partial<ShipRegisterEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public shipUID?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Boolean)
    public isThirdPartyShip?: boolean;

    @StrictlyTyped(Number)
    public eventType?: ShipEventTypeEnum;

    @StrictlyTyped(Date)
    public eventDate?: Date;

    @StrictlyTyped(String)
    public cfr?: string;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public externalMark?: string;

    @StrictlyTyped(String)
    public registrationNumber?: string;

    @StrictlyTyped(Date)
    public registrationDate?: Date;

    @StrictlyTyped(Number)
    public fleetTypeId?: number;

    @StrictlyTyped(Number)
    public fleetSegmentId?: number;

    @StrictlyTyped(Number)
    public countryFlagId?: number;

    @StrictlyTyped(String)
    public ircsCallSign?: string;

    @StrictlyTyped(String)
    public mmsi?: string;

    @StrictlyTyped(String)
    public uvi?: string;

    @StrictlyTyped(Boolean)
    public hasAIS?: boolean;

    @StrictlyTyped(Boolean)
    public hasERS?: boolean;

    @StrictlyTyped(Boolean)
    public hasERSException?: boolean;

    @StrictlyTyped(Boolean)
    public hasVMS?: boolean;

    @StrictlyTyped(Number)
    public vesselTypeId?: number;

    @StrictlyTyped(String)
    public regLicenceNum?: string;

    @StrictlyTyped(Date)
    public regLicenceDate?: Date;

    @StrictlyTyped(String)
    public regLicencePublisher?: string;

    @StrictlyTyped(String)
    public regLicencePublishVolume?: string;

    @StrictlyTyped(String)
    public regLicencePublishPage?: string;

    @StrictlyTyped(String)
    public regLicencePublishNum?: string;

    @StrictlyTyped(Date)
    public exploitationStartDate?: Date;

    @StrictlyTyped(Number)
    public buildYear?: number;

    @StrictlyTyped(String)
    public buildPlace?: string;

    @StrictlyTyped(String)
    public adminDecisionNum?: string;

    @StrictlyTyped(Date)
    public adminDecisionDate?: Date;

    @StrictlyTyped(Number)
    public publicAidTypeId?: number;

    @StrictlyTyped(Number)
    public portId?: number;

    @StrictlyTyped(Number)
    public stayPortId?: number;

    @StrictlyTyped(Number)
    public sailAreaId?: number;

    @StrictlyTyped(String)
    public sailAreaName?: string;

    @StrictlyTyped(Number)
    public totalLength?: number;

    @StrictlyTyped(Number)
    public totalWidth?: number;

    @StrictlyTyped(Number)
    public grossTonnage?: number;

    @StrictlyTyped(Number)
    public netTonnage?: number;

    @StrictlyTyped(Number)
    public otherTonnage?: number;

    @StrictlyTyped(Number)
    public boardHeight?: number;

    @StrictlyTyped(Number)
    public shipDraught?: number;

    @StrictlyTyped(Number)
    public lengthBetweenPerpendiculars?: number;

    @StrictlyTyped(Number)
    public mainEnginePower?: number;

    @StrictlyTyped(Number)
    public auxiliaryEnginePower?: number;

    @StrictlyTyped(String)
    public mainEngineNum?: string;

    @StrictlyTyped(String)
    public mainEngineModel?: string;

    @StrictlyTyped(Number)
    public mainFishingGearId?: number;

    @StrictlyTyped(Number)
    public additionalFishingGearId?: number;

    @StrictlyTyped(Number)
    public hullMaterialId?: number;

    @StrictlyTyped(Number)
    public fuelTypeId?: number;

    @StrictlyTyped(Number)
    public totalPassengerCapacity?: number;

    @StrictlyTyped(Number)
    public crewCount?: number;

    @StrictlyTyped(Boolean)
    public hasControlCard?: boolean;

    @StrictlyTyped(String)
    public controlCardNum?: string;

    @StrictlyTyped(Date)
    public controlCardDate?: Date;

    @StrictlyTyped(Boolean)
    public hasValidityCertificate?: boolean;

    @StrictlyTyped(String)
    public controlCardValidityCertificateNum?: string;

    @StrictlyTyped(Date)
    public controlCardValidityCertificateDate?: Date;

    @StrictlyTyped(Date)
    public controlCardDateOfLastAttestation?: Date;

    @StrictlyTyped(Boolean)
    public hasFoodLawLicence?: boolean;

    @StrictlyTyped(String)
    public foodLawLicenceNum?: string;

    @StrictlyTyped(Date)
    public foodLawLicenceDate?: Date;

    @StrictlyTyped(Number)
    public shipAssociationId?: number;

    @StrictlyTyped(Number)
    public importCountryId?: number;

    @StrictlyTyped(Number)
    public exportCountryId?: number;

    @StrictlyTyped(Number)
    public exportType?: ShipExportTypeEnum;

    @StrictlyTyped(AcquiredFishingCapacityDTO)
    public acquiredFishingCapacity?: AcquiredFishingCapacityDTO;

    @StrictlyTyped(FishingCapacityFreedActionsDTO)
    public remainingCapacityAction?: FishingCapacityFreedActionsDTO;

    @StrictlyTyped(Boolean)
    public isNoApplicationDeregistration?: boolean;

    @StrictlyTyped(String)
    public comments?: string;

    @StrictlyTyped(CancellationDetailsDTO)
    public cancellationDetails?: CancellationDetailsDTO;

    @StrictlyTyped(Boolean)
    public hasFishingPermit?: boolean;

    @StrictlyTyped(Boolean)
    public isForbiddenForRSR?: boolean;

    @StrictlyTyped(Date)
    public forbiddenStartDate?: Date;

    @StrictlyTyped(Date)
    public forbiddenEndDate?: Date;

    @StrictlyTyped(String)
    public forbiddenReason?: string;

    @StrictlyTyped(ShipOwnerDTO)
    public owners?: ShipOwnerDTO[];

    @StrictlyTyped(ShipRegisterUserDTO)
    public shipUsers?: ShipRegisterUserDTO[];

    @StrictlyTyped(ShipRegisterUsedCertificateDTO)
    public usedCapacityCertificates?: ShipRegisterUsedCertificateDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}
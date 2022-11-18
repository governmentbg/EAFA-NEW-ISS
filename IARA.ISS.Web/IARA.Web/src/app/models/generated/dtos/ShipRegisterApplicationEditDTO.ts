

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ShipRegisterBaseRegixDataDTO } from './ShipRegisterBaseRegixDataDTO';
import { ApplicationSubmittedByDTO } from './ApplicationSubmittedByDTO';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { AcquiredFishingCapacityDTO } from './AcquiredFishingCapacityDTO';
import { FishingCapacityFreedActionsDTO } from './FishingCapacityFreedActionsDTO';
import { ShipOwnerDTO } from './ShipOwnerDTO';
import { ApplicationPaymentInformationDTO } from './ApplicationPaymentInformationDTO';
import { ApplicationBaseDeliveryDTO } from './ApplicationBaseDeliveryDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { ShipRegisterRegixDataDTO } from './ShipRegisterRegixDataDTO'; 

export class ShipRegisterApplicationEditDTO extends ShipRegisterBaseRegixDataDTO {
    public constructor(obj?: Partial<ShipRegisterApplicationEditDTO>) {
        if (obj != undefined) {
            super(obj as ShipRegisterBaseRegixDataDTO);
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
    public externalMark?: string;

    @StrictlyTyped(String)
    public registrationNumber?: string;

    @StrictlyTyped(Date)
    public registrationDate?: Date;

    @StrictlyTyped(Number)
    public fleetTypeId?: number;

    @StrictlyTyped(Number)
    public fleetSegmentId?: number;

    @StrictlyTyped(String)
    public ircsCallSign?: string;

    @StrictlyTyped(String)
    public mmsi?: string;

    @StrictlyTyped(String)
    public uvi?: string;

    @StrictlyTyped(Number)
    public countryFlagId?: number;

    @StrictlyTyped(Boolean)
    public hasAIS?: boolean;

    @StrictlyTyped(Boolean)
    public hasERS?: boolean;

    @StrictlyTyped(Boolean)
    public hasVMS?: boolean;

    @StrictlyTyped(Date)
    public regLicenceDate?: Date;

    @StrictlyTyped(String)
    public regLicencePublisher?: string;

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
    public otherTonnage?: number;

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

    @StrictlyTyped(AcquiredFishingCapacityDTO)
    public acquiredFishingCapacity?: AcquiredFishingCapacityDTO;

    @StrictlyTyped(FishingCapacityFreedActionsDTO)
    public remainingCapacityAction?: FishingCapacityFreedActionsDTO;

    @StrictlyTyped(String)
    public comments?: string;

    @StrictlyTyped(ShipOwnerDTO)
    public owners?: ShipOwnerDTO[];

    @StrictlyTyped(ApplicationPaymentInformationDTO)
    public paymentInformation?: ApplicationPaymentInformationDTO;

    @StrictlyTyped(ApplicationBaseDeliveryDTO)
    public deliveryData?: ApplicationBaseDeliveryDTO;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];

    @StrictlyTyped(ShipRegisterRegixDataDTO)
    public regiXDataModel?: ShipRegisterRegixDataDTO;
}
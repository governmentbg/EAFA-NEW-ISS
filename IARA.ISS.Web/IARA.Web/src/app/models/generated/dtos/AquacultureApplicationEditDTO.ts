

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AquacultureBaseRegixDataDTO } from './AquacultureBaseRegixDataDTO';
import { ApplicationSubmittedByDTO } from './ApplicationSubmittedByDTO';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { AquacultureCoordinateDTO } from './AquacultureCoordinateDTO';
import { AquacultureInstallationEditDTO } from './AquacultureInstallationEditDTO';
import { AquacultureHatcheryEquipmentDTO } from './AquacultureHatcheryEquipmentDTO';
import { UsageDocumentDTO } from './UsageDocumentDTO';
import { AquacultureWaterLawCertificateDTO } from './AquacultureWaterLawCertificateDTO';
import { CommonDocumentDTO } from './CommonDocumentDTO';
import { ApplicationPaymentInformationDTO } from './ApplicationPaymentInformationDTO';
import { ApplicationBaseDeliveryDTO } from './ApplicationBaseDeliveryDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { AquacultureSalinityEnum } from '@app/enums/aquaculture-salinity.enum';
import { AquacultureTemperatureEnum } from '@app/enums/aquaculture-temperature.enum';
import { AquacultureSystemEnum } from '@app/enums/aquaculture-system.enum'; 

export class AquacultureApplicationEditDTO extends AquacultureBaseRegixDataDTO {
    public constructor(obj?: Partial<AquacultureApplicationEditDTO>) {
        if (obj != undefined) {
            super(obj as AquacultureBaseRegixDataDTO);
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

    @StrictlyTyped(ApplicationSubmittedByDTO)
    public submittedBy?: ApplicationSubmittedByDTO;

    @StrictlyTyped(ApplicationSubmittedForDTO)
    public submittedFor?: ApplicationSubmittedForDTO;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(Number)
    public territoryUnitId?: number;

    @StrictlyTyped(Number)
    public waterAreaTypeId?: number;

    @StrictlyTyped(Number)
    public populatedAreaId?: number;

    @StrictlyTyped(String)
    public locationDescription?: string;

    @StrictlyTyped(AquacultureCoordinateDTO)
    public coordinates?: AquacultureCoordinateDTO[];

    @StrictlyTyped(Number)
    public aquaticOrganismIds?: number[];

    @StrictlyTyped(Number)
    public waterSalinity?: AquacultureSalinityEnum;

    @StrictlyTyped(Number)
    public waterTemperature?: AquacultureTemperatureEnum;

    @StrictlyTyped(Number)
    public system?: AquacultureSystemEnum;

    @StrictlyTyped(Number)
    public powerSupplyTypeId?: number;

    @StrictlyTyped(String)
    public powerSupplyName?: string;

    @StrictlyTyped(Number)
    public powerSupplyDebit?: number;

    @StrictlyTyped(Number)
    public totalWaterArea?: number;

    @StrictlyTyped(Number)
    public totalProductionCapacity?: number;

    @StrictlyTyped(AquacultureInstallationEditDTO)
    public installations?: AquacultureInstallationEditDTO[];

    @StrictlyTyped(Number)
    public hatcheryCapacity?: number;

    @StrictlyTyped(AquacultureHatcheryEquipmentDTO)
    public hatcheryEquipment?: AquacultureHatcheryEquipmentDTO[];

    @StrictlyTyped(UsageDocumentDTO)
    public usageDocument?: UsageDocumentDTO;

    @StrictlyTyped(AquacultureWaterLawCertificateDTO)
    public waterLawCertificate?: AquacultureWaterLawCertificateDTO;

    @StrictlyTyped(CommonDocumentDTO)
    public ovosCertificate?: CommonDocumentDTO;

    @StrictlyTyped(Boolean)
    public hasBabhCertificate?: boolean;

    @StrictlyTyped(CommonDocumentDTO)
    public babhCertificate?: CommonDocumentDTO;

    @StrictlyTyped(String)
    public comments?: string;

    @StrictlyTyped(ApplicationPaymentInformationDTO)
    public paymentInformation?: ApplicationPaymentInformationDTO;

    @StrictlyTyped(ApplicationBaseDeliveryDTO)
    public deliveryData?: ApplicationBaseDeliveryDTO;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}
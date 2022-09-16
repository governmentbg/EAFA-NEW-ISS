

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { AquacultureCoordinateDTO } from './AquacultureCoordinateDTO';
import { AquacultureInstallationEditDTO } from './AquacultureInstallationEditDTO';
import { AquacultureHatcheryEquipmentDTO } from './AquacultureHatcheryEquipmentDTO';
import { UsageDocumentDTO } from './UsageDocumentDTO';
import { AquacultureWaterLawCertificateDTO } from './AquacultureWaterLawCertificateDTO';
import { CommonDocumentDTO } from './CommonDocumentDTO';
import { CancellationHistoryEntryDTO } from './CancellationHistoryEntryDTO';
import { LogBookEditDTO } from './LogBookEditDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { AquacultureStatusEnum } from '@app/enums/aquaculture-status.enum';
import { AquacultureSalinityEnum } from '@app/enums/aquaculture-salinity.enum';
import { AquacultureTemperatureEnum } from '@app/enums/aquaculture-temperature.enum';
import { AquacultureSystemEnum } from '@app/enums/aquaculture-system.enum';

export class AquacultureFacilityEditDTO { 
    public constructor(obj?: Partial<AquacultureFacilityEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(Number)
    public regNum?: number;

    @StrictlyTyped(String)
    public urorNum?: string;

    @StrictlyTyped(Number)
    public status?: AquacultureStatusEnum;

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
    public usageDocuments?: UsageDocumentDTO[];

    @StrictlyTyped(AquacultureWaterLawCertificateDTO)
    public waterLawCertificates?: AquacultureWaterLawCertificateDTO[];

    @StrictlyTyped(CommonDocumentDTO)
    public ovosCertificates?: CommonDocumentDTO[];

    @StrictlyTyped(CommonDocumentDTO)
    public babhCertificates?: CommonDocumentDTO[];

    @StrictlyTyped(String)
    public comments?: string;

    @StrictlyTyped(CancellationHistoryEntryDTO)
    public cancellationHistory?: CancellationHistoryEntryDTO[];

    @StrictlyTyped(LogBookEditDTO)
    public logBooks?: LogBookEditDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BaseRegixChecksDTO } from './BaseRegixChecksDTO';
import { ApplicationSubmittedByDTO } from './ApplicationSubmittedByDTO';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { StatisticalFormGivenMedicineDTO } from './StatisticalFormGivenMedicineDTO';
import { StatisticalFormAquaFarmFishOrganismDTO } from './StatisticalFormAquaFarmFishOrganismDTO';
import { StatisticalFormAquaFarmBroodstockDTO } from './StatisticalFormAquaFarmBroodstockDTO';
import { StatisticalFormAquaFarmInstallationSystemFullDTO } from './StatisticalFormAquaFarmInstallationSystemFullDTO';
import { StatisticalFormAquaFarmInstallationSystemNotFullDTO } from './StatisticalFormAquaFarmInstallationSystemNotFullDTO';
import { StatisticalFormNumStatGroupDTO } from './StatisticalFormNumStatGroupDTO';
import { StatisticalFormEmployeeInfoGroupDTO } from './StatisticalFormEmployeeInfoGroupDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { StatisticalFormAquaFarmRegixDataDTO } from './StatisticalFormAquaFarmRegixDataDTO'; 

export class StatisticalFormAquaFarmApplicationEditDTO extends BaseRegixChecksDTO {
    public constructor(obj?: Partial<StatisticalFormAquaFarmApplicationEditDTO>) {
        if (obj != undefined) {
            super(obj as BaseRegixChecksDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(ApplicationSubmittedByDTO)
    public submittedBy?: ApplicationSubmittedByDTO;

    @StrictlyTyped(String)
    public submittedByWorkPosition?: string;

    @StrictlyTyped(ApplicationSubmittedForDTO)
    public submittedFor?: ApplicationSubmittedForDTO;

    @StrictlyTyped(Number)
    public aquacultureFacilityId?: number;

    @StrictlyTyped(Number)
    public year?: number;

    @StrictlyTyped(Number)
    public breedingMaterialDeathRate?: number;

    @StrictlyTyped(Number)
    public consumationFishDeathRate?: number;

    @StrictlyTyped(Number)
    public broodstockDeathRate?: number;

    @StrictlyTyped(Boolean)
    public isOwnerEmployee?: boolean;

    @StrictlyTyped(StatisticalFormGivenMedicineDTO)
    public medicine?: StatisticalFormGivenMedicineDTO[];

    @StrictlyTyped(StatisticalFormAquaFarmFishOrganismDTO)
    public producedFishOrganism?: StatisticalFormAquaFarmFishOrganismDTO[];

    @StrictlyTyped(StatisticalFormAquaFarmFishOrganismDTO)
    public soldFishOrganism?: StatisticalFormAquaFarmFishOrganismDTO[];

    @StrictlyTyped(StatisticalFormAquaFarmFishOrganismDTO)
    public unrealizedFishOrganism?: StatisticalFormAquaFarmFishOrganismDTO[];

    @StrictlyTyped(StatisticalFormAquaFarmBroodstockDTO)
    public broodstock?: StatisticalFormAquaFarmBroodstockDTO[];

    @StrictlyTyped(StatisticalFormAquaFarmInstallationSystemFullDTO)
    public installationSystemFull?: StatisticalFormAquaFarmInstallationSystemFullDTO[];

    @StrictlyTyped(StatisticalFormAquaFarmInstallationSystemNotFullDTO)
    public installationSystemNotFull?: StatisticalFormAquaFarmInstallationSystemNotFullDTO[];

    @StrictlyTyped(String)
    public medicineComments?: string;

    @StrictlyTyped(StatisticalFormNumStatGroupDTO)
    public numStatGroups?: StatisticalFormNumStatGroupDTO[];

    @StrictlyTyped(StatisticalFormEmployeeInfoGroupDTO)
    public employeeInfoGroups?: StatisticalFormEmployeeInfoGroupDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];

    @StrictlyTyped(StatisticalFormAquaFarmRegixDataDTO)
    public regiXDataModel?: StatisticalFormAquaFarmRegixDataDTO;
}
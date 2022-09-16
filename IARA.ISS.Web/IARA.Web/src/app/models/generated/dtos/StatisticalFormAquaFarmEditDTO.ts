

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { StatisticalFormGivenMedicineDTO } from './StatisticalFormGivenMedicineDTO';
import { StatisticalFormAquaFarmFishOrganismDTO } from './StatisticalFormAquaFarmFishOrganismDTO';
import { StatisticalFormAquaFarmBroodstockDTO } from './StatisticalFormAquaFarmBroodstockDTO';
import { StatisticalFormAquaFarmInstallationSystemFullDTO } from './StatisticalFormAquaFarmInstallationSystemFullDTO';
import { StatisticalFormAquaFarmInstallationSystemNotFullDTO } from './StatisticalFormAquaFarmInstallationSystemNotFullDTO';
import { StatisticalFormNumStatGroupDTO } from './StatisticalFormNumStatGroupDTO';
import { StatisticalFormEmployeeInfoGroupDTO } from './StatisticalFormEmployeeInfoGroupDTO';
import { FileInfoDTO } from './FileInfoDTO';

export class StatisticalFormAquaFarmEditDTO { 
    public constructor(obj?: Partial<StatisticalFormAquaFarmEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(ApplicationSubmittedForDTO)
    public submittedFor?: ApplicationSubmittedForDTO;

    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(Number)
    public aquacultureFacilityId?: number;

    @StrictlyTyped(Number)
    public year?: number;

    @StrictlyTyped(String)
    public formNum?: string;

    @StrictlyTyped(Number)
    public breedingMaterialDeathRate?: number;

    @StrictlyTyped(Number)
    public consumationFishDeathRate?: number;

    @StrictlyTyped(Number)
    public broodstockDeathRate?: number;

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
}
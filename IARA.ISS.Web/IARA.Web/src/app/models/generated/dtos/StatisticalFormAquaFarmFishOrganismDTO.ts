

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AquaFarmFishOrganismReportTypeEnum } from '@app/enums/aqua-farm-fish-organism-report-type.enum';

export class StatisticalFormAquaFarmFishOrganismDTO { 
    public constructor(obj?: Partial<StatisticalFormAquaFarmFishOrganismDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public installationTypeId?: number;

    @StrictlyTyped(Number)
    public reportType?: AquaFarmFishOrganismReportTypeEnum;

    @StrictlyTyped(Number)
    public fishTypeId?: number;

    @StrictlyTyped(Number)
    public fishLarvaeCount?: number;

    @StrictlyTyped(Number)
    public oneStripBreedingMaterialCount?: number;

    @StrictlyTyped(Number)
    public oneStripBreedingMaterialWeight?: number;

    @StrictlyTyped(Number)
    public oneYearBreedingMaterialCount?: number;

    @StrictlyTyped(Number)
    public oneYearBreedingMaterialWeight?: number;

    @StrictlyTyped(Number)
    public forConsumption?: number;

    @StrictlyTyped(Number)
    public caviarForConsumption?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
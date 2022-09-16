

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class StatisticalFormAquaFarmInstallationSystemNotFullDTO { 
    public constructor(obj?: Partial<StatisticalFormAquaFarmInstallationSystemNotFullDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public installationTypeId?: number;

    @StrictlyTyped(Boolean)
    public isInstallationUsedBreedingMaterial?: boolean;

    @StrictlyTyped(Boolean)
    public isInstallationUsedConsumationFish?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
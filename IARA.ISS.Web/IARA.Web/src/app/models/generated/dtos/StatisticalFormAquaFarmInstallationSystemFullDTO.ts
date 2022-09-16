

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class StatisticalFormAquaFarmInstallationSystemFullDTO { 
    public constructor(obj?: Partial<StatisticalFormAquaFarmInstallationSystemFullDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public installationTypeId?: number;

    @StrictlyTyped(Boolean)
    public isInstallationUsed?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
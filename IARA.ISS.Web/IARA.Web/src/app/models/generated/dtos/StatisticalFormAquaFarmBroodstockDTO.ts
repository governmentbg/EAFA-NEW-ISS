

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class StatisticalFormAquaFarmBroodstockDTO { 
    public constructor(obj?: Partial<StatisticalFormAquaFarmBroodstockDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public installationTypeId?: number;

    @StrictlyTyped(Number)
    public fishTypeId?: number;

    @StrictlyTyped(Number)
    public maleCount?: number;

    @StrictlyTyped(Number)
    public maleWeight?: number;

    @StrictlyTyped(Number)
    public maleAge?: number;

    @StrictlyTyped(Number)
    public femaleCount?: number;

    @StrictlyTyped(Number)
    public femaleWeight?: number;

    @StrictlyTyped(Number)
    public femaleAge?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
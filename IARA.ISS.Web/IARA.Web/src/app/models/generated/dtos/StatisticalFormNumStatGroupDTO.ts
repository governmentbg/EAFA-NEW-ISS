

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { StatisticalFormNumStatDTO } from './StatisticalFormNumStatDTO';
import { NumericStatTypeGroupsEnum } from '@app/enums/numeric-stat-type-groups.enum';

export class StatisticalFormNumStatGroupDTO { 
    public constructor(obj?: Partial<StatisticalFormNumStatGroupDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public statFormTypeId?: number;

    @StrictlyTyped(String)
    public groupName?: string;

    @StrictlyTyped(Number)
    public groupType?: NumericStatTypeGroupsEnum;

    @StrictlyTyped(StatisticalFormNumStatDTO)
    public numericStatTypes?: StatisticalFormNumStatDTO[];

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
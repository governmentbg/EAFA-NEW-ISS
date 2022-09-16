

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { StatisticalFormEmployeeInfoDTO } from './StatisticalFormEmployeeInfoDTO';

export class StatisticalFormEmployeeInfoGroupDTO { 
    public constructor(obj?: Partial<StatisticalFormEmployeeInfoGroupDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public statFormTypeId?: number;

    @StrictlyTyped(String)
    public groupName?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(StatisticalFormEmployeeInfoDTO)
    public employeeTypes?: StatisticalFormEmployeeInfoDTO[];
}
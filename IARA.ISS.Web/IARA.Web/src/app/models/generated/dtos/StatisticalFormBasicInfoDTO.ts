

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';

export class StatisticalFormBasicInfoDTO { 
    public constructor(obj?: Partial<StatisticalFormBasicInfoDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public processUserId?: number;

    @StrictlyTyped(Number)
    public year?: number;

    @StrictlyTyped(Date)
    public submissionDate?: Date;

    @StrictlyTyped(RegixPersonDataDTO)
    public submissionPerson?: RegixPersonDataDTO;

    @StrictlyTyped(String)
    public submissionPersonWorkPosition?: string;
}
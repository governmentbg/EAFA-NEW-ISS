

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PointsTypeEnum } from '@app/enums/points-type.enum';

export class PenalPointsDTO { 
    public constructor(obj?: Partial<PenalPointsDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public decreeNum?: string;

    @StrictlyTyped(String)
    public penalDecreeNum?: string;

    @StrictlyTyped(Number)
    public penalDecreeId?: number;

    @StrictlyTyped(Number)
    public pointsType?: PointsTypeEnum;

    @StrictlyTyped(String)
    public pointsTypeName?: string;

    @StrictlyTyped(String)
    public orderTypeName?: string;

    @StrictlyTyped(String)
    public orderNum?: string;

    @StrictlyTyped(Date)
    public issueDate?: Date;

    @StrictlyTyped(String)
    public ship?: string;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(Number)
    public pointsAmount?: number;

    @StrictlyTyped(Number)
    public pointsTotalCount?: number;

    @StrictlyTyped(Boolean)
    public isIncreasePoints?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
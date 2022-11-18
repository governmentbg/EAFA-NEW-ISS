

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AuanInspectedEntityDTO } from './AuanInspectedEntityDTO';

export class PenalPointsAuanDecreeDataDTO { 
    public constructor(obj?: Partial<PenalPointsAuanDecreeDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(Number)
    public territoryUnitId?: number;

    @StrictlyTyped(String)
    public auanNum?: string;

    @StrictlyTyped(Date)
    public auanDate?: Date;

    @StrictlyTyped(String)
    public decreeNum?: string;

    @StrictlyTyped(Date)
    public decreeIssueDate?: Date;

    @StrictlyTyped(Date)
    public decreeEffectiveDate?: Date;

    @StrictlyTyped(AuanInspectedEntityDTO)
    public inspectedEntity?: AuanInspectedEntityDTO;
}
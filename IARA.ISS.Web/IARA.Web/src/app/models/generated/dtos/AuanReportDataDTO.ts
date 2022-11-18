

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AuanInspectedEntityDTO } from './AuanInspectedEntityDTO';
import { AuanViolatedRegulationDTO } from './AuanViolatedRegulationDTO';

export class AuanReportDataDTO { 
    public constructor(obj?: Partial<AuanReportDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public reportNum?: string;

    @StrictlyTyped(Number)
    public inspectionTypeId?: number;

    @StrictlyTyped(Number)
    public territoryUnitId?: number;

    @StrictlyTyped(AuanInspectedEntityDTO)
    public inspectedEntities?: AuanInspectedEntityDTO[];

    @StrictlyTyped(AuanViolatedRegulationDTO)
    public violatedRegulations?: AuanViolatedRegulationDTO[];
}
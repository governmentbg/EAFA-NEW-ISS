

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { VesselDuringInspectionDTO } from './VesselDuringInspectionDTO';
import { CatchActionEnum } from '@app/enums/catch-action.enum';

export class InspectionCatchMeasureDTO { 
    public constructor(obj?: Partial<InspectionCatchMeasureDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public catchInspectionTypeId?: number;

    @StrictlyTyped(Number)
    public fishId?: number;

    @StrictlyTyped(Number)
    public catchQuantity?: number;

    @StrictlyTyped(Number)
    public allowedDeviation?: number;

    @StrictlyTyped(Number)
    public catchZoneId?: number;

    @StrictlyTyped(Boolean)
    public isTaken?: boolean;

    @StrictlyTyped(Number)
    public action?: CatchActionEnum;

    @StrictlyTyped(String)
    public storageLocation?: string;

    @StrictlyTyped(Number)
    public unloadedQuantity?: number;

    @StrictlyTyped(Number)
    public averageSize?: number;

    @StrictlyTyped(Number)
    public fishSexId?: number;

    @StrictlyTyped(Number)
    public catchCount?: number;

    @StrictlyTyped(VesselDuringInspectionDTO)
    public originShip?: VesselDuringInspectionDTO;
}
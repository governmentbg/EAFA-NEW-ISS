

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class WaterInspectionVesselDTO { 
    public constructor(obj?: Partial<WaterInspectionVesselDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public vesselTypeId?: number;

    @StrictlyTyped(String)
    public number?: string;

    @StrictlyTyped(Number)
    public length?: number;

    @StrictlyTyped(Number)
    public width?: number;

    @StrictlyTyped(String)
    public color?: string;

    @StrictlyTyped(Number)
    public totalCount?: number;

    @StrictlyTyped(Boolean)
    public isTaken?: boolean;

    @StrictlyTyped(Boolean)
    public isStored?: boolean;

    @StrictlyTyped(String)
    public storageLocation?: string;
}
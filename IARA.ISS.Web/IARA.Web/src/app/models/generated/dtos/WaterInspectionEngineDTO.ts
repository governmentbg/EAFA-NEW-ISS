

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class WaterInspectionEngineDTO { 
    public constructor(obj?: Partial<WaterInspectionEngineDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public model?: string;

    @StrictlyTyped(Number)
    public power?: number;

    @StrictlyTyped(String)
    public type?: string;

    @StrictlyTyped(Number)
    public totalCount?: number;

    @StrictlyTyped(Boolean)
    public isTaken?: boolean;

    @StrictlyTyped(Boolean)
    public isStored?: boolean;

    @StrictlyTyped(String)
    public storageLocation?: string;

    @StrictlyTyped(String)
    public engineDescription?: string;
}
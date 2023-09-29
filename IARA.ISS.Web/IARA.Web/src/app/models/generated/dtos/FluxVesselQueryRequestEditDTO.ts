

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FluxVesselQueryRequestEditDTO { 
    public constructor(obj?: Partial<FluxVesselQueryRequestEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Date)
    public dateTimeFrom?: Date;

    @StrictlyTyped(Date)
    public dateTimeTo?: Date;

    @StrictlyTyped(String)
    public cfr?: string;

    @StrictlyTyped(String)
    public uvi?: string;

    @StrictlyTyped(String)
    public ircs?: string;

    @StrictlyTyped(String)
    public mmsi?: string;

    @StrictlyTyped(String)
    public flagStateCode?: string;

    @StrictlyTyped(Boolean)
    public histYes?: boolean;

    @StrictlyTyped(Boolean)
    public histNo?: boolean;

    @StrictlyTyped(Boolean)
    public vesselActive?: boolean;

    @StrictlyTyped(Boolean)
    public vesselAll?: boolean;

    @StrictlyTyped(Boolean)
    public dataVcd?: boolean;

    @StrictlyTyped(Boolean)
    public dataAll?: boolean;
}
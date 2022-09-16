

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class VesselDTO { 
    public constructor(obj?: Partial<VesselDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(Number)
    public unregisteredVesselId?: number;

    @StrictlyTyped(Boolean)
    public isRegistered?: boolean;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public externalMark?: string;

    @StrictlyTyped(String)
    public cfr?: string;

    @StrictlyTyped(String)
    public uvi?: string;

    @StrictlyTyped(String)
    public regularCallsign?: string;

    @StrictlyTyped(String)
    public mmsi?: string;

    @StrictlyTyped(Number)
    public flagCountryId?: number;

    @StrictlyTyped(Number)
    public patrolVehicleTypeId?: number;

    @StrictlyTyped(Number)
    public vesselTypeId?: number;

    @StrictlyTyped(Number)
    public institutionId?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
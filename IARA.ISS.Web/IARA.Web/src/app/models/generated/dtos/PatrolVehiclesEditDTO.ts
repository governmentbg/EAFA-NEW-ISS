

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PatrolVehiclesEditDTO { 
    public constructor(obj?: Partial<PatrolVehiclesEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(Number)
    public flagCountryId?: number;

    @StrictlyTyped(String)
    public externalMark?: string;

    @StrictlyTyped(Number)
    public patrolVehicleTypeId?: number;

    @StrictlyTyped(String)
    public cfr?: string;

    @StrictlyTyped(String)
    public uvi?: string;

    @StrictlyTyped(String)
    public ircscallSign?: string;

    @StrictlyTyped(String)
    public mmsi?: string;

    @StrictlyTyped(Number)
    public vesselTypeId?: number;

    @StrictlyTyped(Number)
    public institutionId?: number;
}
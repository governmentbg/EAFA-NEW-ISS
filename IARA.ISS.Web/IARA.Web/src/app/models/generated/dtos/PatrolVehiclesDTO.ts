

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PatrolVehiclesDTO { 
    public constructor(obj?: Partial<PatrolVehiclesDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(Number)
    public flagCountryId?: number;

    @StrictlyTyped(String)
    public flagCountry?: string;

    @StrictlyTyped(String)
    public externalMark?: string;

    @StrictlyTyped(Number)
    public patrolVehicleTypeId?: number;

    @StrictlyTyped(String)
    public patrolVehicleType?: string;

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

    @StrictlyTyped(String)
    public vesselType?: string;

    @StrictlyTyped(Number)
    public institutionId?: number;

    @StrictlyTyped(String)
    public institution?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
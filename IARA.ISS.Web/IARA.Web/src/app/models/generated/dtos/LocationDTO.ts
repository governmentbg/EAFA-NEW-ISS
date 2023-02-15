

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class LocationDTO { 
    public constructor(obj?: Partial<LocationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public dmsLongitude?: string;

    @StrictlyTyped(String)
    public dmsLatitude?: string;
}
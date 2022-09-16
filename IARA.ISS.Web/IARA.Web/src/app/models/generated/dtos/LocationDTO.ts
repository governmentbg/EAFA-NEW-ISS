
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class LocationDTO {
    public constructor(obj?: Partial<LocationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public longitude?: number;

    @StrictlyTyped(Number)
    public latitude?: number;
}
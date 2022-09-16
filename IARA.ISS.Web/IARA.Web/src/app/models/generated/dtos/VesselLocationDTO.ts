
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { VesselDTO } from './VesselDTO';
import { LocationDTO } from './LocationDTO';

export class VesselLocationDTO {
    public constructor(obj?: Partial<VesselLocationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(VesselDTO)
    public vessel?: VesselDTO;

    @StrictlyTyped(LocationDTO)
    public location?: LocationDTO;
}
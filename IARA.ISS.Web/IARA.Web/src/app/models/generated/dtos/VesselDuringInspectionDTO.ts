

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { VesselDTO } from './VesselDTO';
import { LocationDTO } from './LocationDTO'; 

export class VesselDuringInspectionDTO extends VesselDTO {
    public constructor(obj?: Partial<VesselDuringInspectionDTO>) {
        if (obj != undefined) {
            super(obj as VesselDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(LocationDTO)
    public location?: LocationDTO;

    @StrictlyTyped(Number)
    public catchZoneId?: number;

    @StrictlyTyped(String)
    public locationDescription?: string;

    @StrictlyTyped(Number)
    public shipAssociationId?: number;
}
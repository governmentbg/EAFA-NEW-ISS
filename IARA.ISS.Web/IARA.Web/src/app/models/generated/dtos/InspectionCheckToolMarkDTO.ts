

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectionEditDTO } from './InspectionEditDTO';
import { VesselDuringInspectionDTO } from './VesselDuringInspectionDTO';
import { PortVisitDTO } from './PortVisitDTO';
import { InspectedFishingGearDTO } from './InspectedFishingGearDTO'; 

export class InspectionCheckToolMarkDTO extends InspectionEditDTO {
    public constructor(obj?: Partial<InspectionCheckToolMarkDTO>) {
        if (obj != undefined) {
            super(obj as InspectionEditDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(VesselDuringInspectionDTO)
    public inspectedShip?: VesselDuringInspectionDTO;

    @StrictlyTyped(PortVisitDTO)
    public port?: PortVisitDTO;

    @StrictlyTyped(Number)
    public poundNetId?: number;

    @StrictlyTyped(Number)
    public checkReasonId?: number;

    @StrictlyTyped(Number)
    public recheckReasonId?: number;

    @StrictlyTyped(String)
    public otherRecheckReason?: string;

    @StrictlyTyped(String)
    public ownerComment?: string;

    @StrictlyTyped(Number)
    public permitId?: number;

    @StrictlyTyped(String)
    public unregisteredPermitNumber?: string;

    @StrictlyTyped(Number)
    public unregisteredPermitYear?: number;

    @StrictlyTyped(InspectedFishingGearDTO)
    public fishingGears?: InspectedFishingGearDTO[];
}
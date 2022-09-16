

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectionEditDTO } from './InspectionEditDTO';
import { VesselDuringInspectionDTO } from './VesselDuringInspectionDTO';
import { PortVisitDTO } from './PortVisitDTO';
import { InspectionCatchMeasureDTO } from './InspectionCatchMeasureDTO';
import { InspectedFishingGearDTO } from './InspectedFishingGearDTO';
import { InspectionPermitDTO } from './InspectionPermitDTO';
import { InspectionLogBookDTO } from './InspectionLogBookDTO'; 

export class InspectionAtSeaDTO extends InspectionEditDTO {
    public constructor(obj?: Partial<InspectionAtSeaDTO>) {
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
    public lastPortVisit?: PortVisitDTO;

    @StrictlyTyped(String)
    public captainComment?: string;

    @StrictlyTyped(InspectionCatchMeasureDTO)
    public catchMeasures?: InspectionCatchMeasureDTO[];

    @StrictlyTyped(InspectedFishingGearDTO)
    public fishingGears?: InspectedFishingGearDTO[];

    @StrictlyTyped(InspectionPermitDTO)
    public permitLicenses?: InspectionPermitDTO[];

    @StrictlyTyped(InspectionLogBookDTO)
    public logBooks?: InspectionLogBookDTO[];
}
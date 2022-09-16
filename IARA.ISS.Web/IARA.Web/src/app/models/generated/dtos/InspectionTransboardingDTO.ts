

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectionEditDTO } from './InspectionEditDTO';
import { InspectionCatchMeasureDTO } from './InspectionCatchMeasureDTO';
import { InspectionTransboardingShipDTO } from './InspectionTransboardingShipDTO';
import { InspectedFishingGearDTO } from './InspectedFishingGearDTO'; 

export class InspectionTransboardingDTO extends InspectionEditDTO {
    public constructor(obj?: Partial<InspectionTransboardingDTO>) {
        if (obj != undefined) {
            super(obj as InspectionEditDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(InspectionCatchMeasureDTO)
    public transboardedCatchMeasures?: InspectionCatchMeasureDTO[];

    @StrictlyTyped(InspectionTransboardingShipDTO)
    public receivingShipInspection?: InspectionTransboardingShipDTO;

    @StrictlyTyped(InspectionTransboardingShipDTO)
    public sendingShipInspection?: InspectionTransboardingShipDTO;

    @StrictlyTyped(InspectedFishingGearDTO)
    public fishingGears?: InspectedFishingGearDTO[];
}
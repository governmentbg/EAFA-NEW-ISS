

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectionEditDTO } from './InspectionEditDTO';
import { LocationDTO } from './LocationDTO';
import { WaterInspectionFishingGearDTO } from './WaterInspectionFishingGearDTO';
import { WaterInspectionVesselDTO } from './WaterInspectionVesselDTO';
import { WaterInspectionEngineDTO } from './WaterInspectionEngineDTO';
import { InspectionCatchMeasureDTO } from './InspectionCatchMeasureDTO'; 

export class InspectionCheckWaterObjectDTO extends InspectionEditDTO {
    public constructor(obj?: Partial<InspectionCheckWaterObjectDTO>) {
        if (obj != undefined) {
            super(obj as InspectionEditDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(String)
    public objectName?: string;

    @StrictlyTyped(Number)
    public waterObjectTypeId?: number;

    @StrictlyTyped(LocationDTO)
    public waterObjectLocation?: LocationDTO;

    @StrictlyTyped(WaterInspectionFishingGearDTO)
    public fishingGears?: WaterInspectionFishingGearDTO[];

    @StrictlyTyped(WaterInspectionVesselDTO)
    public vessels?: WaterInspectionVesselDTO[];

    @StrictlyTyped(WaterInspectionEngineDTO)
    public engines?: WaterInspectionEngineDTO[];

    @StrictlyTyped(InspectionCatchMeasureDTO)
    public catches?: InspectionCatchMeasureDTO[];
}
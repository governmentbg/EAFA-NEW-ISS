

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectionEditDTO } from './InspectionEditDTO';
import { LocationDTO } from './LocationDTO';
import { InspectionCatchMeasureDTO } from './InspectionCatchMeasureDTO'; 

export class InspectionFisherDTO extends InspectionEditDTO {
    public constructor(obj?: Partial<InspectionFisherDTO>) {
        if (obj != undefined) {
            super(obj as InspectionEditDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(String)
    public ticketNum?: string;

    @StrictlyTyped(Number)
    public fishingRodsCount?: number;

    @StrictlyTyped(Number)
    public fishingHooksCount?: number;

    @StrictlyTyped(String)
    public fishermanComment?: string;

    @StrictlyTyped(String)
    public inspectionAddress?: string;

    @StrictlyTyped(LocationDTO)
    public inspectionLocation?: LocationDTO;

    @StrictlyTyped(InspectionCatchMeasureDTO)
    public catchMeasures?: InspectionCatchMeasureDTO[];
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectionEditDTO } from './InspectionEditDTO';
import { LocationDTO } from './LocationDTO';
import { InspectionCatchMeasureDTO } from './InspectionCatchMeasureDTO'; 

export class InspectionAquacultureDTO extends InspectionEditDTO {
    public constructor(obj?: Partial<InspectionAquacultureDTO>) {
        if (obj != undefined) {
            super(obj as InspectionEditDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public aquacultureId?: number;

    @StrictlyTyped(String)
    public representativeComment?: string;

    @StrictlyTyped(String)
    public otherFishingGear?: string;

    @StrictlyTyped(LocationDTO)
    public location?: LocationDTO;

    @StrictlyTyped(InspectionCatchMeasureDTO)
    public catchMeasures?: InspectionCatchMeasureDTO[];
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectionEditDTO } from './InspectionEditDTO';
import { InspectedCPFishingGearDTO } from './InspectedCPFishingGearDTO';
import { InspectedCPCatchDTO } from './InspectedCPCatchDTO'; 

export class InspectionConstativeProtocolDTO extends InspectionEditDTO {
    public constructor(obj?: Partial<InspectionConstativeProtocolDTO>) {
        if (obj != undefined) {
            super(obj as InspectionEditDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(String)
    public inspectedPersonName?: string;

    @StrictlyTyped(String)
    public inspectedObjectName?: string;

    @StrictlyTyped(String)
    public witness2Name?: string;

    @StrictlyTyped(String)
    public witness1Name?: string;

    @StrictlyTyped(String)
    public inspectorName?: string;

    @StrictlyTyped(String)
    public location?: string;

    @StrictlyTyped(InspectedCPFishingGearDTO)
    public fishingGears?: InspectedCPFishingGearDTO[];

    @StrictlyTyped(InspectedCPCatchDTO)
    public catches?: InspectedCPCatchDTO[];
}
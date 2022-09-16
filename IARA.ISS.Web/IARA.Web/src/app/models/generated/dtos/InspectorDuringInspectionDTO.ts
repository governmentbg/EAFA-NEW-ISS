

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectorDTO } from './InspectorDTO'; 

export class InspectorDuringInspectionDTO extends InspectorDTO {
    public constructor(obj?: Partial<InspectorDuringInspectionDTO>) {
        if (obj != undefined) {
            super(obj as InspectorDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Boolean)
    public isInCharge?: boolean;

    @StrictlyTyped(Boolean)
    public hasIdentifiedHimself?: boolean;
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectionObjectCheckDTO } from './InspectionObjectCheckDTO'; 

export class InspectionSpecialPermitDTO extends InspectionObjectCheckDTO {
    public constructor(obj?: Partial<InspectionSpecialPermitDTO>) {
        if (obj != undefined) {
            super(obj as InspectionObjectCheckDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Date)
    public from?: Date;

    @StrictlyTyped(Date)
    public to?: Date;
}
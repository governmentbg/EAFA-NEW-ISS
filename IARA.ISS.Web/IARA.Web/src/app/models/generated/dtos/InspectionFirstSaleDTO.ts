

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectionEditDTO } from './InspectionEditDTO';
import { InspectionLogBookPageDTO } from './InspectionLogBookPageDTO'; 

export class InspectionFirstSaleDTO extends InspectionEditDTO {
    public constructor(obj?: Partial<InspectionFirstSaleDTO>) {
        if (obj != undefined) {
            super(obj as InspectionEditDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(String)
    public subjectName?: string;

    @StrictlyTyped(String)
    public subjectAddress?: string;

    @StrictlyTyped(String)
    public representativeComment?: string;

    @StrictlyTyped(InspectionLogBookPageDTO)
    public inspectionLogBookPages?: InspectionLogBookPageDTO[];
}
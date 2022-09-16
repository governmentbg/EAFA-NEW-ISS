

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectionEditDTO } from './InspectionEditDTO';
import { InspectionCatchMeasureDTO } from './InspectionCatchMeasureDTO'; 

export class InspectionFristSaleDTO extends InspectionEditDTO {
    public constructor(obj?: Partial<InspectionFristSaleDTO>) {
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
    public unloadDeclarationNum?: string;

    @StrictlyTyped(Date)
    public unloadDeclarationDate?: Date;

    @StrictlyTyped(String)
    public acceptanceDeclarationNum?: string;

    @StrictlyTyped(Date)
    public acceptanceDeclarationDate?: Date;

    @StrictlyTyped(String)
    public certificateNum?: string;

    @StrictlyTyped(Date)
    public certificateDate?: Date;

    @StrictlyTyped(String)
    public representativeComment?: string;

    @StrictlyTyped(InspectionCatchMeasureDTO)
    public catchMeasures?: InspectionCatchMeasureDTO[];
}
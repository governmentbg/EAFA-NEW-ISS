

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AuanViolatedRegulationDTO } from './AuanViolatedRegulationDTO'; 

export class AuanLawSectionDTO extends AuanViolatedRegulationDTO {
    public constructor(obj?: Partial<AuanLawSectionDTO>) {
        if (obj != undefined) {
            super(obj as AuanViolatedRegulationDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public lawId?: number;

    @StrictlyTyped(String)
    public lawText?: string;

    @StrictlyTyped(Boolean)
    public isChecked?: boolean;
}
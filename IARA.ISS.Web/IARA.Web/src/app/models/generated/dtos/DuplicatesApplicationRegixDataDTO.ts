

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { DuplicatesApplicationBaseRegixDataDTO } from './DuplicatesApplicationBaseRegixDataDTO';
import { ApplicationSubmittedByRegixDataDTO } from './ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from './ApplicationSubmittedForRegixDataDTO'; 

export class DuplicatesApplicationRegixDataDTO extends DuplicatesApplicationBaseRegixDataDTO {
    public constructor(obj?: Partial<DuplicatesApplicationRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as DuplicatesApplicationBaseRegixDataDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(ApplicationSubmittedByRegixDataDTO)
    public submittedBy?: ApplicationSubmittedByRegixDataDTO;

    @StrictlyTyped(ApplicationSubmittedForRegixDataDTO)
    public submittedFor?: ApplicationSubmittedForRegixDataDTO;
}
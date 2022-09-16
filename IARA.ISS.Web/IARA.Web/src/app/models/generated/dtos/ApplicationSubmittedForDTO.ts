

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ApplicationSubmittedForRegixDataDTO } from './ApplicationSubmittedForRegixDataDTO';
import { LetterOfAttorneyDTO } from './LetterOfAttorneyDTO'; 

export class ApplicationSubmittedForDTO extends ApplicationSubmittedForRegixDataDTO {
    public constructor(obj?: Partial<ApplicationSubmittedForDTO>) {
        if (obj != undefined) {
            super(obj as ApplicationSubmittedForRegixDataDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(LetterOfAttorneyDTO)
    public submittedByLetterOfAttorney?: LetterOfAttorneyDTO;
}
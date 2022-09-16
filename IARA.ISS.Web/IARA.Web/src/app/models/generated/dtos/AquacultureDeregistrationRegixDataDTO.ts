

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AquacultureDeregistrationBaseRegixDataDTO } from './AquacultureDeregistrationBaseRegixDataDTO';
import { ApplicationSubmittedByRegixDataDTO } from './ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from './ApplicationSubmittedForRegixDataDTO';
import { ApplicationRegiXCheckDTO } from './ApplicationRegiXCheckDTO'; 

export class AquacultureDeregistrationRegixDataDTO extends AquacultureDeregistrationBaseRegixDataDTO {
    public constructor(obj?: Partial<AquacultureDeregistrationRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as AquacultureDeregistrationBaseRegixDataDTO);
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

    @StrictlyTyped(ApplicationRegiXCheckDTO)
    public applicationRegiXChecks?: ApplicationRegiXCheckDTO[];
}
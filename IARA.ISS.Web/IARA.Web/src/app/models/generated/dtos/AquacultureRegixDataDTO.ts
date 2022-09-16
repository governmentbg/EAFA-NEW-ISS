

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AquacultureBaseRegixDataDTO } from './AquacultureBaseRegixDataDTO';
import { ApplicationSubmittedByRegixDataDTO } from './ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from './ApplicationSubmittedForRegixDataDTO';
import { UsageDocumentRegixDataDTO } from './UsageDocumentRegixDataDTO';
import { ApplicationRegiXCheckDTO } from './ApplicationRegiXCheckDTO'; 

export class AquacultureRegixDataDTO extends AquacultureBaseRegixDataDTO {
    public constructor(obj?: Partial<AquacultureRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as AquacultureBaseRegixDataDTO);
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

    @StrictlyTyped(UsageDocumentRegixDataDTO)
    public usageDocument?: UsageDocumentRegixDataDTO;

    @StrictlyTyped(ApplicationRegiXCheckDTO)
    public applicationRegiXChecks?: ApplicationRegiXCheckDTO[];
}
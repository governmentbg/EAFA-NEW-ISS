

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AquacultureChangeOfCircumstancesBaseRegixDataDTO } from './AquacultureChangeOfCircumstancesBaseRegixDataDTO';
import { ApplicationSubmittedByRegixDataDTO } from './ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from './ApplicationSubmittedForRegixDataDTO';
import { ChangeOfCircumstancesDTO } from './ChangeOfCircumstancesDTO';
import { ApplicationRegiXCheckDTO } from './ApplicationRegiXCheckDTO'; 

export class AquacultureChangeOfCircumstancesRegixDataDTO extends AquacultureChangeOfCircumstancesBaseRegixDataDTO {
    public constructor(obj?: Partial<AquacultureChangeOfCircumstancesRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as AquacultureChangeOfCircumstancesBaseRegixDataDTO);
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

    @StrictlyTyped(ChangeOfCircumstancesDTO)
    public changes?: ChangeOfCircumstancesDTO[];

    @StrictlyTyped(ApplicationRegiXCheckDTO)
    public applicationRegiXChecks?: ApplicationRegiXCheckDTO[];
}
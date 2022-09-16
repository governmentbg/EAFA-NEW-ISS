

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ShipChangeOfCircumstancesBaseRegixDataDTO } from './ShipChangeOfCircumstancesBaseRegixDataDTO';
import { ApplicationSubmittedByRegixDataDTO } from './ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from './ApplicationSubmittedForRegixDataDTO';
import { ChangeOfCircumstancesDTO } from './ChangeOfCircumstancesDTO';
import { ApplicationRegiXCheckDTO } from './ApplicationRegiXCheckDTO'; 

export class ShipChangeOfCircumstancesRegixDataDTO extends ShipChangeOfCircumstancesBaseRegixDataDTO {
    public constructor(obj?: Partial<ShipChangeOfCircumstancesRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as ShipChangeOfCircumstancesBaseRegixDataDTO);
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
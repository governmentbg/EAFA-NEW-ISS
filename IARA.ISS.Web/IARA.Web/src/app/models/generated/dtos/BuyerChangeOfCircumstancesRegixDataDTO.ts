

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BuyerChangeOfCircumstancesBaseRegixDataDTO } from './BuyerChangeOfCircumstancesBaseRegixDataDTO';
import { ApplicationSubmittedByRegixDataDTO } from './ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from './ApplicationSubmittedForRegixDataDTO';
import { ChangeOfCircumstancesDTO } from './ChangeOfCircumstancesDTO'; 

export class BuyerChangeOfCircumstancesRegixDataDTO extends BuyerChangeOfCircumstancesBaseRegixDataDTO {
    public constructor(obj?: Partial<BuyerChangeOfCircumstancesRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as BuyerChangeOfCircumstancesBaseRegixDataDTO);
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
}
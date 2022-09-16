

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { CapacityCertificateDuplicateBaseRegixDataDTO } from './CapacityCertificateDuplicateBaseRegixDataDTO';
import { ApplicationSubmittedByRegixDataDTO } from './ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from './ApplicationSubmittedForRegixDataDTO'; 

export class CapacityCertificateDuplicateRegixDataDTO extends CapacityCertificateDuplicateBaseRegixDataDTO {
    public constructor(obj?: Partial<CapacityCertificateDuplicateRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as CapacityCertificateDuplicateBaseRegixDataDTO);
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
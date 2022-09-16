

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BaseRegixChecksDTO } from './BaseRegixChecksDTO';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { RegixLegalDataDTO } from './RegixLegalDataDTO'; 

export class ScientificFishingPermitBaseRegixDataDTO extends BaseRegixChecksDTO {
    public constructor(obj?: Partial<ScientificFishingPermitBaseRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as BaseRegixChecksDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(RegixPersonDataDTO)
    public requester?: RegixPersonDataDTO;

    @StrictlyTyped(RegixLegalDataDTO)
    public receiver?: RegixLegalDataDTO;
}
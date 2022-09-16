

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BaseRegixChecksDTO } from './BaseRegixChecksDTO'; 

export class AquacultureChangeOfCircumstancesBaseRegixDataDTO extends BaseRegixChecksDTO {
    public constructor(obj?: Partial<AquacultureChangeOfCircumstancesBaseRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as BaseRegixChecksDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Number)
    public aquacultureFacilityId?: number;
}
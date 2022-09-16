

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BaseRegixChecksDTO } from './BaseRegixChecksDTO'; 

export class AquacultureDeregistrationBaseRegixDataDTO extends BaseRegixChecksDTO {
    public constructor(obj?: Partial<AquacultureDeregistrationBaseRegixDataDTO>) {
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
}
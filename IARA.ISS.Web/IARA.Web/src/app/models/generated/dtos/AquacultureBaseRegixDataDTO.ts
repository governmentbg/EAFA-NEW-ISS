

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BaseRegixChecksDTO } from './BaseRegixChecksDTO'; 

export class AquacultureBaseRegixDataDTO extends BaseRegixChecksDTO {
    public constructor(obj?: Partial<AquacultureBaseRegixDataDTO>) {
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

    @StrictlyTyped(Number)
    public applicationId?: number;
}
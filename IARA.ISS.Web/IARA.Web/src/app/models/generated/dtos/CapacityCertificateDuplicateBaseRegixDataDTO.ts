

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BaseRegixChecksDTO } from './BaseRegixChecksDTO'; 

export class CapacityCertificateDuplicateBaseRegixDataDTO extends BaseRegixChecksDTO {
    public constructor(obj?: Partial<CapacityCertificateDuplicateBaseRegixDataDTO>) {
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
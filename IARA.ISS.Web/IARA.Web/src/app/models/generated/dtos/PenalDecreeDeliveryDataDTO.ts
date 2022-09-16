

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AuanDeliveryDataDTO } from './AuanDeliveryDataDTO'; 

export class PenalDecreeDeliveryDataDTO extends AuanDeliveryDataDTO {
    public constructor(obj?: Partial<PenalDecreeDeliveryDataDTO>) {
        if (obj != undefined) {
            super(obj as AuanDeliveryDataDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  }
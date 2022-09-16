

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BaseRegixChecksDTO } from './BaseRegixChecksDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum'; 

export class BuyerChangeOfCircumstancesBaseRegixDataDTO extends BaseRegixChecksDTO {
    public constructor(obj?: Partial<BuyerChangeOfCircumstancesBaseRegixDataDTO>) {
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
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(Boolean)
    public isOnline?: boolean;

    @StrictlyTyped(Number)
    public buyerId?: number;

    @StrictlyTyped(String)
    public buyerUrorrNumber?: string;
}
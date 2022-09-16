

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class BuyerDuplicateDataDTO { 
    public constructor(obj?: Partial<BuyerDuplicateDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Boolean)
    public isOnline?: boolean;

    @StrictlyTyped(Number)
    public buyerId?: number;

    @StrictlyTyped(String)
    public buyerUrorrNumber?: string;
}
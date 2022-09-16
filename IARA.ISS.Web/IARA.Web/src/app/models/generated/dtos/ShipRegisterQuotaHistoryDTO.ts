

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ShipRegisterQuotaHistoryDTO { 
    public constructor(obj?: Partial<ShipRegisterQuotaHistoryDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Date)
    public dateOfChange?: Date;

    @StrictlyTyped(Number)
    public shipQuotaSize?: number;

    @StrictlyTyped(Number)
    public shipQuotaIncrement?: number;

    @StrictlyTyped(String)
    public incrementReason?: string;
}
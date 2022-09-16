

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ShipRegisterQuotaHistoryDTO } from './ShipRegisterQuotaHistoryDTO';
import { ShipRegisterCatchHistoryDTO } from './ShipRegisterCatchHistoryDTO';

export class ShipRegisterYearlyQuotaDTO { 
    public constructor(obj?: Partial<ShipRegisterYearlyQuotaDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public catchQuotaId?: number;

    @StrictlyTyped(Number)
    public quotaKg?: number;

    @StrictlyTyped(Number)
    public totalCatch?: number;

    @StrictlyTyped(Number)
    public leftoverQuotaKg?: number;

    @StrictlyTyped(ShipRegisterQuotaHistoryDTO)
    public quotaHistory?: ShipRegisterQuotaHistoryDTO[];

    @StrictlyTyped(ShipRegisterCatchHistoryDTO)
    public catchHistory?: ShipRegisterCatchHistoryDTO[];
}
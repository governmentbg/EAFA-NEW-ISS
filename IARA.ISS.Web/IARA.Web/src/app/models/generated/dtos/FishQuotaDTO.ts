

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { QuotaSpiciesPortDTO } from './QuotaSpiciesPortDTO';

export class FishQuotaDTO { 
    public constructor(obj?: Partial<FishQuotaDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Date)
    public periodFrom?: Date;

    @StrictlyTyped(Date)
    public periodTo?: Date;

    @StrictlyTyped(QuotaSpiciesPortDTO)
    public permittedPorts?: QuotaSpiciesPortDTO[];

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class QuotaAquaticOrganismDTO { 
    public constructor(obj?: Partial<QuotaAquaticOrganismDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public aquaticOrganismId?: number;

    @StrictlyTyped(Number)
    public portId?: number;
}
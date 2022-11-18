

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { EgnLncDTO } from './EgnLncDTO';

export class RecreationalFishingRepresentativeCountDTO { 
    public constructor(obj?: Partial<RecreationalFishingRepresentativeCountDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(EgnLncDTO)
    public egnLnc?: EgnLncDTO;

    @StrictlyTyped(Number)
    public count?: number;
}
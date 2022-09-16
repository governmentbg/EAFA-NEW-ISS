

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ScientificFishingOutingCatchDTO } from './ScientificFishingOutingCatchDTO';

export class ScientificFishingOutingDTO { 
    public constructor(obj?: Partial<ScientificFishingOutingDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public permitId?: number;

    @StrictlyTyped(Date)
    public dateOfOuting?: Date;

    @StrictlyTyped(String)
    public waterArea?: string;

    @StrictlyTyped(ScientificFishingOutingCatchDTO)
    public catches?: ScientificFishingOutingCatchDTO[];

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}


import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FishInformationDTO { 
    public constructor(obj?: Partial<FishInformationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public logBookPageId?: number;

    @StrictlyTyped(String)
    public fishData?: string;
}
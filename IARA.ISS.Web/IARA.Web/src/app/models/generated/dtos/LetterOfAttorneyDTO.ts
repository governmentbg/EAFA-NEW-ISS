

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class LetterOfAttorneyDTO { 
    public constructor(obj?: Partial<LetterOfAttorneyDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public letterNum?: string;

    @StrictlyTyped(Date)
    public letterValidFrom?: Date;

    @StrictlyTyped(Date)
    public letterValidTo?: Date;

    @StrictlyTyped(Boolean)
    public isUnlimited?: boolean;

    @StrictlyTyped(String)
    public notaryName?: string;
}
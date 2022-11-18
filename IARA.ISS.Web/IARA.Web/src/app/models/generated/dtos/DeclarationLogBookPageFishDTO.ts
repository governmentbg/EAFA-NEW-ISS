

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class DeclarationLogBookPageFishDTO { 
    public constructor(obj?: Partial<DeclarationLogBookPageFishDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public logBookId?: number;

    @StrictlyTyped(Number)
    public fishId?: number;

    @StrictlyTyped(Number)
    public presentationId?: number;

    @StrictlyTyped(Number)
    public quantity?: number;
}
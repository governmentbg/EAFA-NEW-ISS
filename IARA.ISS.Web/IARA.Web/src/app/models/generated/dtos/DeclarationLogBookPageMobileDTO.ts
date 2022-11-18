

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { DeclarationLogBookPageFishDTO } from './DeclarationLogBookPageFishDTO';

export class DeclarationLogBookPageDTO { 
    public constructor(obj?: Partial<DeclarationLogBookPageDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public num?: string;

    @StrictlyTyped(Date)
    public date?: Date;

    @StrictlyTyped(DeclarationLogBookPageFishDTO)
    public fishes?: DeclarationLogBookPageFishDTO[];
}
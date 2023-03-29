

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PrefixInputDTO { 
    public constructor(obj?: Partial<PrefixInputDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public prefix?: string;

    @StrictlyTyped(String)
    public inputValue?: string;
}
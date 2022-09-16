

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class BaseCrossCheckResultDTO { 
    public constructor(obj?: Partial<BaseCrossCheckResultDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public pageCode?: string;

    @StrictlyTyped(Number)
    public tableId?: number;

    @StrictlyTyped(String)
    public errorDescription?: string;
}
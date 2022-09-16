

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class LogBookPageCancellationReasonDTO { 
    public constructor(obj?: Partial<LogBookPageCancellationReasonDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public logBookId?: number;

    @StrictlyTyped(String)
    public logBookPageNumber?: string;

    @StrictlyTyped(String)
    public reason?: string;
}
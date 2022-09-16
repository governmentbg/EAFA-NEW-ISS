

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class CancellationHistoryEntryDTO { 
    public constructor(obj?: Partial<CancellationHistoryEntryDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Boolean)
    public isCancelled?: boolean;

    @StrictlyTyped(Number)
    public cancellationReasonId?: number;

    @StrictlyTyped(Date)
    public dateOfChange?: Date;

    @StrictlyTyped(String)
    public issueOrderNum?: string;

    @StrictlyTyped(String)
    public description?: string;
}
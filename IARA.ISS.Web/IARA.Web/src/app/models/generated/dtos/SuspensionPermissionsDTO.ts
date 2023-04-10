

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class SuspensionPermissionsDTO { 
    public constructor(obj?: Partial<SuspensionPermissionsDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Boolean)
    public canReadSuspensions?: boolean;

    @StrictlyTyped(Boolean)
    public canAddSuspensions?: boolean;

    @StrictlyTyped(Boolean)
    public canEditSuspensions?: boolean;

    @StrictlyTyped(Boolean)
    public canDeleteSuspensions?: boolean;

    @StrictlyTyped(Boolean)
    public canRestoreSuspensions?: boolean;
}
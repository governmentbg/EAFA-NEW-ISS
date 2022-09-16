

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class InspectorsRegisterDTO { 
    public constructor(obj?: Partial<InspectorsRegisterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public userId?: number;

    @StrictlyTyped(String)
    public egnLnc?: string;

    @StrictlyTyped(String)
    public identifierType?: string;

    @StrictlyTyped(String)
    public firstName?: string;

    @StrictlyTyped(String)
    public lastName?: string;

    @StrictlyTyped(String)
    public inspectorCardNum?: string;

    @StrictlyTyped(Number)
    public institutionId?: number;

    @StrictlyTyped(String)
    public comments?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
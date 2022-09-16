

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class InspectorsRegisterDTO { 
    public constructor(obj?: Partial<InspectorsRegisterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public username?: string;

    @StrictlyTyped(Number)
    public inspectionSequenceNum?: number;

    @StrictlyTyped(String)
    public inspectorCardNum?: string;
}
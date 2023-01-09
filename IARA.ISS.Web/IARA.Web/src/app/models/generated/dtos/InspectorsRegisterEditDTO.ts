

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class InspectorsRegisterEditDTO { 
    public constructor(obj?: Partial<InspectorsRegisterEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public userId?: number;

    @StrictlyTyped(String)
    public inspectorCardNum?: string;

    @StrictlyTyped(Number)
    public inspectionSequenceNum?: number;
}
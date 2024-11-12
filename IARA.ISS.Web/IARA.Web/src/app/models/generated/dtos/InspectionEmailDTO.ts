

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectedEntityEmailDTO } from './InspectedEntityEmailDTO';

export class InspectionEmailDTO { 
    public constructor(obj?: Partial<InspectionEmailDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public inspectionId?: number;

    @StrictlyTyped(InspectedEntityEmailDTO)
    public inspectedEntityEmails?: InspectedEntityEmailDTO[];
}
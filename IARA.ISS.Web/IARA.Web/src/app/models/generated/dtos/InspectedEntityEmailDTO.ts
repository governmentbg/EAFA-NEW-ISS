

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class InspectedEntityEmailDTO { 
    public constructor(obj?: Partial<InspectedEntityEmailDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public inspectionId?: number;

    @StrictlyTyped(Number)
    public inspectedPersonId?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public email?: string;

    @StrictlyTyped(Number)
    public personId?: number;

    @StrictlyTyped(Number)
    public legalId?: number;

    @StrictlyTyped(Number)
    public unregisteredPersonId?: number;

    @StrictlyTyped(Boolean)
    public sendEmail?: boolean;

    @StrictlyTyped(String)
    public inspectedPersonType?: string;
}
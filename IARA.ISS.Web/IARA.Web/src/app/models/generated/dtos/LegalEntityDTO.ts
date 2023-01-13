

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class LegalEntityDTO { 
    public constructor(obj?: Partial<LegalEntityDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public eik?: string;

    @StrictlyTyped(Date)
    public registrationDate?: Date;

    @StrictlyTyped(Number)
    public activeUsersCount?: number;
}
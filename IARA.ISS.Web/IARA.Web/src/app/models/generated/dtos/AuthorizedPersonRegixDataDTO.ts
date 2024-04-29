

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';

export class AuthorizedPersonRegixDataDTO { 
    public constructor(obj?: Partial<AuthorizedPersonRegixDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public userId?: number;

    @StrictlyTyped(RegixPersonDataDTO)
    public person?: RegixPersonDataDTO;

    @StrictlyTyped(Boolean)
    public hasRegixDataDiscrepancy?: boolean;

    @StrictlyTyped(String)
    public fullName?: string;

    @StrictlyTyped(String)
    public rolesAll?: string;

    @StrictlyTyped(String)
    public email?: string;

    @StrictlyTyped(Boolean)
    public isEmailConfirmed?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
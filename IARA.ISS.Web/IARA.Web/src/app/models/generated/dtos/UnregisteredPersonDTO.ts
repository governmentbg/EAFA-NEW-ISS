

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { EgnLncDTO } from './EgnLncDTO';

export class UnregisteredPersonDTO { 
    public constructor(obj?: Partial<UnregisteredPersonDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public firstName?: string;

    @StrictlyTyped(String)
    public middleName?: string;

    @StrictlyTyped(String)
    public lastName?: string;

    @StrictlyTyped(String)
    public address?: string;

    @StrictlyTyped(Boolean)
    public hasBulgarianAddressRegistration?: boolean;

    @StrictlyTyped(EgnLncDTO)
    public egnLnc?: EgnLncDTO;

    @StrictlyTyped(String)
    public eik?: string;

    @StrictlyTyped(Boolean)
    public isLegal?: boolean;

    @StrictlyTyped(Number)
    public citizenshipId?: number;

    @StrictlyTyped(String)
    public comment?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
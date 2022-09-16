

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { IdentifierTypeEnum } from '@app/enums/identifier-type.enum';

export class UnregisteredPersonEditDTO { 
    public constructor(obj?: Partial<UnregisteredPersonEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public egnLnc?: string;

    @StrictlyTyped(Number)
    public identifierType?: IdentifierTypeEnum;

    @StrictlyTyped(String)
    public firstName?: string;

    @StrictlyTyped(String)
    public lastName?: string;

    @StrictlyTyped(Number)
    public institutionId?: number;

    @StrictlyTyped(String)
    public inspectorCardNum?: string;

    @StrictlyTyped(String)
    public comments?: string;
}


import { UserLegalStatusEnum } from '@app/enums/user-legal-status.enum';
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class UserLegalDTO {
    public constructor(obj?: Partial<UserLegalDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public legalId?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public eik?: string;

    @StrictlyTyped(Number)
    public roleId?: number;

    @StrictlyTyped(String)
    public role?: string;

    @StrictlyTyped(Number)
    public status?: UserLegalStatusEnum;

    @StrictlyTyped(String)
    public statusName?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}
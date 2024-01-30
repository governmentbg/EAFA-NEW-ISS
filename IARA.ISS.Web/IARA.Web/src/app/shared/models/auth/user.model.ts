import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ClaimGroup } from './grouped-claim.model';

export class User {

    public constructor(obj?: Partial<User>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public id!: string;

    @StrictlyTyped(String)
    public username!: string;

    @StrictlyTyped(String)
    public email?: string;

    @StrictlyTyped(String)
    public avatar?: string;

    @StrictlyTyped(ClaimGroup)
    public groupedPermissions!: ClaimGroup[];

    @StrictlyTyped(String)
    public personName?: string;

    @StrictlyTyped(Boolean)
    public isInternalUser!: boolean;

    @StrictlyTyped(Boolean)
    public userMustChangePassword?: boolean;

    @StrictlyTyped(Number)
    public railwayUndertakingId?: number;

    @StrictlyTyped(Number)
    public organizationUnitId?: number;

    @StrictlyTyped(Boolean)
    public requiresShift?: boolean;

    @StrictlyTyped(String)
    public defaultSchedule?: string;

    @StrictlyTyped(Date)
    public passwordExpiryDate?: Date;
}

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class User<TIdentifier> {

    public constructor(obj?: Partial<User<TIdentifier>>) {
        if (obj != undefined) {
            Object.assign(this, obj);
        }
    }

    public id!: TIdentifier;

    @StrictlyTyped(String)
    public username!: string;

    @StrictlyTyped(String)
    public email?: string;

    @StrictlyTyped(String)
    public avatar?: string;

    @StrictlyTyped(String)
    public permissions!: string[];

    @StrictlyTyped(String)
    public personName?: string;

    @StrictlyTyped(Boolean)
    public userMustChangePassword?: boolean;

    @StrictlyTyped(Date)
    public passwordExpiryDate?: Date;
}

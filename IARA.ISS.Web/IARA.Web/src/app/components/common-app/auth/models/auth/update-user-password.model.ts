import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ChangeUserPasswordModel {

    public constructor(obj?: Partial<ChangeUserPasswordModel>) {
        if (obj != undefined) {
            Object.assign(this, obj);
        }
    }

    @StrictlyTyped(String)
    public token!: string;

    @StrictlyTyped(String)
    public password!: string;
}

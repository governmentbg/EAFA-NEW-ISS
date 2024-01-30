import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class UpdatePasswordModel {

    public constructor(obj?: Partial<UpdatePasswordModel>) {
        if (obj != undefined) {
            Object.assign(this, obj);
        }
    }

    @StrictlyTyped(String)
    public oldPassword!: string;

    @StrictlyTyped(String)
    public newPassword!: string;
}

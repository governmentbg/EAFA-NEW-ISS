import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class JwtToken {
    public constructor(obj?: Partial<JwtToken>) {
        if (obj != undefined) {
            Object.assign(this, obj);
        }
    }

    @StrictlyTyped(String)
    public tokenId!: string;

    @StrictlyTyped(String)
    public token!: string;

    @StrictlyTyped(Boolean)
    public isTempToken = false;

    @StrictlyTyped(Date)
    public validTo!: Date;
}

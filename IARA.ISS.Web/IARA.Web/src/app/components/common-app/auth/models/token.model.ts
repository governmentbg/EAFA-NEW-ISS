import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class TokenModel {

    public constructor(token: string) {
        this.token = token;
    }

    @StrictlyTyped(String)
    public token!: string;
}
import { LoginResultTypes } from '@app/components/common-app/auth/enums/login-result-types.enum';

export class LoginResult {

    public constructor(type: LoginResultTypes) {
        this.type = type;
        this.isTempToken = false;
    }

    public type!: LoginResultTypes;
    public lockedUntil?: Date;
    public isTempToken: boolean;
    public message?: string;
}
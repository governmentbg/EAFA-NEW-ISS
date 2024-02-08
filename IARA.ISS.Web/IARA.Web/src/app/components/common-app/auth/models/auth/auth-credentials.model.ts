export class AuthCredentials {
    public constructor(obj?: Partial<AuthCredentials>) {
        this.userName = "";
        this.password = "";
        this.rememberMe = false;
        this.revokeExistingToken = false;
        if (obj != undefined) {
            Object.assign(this, obj);
        }
    }

    public userName: string;
    public password: string;
    public revokeExistingToken: boolean;
    public rememberMe: boolean;
}

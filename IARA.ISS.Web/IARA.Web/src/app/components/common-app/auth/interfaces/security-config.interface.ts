export class SecurityConfig {

    public constructor(obj?: Partial<SecurityConfig>) {
        if (obj != undefined) {
            Object.assign(this, obj);
        }
    }

    public eauthRedirectMethodName: string = 'EAuthLogin';
    public authModulePath: string = '/account';
    public securityController: string = 'Security';
    public userController: string = 'User';
    public signInPageName: string = 'sign-in';
    public baseRoute: string = 'Common';
    public logoutMethodName: string = 'Logout';
    public loginMethodName: string = 'SignIn';
    public userMethodName: string = 'GetUser';
    public readTokenDataMethodName: string = 'ReadToken';
    public forgotPasswordMethodName: string = 'ForgotPassword';
    public changePasswordMethodName: string = 'ChangePassword';
    public passwordValidatorsMethodName: string = 'GetPasswordValidators';
    public checkEmailTokenMethodName: string = 'CheckTokenStatus';
}
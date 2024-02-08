import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { TLSnackbar } from '@app/shared/components/snackbar/tl.snackbar';
import { SECURITY_SERVICE_TOKEN, USER_SERVICE_TOKEN } from '../di/auth-di.tokens';
import { LoginResultTypes } from '../enums/login-result-types.enum';
import { ISecurityService } from '../interfaces/security-service.interface';
import { IGenericUserService } from '../interfaces/user-service.interface';
import { User } from '../models/auth/user.model';

@Component({
    selector: 'external-auth-redirect',
    template: '',
})
export class ExternalAuthRedirectComponent<TUserIdentifier, TUser extends User<TUserIdentifier>> implements OnInit {

    private route: ActivatedRoute;
    private securityService: ISecurityService;
    private router: Router;
    private snackbar: TLSnackbar;
    private userService: IGenericUserService<TUserIdentifier, TUser>;

    public constructor(route: ActivatedRoute,
        router: Router,
        @Inject(SECURITY_SERVICE_TOKEN) securityService: ISecurityService,
        @Inject(USER_SERVICE_TOKEN) userService: IGenericUserService<TUserIdentifier, TUser>,
        snackbar: TLSnackbar) {
        this.userService = userService;
        this.snackbar = snackbar;
        this.route = route;
        this.router = router;
        this.securityService = securityService;
    }

    ngOnInit(): void {
        this.route.queryParams.subscribe({
            next: (params: Params) => {
                const loginType: LoginResultTypes = this.getParamsValue(params, 'type') as LoginResultTypes;

                if (loginType == LoginResultTypes.Success) {

                    const token: string = params['token'] as string;

                    this.securityService.readTokenMeta(token, true).then(result => {
                        this.userService.getUser().subscribe({
                            next: (result) => {
                                const stateObject = { tokenProcessed: true, loginResult: result };
                                this.router.navigate(['/account/sign-in'], { state: stateObject });
                            },
                            error: (error) => {
                                this.securityService.logout().subscribe(() => {
                                    this.showLoginTypeError(loginType);
                                    this.router.navigate(['/']);
                                });
                            }
                        });
                    });
                } else {

                    const message: string = params['message'] as string;

                    if (message != null) {
                        this.snackbar.error(message);
                    } else {
                        this.showLoginTypeError(loginType);
                    }

                    this.router.navigate(['']);
                }
            }
        });
    }

    private showLoginTypeError(loginType: LoginResultTypes) {
        switch (loginType) {
            case LoginResultTypes.Fail:
                this.snackbar.errorResource('auth.failed-to-login');
                break;
            case LoginResultTypes.Locked:
                this.snackbar.errorResource('auth.user-locked');
                break;
            case LoginResultTypes.EmailNotConfirmed:
                this.snackbar.errorResource('auth.email-not-confirmed');
                break;
            case LoginResultTypes.Blocked:
                this.snackbar.errorResource('auth.user-blocked');
                break;
            case LoginResultTypes.OtherSessionExists:
                this.snackbar.errorResource('auth.other-session-exists');
                break;
            case LoginResultTypes.ForbiddenIP:
                this.snackbar.errorResource('auth.forbidden-ip-address');
                break;
            case LoginResultTypes.LoginTypeForbidden:
                this.snackbar.errorResource('auth.login-type-forbidden');
                break;
            case LoginResultTypes.UserMissingInDB:
                this.snackbar.errorResource('auth.user-missing-in-db');
                break;
            default:
                this.snackbar.errorResource('auth.failed-to-login');
                break;
        }
    }

    private getParamsValue(params: Params, key: string): number | undefined {
        if (Object.keys(params).find(x => x == key) != undefined) {
            return Number.parseInt(params[key]);
        } else {
            return undefined;
        }
    }

}
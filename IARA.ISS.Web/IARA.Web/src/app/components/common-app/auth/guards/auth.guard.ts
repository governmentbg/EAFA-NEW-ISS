import { Inject, Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateChild, CanLoad, Route, Router, RouterStateSnapshot, UrlSegment } from '@angular/router';
import { PERMISSIONS_SERVICE_TOKEN, SECURITY_CONFIG_TOKEN, SECURITY_SERVICE_TOKEN } from '@app/components/common-app/auth/di/auth-di.tokens';
import { IPermissionsService } from '@app/components/common-app/auth/interfaces/permissions-service.interface';
import { ISecurityService } from '@app/components/common-app/auth/interfaces/security-service.interface';
import { SecurityConfig } from '../interfaces/security-config.interface';

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate, CanActivateChild, CanLoad {
    protected securityService: ISecurityService;
    protected router: Router;
    protected permissionsService: IPermissionsService;
    protected securityConfig: SecurityConfig;

    /**
     * Constructor
     */
    public constructor(@Inject(SECURITY_SERVICE_TOKEN) securityService: ISecurityService,
        @Inject(PERMISSIONS_SERVICE_TOKEN) permissionsService: IPermissionsService,
        @Inject(SECURITY_CONFIG_TOKEN) securityConfig: SecurityConfig,
        router: Router) {
        this.securityConfig = securityConfig;
        this.permissionsService = permissionsService;
        this.securityService = securityService;
        this.router = router;
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Can activate
     *
     * @param route
     * @param state
     */
    public async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {

        let permissions: string[] | undefined = undefined;

        if (route.data != undefined && route.data.permissions != undefined && route.data.permissions.only != undefined) {
            permissions = route?.data?.permissions?.only;
        }

        const redirectUrl = state.url === `${this.securityConfig.authModulePath}/sign-out` ? '/' : state.url;
        const result: boolean = await this.check(redirectUrl) && await this.checkPermissions(permissions);
        return result;
    }

    /**
     * Can activate child
     *
     * @param childRoute
     * @param state
     */
    public async canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
        const redirectUrl = state.url === `${this.securityConfig.authModulePath}/sign-out` ? '/' : state.url;

        return await this.check(redirectUrl);
    }

    /**
     * Can load
     *
     * @param route
     * @param segments
     */
    public async canLoad(route: Route, segments: UrlSegment[]): Promise<boolean> {
        return await this.check('/');
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Private methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Check the authenticated status
     *
     * @param redirectURL
     * @private
     */
    private async check(redirectURL: string): Promise<boolean> {
        // Check the authentication status
        return await this.securityService.isAuthenticated().then(authenticated => {
            // If the user is not authenticated...
            if (!authenticated) {

                // Redirect to the sign-in page
                this.router.navigate([`${this.securityConfig.authModulePath}/auth-redirect`], { queryParams: { redirectURL } });

                // Prevent the access
                return false;
            }

            // Allow the access
            return true;
        });
    }

    private async checkPermissions(permissions: string[] | undefined): Promise<boolean> {
        if (permissions != undefined && permissions.length > 0) {
            const hasPermision = await this.permissionsService.hasAnyWait(...permissions);

            if (!hasPermision) {
                const path = await this.securityService.getUserRedirectPath();

                return this.router.navigate([path]).then(() => {
                    return false;
                });
            }

            return Promise.resolve(hasPermision);
        } else {
            return Promise.resolve(true);
        }
    }
}

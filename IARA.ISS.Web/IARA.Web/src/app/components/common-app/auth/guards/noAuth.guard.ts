import { Inject, Injectable } from '@angular/core';
import {
    ActivatedRouteSnapshot,
    CanActivate,
    CanActivateChild,
    CanLoad,
    Route,
    Router,
    RouterStateSnapshot,
    UrlSegment,
    UrlTree
} from '@angular/router';
import { SECURITY_CONFIG_TOKEN, SECURITY_SERVICE_TOKEN } from '@app/components/common-app/auth/di/auth-di.tokens';
import { ISecurityService } from '@app/components/common-app/auth/interfaces/security-service.interface';
import { Observable } from 'rxjs';
import { SecurityConfig } from '../interfaces/security-config.interface';

@Injectable({
    providedIn: 'root'
})
export class NoAuthGuard implements CanActivate, CanActivateChild, CanLoad {

    private securityConfig: SecurityConfig;
    private securityService: ISecurityService;
    private router: Router;
    /**
     * Constructor
     */
    public constructor(@Inject(SECURITY_SERVICE_TOKEN) securityService: ISecurityService, router: Router, @Inject(SECURITY_CONFIG_TOKEN) securityConfig: SecurityConfig) {
        this.securityService = securityService;
        this.router = router;
        this.securityConfig = securityConfig;
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
    public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {

        return this.check();
    }

    /**
     * Can activate child
     *
     * @param childRoute
     * @param state
     */
    public canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
        return this.check();
    }

    /**
     * Can load
     *
     * @param route
     * @param segments
     */
    public canLoad(route: Route, segments: UrlSegment[]): Observable<boolean> | Promise<boolean> | boolean {
        return this.check();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Private methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Check the authenticated status
     *
     * @private
     */
    private check(): Promise<boolean> {
        // Check the authentication status
        return this.securityService.isAuthenticated().then(authenticated => {
            // If the user is authenticated...
            if (authenticated) {
                // Redirect to the root
                this.router.navigate([`${this.securityConfig.authModulePath}/auth-redirect`], { skipLocationChange: true, relativeTo: null });

                // Prevent the access
                return false;
            }

            // Allow the access
            return true;
        });
    }
}

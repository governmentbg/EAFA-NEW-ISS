import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Observable } from 'rxjs';
import { map, take } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class AuthenticationGuard implements CanActivate {
    private router: Router;
    private oidcSecurityService: OidcSecurityService;

    constructor(router: Router, oidcSecurityService: OidcSecurityService) {
        this.router = router;
        this.oidcSecurityService = oidcSecurityService;
    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {
        return this.oidcSecurityService.isAuthenticated$.pipe(
            take(1),
            map(isAuthorized => {
                if (!isAuthorized) {
                    //this.oidcSecurityService.authorize();
                    //this.router.navigateByUrl("https://localhost:5050/Account/Login");
                    this.router.navigate(['/unauthorized']);
                }
                return isAuthorized;
            }));
    }
}
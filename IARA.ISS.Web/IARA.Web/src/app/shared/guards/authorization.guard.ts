import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { NgxPermissionsGuard, NgxPermissionsService, NgxRolesService } from 'ngx-permissions';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { AuthService } from '../services/auth.service';

@Injectable({
    providedIn: 'root'
})
export class AuthorizationGuard implements CanActivate {
    private permissionsGuard: NgxPermissionsGuard;
    private router: Router;
    private snackbar: MatSnackBar;
    private translate: FuseTranslationLoaderService;
    private authService: AuthService;

    constructor(
        ngxPermissionsService: NgxPermissionsService,
        ngxRolesService: NgxRolesService,
        authService: AuthService,
        router: Router,
        snackbar: MatSnackBar,
        translate: FuseTranslationLoaderService) {
        this.router = router;
        this.authService = authService;
        this.snackbar = snackbar;
        this.translate = translate;
        this.permissionsGuard = new NgxPermissionsGuard(ngxPermissionsService, ngxRolesService, router);
    }

    public async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
        if (await this.authService.checkAuthAndLogin()) {
            return this.checkPagePermissions(route, state).then((hasPermissions) => {
                if (hasPermissions) {
                    this.authService.openUserModal();
                }

                return hasPermissions;
            });
        } else {
            this.authService.redirectToHome();
            return false;
        }
    }

    private async checkPagePermissions(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
        const canActive: boolean = await this.canActivateHandler(route, state);

        if (canActive) {
            return true;
        } else {
            this.openNoAccessSnackbar();
            this.router.navigateByUrl('/redirect');
            return false;
        }
    }

    private async canActivateHandler(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
        return this.permissionsGuard.canActivate(route, state);
    }

    private openNoAccessSnackbar(): void {
        this.snackbar.open(this.translate.getValue('permissions.no-access'), undefined, {
            duration: 5000,
            panelClass: ['snack-bar-error-color']
        });
    }
}
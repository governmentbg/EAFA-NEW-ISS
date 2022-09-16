import { Injectable } from '@angular/core';
import { NgxPermissionsObject, NgxPermissionsService } from 'ngx-permissions';
import { NgxPermission } from 'ngx-permissions/lib/model/permission.model';
import { PermissionsEnum } from '../enums/permissions.enum';

@Injectable({
    providedIn: 'root'
})
export class PermissionsService {
    private ngxPermissionsService: NgxPermissionsService;

    public constructor(ngxPermissionsService: NgxPermissionsService) {
        this.ngxPermissionsService = ngxPermissionsService;
    }

    public has(permission: PermissionsEnum): boolean {
        const ngxPermission: NgxPermission = this.ngxPermissionsService.getPermission(permission);
        return ngxPermission !== null && ngxPermission !== undefined;
    }

    public hasAny(...permissions: PermissionsEnum[]): boolean {
        const allPermissions: string[] = this.getAllPermissions();
        for (const permission of permissions) {
            if (allPermissions.includes(permission)) {
                return true;
            }
        }
        return false;
    }

    public hasAll(...permissions: PermissionsEnum[]): boolean {
        const allPermissions: string[] = this.getAllPermissions();
        for (const permission of permissions) {
            if (!allPermissions.includes(permission)) {
                return false;
            }
        }
        return true;
    }

    public hasNone(...permissions: PermissionsEnum[]): boolean {
        return !this.hasAny(...permissions);
    }

    private getAllPermissions(): string[] {
        const ngxPermissions: NgxPermissionsObject = this.ngxPermissionsService.getPermissions();
        return Object.keys(ngxPermissions).map((key: string) => {
            return ngxPermissions[key].name;
        });
    }
}
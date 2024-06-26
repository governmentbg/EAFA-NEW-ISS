﻿import { FuseNavigation } from '@/@fuse/types';
import { Routes } from '@angular/router';
import { AuthGuard } from '@app/components/common-app/auth/guards/auth.guard';
import { IPermissionsService } from '@app/components/common-app/auth/interfaces/permissions-service.interface';
import { Navigation } from '../tl-navigation';
import { ITLNavigation } from './tl-navigation.interface';

export class MainNavigation {

    public static getRoutes(): Routes {
        const routes: Routes = [];
        this.buildRoutesFromNavigation(Navigation.Menu, routes);
        return routes;
    }

    public static async getFuseNavigation(permissionsService: IPermissionsService, isAuthenticated: boolean): Promise<FuseNavigation[]> {
        return await MainNavigation.getRecursiveFuseNavigation(Navigation.Menu, isAuthenticated, permissionsService) as FuseNavigation[];
    }

    private static buildRoutesFromNavigation(menu: ITLNavigation[], routes: Routes) {
        if (menu !== undefined) {
            for (const item of menu) {
                if (item.component !== undefined) {
                    let path: string = '';

                    if (item.url?.startsWith('/')) {
                        path = item.url.slice(1);
                    }
                    if (item.isPublic) {
                        routes.push({
                            path: path,
                            component: item.component,
                            data: {
                                translate: item.translate,
                                permissions: {
                                    only: item.permissions,
                                    except: item.exceptPermissions
                                }
                            }
                        });
                    } else {
                        routes.push({
                            path: path,
                            canActivate: [AuthGuard],
                            component: item.component,
                            data: {
                                translate: item.translate,
                                permissions: {
                                    only: item.permissions,
                                    except: item.exceptPermissions
                                }
                            }
                        });
                    }
                }

                if (item.children !== undefined) {
                    MainNavigation.buildRoutesFromNavigation(item.children, routes);
                }
            }
        }
    }

    private static async getRecursiveFuseNavigation(menu: ITLNavigation[], isAuthenticated: boolean, permissionsService: IPermissionsService): Promise<FuseNavigation[] | undefined> {
        let navigation: FuseNavigation[] | undefined = [];

        if (menu !== undefined) {
            navigation = [];
            for (const item of menu) {

                if ((!isAuthenticated && item.isPublic)
                    || (isAuthenticated && await MainNavigation.hasPermissions(item.permissions, permissionsService) && (!item.exceptPermissions || !(await MainNavigation.hasPermissions(item.exceptPermissions, permissionsService))))) {

                    let iconTypeCode: 'IC_ICON' | 'FA_ICON' | 'MAT_ICON';
                    if (item.icon?.startsWith('ic-')) {
                        iconTypeCode = 'IC_ICON';
                    }
                    else if (item.icon?.startsWith('fa-')) {
                        iconTypeCode = 'FA_ICON';
                    }
                    else {
                        iconTypeCode = 'MAT_ICON';
                    }

                    const fuseItem: FuseNavigation = {
                        id: item.id,
                        title: item.title,
                        type: item.type,
                        classes: item.classes,
                        icon: item.icon,
                        iconType: iconTypeCode,
                        url: item.url,
                        translate: item.translate,
                        permissions: item.permissions,
                        hidden: item.hideInMenu
                    };

                    if (item.children !== undefined && item.children.length > 0) {
                        fuseItem.children = await this.getRecursiveFuseNavigation(item.children, isAuthenticated, permissionsService);
                    }

                    if ((item.type == 'group' || item.type == 'collapsable') && fuseItem.children != undefined && fuseItem.children.length > 0) {
                        navigation.push(fuseItem);
                    } else if (item.type == 'item') {
                        navigation.push(fuseItem);
                    }
                }
            }
        }

        return navigation;
    }

    private static async hasPermissions(permissions: string[] | undefined, permissionsService: IPermissionsService): Promise<boolean> {

        let hasPermission: boolean = false;
        if (permissions != undefined && permissions != null && permissions.length > 0) {
            for (const permission of permissions) {
                const result: boolean = await permissionsService.has(permission);

                if (result) {
                    hasPermission = true;
                    break;
                }
            }
        } else {
            return true;
        }

        return hasPermission;
    }
}
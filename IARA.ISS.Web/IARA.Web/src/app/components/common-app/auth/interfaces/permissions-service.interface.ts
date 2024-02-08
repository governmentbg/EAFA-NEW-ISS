import { Subject } from "rxjs";

export interface IPermissionsService {
    has(permission: string): boolean;
    hasAny(...permissions: string[]): boolean;
    hasAnyWait(...permissions: string[]): Promise<boolean>;
    hasAll(...permissions: string[]): boolean;
    hasNone(...permissions: string[]): boolean;
    loadPermissions(permissions: string[]): void;
    permissionsLoaded: Subject<void>;
}
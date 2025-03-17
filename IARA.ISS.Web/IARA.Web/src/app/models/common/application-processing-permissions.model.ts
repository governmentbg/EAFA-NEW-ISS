import { PermissionsEnum } from "@app/shared/enums/permissions.enum";

export class ApplicationProcessingPermissions {
    public addPermission!: PermissionsEnum;
    public editPermission!: PermissionsEnum;
    public deletePermission!: PermissionsEnum;
    public restorePermission!: PermissionsEnum;
    public enterEventisNumberPermission!: PermissionsEnum;
    public cancelPermssion!: PermissionsEnum;
    public inspectAndCorrectPermssion!: PermissionsEnum;
    public processPaymentDataPermission!: PermissionsEnum;
    public checkDataRegularityPermission!: PermissionsEnum;
    public addAdministrativeActPermission!: PermissionsEnum;
    public downloadOnlineApplicationsPermission!: PermissionsEnum;
    public uploadOnlineApplicationsPermission!: PermissionsEnum;
    public readAdministrativeActPermission!: PermissionsEnum;
    public canReAssignApplicationsPermission!: PermissionsEnum;
    public canInspectCorrectAndAddAdmActPermission!: PermissionsEnum;
    public canReassingToDifferentTerritoryUnitPermission!: PermissionsEnum;

    constructor(obj?: Partial<ApplicationProcessingPermissions>) {
        Object.assign(this, obj);
    }
}
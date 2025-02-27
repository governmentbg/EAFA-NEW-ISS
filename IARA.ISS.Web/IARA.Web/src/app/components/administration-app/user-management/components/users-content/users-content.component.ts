import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IUserManagementService } from '@app/interfaces/administration-app/user-management.interface';
import { ExternalUserDTO } from '@app/models/generated/dtos/ExternalUserDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { InternalUserDTO } from '@app/models/generated/dtos/InternalUserDTO';
import { UserDTO } from '@app/models/generated/dtos/UserDTO';
import { UserManagementFilters } from '@app/models/generated/filters/UserManagementFilters';
import { RolesService } from '@app/services/administration-app/roles.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { EditAccessDialogComponent } from '../../internal-users/components/edit-access-device-mobile-dialog/edit-access-dialog.component';
import { EditAccessDialogParams } from '../models/edit-access-dialog-params';
import { EditUserDialogParams } from '../models/edit-user-dialog-params';
import { EditUserComponent } from './edit-users/edit-user.component';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { InternalUserManagementService } from '@app/services/administration-app/user-management/internal-user-management.service';

@Component({
    selector: 'users-content',
    templateUrl: './users-content.component.html',
    styleUrls: ['./users-content.component.scss']
})
export class UsersContentComponent implements OnInit, AfterViewInit {
    @Input() public service!: IUserManagementService;
    @Input() public isInternalUser!: boolean;

    public translationService: FuseTranslationLoaderService;
    public permissions: PermissionsService;
    public confirmDialog: TLConfirmDialog;
    public userFormGroup: FormGroup;
    public roles!: NomenclatureDTO<number>[];
    public showInactiveUsersLabel!: string;

    public canAddRecords!: boolean;
    public canEditRecords!: boolean;
    public canDeleteRecords!: boolean;
    public canRestoreRecords!: boolean;
    public canAddMobileDevices!: boolean;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private gridManager!: DataTableManager<any, UserManagementFilters>;
    private editDialog: TLMatDialog<EditUserComponent>;
    private editAccessDialog: TLMatDialog<EditAccessDialogComponent>;
    private rolesService: RolesService;

    public constructor(
        translationService: FuseTranslationLoaderService,
        rolesService: RolesService,
        confirm: TLConfirmDialog,
        editDialog: TLMatDialog<EditUserComponent>,
        editAccessDialog: TLMatDialog<EditAccessDialogComponent>,
        permissions: PermissionsService
    ) {
        this.translationService = translationService;
        this.rolesService = rolesService;
        this.confirmDialog = confirm;
        this.editDialog = editDialog;
        this.editAccessDialog = editAccessDialog;
        this.permissions = permissions;

        this.userFormGroup = new FormGroup({
            firstNameControl: new FormControl(),
            lastNameControl: new FormControl(),
            roleControl: new FormControl(),
            requestedAccessControl: new FormControl(),
            emailControl: new FormControl(),
            registeredDateRangeControl: new FormControl()
        });
    }

    public ngOnInit(): void {
        this.showInactiveUsersLabel = this.translationService.getValue('users-page.show-inactive-users-label');

        if (this.isInternalUser) {
            this.rolesService.getAllActiveRoles().subscribe(result => {
                if (result) {

                    this.roles = result;
                }
            });

            this.canAddRecords = this.permissions.has(PermissionsEnum.InternalUsersAddRecords);
            this.canEditRecords = this.permissions.has(PermissionsEnum.InternalUsersEditRecords);
            this.canDeleteRecords = this.permissions.has(PermissionsEnum.InternalUsersDeleteRecords);
            this.canRestoreRecords = this.permissions.has(PermissionsEnum.InternalUsersRestoreRecords);
            this.canAddMobileDevices = this.permissions.has(PermissionsEnum.InternalUsersAddMobileDevices);
        }
        else {
            this.rolesService.getPublicActiveRoles().subscribe(result => {
                if (result) {
                    this.roles = result;
                }
            });

            this.canEditRecords = this.permissions.has(PermissionsEnum.ExternalUsersEditRecords);
            this.canDeleteRecords = this.permissions.has(PermissionsEnum.ExternalUsersDeleteRecords);
            this.canRestoreRecords = this.permissions.has(PermissionsEnum.ExternalUsersRestoreRecords);
        }
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<any, UserManagementFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAll.bind(this.service),
            filtersMapper: this.mapFilters
        });

        const personId: number | undefined = window.history.state?.id;
        const legalId: number | undefined = window.history.state?.id;
        const tableId: number | undefined = window.history.state.tableId;

        if (!CommonUtils.isNullOrEmpty(personId)) {
            this.gridManager.advancedFilters = new UserManagementFilters({ personId: personId });
        }

        if (!CommonUtils.isNullOrEmpty(legalId)) {
            this.gridManager.advancedFilters = new UserManagementFilters({ legalId: legalId });
        }

        if (!CommonUtils.isNullOrEmpty(tableId)) {
            this.gridManager.advancedFilters = new UserManagementFilters({ id: tableId });
        }

        this.gridManager.refreshData();
    }

    public deactivateUser(user: UserDTO): void {
        this.confirmDialog.open({
            title: this.translationService.getValue('users-page.deactivate-user'),
            message: `${this.translationService.getValue('users-page.are-you-sure-you-want-do-deactivate')} ${user.firstName} ${user.lastName} (${user.email})?`,
            okBtnLabel: this.translationService.getValue('users-page.deactivate')
        }).subscribe(result => {
            if (result) {
                if (user.id !== undefined) {
                    this.service.delete(user.id).subscribe({
                        next: () => {
                            this.gridManager.deleteRecord(user);
                        }
                    });
                }
            }
        });
    }

    public restoreUser(user: UserDTO): void {
        this.confirmDialog.open().subscribe(result => {
            if (result) {
                if (user.id !== undefined) {
                    this.service.undoDelete(user.id).subscribe({
                        next: () => {
                            this.gridManager.undoDeleteRecord(user);
                        }
                    });
                }
            }
        });
    }

    public createEditUser(id?: number, readOnly?: boolean): void {
        let data: EditUserDialogParams | undefined;
        let headerAuditBtn: IHeaderAuditButton | undefined;
        let headerTitle: string = '';
        const rightButtons: IActionInfo[] = [];

        if (id !== undefined) {

            data = new EditUserDialogParams(id, this.service, this.isInternalUser, false, readOnly);

            headerAuditBtn = {
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                id: id,
                tableName: 'UsrMgmt.Users'
            } as IHeaderAuditButton;

            headerTitle = this.isInternalUser ?
                this.translationService.getValue('users-page.edit-internal-user-dialog-title') :
                this.translationService.getValue('users-page.edit-external-user-dialog-title');

            if (!this.isInternalUser) {
                rightButtons.push({
                    id: 'change-user-status',
                    color: 'warn',
                    translateValue: this.translationService.getValue('users-page.change-user-status')
                });
            }

            rightButtons.push({
                id: 'send-msg-for-password-change',
                color: 'warn',
                translateValue: this.translationService.getValue('users-page.send-pass-reset-msg')
            });

            if (this.permissions.has(PermissionsEnum.ImpersonateUser)) {
                rightButtons.push({
                    id: 'impersonate-user',
                    color: 'warn',
                    translateValue: this.translationService.getValue('users-page.impersonate-user')
                });
            }
        }
        else if (id === undefined && this.isInternalUser) {
            headerTitle = this.translationService.getValue('users-page.add-internal-user-dialog-title');
            data = new EditUserDialogParams(-1, this.service, this.isInternalUser, true, readOnly);
        }

        const dialogResult = this.editDialog.open({
            title: headerTitle,
            TCtor: EditUserComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translationService,
            disableDialogClose: true,
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translationService.getValue('common.save')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translationService.getValue('common.cancel'),
            },
            rightSideActionsCollection: rightButtons
        });

        dialogResult.subscribe(result => {
            if (result !== undefined) {
                let user;
                if (this.isInternalUser) {
                    user = result as InternalUserDTO;
                } else {
                    user = result as ExternalUserDTO;
                }

                if (id === undefined) {
                    this.gridManager.addRecord(user);
                } else {
                    this.gridManager.editRecord(user);
                }
            }
        });
    }

    public showUserInfo(id?: number): void {
        let data: EditUserDialogParams | undefined;
        let headerAuditBtn: IHeaderAuditButton | undefined;

        if (id !== undefined) {
            data = new EditUserDialogParams(id, this.service, this.isInternalUser, false, true);
            headerAuditBtn = {
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                id: id,
                tableName: 'UsrMgmt.Users'
            } as IHeaderAuditButton;
        }

        this.editDialog.open({
            title: this.translationService.getValue('users-page.show-info-tooltip'),
            TCtor: EditUserComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translationService,
            viewMode: true
        });
    }

    public accessFromDeviceToMobileForUser(user: UserDTO): void {
        let data: EditAccessDialogParams | undefined;
        let headerAuditBtn: IHeaderAuditButton | undefined;

        if (user.id !== undefined) {
            const userFullName: string = `${user.firstName !== undefined ? user.firstName : ''} 
                                          ${user.middleName !== undefined && user.middleName !== null ? user.middleName : ''} 
                                          ${user.lastName !== undefined ? user.lastName : ''}`;
            const userRole: string = user.userRoles !== undefined ? user.userRoles : '';
            const matCardTitleLabel: string = `${this.translationService.getValue('users-page.devices')} ${userFullName} (${userRole})`;
            data = new EditAccessDialogParams(user.id, this.service, matCardTitleLabel, userFullName, false);
            headerAuditBtn = {
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                id: user.id,
                tableName: 'UsrMgmt.Users'
            } as IHeaderAuditButton;
        }

        this.editAccessDialog.openWithTwoButtons({
            title: this.translationService.getValue('users-page.mobile-access-dialog-title'),
            TCtor: EditAccessDialogComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translationService,
        }).subscribe({
            next: () => {
                this.gridManager.editRecord(user);
            }
        });
    }

    public reloadAllMobileDevicesAppDatabase(): void {
        if (this.isInternalUser && this.canAddMobileDevices) {
            this.confirmDialog.open({
                title: this.translationService.getValue('users-page.reload-all-user-mobile-devices-database-confirm-dialog-title'),
                message: this.translationService.getValue('users-page.all-users-mobile-devices-must-reload-local-database'),
                okBtnLabel: this.translationService.getValue('users-page.all-users-must-reload-database-ok-btn-label'),
                cancelBtnLabel: this.translationService.getValue('users-page.all-users-must-reload-database-cancel-btn-label')
            }).subscribe((yes: boolean) => {
                if (yes) {
                    (this.service as InternalUserManagementService).reloadAllMobileDevicesAppDatabase().subscribe({
                        next: () => {
                            // nothing to do
                        }
                    });
                }
            });
        }
    }

    private mapFilters(filters: FilterEventArgs): UserManagementFilters {
        const result = new UserManagementFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords
        });

        result.firstName = filters.getValue('firstNameControl');
        result.lastName = filters.getValue('lastNameControl');
        result.roleId = filters.getValue('roleControl');
        result.email = filters.getValue('emailControl');
        result.registeredDateFrom = filters.getValue<DateRangeData>('registeredDateRangeControl')?.start;
        result.registeredDateTo = filters.getValue<DateRangeData>('registeredDateRangeControl')?.end;
        result.isRequestedAccess = filters.getValue('requestedAccessControl');

        return result;
    }

    private closeDialogBtnClicked(closeFn: () => void): void {
        closeFn();
    }
}
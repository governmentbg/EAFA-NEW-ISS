import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { UserLegalStatusEnum } from '@app/enums/user-legal-status.enum';
import { IUserManagementService } from '@app/interfaces/administration-app/user-management.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ExternalUserDTO } from '@app/models/generated/dtos/ExternalUserDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { InternalUserDTO } from '@app/models/generated/dtos/InternalUserDTO';
import { RegixLegalDataDTO } from '@app/models/generated/dtos/RegixLegalDataDTO';
import { RoleDTO } from '@app/models/generated/dtos/RoleDTO';
import { UserLegalDTO } from '@app/models/generated/dtos/UserLegalDTO';
import { RolesService } from '@app/services/administration-app/roles.service';
import { ExternalUserManagementService } from '@app/services/administration-app/user-management/external-user-management.service';
import { InternalUserManagementService } from '@app/services/administration-app/user-management/internal-user-management.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { AuthService } from '@app/shared/services/auth.service';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { RequestProperties } from '@app/shared/services/request-properties';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { forkJoin } from 'rxjs';
import { CommandTypes } from '@app/shared/components/data-table/enums/command-type.enum';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { EditUserDialogParams } from '../../models/edit-user-dialog-params';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';

@Component({
    selector: 'edit-user',
    templateUrl: './edit-user.component.html',
})
export class EditUserComponent implements OnInit, IDialogComponent {
    @ViewChild('roleDataTable')
    public roleDataTable!: TLDataTableComponent;

    @ViewChild('legalDataTable')
    public legalDataTable!: TLDataTableComponent;

    public territoryUnits: NomenclatureDTO<number>[] = [];
    public sectors: NomenclatureDTO<number>[] = [];
    public departments: NomenclatureDTO<number>[] = [];
    public legals: NomenclatureDTO<number>[] = [];
    public roles: NomenclatureDTO<number>[] = [];
    public userRoles: RoleDTO[] = [];
    public userLegals: UserLegalDTO[] = [];
    public editUserForm!: FormGroup;
    public userRoleForm!: FormGroup;
    public userLegalForm!: FormGroup;
    public translationService: FuseTranslationLoaderService;
    public isInternalUser!: boolean;
    public readOnly!: boolean;
    public userId!: number;
    public mobileDevicesMatCardTitleLabel!: string;
    public userFullName!: string;
    public legalStatusEnum: typeof UserLegalStatusEnum = UserLegalStatusEnum;

    public canRestoreRecords!: boolean;

    private service!: IUserManagementService;
    private rolesService: RolesService;
    private permissions: PermissionsService;
    private nomenclatureService: CommonNomenclatures;
    private confirmDialog: TLConfirmDialog;
    private internalUserModel!: InternalUserDTO;
    private externalUserModel!: ExternalUserDTO;
    private snackbar: MatSnackBar;
    private authService: AuthService;

    constructor(translationService: FuseTranslationLoaderService,
        rolesService: RolesService,
        nomenclatureService: CommonNomenclatures,
        confirmDialog: TLConfirmDialog,
        permissions: PermissionsService,
        snackbar: MatSnackBar,
        authService: AuthService) {
        this.translationService = translationService;
        this.rolesService = rolesService;
        this.nomenclatureService = nomenclatureService;
        this.confirmDialog = confirmDialog;
        this.permissions = permissions;
        this.snackbar = snackbar;
        this.authService = authService;

        this.mobileDevicesMatCardTitleLabel = this.translationService.getValue('users-page.mobile-devices-mat-card-title-label');
    }

    public async ngOnInit(): Promise<void> {
        await this.getNomenclatures();

        if (this.readOnly) {
            this.editUserForm.get('territorialUnitControl')?.disable();
            this.editUserForm.get('departmentControl')?.disable();
            this.editUserForm.get('sectorControl')?.disable();
        }

        if (this.isInternalUser) {
            this.canRestoreRecords = this.permissions.has(PermissionsEnum.InternalUsersRestoreRecords);

            if (this.userId !== -1 && this.userId !== undefined) {
                this.service.getUser(this.userId).subscribe(result => {
                    this.internalUserModel = result;
                    this.fillForm(this.internalUserModel);
                    this.userFullName = `${this.internalUserModel.firstName} ${this.internalUserModel.middleName !== undefined && this.internalUserModel.middleName !== null ? this.internalUserModel.middleName : ''} ${this.internalUserModel.lastName}`;
                });
            }
        } else {
            if (this.userId !== -1 && this.userId !== undefined) {
                this.service.getUser(this.userId).subscribe(result => {
                    this.externalUserModel = result;
                    this.fillForm(this.externalUserModel);
                });
            }
        }
    }

    public setData(data: EditUserDialogParams, buttons: DialogWrapperData): void {
        this.service = data.service;
        this.isInternalUser = data.isInternalUser;
        this.readOnly = data.isReadonly;

        this.buildForm();

        if (this.isInternalUser) {
            if (data.isAddUser) {
                this.internalUserModel = new InternalUserDTO();
            } else {
                const params = (data as DialogParamsModel);
                this.userId = params.id;

                if (params.isReadonly) {
                    this.editUserForm.disable();
                }

                this.editUserForm.get('egnControl')?.disable();
            }
        } else {
            if (data.isAddUser) {
                this.externalUserModel = new ExternalUserDTO();
            } else {
                const params = (data as DialogParamsModel);
                this.userId = params.id;

                if (params.isReadonly) {
                    this.editUserForm.disable();
                }

                this.editUserForm.get('egnControl')?.disable();
            }
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: (dialogResult?: any) => void): void {
        this.editUserForm.markAllAsTouched();
        if (this.editUserForm.valid || this.readOnly) {
            if (actionInfo.id === 'deactivate') {
                this.deactivateUser(dialogClose);
            } else if (actionInfo.id === 'send-msg-for-password-change') {
                if (this.userId !== undefined) {
                    this.service.sendChangePasswordEmail(this.userId).subscribe(result => {
                        if (result !== null) {
                            const message: string = this.translationService.getValue('users-page.change-password-email-sent');
                            this.snackbar.open(message, undefined, {
                                duration: RequestProperties.DEFAULT.showExceptionDurationSucc,
                                panelClass: RequestProperties.DEFAULT.showExceptionColorClassSucc
                            });
                        }
                    });
                }
            } else if (actionInfo.id === 'impersonate-user') {
                if (this.userId !== undefined) {
                    this.service.impersonateUser(this.userId).subscribe(result => {

                        this.authService.ImpersonationToken = result;
                        const message: string = this.translationService.getValue('service.successful-message');
                        this.snackbar.open(message, undefined, {
                            duration: RequestProperties.DEFAULT.showExceptionDurationSucc,
                            panelClass: RequestProperties.DEFAULT.showExceptionColorClassSucc
                        });

                        window.location.href = '/';
                    });
                }
            } else if (actionInfo.id === 'change-user-status') {
                this.confirmDialog.open({
                    title: this.translationService.getValue('users-page.change-user-status'),
                    message: this.translationService.getValue('users-page.change-user-message'),
                    okBtnLabel: this.translationService.getValue('users-page.yes'),
                    cancelBtnLabel: this.translationService.getValue('users-page.change-user-cancel-btn-label')
                }).subscribe({
                    next: (ok: boolean) => {
                        if (ok) {
                            (this.service as ExternalUserManagementService).changeUserStatus(this.userId).subscribe(result => {
                                dialogClose(this.externalUserModel);
                            });
                        }
                    }
                });
            }
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: (dialogResult?: any) => void): void {
        if (actionInfo.id === 'save') {
            this.editUserForm.markAllAsTouched();

            if (this.editUserForm.valid) {
                this.fillModel(this.editUserForm);

                if (this.userId !== undefined) {
                    if (this.isInternalUser) {
                        (this.service as InternalUserManagementService).edit(this.internalUserModel).subscribe(result => {
                            dialogClose(this.internalUserModel);
                        });
                    } else {
                        (this.service as ExternalUserManagementService).edit(this.externalUserModel).subscribe(result => {
                            dialogClose(this.externalUserModel);
                        });
                    }
                } else {
                    if (this.isInternalUser) {
                        (this.service as InternalUserManagementService).add(this.internalUserModel).subscribe(result => {
                            dialogClose(this.internalUserModel);
                        });
                    }
                }
            }

            if (this.readOnly) {
                dialogClose(null);
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: (dialogResult?: any) => void): void {
        dialogClose();
    }

    public deactivateUser(dialogClose: (dialogResult?: any) => void): void {
        this.confirmDialog.open({
            title: this.translationService.getValue('users-page.deactivate-user'),
            message: this.isInternalUser
                ? `${this.translationService.getValue('users-page.are-you-sure-you-want-do-deactivate')} ${this.internalUserModel.firstName} ${this.internalUserModel.lastName} (${this.internalUserModel.username})?`
                : `${this.translationService.getValue('users-page.are-you-sure-you-want-do-deactivate')} ${this.externalUserModel.firstName} ${this.externalUserModel.lastName} (${this.externalUserModel.username})?`,
            okBtnLabel: this.translationService.getValue('users-page.deactivate')
        }).subscribe(result => {
            if (result) {
                if (this.userId !== undefined) {
                    this.service.delete(this.userId).subscribe({
                        next: () => {
                            if (this.isInternalUser) {
                                dialogClose(this.internalUserModel);
                            } else {
                                dialogClose(this.externalUserModel);
                            }
                        }
                    });
                }
            }
        });
    }

    public allowLegal(id: number): void {
        const legal: UserLegalDTO | undefined = this.userLegals.find(x => x.legalId === id);
        if (legal !== undefined) {
            legal.status = UserLegalStatusEnum.Approved;
            this.userLegals = this.userLegals.slice();
        }
    }

    public denyLegal(id: number): void {
        const legal: UserLegalDTO | undefined = this.userLegals.find(x => x.legalId === id);
        if (legal !== undefined) {
            legal.status = UserLegalStatusEnum.Blocked;
            this.userLegals = this.userLegals.slice();
        }
    }

    public deactivateUserLegal(legal: GridRow<RegixLegalDataDTO>): void {
        this.confirmDialog.open({
            title: this.translationService.getValue('users-page.deactivate-user-legal'),
            message: this.translationService.getValue('users-page.are-you-sure-you-want-do-deactivate-legal'),
            okBtnLabel: this.translationService.getValue('users-page.deactivate-legal-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.legalDataTable.softDelete(legal);
                }
            }
        });
    }

    public restoreUserLegal(legal: GridRow<RegixLegalDataDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.legalDataTable.softUndoDelete(legal);
                }
            }
        });
    }

    public userRoleChanged(recordChangedEvent: RecordChangedEventArgs<RoleDTO>): void {
        if (recordChangedEvent.Command === CommandTypes.Add || recordChangedEvent.Command === CommandTypes.Edit) {
            if (recordChangedEvent.Record.accessValidFrom === null
                || recordChangedEvent.Record.accessValidFrom === undefined) {
                recordChangedEvent.Record.accessValidFrom = new Date();
            }
            if (recordChangedEvent.Record.accessValidTo === null
                || recordChangedEvent.Record.accessValidTo === undefined) {
                recordChangedEvent.Record.accessValidTo = new Date(9999, 0, 1);
            }
        }
    }

    public userLegalsChanged(recordChangedEvent: RecordChangedEventArgs<UserLegalDTO>): void {
        if (recordChangedEvent.Command === CommandTypes.Add) {
            if (recordChangedEvent.Record.status === null
                || recordChangedEvent.Record.status === undefined) {
                recordChangedEvent.Record.status = UserLegalStatusEnum.Approved;
            }
        }
    }

    public getDatesOverlappingErrorText(controlName: string, error: Record<string, unknown>, errorCode: string): TLError | undefined {
        if (controlName === 'idControl') {
            if (errorCode === 'datesOverlap') {
                return new TLError({
                    text: 'fuheihfi', //this.translate.getValue('roles-register.dates-overlap-with-other-record'),
                    type: 'error'
                });
            }
        }
        return undefined;
    }

    private fillForm(model: InternalUserDTO | ExternalUserDTO): void {
        this.editUserForm.get('firstNameControl')?.setValue(model.firstName);
        this.editUserForm.get('middleNameControl')?.setValue(model.middleName);
        this.editUserForm.get('lastNameControl')?.setValue(model.lastName);
        this.editUserForm.get('emailControl')?.setValue(model.email);
        this.editUserForm.get('usernameControl')?.setValue(model.username);
        this.editUserForm.get('egnControl')?.setValue(model.egnLnc);
        this.editUserForm.get('phoneNumberControl')?.setValue(model.phone);
        this.editUserForm.get('userMustChangePasswordControl')?.setValue(model.userMustChangePassword);
        this.editUserForm.get('isLockedControl')?.setValue(model.isLocked);
        this.editUserForm.get('positionControl')?.setValue(model.position);

        this.userRoles = model.userRoles as RoleDTO[];

        if (this.isInternalUser) {
            const territoryUnit = NomenclatureStore.instance.getNomenclatureItem(NomenclatureTypes.TerritoryUnits, (model as InternalUserDTO).territoryUnitId as number);
            this.editUserForm.controls.territorialUnitControl.setValue(territoryUnit);

            const department = NomenclatureStore.instance.getNomenclatureItem(NomenclatureTypes.Departments, (model as InternalUserDTO).departmentId as number);
            this.editUserForm.controls.departmentControl.setValue(department);

            const sector = NomenclatureStore.instance.getNomenclatureItem(NomenclatureTypes.Sectors, (model as InternalUserDTO).sectorId as number);
            this.editUserForm.controls.sectorControl.setValue(sector);

            setTimeout(() => {
                if ((model as InternalUserDTO).mobileDevices !== undefined && (model as InternalUserDTO).mobileDevices !== null) {
                    this.editUserForm.controls.mobileDevicesControl.setValue((model as InternalUserDTO).mobileDevices);
                }
            });
        } else {
            const externalUser = (model as ExternalUserDTO);
            if (externalUser.territoryUnitId !== null && externalUser.territoryUnitId !== undefined) {

                const territoryUnit = NomenclatureStore.instance.getNomenclatureItem(NomenclatureTypes.TerritoryUnits, externalUser.territoryUnitId as number);
                this.editUserForm.controls.territorialUnitControl.setValue(territoryUnit);

                const department = NomenclatureStore.instance.getNomenclatureItem(NomenclatureTypes.Departments, externalUser.departmentId as number);
                this.editUserForm.controls.departmentControl.setValue(department);

                const sector = NomenclatureStore.instance.getNomenclatureItem(NomenclatureTypes.Sectors, externalUser.sectorId as number);
                this.editUserForm.controls.sectorControl.setValue(sector);
            }
            if (externalUser !== undefined && externalUser.userLegals !== undefined && externalUser.userLegals !== null) {
                this.userLegals = (model as ExternalUserDTO).userLegals as UserLegalDTO[];
            }
        }
    }

    private fillModel(form: FormGroup): void {
        if (this.isInternalUser) {
            this.internalUserModel.firstName = form.controls.firstNameControl.value;
            this.internalUserModel.middleName = form.controls.middleNameControl.value;
            this.internalUserModel.lastName = form.controls.lastNameControl.value;
            this.internalUserModel.email = form.controls.emailControl.value;
            this.internalUserModel.username = form.controls.usernameControl.value;
            this.internalUserModel.egnLnc = form.controls.egnControl.value;
            this.internalUserModel.phone = form.controls.phoneNumberControl.value;
            this.internalUserModel.userMustChangePassword = form.controls.userMustChangePasswordControl.value;
            this.internalUserModel.isLocked = form.controls.isLockedControl.value ?? false;
            this.internalUserModel.position = form.controls.positionControl.value;

            this.internalUserModel.departmentId = NomenclatureStore.getValue(form.controls.departmentControl.value);
            this.internalUserModel.sectorId = NomenclatureStore.getValue(form.controls.sectorControl.value);
            this.internalUserModel.territoryUnitId = NomenclatureStore.getValue(form.controls.territorialUnitControl.value);

            if (this.userRoleForm.valid && this.roleDataTable.rows.length !== 0) {
                this.internalUserModel.userRoles = this.roleDataTable.rows as RoleDTO[];
            }

            this.internalUserModel.mobileDevices = this.editUserForm.controls.mobileDevicesControl.value;
        } else {
            this.externalUserModel.firstName = form.controls.firstNameControl.value;
            this.externalUserModel.middleName = form.controls.middleNameControl.value;
            this.externalUserModel.lastName = form.controls.lastNameControl.value;
            this.externalUserModel.email = form.controls.emailControl.value;
            this.externalUserModel.username = form.controls.usernameControl.value;
            this.externalUserModel.egnLnc = form.controls.egnControl.value;
            this.externalUserModel.phone = form.controls.phoneNumberControl.value;
            this.externalUserModel.userMustChangePassword = form.controls.userMustChangePasswordControl.value;
            this.externalUserModel.isLocked = form.controls.isLockedControl.value ?? false;
            this.externalUserModel.position = form.controls.positionControl.value;

            this.externalUserModel.departmentId = NomenclatureStore.getValue(form.controls.departmentControl.value);
            this.externalUserModel.sectorId = NomenclatureStore.getValue(form.controls.sectorControl.value);
            this.externalUserModel.territoryUnitId = NomenclatureStore.getValue(form.controls.territorialUnitControl.value);

            if (this.userRoleForm.valid && this.roleDataTable.rows.length !== 0) {
                this.externalUserModel.userRoles = this.roleDataTable.rows as RoleDTO[];
            }

            if (this.userLegalForm.valid && this.legalDataTable.rows.length !== 0) {

                this.externalUserModel.userLegals = (this.legalDataTable.rows as UserLegalDTO[]).map(x => {
                    const legal = this.legals.find(y => y.value == x.legalId);
                    const role = this.roles.find(y => y.value === x.roleId);
                    return new UserLegalDTO({
                        legalId: legal?.value,
                        name: legal?.displayName?.split(' - ')[0],
                        eik: legal?.displayName?.split(' - ')[1],
                        roleId: x.roleId,
                        role: role?.displayName,
                        status: x.status,
                        isActive: x.isActive

                    });
                });
            }
        }
    }

    private buildForm(): void {
        this.editUserForm = new FormGroup({
            firstNameControl: new FormControl(null, [Validators.required, Validators.maxLength(200)]),
            middleNameControl: new FormControl(null, Validators.maxLength(200)),
            lastNameControl: new FormControl(null, [Validators.required, Validators.maxLength(200)]),
            egnControl: new FormControl(null, [Validators.required, Validators.maxLength(20)]),
            usernameControl: new FormControl(null, [Validators.required, Validators.maxLength(100)]),
            emailControl: new FormControl(null, [Validators.email, Validators.maxLength(200)]),
            phoneNumberControl: new FormControl(null, Validators.maxLength(50)),
            userMustChangePasswordControl: new FormControl(false),
            isLockedControl: new FormControl(),
            territorialUnitControl: new FormControl(),
            departmentControl: new FormControl(),
            sectorControl: new FormControl(),
            positionControl: new FormControl()
        });

        this.userRoleForm = new FormGroup({
            idControl: new FormControl(null, [Validators.required, this.datesOverlappingValidator()]),
            accessValidToControl: new FormControl(),
            accessValidFromControl: new FormControl()
        });

        this.editUserForm.controls.sectorControl.disable();
        this.editUserForm.controls.departmentControl.disable();

        if (this.isInternalUser) {
            this.editUserForm.controls.territorialUnitControl.setValidators(Validators.required);
            this.editUserForm.addControl('mobileDevicesControl', new FormControl());
        } else {
            this.userLegalForm = new FormGroup({
                legalIdControl: new FormControl(null, Validators.required),
                roleIdControl: new FormControl(null, Validators.required)
            });
        }
    }

    private async getNomenclatures() {
        const results = await forkJoin({
            territoryUnits: NomenclatureStore.instance.getNomenclature<number>(NomenclatureTypes.TerritoryUnits, this.nomenclatureService.getTerritoryUnits.bind(this.nomenclatureService), false),
            sectors: NomenclatureStore.instance.getNomenclature<number>(NomenclatureTypes.Sectors, this.nomenclatureService.getSectors.bind(this.nomenclatureService), false),
            departments: NomenclatureStore.instance.getNomenclature<number>(NomenclatureTypes.Departments, this.nomenclatureService.getDepartments.bind(this.nomenclatureService), false),
        }).toPromise();
        this.territoryUnits = results.territoryUnits;
        this.sectors = results.sectors;
        this.departments = results.departments;

        if (this.isInternalUser) {
            this.roles = await (await this.rolesService.getAllActiveRoles().toPromise()).slice();
        }
        else {
            this.legals = await NomenclatureStore.instance.getNomenclature<number>(NomenclatureTypes.Legals, (this.service as ExternalUserManagementService).getActiveLegals.bind(this.service), false).toPromise();
            this.roles = await this.rolesService.getPublicActiveRoles().toPromise();
        }
    }

    private datesOverlappingValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const max = (lhs: Date, rhs: Date) => lhs > rhs ? lhs : rhs;
            const min = (lhs: Date, rhs: Date) => lhs < rhs ? lhs : rhs;

            if (this.roleDataTable) {
                const id: number | undefined = this.userRoleForm.get('idControl')!.value;
                const validFrom: Date | undefined = this.userRoleForm.get('accessValidFromControl')!.value;
                const validTo: Date | undefined = this.userRoleForm.get('accessValidToControl')!.value;

                if (id !== undefined && validFrom !== undefined && validTo !== undefined) {
                    const otherEntries: RoleDTO[] = (this.roleDataTable.rows as RoleDTO[]).filter(
                        x => x.id === id && x.isActive !== false && x.accessValidFrom !== validFrom && x.accessValidTo !== validTo
                    );

                    for (const otherEntry of otherEntries) {
                        if (max(validFrom, otherEntry.accessValidFrom!) < min(validTo, otherEntry.accessValidTo!)) {
                            return { 'datesOverlap': true };
                        }
                    }
                }
            }
            return null;
        };
    }
}
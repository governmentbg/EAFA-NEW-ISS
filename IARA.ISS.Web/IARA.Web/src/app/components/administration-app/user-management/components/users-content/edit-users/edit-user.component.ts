import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { forkJoin } from 'rxjs';
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
import { CommandTypes } from '@app/shared/components/data-table/enums/command-type.enum';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { EditUserDialogParams } from '../../models/edit-user-dialog-params';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { EgnLncDTO } from '@app/models/generated/dtos/EgnLncDTO';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';

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
    public userId: number | undefined;
    public mobileDevicesMatCardTitleLabel!: string;
    public userFullName!: string;
    public legalStatusEnum: typeof UserLegalStatusEnum = UserLegalStatusEnum;

    public canRestoreRecords!: boolean;
    public isAdd: boolean = false;

    public getControlErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelText.bind(this);

    private service!: IUserManagementService;
    private rolesService: RolesService;
    private permissions: PermissionsService;
    private nomenclatureService: CommonNomenclatures;
    private confirmDialog: TLConfirmDialog;
    private internalUserModel!: InternalUserDTO;
    private externalUserModel!: ExternalUserDTO;
    private snackbar: MatSnackBar;
    private authService: AuthService;

    private hasEmailExistsError: boolean = false;
    private hasInvalidEgnLnchError: boolean = false;

    private lastEgnLnc: EgnLncDTO | undefined;

    public constructor(
        translationService: FuseTranslationLoaderService,
        rolesService: RolesService,
        nomenclatureService: CommonNomenclatures,
        confirmDialog: TLConfirmDialog,
        permissions: PermissionsService,
        snackbar: MatSnackBar,
        authService: AuthService
    ) {
        this.translationService = translationService;
        this.rolesService = rolesService;
        this.nomenclatureService = nomenclatureService;
        this.confirmDialog = confirmDialog;
        this.permissions = permissions;
        this.snackbar = snackbar;
        this.authService = authService;

        this.mobileDevicesMatCardTitleLabel = this.translationService.getValue('users-page.mobile-devices-mat-card-title-label');

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        await this.getNomenclatures();

        if (this.readOnly) {
            this.editUserForm.get('territorialUnitControl')?.disable();
            this.editUserForm.get('departmentControl')?.disable();
            this.editUserForm.get('sectorControl')?.disable();
        }

        if (this.isInternalUser) {
            this.editUserForm.get('territorialUnitControl')!.setValidators(Validators.required);
            this.editUserForm.get('territorialUnitControl')!.markAsPending();

            this.canRestoreRecords = this.permissions.has(PermissionsEnum.InternalUsersRestoreRecords);

            if (this.userId !== -1 && this.userId !== undefined) {
                this.service.getUser(this.userId).subscribe(result => {
                    this.internalUserModel = result;
                    this.fillForm(this.internalUserModel);
                    this.userFullName = `${this.internalUserModel.firstName} ${this.internalUserModel.middleName !== undefined && this.internalUserModel.middleName !== null ? this.internalUserModel.middleName : ''} ${this.internalUserModel.lastName}`;
                });
            }
        }
        else {
            if (this.userId !== -1 && this.userId !== undefined && this.userId !== null) {
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
        this.isAdd = data.isAddUser;

        if (this.isInternalUser) {
            if (data.isAddUser) {
                this.internalUserModel = new InternalUserDTO();
            }
            else {
                const params = (data as DialogParamsModel);
                this.userId = params.id;

                if (params.isReadonly) {
                    this.editUserForm.disable();
                }
            }
        } else {
            if (data.isAddUser) {
                this.externalUserModel = new ExternalUserDTO();
            }
            else {
                const params = (data as DialogParamsModel);
                this.userId = params.id;

                if (params.isReadonly) {
                    this.editUserForm.disable();
                }
            }
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: (dialogResult?: any) => void): void {
        if (this.readOnly) {
            dialogClose();
            return;
        }

        this.editUserForm.markAllAsTouched();

        if (this.editUserForm.valid) {
            if (actionInfo.id === 'deactivate') {
                this.deactivateUser(dialogClose);
            }
            else if (actionInfo.id === 'send-msg-for-password-change') {
                if (this.userId !== undefined && this.userId !== null) {
                    this.service.sendChangePasswordEmail(this.userId).subscribe(result => {
                        if (result !== null && result !== undefined) {
                            const message: string = this.translationService.getValue('users-page.change-password-email-sent');
                            this.snackbar.open(message, undefined, {
                                duration: RequestProperties.DEFAULT.showExceptionDurationSucc,
                                panelClass: RequestProperties.DEFAULT.showExceptionColorClassSucc
                            });
                        }
                    });
                }
            }
            else if (actionInfo.id === 'impersonate-user') {
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
            }
            else if (actionInfo.id === 'change-user-status') {
                this.confirmDialog.open({
                    title: this.translationService.getValue('users-page.change-user-status'),
                    message: this.translationService.getValue('users-page.change-user-message'),
                    okBtnLabel: this.translationService.getValue('users-page.yes'),
                    cancelBtnLabel: this.translationService.getValue('users-page.change-user-cancel-btn-label')
                }).subscribe({
                    next: (ok: boolean) => {
                        if (ok) {
                            (this.service as ExternalUserManagementService).changeUserStatus(this.userId!).subscribe(result => {
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

                if (this.isInternalUser) {
                    this.internalUserModel = CommonUtils.sanitizeModelStrings(this.internalUserModel);
                }
                else {
                    this.externalUserModel = CommonUtils.sanitizeModelStrings(this.externalUserModel);
                }

                if (this.userId !== null && this.userId !== undefined) {
                    if (this.isInternalUser) {
                        (this.service as InternalUserManagementService).edit(this.internalUserModel).subscribe({
                            next: () => {
                                dialogClose(this.internalUserModel);
                            },
                            error: (httpErrorResponse: HttpErrorResponse) => {
                                this.handleErrorResponse(httpErrorResponse);
                            }
                        });
                    }
                    else {
                        (this.service as ExternalUserManagementService).edit(this.externalUserModel).subscribe({
                            next: () => {
                                dialogClose(this.externalUserModel);
                            },
                            error: (httpErrorResponse: HttpErrorResponse) => {
                                this.handleErrorResponse(httpErrorResponse);
                            }
                        });
                    }
                }
                else {
                    if (this.isInternalUser) {
                        (this.service as InternalUserManagementService).add(this.internalUserModel).subscribe({
                            next: () => {
                                dialogClose(this.internalUserModel);
                            },
                            error: (httpErrorResponse: HttpErrorResponse) => {
                                this.handleErrorResponse(httpErrorResponse);
                            }
                        });
                    }
                }
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
                ? `${this.translationService.getValue('users-page.are-you-sure-you-want-do-deactivate')} ${this.internalUserModel.firstName} ${this.internalUserModel.lastName} (${this.internalUserModel.email})?`
                : `${this.translationService.getValue('users-page.are-you-sure-you-want-do-deactivate')} ${this.externalUserModel.firstName} ${this.externalUserModel.lastName} (${this.externalUserModel.email})?`,
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

    public getControlErrorLabelText(controlName: string, errorValue: unknown, errorCode: string): TLError | undefined {
        if (controlName === 'idControl') {
            if (errorCode === 'datesOverlap') {
                return new TLError({
                    text: this.translationService.getValue('users-page.dates-overlap-with-other-record'),
                    type: 'error'
                });
            }
        }

        return undefined;
    }

    private fillForm(model: InternalUserDTO | ExternalUserDTO): void {
        const simpleRegixData: RegixPersonDataDTO = new RegixPersonDataDTO({
            egnLnc: model.egnLnc,
            firstName: model.firstName,
            middleName: model.middleName,
            lastName: model.lastName
        });

        this.editUserForm.get('identificationDataControl')!.setValue(simpleRegixData);
        this.editUserForm.get('usernameEmailGroup.emailControl')!.setValue(model.email);
        this.editUserForm.get('phoneNumberControl')!.setValue(model.phone);
        this.editUserForm.get('userMustChangePasswordControl')!.setValue(model.userMustChangePassword);
        this.editUserForm.get('isLockedControl')!.setValue(model.isLocked);
        this.editUserForm.get('positionControl')!.setValue(model.position);

        this.userRoles = model.userRoles as RoleDTO[];

        if (this.isInternalUser) {
            this.editUserForm.get('titleControl')!.setValue((model as InternalUserDTO).title);

            const territoryUnit = NomenclatureStore.instance.getNomenclatureItem(NomenclatureTypes.TerritoryUnits, (model as InternalUserDTO).territoryUnitId as number);
            this.editUserForm.get('territorialUnitControl')!.setValue(territoryUnit);

            const department = NomenclatureStore.instance.getNomenclatureItem(NomenclatureTypes.Departments, (model as InternalUserDTO).departmentId as number);
            this.editUserForm.get('departmentControl')!.setValue(department);

            const sector = NomenclatureStore.instance.getNomenclatureItem(NomenclatureTypes.Sectors, (model as InternalUserDTO).sectorId as number);
            this.editUserForm.get('sectorControl')!.setValue(sector);

            setTimeout(() => {
                if ((model as InternalUserDTO).mobileDevices !== undefined && (model as InternalUserDTO).mobileDevices !== null) {
                    this.editUserForm.get('mobileDevicesControl')!.setValue((model as InternalUserDTO).mobileDevices);
                }
            });
        }
        else {
            const externalUser: ExternalUserDTO = (model as ExternalUserDTO);
            if (externalUser.territoryUnitId !== null && externalUser.territoryUnitId !== undefined) {

                const territoryUnit = NomenclatureStore.instance.getNomenclatureItem(NomenclatureTypes.TerritoryUnits, externalUser.territoryUnitId as number);
                this.editUserForm.get('territorialUnitControl')!.setValue(territoryUnit);

                const department = NomenclatureStore.instance.getNomenclatureItem(NomenclatureTypes.Departments, externalUser.departmentId as number);
                this.editUserForm.get('departmentControl')!.setValue(department);

                const sector = NomenclatureStore.instance.getNomenclatureItem(NomenclatureTypes.Sectors, externalUser.sectorId as number);
                this.editUserForm.get('sectorControl')!.setValue(sector);
            }
            if (externalUser !== undefined && externalUser.userLegals !== undefined && externalUser.userLegals !== null) {
                this.userLegals = (model as ExternalUserDTO).userLegals as UserLegalDTO[];
            }
        }
    }

    private fillModel(form: FormGroup): void {
        if (this.isInternalUser) {
            const simpleRegixData: RegixPersonDataDTO = form.get('identificationDataControl')!.value;
            this.internalUserModel.firstName = simpleRegixData.firstName;
            this.internalUserModel.middleName = simpleRegixData.middleName;
            this.internalUserModel.lastName = simpleRegixData.lastName;
            this.internalUserModel.egnLnc = simpleRegixData.egnLnc;

            this.internalUserModel.email = form.get('usernameEmailGroup.emailControl')!.value;

            this.internalUserModel.phone = form.get('phoneNumberControl')!.value;
            this.internalUserModel.userMustChangePassword = form.get('userMustChangePasswordControl')!.value;
            this.internalUserModel.isLocked = form.get('isLockedControl')!.value ?? false;
            this.internalUserModel.position = form.get('positionControl')!.value;
            this.internalUserModel.title = form.get('titleControl')!.value;

            this.internalUserModel.departmentId = NomenclatureStore.getValue(form.get('departmentControl')!.value);
            this.internalUserModel.sectorId = NomenclatureStore.getValue(form.get('sectorControl')!.value);
            this.internalUserModel.territoryUnitId = NomenclatureStore.getValue(form.get('territorialUnitControl')!.value);

            if (this.roleDataTable.rows.length !== 0) {
                this.internalUserModel.userRoles = this.roleDataTable.rows as RoleDTO[];
            }

            this.internalUserModel.mobileDevices = this.editUserForm.get('mobileDevicesControl')!.value;
        }
        else {
            const simpleRegixData: RegixPersonDataDTO = form.get('identificationDataControl')!.value;
            this.externalUserModel.firstName = simpleRegixData.firstName;
            this.externalUserModel.middleName = simpleRegixData.middleName;
            this.externalUserModel.lastName = simpleRegixData.lastName;
            this.externalUserModel.egnLnc = simpleRegixData.egnLnc;

            this.externalUserModel.email = form.get('usernameEmailGroup.emailControl')!.value;

            this.externalUserModel.phone = form.get('phoneNumberControl')!.value;
            this.externalUserModel.userMustChangePassword = form.get('userMustChangePasswordControl')!.value;
            this.externalUserModel.isLocked = form.get('isLockedControl')!.value ?? false;
            this.externalUserModel.position = form.get('positionControl')!.value;

            this.externalUserModel.departmentId = NomenclatureStore.getValue(form.get('departmentControl')!.value);
            this.externalUserModel.sectorId = NomenclatureStore.getValue(form.get('sectorControl')!.value);
            this.externalUserModel.territoryUnitId = NomenclatureStore.getValue(form.get('territorialUnitControl')!.value);

            if (this.roleDataTable.rows.length !== 0) {
                this.externalUserModel.userRoles = this.roleDataTable.rows as RoleDTO[];
            }

            if (this.legalDataTable.rows.length !== 0) {
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
        // editUserForm

        this.editUserForm = new FormGroup({
            identificationDataControl: new FormControl(),
            usernameEmailGroup: new FormGroup({
                emailControl: new FormControl(null, [Validators.email, Validators.maxLength(200)])
            }, this.uniqueEmailValidator()),
            phoneNumberControl: new FormControl(null, Validators.maxLength(50)),
            userMustChangePasswordControl: new FormControl(false),
            isLockedControl: new FormControl(),
            territorialUnitControl: new FormControl(),
            departmentControl: new FormControl(),
            sectorControl: new FormControl(),
            positionControl: new FormControl(null, Validators.maxLength(500)),
            titleControl: new FormControl(null, Validators.maxLength(50)),
            mobileDevicesControl: new FormControl()
        }, this.uniqueValidEgnLncValidator());

        this.editUserForm.get('identificationDataControl')!.valueChanges.subscribe({
            next: (regixData: RegixPersonDataDTO | undefined) => {
                if (this.lastEgnLnc?.egnLnc !== regixData?.egnLnc?.egnLnc
                    || this.lastEgnLnc?.identifierType !== regixData?.egnLnc?.identifierType
                ) {
                    this.hasInvalidEgnLnchError = false;
                    this.editUserForm.updateValueAndValidity({ emitEvent: false });
                }

                this.lastEgnLnc = regixData?.egnLnc;
            }
        });

        this.editUserForm.get('usernameEmailGroup.emailControl')!.valueChanges.subscribe({
            next: () => {
                this.hasEmailExistsError = false;
                this.editUserForm.get('usernameEmailGroup')!.updateValueAndValidity({ emitEvent: false });
            }
        });

        this.editUserForm.get('sectorControl')!.disable();
        this.editUserForm.get('departmentControl')!.disable();

        // userRoleForm

        this.userRoleForm = new FormGroup({
            idControl: new FormControl(null, [Validators.required, this.datesOverlappingValidator()]),
            accessValidToControl: new FormControl(),
            accessValidFromControl: new FormControl()
        });

        this.userRoleForm!.get('accessValidFromControl')!.valueChanges.subscribe({
            next: () => {
                this.userRoleForm.get('idControl')!.updateValueAndValidity({ onlySelf: true });
            }
        });

        this.userRoleForm!.get('accessValidToControl')!.valueChanges.subscribe({
            next: () => {
                this.userRoleForm.get('idControl')!.updateValueAndValidity({ onlySelf: true });
            }
        });

        // userLegalForm

        this.userLegalForm = new FormGroup({
            legalIdControl: new FormControl(null, Validators.required),
            roleIdControl: new FormControl(null, Validators.required)
        });
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

    private handleErrorResponse(errorResponse: HttpErrorResponse): void {
        const errorCode: ErrorCode | undefined = (errorResponse.error as ErrorModel)?.code;

        switch (errorCode) {
            case ErrorCode.InvalidEgnLnch: {
                this.hasInvalidEgnLnchError = true;
                this.editUserForm.updateValueAndValidity({ emitEvent: false });
            } break;
            case ErrorCode.EmailExists: {
                this.hasEmailExistsError = true;
                this.editUserForm.get('usernameEmailGroup')!.updateValueAndValidity({ emitEvent: false });
            } break;
        }
    }

    private datesOverlappingValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const max = (lhs: Date, rhs: Date) => lhs > rhs ? lhs : rhs;
            const min = (lhs: Date, rhs: Date) => lhs < rhs ? lhs : rhs;

            if (this.roleDataTable) {
                const id: number | undefined = control.value?.value;
                const validFrom: Date | undefined = this.userRoleForm.get('accessValidFromControl')!.value ?? new Date();
                const validTo: Date | undefined = this.userRoleForm.get('accessValidToControl')!.value ?? new Date(9999, 0, 1);
                
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

    private uniqueEmailValidator(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            if (form === null || form === undefined) {
                return null;
            }

            if (this.hasEmailExistsError) {
                return { 'emailExists': true };
            }

            return null;
        }
    }

    private uniqueValidEgnLncValidator(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            if (form === null || form === undefined) {
                return null;
            }

            if (this.hasInvalidEgnLnchError) {
                return { 'invalidEgnLnc': true };
            }

            return null;
        }
    }
}
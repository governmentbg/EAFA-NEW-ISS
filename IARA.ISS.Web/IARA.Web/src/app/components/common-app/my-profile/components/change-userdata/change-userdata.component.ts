import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ChangeUserDataDTO } from '@app/models/generated/dtos/ChangeUserDataDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { UsersService } from '@app/services/common-app/users.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { UserLegalDTO } from '@app/models/generated/dtos/UserLegalDTO';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { UserLegalStatusEnum } from '@app/enums/user-legal-status.enum';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { ChangeUserDataLegalComponent } from '../change-userdata-legal/change-userdata-legal.component';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { ChangeUserLegalDialogParams } from '../models/change-user-legal-dialog-params';
import { ChangeUserLegalDTO } from '@app/models/generated/dtos/ChangeUserLegalDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';

@Component({
    selector: 'change-userdata',
    templateUrl: './change-userdata.component.html'
})
export class ChangeUserDataComponent implements OnInit, AfterViewInit, IDialogComponent {
    public showDistricts: boolean = false;
    public showAllDistricts: boolean = false;
    public passwordIcon: string = "fa-eye";
    public passwordConfirmationIcon: string = "fa-eye";
    public userMustChangePassword: boolean = false;
    public hasEmailExistsError: boolean = false;
    public hasEgnLncExistsError: boolean = false;
    public isTouched: boolean = false;
    public legalStatusEnum: typeof UserLegalStatusEnum = UserLegalStatusEnum;
    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    public documentTypes: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public genders: NomenclatureDTO<number>[] = [];
    public districts!: NomenclatureDTO<number>[];
    public territoryUnits: NomenclatureDTO<number>[] = [];
    public userLegals: ChangeUserLegalDTO[] = [];
    public changeUserDataForm!: FormGroup;

    private userModel!: ChangeUserDataDTO;
    private userService!: UsersService;

    private readonly nomenclaturesService: CommonNomenclatures;
    private readonly translateService: FuseTranslationLoaderService;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly userLegalDialog: TLMatDialog<ChangeUserDataLegalComponent>;

    @ViewChild('legalDataTable')
    private userLegalsTable!: TLDataTableComponent;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    public constructor(
        nomenclaturesService: CommonNomenclatures,
        translateService: FuseTranslationLoaderService,
        userService: UsersService,
        confirmDialog: TLConfirmDialog,
        userLegalDialog: TLMatDialog<ChangeUserDataLegalComponent>
    ) {
        this.userService = userService;
        this.nomenclaturesService = nomenclaturesService;
        this.translateService = translateService;
        this.confirmDialog = confirmDialog;
        this.userLegalDialog = userLegalDialog;
        this.buildForm();
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.TerritoryUnits, this.nomenclaturesService.getTerritoryUnits.bind(this.nomenclaturesService), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.territoryUnits = result;
            }
        });
    }

    public ngAfterViewInit(): void {
        this.userLegalsTable.recordChanged.subscribe({
            next: () => {
                this.isTouched = true;
            }
        });
    }

    public setData(data: { userId: number, userMustChangePassword: boolean; }, wrapperData: DialogWrapperData): void {
        this.userMustChangePassword = data.userMustChangePassword;

        if (this.userMustChangePassword) {
            this.setPasswordFieldsValidators();
        }

        this.userService.getUserData().subscribe({
            next: (result: ChangeUserDataDTO) => {
                this.userModel = result;
                this.fillForm(this.userModel);
            }
        });
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (actionInfo.id === 'save') {
            this.isTouched = true;
            this.changeUserDataForm.markAllAsTouched();
            this.changeUserDataForm.updateValueAndValidity({ emitEvent: false });
            this.validityCheckerGroup.validate();

            if (this.changeUserDataForm.valid) {
                if (this.userMustChangePassword === false
                    && this.userModel.email !== this.changeUserDataForm.get('basicDataControl')!.value.email
                ) {
                    this.userMustChangePassword = true;
                    this.setPasswordFieldsValidators();
                    setTimeout(() => {
                        this.changeUserDataForm.markAllAsTouched();
                        this.changeUserDataForm.updateValueAndValidity({ emitEvent: false });
                        this.validityCheckerGroup.validate();
                    }, 1);

                    return;
                }
                this.userModel = this.fillModel(this.changeUserDataForm);

                this.userService.updateUserData(this.userModel).subscribe({
                    next: () => {
                        dialogClose(this.userModel);
                    },
                    error: (httpErrorResponse: HttpErrorResponse) => {
                        this.handleErrorResponse(httpErrorResponse);
                    }
                });
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (!actionInfo.disabled) {
            dialogClose();
        }
    }

    public showOrHidePassword(formControlName: string): void {
        if (formControlName === 'password') {
            this.passwordIcon === "fa-eye"
                ? this.passwordIcon = "fa-eye-slash"
                : this.passwordIcon = "fa-eye";
        }
        else if (formControlName === 'passwordConfirmation') {
            this.passwordConfirmationIcon === "fa-eye"
                ? this.passwordConfirmationIcon = "fa-eye-slash"
                : this.passwordConfirmationIcon = "fa-eye";
        }
    }

    public editUserLegal(userLegal: ChangeUserLegalDTO, viewMode: boolean = false): void {
        const title: string = viewMode
            ? this.translateService.getValue('my-profile.view-user-legal-dialog-title')
            : this.translateService.getValue('my-profile.edit-user-legal-dialog-title');

        const data: ChangeUserLegalDialogParams = new ChangeUserLegalDialogParams({
            model: userLegal,
            viewMode: viewMode,
            territoryUnits: this.territoryUnits
        });

        const dialog = this.userLegalDialog.openWithTwoButtons({
            title: title,
            TCtor: ChangeUserDataLegalComponent,
            headerAuditButton: undefined,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translateService,
            viewMode: viewMode
        }, '1200px');

        dialog.subscribe({
            next: (result: ChangeUserLegalDTO | undefined) => {
                if (result !== undefined && result !== null) {
                    userLegal = result;
                    userLegal.hasMissingProperties = false;
                    this.userLegals = this.userLegals.slice();

                    this.changeUserDataForm.updateValueAndValidity({ emitEvent: false });
                }
            }
        });
    }

    public deleteUserLegal(userLegal: GridRow<ChangeUserLegalDTO>): void {
        this.confirmDialog.open({
            title: this.translateService.getValue('my-profile.delete-user-legal-dialog-title'),
            message: this.translateService.getValue('my-profile.delete-user-legal-dialog-message'),
            okBtnLabel: this.translateService.getValue('my-profile.delete-user-legal-dialog-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.userLegalsTable.softDelete(userLegal);
                    this.changeUserDataForm.updateValueAndValidity({ emitEvent: false });
                }
            }
        });
    }

    public undoDeleteUserLegal(userLegal: GridRow<ChangeUserLegalDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.userLegalsTable.softUndoDelete(userLegal);
                    this.changeUserDataForm.updateValueAndValidity({ emitEvent: false });
                }
            }
        });
    }

    private buildForm(): void {
        this.changeUserDataForm = new FormGroup({
            basicDataControl: new FormControl(),
            addressesControl: new FormControl(),
            territoryUnitControl: new FormControl(),
            passwordControl: new FormControl(undefined, [
                Validators.maxLength(64),
                TLValidators.passwordComplexityValidator()]),
            passwordConfirmationControl: new FormControl(undefined, [
                TLValidators.confirmPasswordValidator,
                Validators.maxLength(64),
                TLValidators.passwordComplexityValidator()]),
        }, this.missingUserLegalPropertiesValidator());

        this.changeUserDataForm.get('basicDataControl')!.valueChanges.subscribe({
            next: (data: RegixPersonDataDTO) => {
                this.hasEmailExistsError = false;
            }
        });
    }

    private fillForm(model: ChangeUserDataDTO): void {
        this.changeUserDataForm.get('basicDataControl')!.setValue(model);
        this.changeUserDataForm.controls.addressesControl.setValue(model.userAddresses);

        setTimeout(() => {
            this.userLegals = model.userLegals ?? [];
            this.setUserLegalMissingProperty();
        });
    }

    private fillModel(form: FormGroup): ChangeUserDataDTO {
        this.userModel.userAddresses = form.get('addressesControl')!.value;

        const regixData: RegixPersonDataDTO = form.get('basicDataControl')!.value;

        this.userModel.egnLnc = regixData.egnLnc;
        this.userModel.birthDate = regixData.birthDate;
        this.userModel.phone = regixData.phone;
        this.userModel.firstName = regixData.firstName;
        this.userModel.middleName = regixData.middleName;
        this.userModel.lastName = regixData.lastName;
        this.userModel.citizenshipCountryId = regixData.citizenshipCountryId;
        this.userModel.hasBulgarianAddressRegistration = regixData.hasBulgarianAddressRegistration;
        this.userModel.genderId = regixData.genderId;
        this.userModel.password = form.controls.passwordControl.value;
        this.userModel.email = regixData.email;
        this.userModel.username = regixData.email;
        this.userModel.document = regixData.document;
        this.userModel.territoryUnitId = form.controls.territoryUnitControl.value?.value;
        this.userModel.userLegals = this.getUserLegalsFromTable();

        return this.userModel;
    }

    private setPasswordFieldsValidators(): void {
        this.changeUserDataForm.get('passwordControl')!.setValidators([
            Validators.required,
            Validators.maxLength(64),
            TLValidators.passwordComplexityValidator()]);

        this.changeUserDataForm.get('passwordConfirmationControl')!.setValidators([
            Validators.required,
            TLValidators.confirmPasswordValidator,
            Validators.maxLength(64),
            TLValidators.passwordComplexityValidator()]);
    }

    private missingUserLegalPropertiesValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.userLegals !== undefined && this.userLegals !== null && this.userLegals.length > 0) {

                for (const userLegal of this.userLegals) {
                    if (userLegal.hasMissingProperties && userLegal.isActive) {
                        return { 'missingProperties': true };
                    }
                }
            }

            return null;
        };
    }

    private setUserLegalMissingProperty(): void {
        if (this.userLegals.length > 0) {
            for (const userLegal of this.userLegals) {
                userLegal.hasMissingProperties = this.checkForMissingProperties(userLegal);
            }
        }
    }

    private checkForMissingProperties(userLegal: ChangeUserLegalDTO): boolean {
        if (userLegal.legal?.eik === undefined || userLegal.legal?.eik === null) {
            return true;
        }

        if (userLegal.legal?.eik?.includes('ЛРД') === true) {
            return true;
        }

        if (userLegal.legal?.name === undefined || userLegal.legal?.name === null) {
            return true;
        }

        if (userLegal.territoryUnitId === undefined || userLegal.territoryUnitId === null) {
            return true;
        }

        if (userLegal.addresses !== undefined && userLegal.addresses !== null && userLegal.addresses.length > 0) {
            for (const address of userLegal.addresses) {
                if (address.countryId === undefined || address.countryId === null || address.street === undefined || address.street === null) {
                    return true;
                }
            }
        }
        else {
            return true;
        }

        return false;
    }

    private getUserLegalsFromTable(): ChangeUserLegalDTO[] {
        const rows = this.userLegalsTable.rows as ChangeUserLegalDTO[];

        const userLegals: UserLegalDTO[] = rows.filter(x => x.isActive !== false).map(x => new ChangeUserLegalDTO({
            legalId: x.legalId,
            roleId: x.roleId,
            eik: x.eik,
            name: x.name,
            status: x.status,
            addresses: x.addresses,
            legal: x.legal,
            territoryUnitId: x.territoryUnitId,
            associationId: x.associationId,
            isActive: x.isActive
        }));

        return userLegals ?? [];
    }

    private handleErrorResponse(errorResponse: HttpErrorResponse): void {
        const errorCode: ErrorCode | undefined = (errorResponse.error as ErrorModel)?.code;

        if (errorCode === ErrorCode.EmailExists) {
            this.hasEmailExistsError = true;
            this.changeUserDataForm.setErrors({ emailExists: true });
            this.changeUserDataForm.markAsTouched();
            this.validityCheckerGroup.validate();
        }

        if (errorCode === ErrorCode.InvalidEgnLnch) {
            this.hasEgnLncExistsError = true;
            this.changeUserDataForm.setErrors({ invalidEgnLnc: true });
            this.changeUserDataForm.markAsTouched();
            this.validityCheckerGroup.validate();
        }
    }

    private closeDialogBtnClicked(closeFn: DialogCloseCallback): void {
        closeFn();
    }
}

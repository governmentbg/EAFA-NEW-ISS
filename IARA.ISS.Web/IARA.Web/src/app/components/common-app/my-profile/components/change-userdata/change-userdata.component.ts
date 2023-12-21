import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
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

@Component({
    selector: 'change-userdata',
    templateUrl: './change-userdata.component.html'
})
export class ChangeUserDataComponent implements OnInit, IDialogComponent {
    public showDistricts: boolean = false;
    public showAllDistricts: boolean = false;
    public passwordIcon: string = "fa-eye";
    public passwordConfirmationIcon: string = "fa-eye";
    public userMustChangePassword: boolean = false;
    public legalStatusEnum: typeof UserLegalStatusEnum = UserLegalStatusEnum;

    public documentTypes: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public genders: NomenclatureDTO<number>[] = [];
    public districts!: NomenclatureDTO<number>[];
    public territoryUnits: NomenclatureDTO<number>[] = [];
    public userLegals: UserLegalDTO[] = [];
    public changeUserDataForm!: FormGroup;

    private userModel!: ChangeUserDataDTO;

    private userService!: UsersService;

    private readonly nomenclaturesService: CommonNomenclatures;
    private readonly translateService: FuseTranslationLoaderService;
    private readonly confirmDialog: TLConfirmDialog;

    @ViewChild('legalDataTable')
    private userLegalsTable!: TLDataTableComponent;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    public constructor(
        nomenclaturesService: CommonNomenclatures,
        translateService: FuseTranslationLoaderService,
        userService: UsersService,
        confirmDialog: TLConfirmDialog
    ) {
        this.userService = userService;
        this.nomenclaturesService = nomenclaturesService;
        this.translateService = translateService;
        this.confirmDialog = confirmDialog;
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

    public setData(data: { userId: number, userMustChangePassword: boolean }, wrapperData: DialogWrapperData): void {
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
            this.changeUserDataForm.markAllAsTouched();
            this.changeUserDataForm.updateValueAndValidity({ emitEvent: false });
            this.validityCheckerGroup.validate();

            if (this.changeUserDataForm.valid) {
                this.userModel = this.fillModel(this.changeUserDataForm);

                this.userService.updateUserData(this.userModel).subscribe(() => {
                    dialogClose(this.userModel);
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

    public deleteUserLegal(userLegal: GridRow<UserLegalDTO>): void {
        this.confirmDialog.open({
            title: this.translateService.getValue('my-profile.delete-user-legal-dialog-title'),
            message: this.translateService.getValue('my-profile.delete-user-legal-dialog-message'),
            okBtnLabel: this.translateService.getValue('my-profile.delete-user-legal-dialog-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.userLegalsTable.softDelete(userLegal);
                }
            }
        });
    }

    public undoDeleteUserLegal(userLegal: GridRow<UserLegalDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.userLegalsTable.softUndoDelete(userLegal);
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
        });
    }

    private fillForm(model: ChangeUserDataDTO): void {
        this.changeUserDataForm.get('basicDataControl')!.setValue(model);
        this.changeUserDataForm.controls.addressesControl.setValue(model.userAddresses);

        setTimeout(() => {
            this.userLegals = model.userLegals ?? [];
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

    private getUserLegalsFromTable(): UserLegalDTO[] {
        const rows = this.userLegalsTable.rows as UserLegalDTO[];

        const userLegals: UserLegalDTO[] = rows.map(x => new UserLegalDTO({
            legalId: x.legalId,
            roleId: x.roleId,
            eik: x.eik,
            name: x.name,
            status: x.status,
            isActive: x.isActive
        }));

        return userLegals ?? [];
    }
}
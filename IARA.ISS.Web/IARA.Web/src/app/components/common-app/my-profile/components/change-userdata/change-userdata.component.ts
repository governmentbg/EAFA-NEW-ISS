import { Component, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { ChangeUserDataDTO } from '@app/models/generated/dtos/ChangeUserDataDTO';
import { EgnLncDTO } from '@app/models/generated/dtos/EgnLncDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PersonDocumentDTO } from '@app/models/generated/dtos/PersonDocumentDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { UsersService } from '@app/services/common-app/users.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';

@Component({
    selector: 'change-userdata',
    templateUrl: './change-userdata.component.html'
})
export class ChangeUserDataComponent implements IDialogComponent {
    public showDistricts: boolean = false;
    public showAllDistricts: boolean = false;
    public passwordIcon: string = "fa-eye";
    public passwordConfirmationIcon: string = "fa-eye";
    public userMustChangePassword: boolean = false;

    public documentTypes: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public genders: NomenclatureDTO<number>[] = [];
    public districts!: NomenclatureDTO<number>[];
    public changeUserDataForm!: FormGroup;

    private userModel!: ChangeUserDataDTO;

    private userService!: UsersService;
    private nomenclaturesService: CommonNomenclatures;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    public constructor(nomenclaturesService: CommonNomenclatures, userService: UsersService) {
        this.userService = userService;
        this.nomenclaturesService = nomenclaturesService;
        this.buildForm();
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

    private buildForm(): void {
        this.changeUserDataForm = new FormGroup({
            basicDataControl: new FormControl(),
            addressesControl: new FormControl(),
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
}
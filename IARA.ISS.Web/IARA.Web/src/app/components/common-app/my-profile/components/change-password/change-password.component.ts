import { HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IMyProfileService } from '@app/interfaces/common-app/my-profile.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ErrorCode, ErrorModel, ErrorType } from '@app/models/common/exception.model';
import { UserPasswordDTO } from '@app/models/generated/dtos/UserPasswordDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { TLSnackbar } from '@app/shared/components/snackbar/tl.snackbar';

@Component({
    selector: 'change-password',
    templateUrl: './change-password.component.html',
})
export class ChangePasswordComponent implements IDialogComponent {
    public translationService: FuseTranslationLoaderService;
    public changePasswordForm!: FormGroup;
    public oldPasswordIcon: string = 'fa-eye';
    public passwordIcon: string = 'fa-eye';
    public passwordConfirmationIcon: string = 'fa-eye';
    public wrongPasswordServerMessage: string = '';
    public readonly useMultilineErrors = true;

    public getControlErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelText.bind(this);

    private userPasswordModel: UserPasswordDTO = new UserPasswordDTO();
    private service!: IMyProfileService;
    private personId!: number;
    private snackbar: TLSnackbar;

    constructor(translationService: FuseTranslationLoaderService, snackbar: TLSnackbar) {
        this.translationService = translationService;
        this.snackbar = snackbar;
        this.buildForm();

        this.changePasswordForm.controls.password.valueChanges.subscribe(() => {
            this.changePasswordForm.controls.passwordConfirmation.updateValueAndValidity();
        });
    }

    public setData(data: DialogParamsModel, buttons: DialogWrapperData): void {
        if (data !== undefined) {
            this.personId = data.id;
            this.service = data.service as IMyProfileService;
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (actionInfo.id === 'save') {
            this.changePasswordForm.markAllAsTouched();

            if (this.changePasswordForm.valid) {
                this.fillModel(this.changePasswordForm);

                this.service.changePassword(this.userPasswordModel).subscribe({
                    next: () => {
                        dialogClose(this.userPasswordModel);
                    },
                    error: (errorResponse: HttpErrorResponse) => {
                        const error: ErrorModel = errorResponse.error as ErrorModel;
                        if (error.type === ErrorType.Unhandled) {
                            this.snackbar.error(this.translationService.getValue('my-profile.an-error-occurred'));
                        }
                        else {
                            if (error.code === ErrorCode.WrongPassword) {
                                if (error.messages?.length === 1) {
                                    this.wrongPasswordServerMessage = error.messages[0];
                                }
                                else {
                                    this.wrongPasswordServerMessage = error.messages.join(';');
                                }

                                const oldPassword = this.changePasswordForm.controls.oldPassword;
                                oldPassword.setErrors({ 'invalid': true });
                                oldPassword.markAsTouched();
                            }
                        }
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
        if (formControlName === 'oldPassword') {
            this.oldPasswordIcon === 'fa-eye' ? this.oldPasswordIcon = 'fa-eye-slash' : this.oldPasswordIcon = 'fa-eye';
        } else if (formControlName === 'password') {
            this.passwordIcon === 'fa-eye' ? this.passwordIcon = 'fa-eye-slash' : this.passwordIcon = 'fa-eye';
        } else if (formControlName === 'passwordConfirmation') {
            this.passwordConfirmationIcon === 'fa-eye' ? this.passwordConfirmationIcon = 'fa-eye-slash' : this.passwordConfirmationIcon = 'fa-eye';
        }
    }

    public getControlErrorLabelText(controlName: string, error: unknown, errorCode: string): TLError | undefined {
        switch (controlName) {
            case 'oldPassword': {
                switch (errorCode) {
                    case 'invalid': {
                        return new TLError({
                            text: this.translationService.getValue('my-profile.wrong-password'),
                            type: 'error'
                        })
                    } break;
                    default: {
                        return undefined;
                    }
                }
            } break;
            case 'password':
            case 'passwordConfirmation': {
                switch (errorCode) {
                    case 'passwordsNotMatching':
                        return new TLError({
                            text: this.translationService.getValue('my-profile.passwords-must-match'),
                            type: 'error'
                        });
                    case 'required':
                        return new TLError({
                            text: this.translationService.getValue('my-profile.password-is-required'),
                            type: 'error'
                        });
                    default:
                        return undefined;
                }
            } break;
            default: {
                return undefined;
            }
        }
    }

    private buildForm(): void {
        this.changePasswordForm = new FormGroup({
            oldPassword: new FormControl(null, [Validators.required, Validators.maxLength(200)]),
            password: new FormControl(null, [Validators.required, Validators.maxLength(200), TLValidators.passwordComplexityValidator()]),
            passwordConfirmation: new FormControl(null, [
                Validators.required,
                TLValidators.confirmPasswordValidator.bind(this),
                Validators.maxLength(200),
                TLValidators.passwordComplexityValidator()]),
        });
    }

    private fillModel(changePasswordForm: FormGroup): void {
        this.userPasswordModel.oldPassword = changePasswordForm.controls.oldPassword.value;
        this.userPasswordModel.newPassword = changePasswordForm.controls.passwordConfirmation.value;
        this.userPasswordModel.personId = this.personId;
    }
}
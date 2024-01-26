import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { TLSnackbar } from '@app/shared/components/snackbar/tl.snackbar';
import { TRANSLATE_SERVICE_TOKEN } from '@app/shared/di/shared-di.tokens';
import { ITranslationService } from '@app/shared/interfaces/translate-service.interface';
import { TLUtils } from '@app/shared/utils';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { SECURITY_SERVICE_TOKEN, USER_SERVICE_TOKEN } from '../di/auth-di.tokens';
import { IGenericSecurityService } from '../interfaces/security-service.interface';
import { IUserService } from '../interfaces/user-service.interface';
import { UpdatePasswordModel } from '../models/auth/change-password.model';
import { PasswordValidatorModel } from '../models/auth/password-validator.model';
import { User } from "../models/auth/user.model";
import { PasswordUtils } from '../utils/password.utils';


@Component({
    selector: 'change-password',
    templateUrl: './change-password.component.html',
    styles: [
    ]
})
export class ChangePasswordComponent<TIdentifier, TUser extends User<TIdentifier>> implements OnInit {
    public form!: FormGroup;
    public passwordValidatorsLoaded: boolean = false;
    public serverError: string = '';
    public mustChangePassword: boolean = false;

    private translateService: ITranslationService;
    private userService: IUserService;
    private snackbar: TLSnackbar;
    private router: Router;
    private securityService: IGenericSecurityService<TIdentifier, TUser>;

    public constructor(@Inject(TRANSLATE_SERVICE_TOKEN) translateService: ITranslationService,
        @Inject(USER_SERVICE_TOKEN) userService: IUserService,
        snackbar: TLSnackbar,
        router: Router,
        @Inject(SECURITY_SERVICE_TOKEN) securityService: IGenericSecurityService<TIdentifier, TUser>) {
        this.userService = userService;
        this.translateService = translateService;
        this.snackbar = snackbar;
        this.router = router;
        this.securityService = securityService;
        this.buildForm();
    }

    public ngOnInit(): void {
        this.userService.getPasswordValidators().subscribe({
            next: (validator: PasswordValidatorModel) => {
                this.form.controls.passwordControl.setValidators(PasswordUtils.buildPasswordValidator(validator));
                this.passwordValidatorsLoaded = true;
            },
            error: (error: HttpErrorResponse) => {
                this.snackbar.errorResource('common.an-error-occurred-during-action');
            }
        });

        this.securityService.getUser().subscribe(user => {
            this.mustChangePassword = user.userMustChangePassword ?? false;
        });
    }

    public getErrorLabelText(controlName: string, error: any, errorCode: string): TLError | undefined {
        switch (errorCode) {
            case 'mustMatch':
                return new TLError({ text: this.translateService.getValue('auth.password-must-match') });
            case 'invalid':
                switch (controlName) {
                    case 'oldPasswordControl':
                        return new TLError({ text: this.serverError });
                    case 'passwordControl':
                        return new TLError({ text: this.serverError });
                }
        }

        return undefined;
    }

    public navigateToHome(): void {
        this.router.navigateByUrl('/');
    }

    public save(): void {
        this.form.markAllAsTouched();
        this.form.updateValueAndValidity({ onlySelf: true });

        if (this.form.valid) {
            const data = new UpdatePasswordModel();
            data.oldPassword = this.form.get('oldPasswordControl')?.value;
            data.newPassword = this.form.get('passwordControl')?.value;
            this.userService.updateUserPassword(data).subscribe({
                next: () => {
                    this.signOut();
                },
                error: (errorResponse: HttpErrorResponse) => {
                    TLUtils.handleResponseError(errorResponse, this.snackbar, this.handleServerError.bind(this));
                },
            });
        }
    }

    public cancel(): void {
        this.navigateToHome();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            oldPasswordControl: new FormControl(undefined, [Validators.required]),
            passwordControl: new FormControl(undefined, [Validators.required]),
            passwordConfirmControl: new FormControl(undefined, [Validators.required]),
        }, TLValidators.mustMatch('passwordControl', 'passwordConfirmControl'));
    }

    private signOut(): void {
        this.router.navigate(['/account/sign-out']);
    }

    private handleServerError(error: ErrorModel): void {
        let controlName: string = '';
        this.serverError = error.messages?.length === 1 ? error.messages[0] : error.messages.join(';');
        switch (error.code) {
            case ErrorCode.InvalidPassword:
                controlName = 'oldPasswordControl';
                break;
            case ErrorCode.OldPasswordFound:
                controlName = 'passwordControl';
                break;
            default:
                throw new Error('Not supported custom error');
        }
        const control = this.form.get(controlName)!;
        control.setErrors({ 'invalid': true });
        control.markAsTouched();
    }
}

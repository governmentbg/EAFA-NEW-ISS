import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { TLSnackbar } from '@app/shared/components/snackbar/tl.snackbar';
import { TRANSLATE_SERVICE_TOKEN } from '@app/shared/di/shared-di.tokens';
import { ITranslationService } from '@app/shared/interfaces/translate-service.interface';
import { TLUtils } from '@app/shared/utils';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { fuseAnimations } from '@fuse/animations';
import { USER_SERVICE_TOKEN } from '../../di/auth-di.tokens';
import { TokenStatus } from '../../enums/token-status.enum';
import { IUserService } from '../../interfaces/user-service.interface';
import { PasswordValidatorModel } from '../../models/auth/password-validator.model';
import { ChangeUserPasswordModel } from '../../models/auth/update-user-password.model';
import { PasswordUtils } from '../../utils/password.utils';


@Component({
    selector: 'reset-password-base',
    templateUrl: './reset-password-base.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations,
    host: { 'class': 'flex flex-col flex-auto w-full min-w-0' }
})
export class ResetPasswordBaseComponent implements OnInit {
    public form!: FormGroup;
    public passwordValidatorsLoaded: boolean = false;
    public responseMessage: string | undefined;
    public tokenStatus: TokenStatus | undefined;
    public TokenStatuses = TokenStatus;
    private userService: IUserService;
    private translateService: ITranslationService;
    private route: ActivatedRoute;
    private router: Router;
    private snackbar: TLSnackbar;
    private token!: string;
    private serverError: string = '';

    public constructor(@Inject(USER_SERVICE_TOKEN) userService: IUserService,
        @Inject(TRANSLATE_SERVICE_TOKEN) translateService: ITranslationService,
        snackbar: TLSnackbar,
        route: ActivatedRoute,
        router: Router) {
        this.userService = userService;
        this.translateService = translateService;
        this.route = route;
        this.router = router;
        this.snackbar = snackbar;
        this.buildForm();
    }

    public ngOnInit(): void {
        this.route.queryParams.subscribe({
            next: (params: Params) => {
                this.token = params['token'] as string;
                if (CommonUtils.isNullOrEmpty(this.token)) {
                    this.navigateToLogin();
                    return;
                }

                this.userService.checkTokenStatus(this.token!).subscribe({
                    next: (status: TokenStatus) => {
                        this.tokenStatus = status;
                    }
                });
            }
        });

        //this.userService.getPasswordValidators().subscribe({
        //    next: (validator: PasswordValidatorModel) => {
        //        this.form.controls.passwordControl.setValidators(PasswordUtils.buildPasswordValidator(validator));
        //        this.passwordValidatorsLoaded = true;
        //    },
        //    error: (error: HttpErrorResponse) => {
        //        this.snackbar.errorResource('common.an-error-occurred-during-action');
        //    }
        //});
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

    public resetPassword(): void {
        this.form.markAllAsTouched();
        this.form.updateValueAndValidity({ onlySelf: true });

        if (this.form.valid) {
            this.form.disable();
            const data = new ChangeUserPasswordModel();
            data.token = this.token;
            data.password = this.form.get('passwordControl')?.value;
            this.userService.changeUserPassword(data).subscribe({
                next: () => {
                    this.navigateToLogin();
                },
                error: (errorResponse: HttpErrorResponse) => {
                    this.form.enable();
                    TLUtils.handleResponseError(errorResponse, this.snackbar, this.handleServerError.bind(this));
                },
            });
        }
    }

    private buildForm(): void {
        this.form = new FormGroup({
            passwordControl: new FormControl(undefined, [Validators.required]),
            passwordConfirmControl: new FormControl(undefined, [Validators.required]),
        }, TLValidators.mustMatch('passwordControl', 'passwordConfirmControl'));
    }

    private navigateToLogin(): void {
        this.router.navigateByUrl('/account/sign-in');
    }

    private handleServerError(error: ErrorModel): void {
        let controlName: string = '';
        this.serverError = error.messages?.length === 1 ? error.messages[0] : error.messages.join(';');

        if (error.code == ErrorCode.OldPasswordFound) {
            controlName = 'passwordControl';
        } else {
            throw new Error('Not supported custom error');
        }

        const control = this.form.get(controlName)!;
        control.setErrors({ 'invalid': true });
        control.markAsTouched();
    }
}

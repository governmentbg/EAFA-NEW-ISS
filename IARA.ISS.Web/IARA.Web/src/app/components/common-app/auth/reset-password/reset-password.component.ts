import { Component, Inject, ViewEncapsulation } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { fuseAnimations } from '@fuse/animations';
import { TokenStatus } from '../enums/token-status.enum';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { ITranslationService } from '@app/shared/interfaces/translate-service.interface';
import { TRANSLATE_SERVICE_TOKEN } from '@app/shared/di/shared-di.tokens';
import { TLValidators } from '@app/shared/utils/tl-validators';

@Component({
    selector: 'auth-reset-password',
    templateUrl: './reset-password.component.html',
    styleUrls: ['./reset-password.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations,
})
export class AuthResetPasswordComponent {
    public form!: FormGroup;
    public tokenStatus: TokenStatus | undefined = TokenStatus.Valid;
    public TokenStatuses = TokenStatus;
    public passwordValidatorsLoaded: boolean = true;

    private serverError: string = '';
    private translateService: ITranslationService;

    public constructor(@Inject(TRANSLATE_SERVICE_TOKEN) translateService: ITranslationService) {
        this.translateService = translateService;
        this.buildForm();
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

        // if (this.form.valid) {
        //     this.form.disable();
        //     const data = new UpdateUserPasswordModel();
        //     data.token = this.token;
        //     data.password = this.form.get('passwordControl')?.value;
        //     this.userService.updateUserPassword(data).subscribe({
        //         next: () => {
        //             this.navigateToLogin();
        //         },
        //         error: (errorResponse: HttpErrorResponse) => {
        //             this.form.enable();
        //             TLUtils.handleResponseError(errorResponse, this.snackbar, this.handleServerError.bind(this));
        //         },
        //     });
        // }
    }

    private buildForm(): void {
        this.form = new FormGroup({
            passwordControl: new FormControl(undefined, [Validators.required]),
            passwordConfirmControl: new FormControl(undefined, [Validators.required]),
        }, TLValidators.mustMatch('passwordControl', 'passwordConfirmControl'));
    }
}

import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute, Params, Router } from "@angular/router";
import { UserChangePasswordDTO } from "@app/models/generated/dtos/UserChangePasswordDTO";
import { TLError } from "@app/shared/components/input-controls/models/tl-error.model";
import { CommonUtils } from "@app/shared/components/search-panel/utils";
import { AuthService } from "@app/shared/services/auth.service";
import { TLValidators } from "@app/shared/utils/tl-validators";
import { fuseAnimations } from "@fuse/animations";
import { FuseConfigService } from "@fuse/services/config.service";
import { FuseTranslationLoaderService } from "@fuse/services/translation-loader.service";


@Component({
    selector: 'change-password-page',
    templateUrl: './change-password-page.component.html',
    styleUrls: ['../user-registration-layout.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations
})
export class ChangePasswordPageComponent implements OnInit {
    public changePasswordForm!: FormGroup;

    private router: Router;
    private translationService: FuseTranslationLoaderService;
    private fuseConfigService: FuseConfigService;
    private route: ActivatedRoute;
    private authService: AuthService;
    private token!: string;

    public constructor(router: Router,
        translationService: FuseTranslationLoaderService,
        fuseConfigService: FuseConfigService,
        route: ActivatedRoute,
        authService: AuthService
    ) {
        this.router = router;
        this.translationService = translationService;
        this.fuseConfigService = fuseConfigService;
        this.route = route;
        this.authService = authService;

        // Configure the layout
        this.fuseConfigService.setConfig({
            layout: {
                navbar: {
                    hidden: true
                },
                toolbar: {
                    hidden: true
                },
                footer: {
                    hidden: true
                },
                sidepanel: {
                    hidden: true
                }
            }
        });

        this.changePasswordForm = new FormGroup({
            password: new FormControl('', [
                Validators.maxLength(200),
                Validators.required,
                TLValidators.passwordComplexityValidator()]),
            passwordConfirmation: new FormControl('', [
                Validators.maxLength(200),
                TLValidators.confirmPasswordValidator.bind(this),
                Validators.required,
                TLValidators.passwordComplexityValidator()
            ])
        });

        this.changePasswordForm.controls.password.valueChanges.subscribe(() => {
            this.changePasswordForm.controls.passwordConfirmation.updateValueAndValidity();
        });
    }

    public ngOnInit(): void {
        this.authService.checkAuthentication();
        this.route.queryParams.subscribe({
            next: (params: Params) => {
                this.token = params['token'] as string;

                if (CommonUtils.isNullOrEmpty(this.token)) {
                    this.navigateToRedirect();
                    return;
                }
            }
        });
    }

    public changePassword(): void {
        this.changePasswordForm.markAllAsTouched();

        if (this.changePasswordForm.valid) {
            const model = this.mapFormToModel();
            this.authService.changePassword(model).subscribe(() => {
                this.navigateToSuccessfulChange();
            });
        }
    }

    public getControlErrorLabelText(controlName: string, error: Record<string, unknown>, errorCode: string): TLError | undefined {
        switch (controlName) {
            case 'password':
            case 'passwordConfirmation': {
                switch (errorCode) {
                    case 'passwordsNotMatching':
                        return new TLError({
                            text: this.translationService.getValue('user-registration.passwords-must-match'),
                            type: 'error'
                        });
                    case 'required':
                        return new TLError({
                            text: this.translationService.getValue('user-registration.password-is-required'),
                            type: 'error'
                        });
                    default:
                        return undefined;
                }
            } break;
            default:
                return undefined;
        }
    }

    private mapFormToModel(): UserChangePasswordDTO {
        const userChangePassword: UserChangePasswordDTO = new UserChangePasswordDTO({
            token: this.token,
            password: this.changePasswordForm.controls.password.value
        });

        return userChangePassword;
    }

    private navigateToRedirect(): void {
        this.router.navigateByUrl('/redirect');
    }

    private navigateToSuccessfulChange(): void {
        this.router.navigateByUrl('/successful-change', { state: { token: this.token } });
    }
}
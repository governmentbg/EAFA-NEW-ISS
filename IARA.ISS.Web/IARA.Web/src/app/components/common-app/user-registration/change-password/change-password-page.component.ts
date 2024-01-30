import { Component, OnDestroy, OnInit, ViewEncapsulation } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute, Params, Router } from "@angular/router";
import { UsersService } from '@app/services/common-app/users.service';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { TLError } from "@app/shared/components/input-controls/models/tl-error.model";
import { CommonUtils } from "@app/shared/components/search-panel/utils";
import { TLValidators } from "@app/shared/utils/tl-validators";
import { fuseAnimations } from "@fuse/animations";
import { FuseConfigService } from "@fuse/services/config.service";
import { FuseTranslationLoaderService } from "@fuse/services/translation-loader.service";
import { ChangeUserPasswordModel } from '../../auth/models/auth/update-user-password.model';


@Component({
    selector: 'change-password-page',
    templateUrl: './change-password-page.component.html',
    styleUrls: ['../user-registration-layout.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations
})
export class ChangePasswordPageComponent implements OnInit, OnDestroy {
    public changePasswordForm!: FormGroup;

    public getControlErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelText.bind(this);

    private router: Router;
    private translationService: FuseTranslationLoaderService;
    private fuseConfigService: FuseConfigService;
    private route: ActivatedRoute;
    private token!: string;
    private securityService: UsersService;

    public constructor(router: Router,
        translationService: FuseTranslationLoaderService,
        fuseConfigService: FuseConfigService,
        route: ActivatedRoute,
        securityService: UsersService) {
        this.router = router;
        this.translationService = translationService;
        this.fuseConfigService = fuseConfigService;
        this.route = route;
        this.securityService = securityService;

        this.fuseConfigService.hidePanels();

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

    public ngOnDestroy(): void {
        this.fuseConfigService.restoreConfig();
    }

    public ngOnInit(): void {
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

            this.securityService.changeUserPassword(model).subscribe(() => {
                this.navigateToSuccessfulChange();
            });
        }
    }

    public getControlErrorLabelText(controlName: string, error: unknown, errorCode: string): TLError | undefined {
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

    private mapFormToModel(): ChangeUserPasswordModel {
        const userChangePassword: ChangeUserPasswordModel = new ChangeUserPasswordModel({
            token: this.token,
            password: this.changePasswordForm.controls.password.value
        });

        return userChangePassword;
    }

    private navigateToRedirect(): void {
        this.router.navigateByUrl('/');
    }

    private navigateToSuccessfulChange(): void {
        this.router.navigateByUrl('/successful-change', { state: { token: this.token } });
    }
}
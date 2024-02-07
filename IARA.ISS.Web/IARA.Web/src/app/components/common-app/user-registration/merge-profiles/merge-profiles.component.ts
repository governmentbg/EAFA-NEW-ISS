import { HttpErrorResponse } from "@angular/common/http";
import { Component, ViewEncapsulation } from "@angular/core";
import { FormControl, FormGroup, ValidationErrors, Validators } from "@angular/forms";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Router } from "@angular/router";
import { fuseAnimations } from "@fuse/animations";
import { FuseTranslationLoaderService } from "@fuse/services/translation-loader.service";
import { LoginTypesEnum } from "@app/enums/login-types.enum";
import { UserAuthDTO } from "@app/models/generated/dtos/UserAuthDTO";
import { UserLoginDTO } from "@app/models/generated/dtos/UserLoginDTO";
import { UserRegistrationDTO } from "@app/models/generated/dtos/UserRegistrationDTO";
import { AuthService } from "@app/shared/services/auth.service";
import { RequestProperties } from "@app/shared/services/request-properties";
import { CommonUtils } from '@app/shared/utils/common.utils';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';

@Component({
    selector: 'merge-profiles',
    templateUrl: './merge-profiles.component.html',
    styleUrls: ['../user-registration-layout.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations
})
export class MergeProfilesComponent {
    public readonly appearance: string = 'outline';
    public mergeProfilesForm!: FormGroup;
    public enteredIncorrectLoginData: boolean = false;

    public getControlErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelText.bind(this);

    private model: UserRegistrationDTO;
    private isInternalUser: boolean;
    private router: Router;
    private authService: AuthService;
    private translationService: FuseTranslationLoaderService;
    private snackbar: MatSnackBar;

    public constructor(
        router: Router,
        authService: AuthService,
        translationService: FuseTranslationLoaderService,
        snackbar: MatSnackBar
    ) {
        this.router = router;
        this.authService = authService;
        this.translationService = translationService;
        this.snackbar = snackbar;
        this.model = new UserRegistrationDTO();
        this.isInternalUser = this.authService.userRegistrationInfo?.isInternalUser ?? false;

        this.buildForm();
    }

    public deactivateOldAccount(): void {
        this.authService.deactivateUserPasswordAccount(this.mergeProfilesForm.get('egnControl')!.value).subscribe({
            next: () => {
                this.navigateToRegistrationPage();
            }
        });
    }

    public confirmAccountAndLogin(): void {
        this.mergeProfilesForm.markAllAsTouched();
        this.mergeProfilesForm.updateValueAndValidity({ emitEvent: false });

        if (this.isFormValid()) {
            const loginModel = new UserLoginDTO({
                email: this.mergeProfilesForm.get('emailControl')!.value,
                password: this.mergeProfilesForm.get('passwordControl')!.value,
                firstName: this.mergeProfilesForm.get('firstNameControl')!.value,
                middleName: this.mergeProfilesForm.get('middleNameControl')!.value,
                lastName: this.mergeProfilesForm.get('lastNameControl')!.value
            });

            this.authService.confirmEmailAndPassword(loginModel).subscribe({
                next: (value: boolean) => {
                    if (value) {
                        this.navigateToHomePage();
                    }
                    else {
                        this.enteredIncorrectLoginData = true;
                    }
                },
                error: (errorResponse: HttpErrorResponse) => {
                    this.somethingWentWrongSnackbar();
                }
            });
        }
    }

    public getControlErrorLabelText(controlName: string, errorValue: unknown, errorCode: string): TLError | undefined {
        const result: TLError | undefined = CommonUtils.getControlErrorLabelTextForRegixExpectedValueValidator(controlName, errorValue, errorCode);

        if (result !== undefined) {
            return result;
        }

        if (errorCode === 'egn') {
            if (errorValue === true) {
                return new TLError({ text: this.translationService.getValue('regix-data.invalid-egn'), type: 'warn' });
            }
        }
        else if (errorCode === 'pnf') {
            if (errorValue === true) {
                return new TLError({ text: this.translationService.getValue('regix-data.invalid-pnf'), type: 'warn' });
            }
        }
        else if (errorCode === 'eik') {
            if (errorValue === true) {
                return new TLError({ text: this.translationService.getValue('regix-data.invalid-eik'), type: 'warn' });
            }
        }

        return undefined;
    }

    private isFormValid(): boolean {
        if (this.mergeProfilesForm.valid) {
            return true;
        }
        else {
            const errors: ValidationErrors = {};

            for (const key of Object.keys(this.mergeProfilesForm.controls)) {
                if (key === 'egnControl') {
                    for (const error in this.mergeProfilesForm.controls[key].errors) {
                        if (!['egn', 'pnf', 'eik'].includes(error)) {
                            errors[error] = this.mergeProfilesForm.controls[key].errors![error];
                        }
                    }
                }
                else {
                    const controlErrors: ValidationErrors | null = this.mergeProfilesForm.controls[key].errors;
                    if (controlErrors !== null) {
                        errors[key] = controlErrors;
                    }
                }
            }

            return Object.keys(errors).length === 0 ? true : false;
        }

        return false;
    }

    private buildForm(): void {
        const userRegistrationInfo = this.authService.userRegistrationInfo;

        this.mergeProfilesForm = new FormGroup({
            firstNameControl: new FormControl(userRegistrationInfo?.firstName ?? undefined, [Validators.required, Validators.maxLength(100)]),
            middleNameControl: new FormControl(userRegistrationInfo?.middleName ?? undefined, [Validators.maxLength(100)]),
            lastNameControl: new FormControl(userRegistrationInfo?.lastName ?? undefined, [Validators.required, Validators.maxLength(100)]),
            egnControl: new FormControl(userRegistrationInfo?.egnLnc ?? undefined, [Validators.required, Validators.maxLength(20)]),
            emailControl: new FormControl(undefined, [Validators.required, Validators.email, Validators.maxLength(100)]),
            passwordControl: new FormControl(undefined, [Validators.required, Validators.maxLength(200)])
        });

        this.mergeProfilesForm.get('emailControl')!.valueChanges.subscribe({
            next: () => {
                this.enteredIncorrectLoginData = false;
            }
        });

        this.mergeProfilesForm.get('passwordControl')!.valueChanges.subscribe({
            next: () => {
                this.enteredIncorrectLoginData = false;
            }
        });
    }

    private mapFromToAuthModel(hasUserPassLogin: boolean): UserAuthDTO {
        const userAuthDTO: UserAuthDTO = new UserAuthDTO({
            firstName: this.mergeProfilesForm.get('firstNameControl')!.value,
            middleName: this.mergeProfilesForm.get('middleNameControl')!.value,
            lastName: this.mergeProfilesForm.get('lastNameControl')!.value,
            egnLnc: this.mergeProfilesForm.get('egnControl')!.value,
            hasEAuthLogin: true,
            hasUserPassLogin: hasUserPassLogin,
            currentLoginType: LoginTypesEnum.EAUTH,
            isInternalUser: this.isInternalUser
        });

        return userAuthDTO;
    }

    private somethingWentWrongSnackbar(): void {
        const message: string = this.translationService.getValue('merge-profiles.error-while-updating-user-registration');
        this.snackbar.open(message, undefined, {
            duration: RequestProperties.DEFAULT.showExceptionDurationErr,
            panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
        });
    }

    private navigateToRegistrationPage(): void {
        this.authService.userRegistrationInfo = this.mapFromToAuthModel(false);
        this.router.navigate(['/registration'], { state: { isFromMergeProfilesPage: true } });
    }

    private navigateToHomePage(): void {
        if (this.isInternalUser) {
            this.router.navigate(['/dashboard']);
        }
        else {
            this.router.navigate(['/news']);
        }

    }

}
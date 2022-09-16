import { HttpErrorResponse } from "@angular/common/http";
import { Component, ViewEncapsulation } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
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


@Component({
    selector: 'merge-profiles',
    templateUrl: './merge-profiles.component.html',
    styleUrls: ['../user-registration-layout.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations
})
export class MergeProfilesComponent {

    public mergeProfilesForm!: FormGroup;
    public enteredIncorrectLoginData: boolean = false;

    private model: UserRegistrationDTO;
    private isInternalUser: boolean;
    private router: Router;
    private authService: AuthService;
    private translationService: FuseTranslationLoaderService;
    private snackbar: MatSnackBar;

    public constructor(router: Router,
        authService: AuthService,
        translationService: FuseTranslationLoaderService,
        snackbar: MatSnackBar) {

        this.router = router;
        this.authService = authService;
        this.translationService = translationService;
        this.snackbar = snackbar;
        this.model = new UserRegistrationDTO();
        this.isInternalUser = this.authService.userRegistrationInfo?.isInternalUser ?? false;

        this.buildForm();
    }

    public deactivateOldAccount(): void {
        this.authService.deactivateUserPasswordAccount(this.mergeProfilesForm.controls.egn.value).subscribe({
            next: () => {
                this.navigateToRegistrationPage();
            }
        });
    }

    public confirmAccountAndLogin(): void {
        this.mergeProfilesForm.markAllAsTouched();
        if (this.mergeProfilesForm.valid) {
            const loginModel = new UserLoginDTO({
                email: this.mergeProfilesForm.controls.email.value,
                password: this.mergeProfilesForm.controls.password.value,
                firstName: this.mergeProfilesForm.controls.firstName.value,
                middleName: this.mergeProfilesForm.controls.middleName.value,
                lastName: this.mergeProfilesForm.controls.lastName.value
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

    private buildForm(): void {
        const userRegistrationInfo = this.authService.userRegistrationInfo;
        this.mergeProfilesForm = new FormGroup({
            firstName: new FormControl(userRegistrationInfo?.firstName ?? '', [Validators.required, Validators.maxLength(100)]),
            middleName: new FormControl(userRegistrationInfo?.middleName ?? '', [Validators.maxLength(100)]),
            lastName: new FormControl(userRegistrationInfo?.lastName ?? '', [Validators.required, Validators.maxLength(100)]),
            egn: new FormControl(userRegistrationInfo?.egnLnc ?? '', [Validators.required, Validators.maxLength(20)]),
            email: new FormControl('', [Validators.required, Validators.email, Validators.maxLength(100)]),
            password: new FormControl('', [Validators.required, Validators.maxLength(200)])
        });
    }

    private mapFromToAuthModel(): UserAuthDTO {
        const userAuthDTO: UserAuthDTO = new UserAuthDTO({
            firstName: this.mergeProfilesForm.controls.firstName.value,
            middleName: this.mergeProfilesForm.controls.middleName.value,
            lastName: this.mergeProfilesForm.controls.lastName.value,
            egnLnc: this.mergeProfilesForm.controls.egn.value,
            hasEAuthLogin: true,
            hasUserPassLogin: true,
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
        this.authService.userRegistrationInfo = this.mapFromToAuthModel();
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
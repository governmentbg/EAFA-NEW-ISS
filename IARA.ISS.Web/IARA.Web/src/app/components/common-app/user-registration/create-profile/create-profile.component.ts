import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { LoginTypesEnum } from '@app/enums/login-types.enum';
import { ErrorCode, ErrorModel, ErrorType } from '@app/models/common/exception.model';
import { UserAuthDTO } from '@app/models/generated/dtos/UserAuthDTO';
import { UserRegistrationDTO } from '@app/models/generated/dtos/UserRegistrationDTO';
import { UsersService } from '@app/services/common-app/users.service';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { TLSnackbar } from '@app/shared/components/snackbar/tl.snackbar';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfigService } from '@fuse/services/config.service';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';


@Component({
    selector: 'create-profile',
    templateUrl: './create-profile.component.html',
    styleUrls: ['../user-registration-layout.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations
})
export class CreateProfileComponent implements OnInit, OnDestroy {

    public registerForm!: FormGroup;
    public isEgnLnchReadonly: boolean = false;
    public readonly termsAndConditionsPageUrl: string[] = ['/terms-and-conditions'];
    public invalidEgnLnchServerMessage: string = '';
    public invalidEmailServerMessage: string = '';
    public invalidEgnLnch: boolean = false;

    public getControlErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelText.bind(this);
    
    private unsubscribeAll: Subject<unknown>;
    private fuseConfigService: FuseConfigService;
    private userService: UsersService;
    private router: Router;
    private translationService: FuseTranslationLoaderService;
    private model: UserRegistrationDTO;
    private snackbar: TLSnackbar;
    private isFromMergeProfilePage: boolean;

    public readonly useMultilineErrors = true;

    public constructor(fuseConfigService: FuseConfigService,
        userService: UsersService,
        router: Router,
        translationService: FuseTranslationLoaderService,
        snackbar: TLSnackbar) {

        this.fuseConfigService = fuseConfigService;
        this.userService = userService;
        this.router = router;
        this.translationService = translationService;
        this.snackbar = snackbar;
        this.unsubscribeAll = new Subject();
        this.model = new UserRegistrationDTO();

        const navigation = this.router.getCurrentNavigation();
        this.isFromMergeProfilePage = navigation?.extras?.state?.isFromMergeProfilesPage ?? false;

        this.registerForm = new FormGroup({
            firstName: new FormControl('', [Validators.required, Validators.maxLength(100)]),
            middleName: new FormControl('', [Validators.maxLength(100)]),
            lastName: new FormControl('', [Validators.required, Validators.maxLength(100)]),
            egnControl: new FormControl(null, [Validators.required]),
            email: new FormControl('', [Validators.required, Validators.email, Validators.maxLength(100)]),
            password: new FormControl('', [Validators.maxLength(200), TLValidators.passwordComplexityValidator()]),
            passwordConfirmation: new FormControl(null, [
                Validators.maxLength(200),
                TLValidators.confirmPasswordValidator.bind(this),
                TLValidators.passwordComplexityValidator()]),
            termsAndConditionsCheckbox: new FormControl(null, Validators.requiredTrue)
        });

        this.registerForm.controls.password.valueChanges.subscribe(() => {
            this.registerForm.controls.passwordConfirmation.updateValueAndValidity();
        });

        this.registerForm.controls.egnControl.valueChanges.subscribe(() => {
            this.invalidEgnLnch = false;
        });
    }

    public ngOnInit(): void {
        this.registerForm.get('password')!.valueChanges // Update the validity of the 'passwordConfirmation' field when the 'password' field changes
            .pipe(takeUntil(this.unsubscribeAll))
            .subscribe(() => {
                this.registerForm.get('passwordConfirmation')!.updateValueAndValidity();
            });

        const userAuth = this.userService.User!;

        if (this.isFromMergeProfilePage) {
            this.setupFirstTimeEAuthSettings(userAuth);
        }
        else {
            if (userAuth === undefined || userAuth === null) { // came form registration button
                this.model = this.mapAuthModelToModel(userAuth);
                this.addAdditionalRequiredValidators(); // person must enter email and password in order to registrate
                this.model.hasEAuthLogin = false;
            } else if (userAuth.currentLoginType === LoginTypesEnum.EAUTH) { // logged in with eAuth
                if (!userAuth.hasEAuthLogin && !userAuth.hasUserPassLogin) { // user doesn't have any account yet, so he must stay on the registration page and fill password
                    this.setupFirstTimeEAuthSettings(userAuth);
                }
            }
        }
    }

    public ngOnDestroy(): void {
        this.unsubscribeAll.next();
        this.unsubscribeAll.complete();
    }

    public registrationButtonClicked(): void {
        this.registerForm.markAllAsTouched();
        if (this.registerForm.valid && !this.invalidEgnLnch) {
            this.model = this.mapFormToModel();
            if (!CommonUtils.isNullOrEmpty(this.model.password)) {
                this.model.hasUserPassLogin = true;
            }

            if (this.isFromMergeProfilePage) {
                this.userService.updateUserRegistration(this.model).subscribe({
                    next: (id: number) => {
                        this.navigateToSuccessfulRegistrationPage();
                    },
                    error: (errorResponse: HttpErrorResponse) => {
                        const message: string = this.translationService.getValue('user-registration.error-while-updating-user-registration-info');
                        this.snackbar.error(message);
                    }
                });
            }
            else {
                this.userService.registerUser(this.model).subscribe({
                    next: (id: number) => {
                        this.navigateToSuccessfulRegistrationPage();
                    },
                    error: (errorResponse: HttpErrorResponse) => {
                        const error: ErrorModel | undefined = errorResponse.error as ErrorModel;
                        if (error !== null && error !== undefined) {
                            if (error.type === ErrorType.Unhandled) {
                                const errorMessage = this.translationService.getValue('user-registration.an-error-occurred-during-registration');
                                this.snackbar.error(errorMessage);
                            }
                            else {
                                if (error.code === ErrorCode.InvalidEgnLnch) {
                                    if (error.messages?.length === 1) {
                                        this.invalidEgnLnchServerMessage = error.messages[0];
                                    }
                                    else {
                                        this.invalidEgnLnchServerMessage = error.messages.join(';');
                                    }

                                    this.invalidEgnLnch = true;
                                    this.registerForm.updateValueAndValidity({ emitEvent: false });
                                }
                                else if (error.code === ErrorCode.InvalidEmail) {
                                    if (error.messages?.length === 1) {
                                        this.invalidEmailServerMessage = error.messages[0];
                                    }
                                    else {
                                        this.invalidEmailServerMessage = error.messages.join(';');
                                    }

                                    const email = this.registerForm.controls.email;
                                    email.setErrors({ 'invalid': true });
                                    email.markAsTouched();
                                }
                            }
                        }
                        else {
                            this.snackbar.error(this.translationService.getValue('service.an-error-occurred-in-the-app'));
                        }
                    }
                });
            }
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
            }
            case 'email': {
                switch (errorCode) {
                    case 'invalid':
                        return new TLError({
                            text: this.invalidEmailServerMessage,
                            type: 'error'
                        });
                    default:
                        return undefined;
                }
            }
            default: {
                return undefined;
            }
        }
    }

    private setupFirstTimeEAuthSettings(value: UserAuthDTO): void {
        this.model = this.mapAuthModelToModel(value);
        this.mapModelToForm(this.model);
        this.isEgnLnchReadonly = true;
        this.model.hasEAuthLogin = true;
    }

    private navigateToSuccessfulRegistrationPage(): void {
        this.router.navigate(['/successful-registration']);
    }

    private mapFormToModel(): UserRegistrationDTO {
        this.model.egnLnc = this.registerForm.controls.egnControl.value;
        this.model.firstName = this.registerForm.controls.firstName.value;
        this.model.middleName = this.registerForm.controls.middleName.value;
        this.model.lastName = this.registerForm.controls.lastName.value;
        this.model.email = this.registerForm.controls.email.value;
        this.model.password = this.registerForm.controls.password.value;

        return this.model;
    }

    private mapAuthModelToModel(user: UserAuthDTO): UserRegistrationDTO {

        if (user !== undefined && user !== null) {
            this.model.egnLnc = user.egnLnc;
            this.model.firstName = user.firstName;
            this.model.middleName = user.middleName;
            this.model.lastName = user.lastName;
            this.model.hasEAuthLogin = user.hasEAuthLogin;
            this.model.hasUserPassLogin = user.hasUserPassLogin;
            this.model.currentLoginType = user.currentLoginType;
        }
        return this.model;
    }

    private mapModelToForm(dto: UserRegistrationDTO): void {
        this.registerForm.controls.firstName.setValue(dto.firstName);
        this.registerForm.controls.middleName.setValue(dto.middleName);
        this.registerForm.controls.lastName.setValue(dto.lastName);
        this.registerForm.controls.egnControl.setValue(dto.egnLnc);
        this.registerForm.controls.email.setValue(dto.email);
    }

    private mapModelToAuthDTO(): UserAuthDTO {
        const authDto: UserAuthDTO = new UserAuthDTO({
            egnLnc: this.model.egnLnc,
            firstName: this.model.firstName,
            middleName: this.model.middleName,
            lastName: this.model.lastName,
            hasEAuthLogin: this.model.hasEAuthLogin,
            hasUserPassLogin: this.model.hasUserPassLogin,
            currentLoginType: this.model.currentLoginType
        });

        return authDto;
    }

    private addAdditionalRequiredValidators(): void {
        const passwordControl: FormControl = this.registerForm.controls.password as FormControl;
        if (passwordControl.validator !== null && passwordControl.validator !== undefined) {
            passwordControl.setValidators([passwordControl.validator, Validators.required]);
        }
        else {
            passwordControl.setValidators(Validators.required);
        }
        passwordControl.markAsPending();

        const passwordConfirmationControl: FormControl = this.registerForm.controls.passwordConfirmation as FormControl;
        if (passwordConfirmationControl.validator !== null && passwordConfirmationControl.validator !== undefined) {
            passwordConfirmationControl.setValidators([passwordConfirmationControl.validator, Validators.required]);
        }
        else {
            passwordConfirmationControl.setValidators(Validators.required);
        }
        passwordConfirmationControl.markAsPending();
    }
}

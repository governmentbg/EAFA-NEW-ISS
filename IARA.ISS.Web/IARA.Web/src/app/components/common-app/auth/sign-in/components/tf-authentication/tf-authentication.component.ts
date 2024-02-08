import { Component, EventEmitter, Inject, Input, OnInit, Output } from '@angular/core';
import { FormControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { TLSnackbar } from '@app/shared/components/snackbar/tl.snackbar';
import { TRANSLATE_SERVICE_TOKEN } from '@app/shared/di/shared-di.tokens';
import { ITranslationService } from '@app/shared/interfaces/translate-service.interface';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { TF_SECURITY_SERVICE_TOKEN } from '../../../di/auth-di.tokens';
import { AuthenticationOptionEnum } from '../../../enums/authentication-option.enum';
import { ITFSecurityService } from '../../../interfaces/tf-security-service.interface';
import { TFAuthenticationModel } from '../../../models/auth/tf-authentication.model';
import { AuthenticationUtils } from '../../../utils/authentication.utils';


@Component({
    selector: 'sign-in-tf-authentication',
    templateUrl: './tf-authentication.component.html'
})
export class TfAuthenticationComponent implements OnInit {
    @Input()
    public authenticationData!: TFAuthenticationModel;

    @Output()
    public validatedPIN: EventEmitter<void> = new EventEmitter<void>();
    public isPINValid: boolean = true;

    public readonly authenticationOptionsEnum: typeof AuthenticationOptionEnum = AuthenticationOptionEnum;

    public authCodeControl: FormControl;
    public authTypeControl: FormControl;

    public authenticationMethodSet: boolean = true;
    public authenticationUsed?: AuthenticationOptionEnum;

    public smsRecipient?: string;
    public emailRecipient?: string;
    public codeSendMsg!: string;
    public prompt!: string;
    public label!: string;
    public notReceived!: string;
    public chooseBtnLabel!: string;

    public allAuthenticationOptions: NomenclatureDTO<AuthenticationOptionEnum>[];
    public authenticationOptions: NomenclatureDTO<AuthenticationOptionEnum>[];


    private systemTFMethod?: AuthenticationOptionEnum;

    private readonly securityService: ITFSecurityService;
    private readonly translateService: ITranslationService;
    private readonly snackBar: TLSnackbar;

    public constructor(@Inject(TF_SECURITY_SERVICE_TOKEN) securityService: ITFSecurityService,
        @Inject(TRANSLATE_SERVICE_TOKEN) translateService: ITranslationService,
        snackBar: TLSnackbar) {
        this.securityService = securityService;
        this.translateService = translateService;
        this.snackBar = snackBar;

        this.allAuthenticationOptions = AuthenticationUtils.getAuthenticationOptions(this.translateService);

        this.authenticationOptions = this.allAuthenticationOptions.slice();

        this.authCodeControl = new FormControl(
            undefined,
            [Validators.required, TLValidators.exactLength(6), TLValidators.digitsOnly, this.checkPINValid()]
        );
        this.authTypeControl = new FormControl(
            undefined,
            [Validators.required]
        );

        this.authCodeControl.valueChanges.subscribe(() => {
            this.authCodeControl.updateValueAndValidity({
                emitEvent: false
            });
            this.isPINValid = true;
        });
    }

    public async ngOnInit(): Promise<void> {
        this.setAvailableMethods();
        this.translateDescriptions();
    }

    public verifyPIN(): void {
        this.authCodeControl.markAllAsTouched();
        if (this.authCodeControl.valid) {
            if (this.authenticationUsed === AuthenticationOptionEnum.AUTHENTICATOR) {
                this.securityService.verifyPin(this.authCodeControl!.value).subscribe({
                    next: (result: boolean) => {
                        this.showValidationResult(result);
                    }
                });
            } else {
                this.securityService.validateNonce(this.authCodeControl!.value).subscribe({
                    next: (result: boolean) => {
                        this.showValidationResult(result);
                    }
                });
            }
        }
    }

    public showMethods(): void {
        this.authenticationMethodSet = false;
    }

    public setMethod(): void {
        this.authenticationUsed = this.authTypeControl.value.value;
        this.translateDescriptions();
        this.authenticationMethodSet = true;
        if (this.authenticationUsed !== AuthenticationOptionEnum.AUTHENTICATOR) {
            this.sendMessage();
        }
    }

    public reSendMessage(): void {
        this.sendMessage();
        this.snackBar.success(this.translateService.getValue('auth.message-sent'));
    }

    public sendMessage(): void {
        if (this.authenticationUsed === AuthenticationOptionEnum.SMS) {
            this.securityService.sendSMSNonce().subscribe();
        } else if (this.authenticationUsed === AuthenticationOptionEnum.EMAIL) {
            this.securityService.sendEmailNonce().subscribe();
        }
    }

    public showInvalidError(controlName: string, error: unknown, errorCode: string): TLError | undefined {
        if (errorCode === 'invalidPIN') {
            return new TLError({
                text: this.translateService.getValue('auth.invalid-two-factor-pin'),
                type: 'error'
            });
        }

        return undefined;
    }

    private obfuscateData(): void {
        const smsOptionObject = this.authenticationOptions.find(obj => obj.value === AuthenticationOptionEnum.SMS);
        const emailOptionObject = this.authenticationOptions.find(obj => obj.value === AuthenticationOptionEnum.EMAIL);

        if (smsOptionObject) {
            this.smsRecipient = AuthenticationUtils.obfuscatePhone(this.authenticationData.phone!);
            smsOptionObject!.displayName += ` ${this.smsRecipient}`;
        }

        if (emailOptionObject) {
            this.emailRecipient = AuthenticationUtils.obfuscateEmail(this.authenticationData.email!);
            emailOptionObject!.displayName += ` ${this.emailRecipient}`;
        }

    }

    private setAvailableMethods(): void {
        if (
            this.authenticationData.phone === undefined ||
            this.authenticationData.phone === null ||
            this.authenticationData.phone === ''
        ) {
            this.authenticationOptions = this.authenticationOptions.filter(x => x.value !== AuthenticationOptionEnum.SMS);
        }

        if (!this.authenticationData.hasTFAuthenticator) {
            this.authenticationOptions = this.authenticationOptions.filter(x => x.value !== AuthenticationOptionEnum.AUTHENTICATOR);
        }

        this.obfuscateData();

        let tfMethod: AuthenticationOptionEnum | undefined;

        if (this.authenticationData.twoFactorMethod === undefined ||
            this.authenticationData.twoFactorMethod === null) {
            tfMethod = this.systemTFMethod;
        } else {
            tfMethod = this.authenticationData.twoFactorMethod;
        }

        if (tfMethod === AuthenticationOptionEnum.SMS
            && this.authenticationOptions.find(obj => obj.value === AuthenticationOptionEnum.SMS)) {

            this.authenticationUsed = AuthenticationOptionEnum.SMS;
            this.sendMessage();
        } else if (tfMethod === AuthenticationOptionEnum.EMAIL
            && this.authenticationOptions.find(obj => obj.value === AuthenticationOptionEnum.EMAIL)) {

            this.authenticationUsed = AuthenticationOptionEnum.EMAIL;
            this.sendMessage();
        } else if (tfMethod === AuthenticationOptionEnum.AUTHENTICATOR
            && this.authenticationOptions.find(obj => obj.value === AuthenticationOptionEnum.AUTHENTICATOR)) {

            this.authenticationUsed = AuthenticationOptionEnum.AUTHENTICATOR;
        } else {
            this.authenticationMethodSet = false;
        }

        if (this.authenticationMethodSet) {
            this.authTypeControl.setValue(this.authenticationOptions.find(obj => obj.value === this.authenticationUsed));
        }

        this.authTypeControl.valueChanges.subscribe(() => {
            this.chooseBtnLabel = this.translateService.getValue(
                (this.authTypeControl.value.value === AuthenticationOptionEnum.AUTHENTICATOR) ? 'auth.choose' : 'auth.send'
            );
        });
    }

    private translateDescriptions(): void {
        this.chooseBtnLabel = this.translateService.getValue(
            (this.authenticationUsed === AuthenticationOptionEnum.AUTHENTICATOR) ? 'auth.choose' : 'auth.send'
        );

        this.prompt = this.translateService.getValue(`auth.${AuthenticationOptionEnum[this.authenticationUsed!].toLowerCase()}-auth-prompt`);
        this.label = this.translateService.getValue(`auth.${AuthenticationOptionEnum[this.authenticationUsed!].toLowerCase()}-auth-label`);
        this.notReceived = this.translateService.getValue(`auth.${AuthenticationOptionEnum[this.authenticationUsed!].toLowerCase()}-auth-not-recieved`);

        if (this.authenticationUsed === AuthenticationOptionEnum.SMS) {
            const wasSent: string = this.translateService.getValue('auth.sms-auth-was-sent');

            this.codeSendMsg = `${wasSent} ${this.smsRecipient}`;

        } else if (this.authenticationUsed === AuthenticationOptionEnum.EMAIL) {
            const wasSend: string = this.translateService.getValue('auth.email-auth-was-sent');

            this.codeSendMsg = `${wasSend} ${this.emailRecipient}`;
        } else {
            this.codeSendMsg = '';
        }
    }

    private showValidationResult(result: boolean): void {
        if (result) {
            this.snackBar.success(this.translateService.getValue('auth.valid-two-factor-pin'));
            this.authCodeControl.reset();
            this.validatedPIN.emit();
        }
        else {
            this.isPINValid = false;
            this.authCodeControl.updateValueAndValidity();
            this.authCodeControl.markAsTouched();
        }
    }

    private checkPINValid(): ValidatorFn {
        return (): ValidationErrors | null => {
            if (this.isPINValid) {
                return null;
            }
            return {
                invalidPIN: true
            };
        };
    }
}

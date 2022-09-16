import { Component, ViewEncapsulation } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { fuseAnimations } from "@fuse/animations";
import { FuseTranslationLoaderService } from "@fuse/services/translation-loader.service";
import { Environment } from "../../../../../../environments/environment";
import { AuthService } from "@app/shared/services/auth.service";
import { RequestProperties } from "@app/shared/services/request-properties";
import { CommonUtils } from "@app/shared/utils/common.utils";



@Component({
    selector: 'successful-registration',
    templateUrl: './successful-registration.component.html',
    styleUrls: ['../success-pages.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations
})
export class SuccessfulRegistrationComponent {
    public homeUrl: string;
    public userEmail: string;

    private authService: AuthService;
    private snackbar: MatSnackBar;
    private translationService: FuseTranslationLoaderService;

    public constructor(authService: AuthService,
        snackbar: MatSnackBar,
        translationService: FuseTranslationLoaderService) {

        this.authService = authService;
        this.snackbar = snackbar;
        this.translationService = translationService;
        this.homeUrl = Environment.Instance.frontendBaseUrl;
        this.userEmail = this.authService.userEmail ?? '';
    }

    public resendConfirmationEmail(): void {
        if (!CommonUtils.isNullOrUndefined(this.authService?.userEmail)) {
            this.authService.resendConfirmationEmail(this.authService!.userEmail!).subscribe(() => {
                const message: string = this.translationService.getValue('successful-registration.confirmation-email-resent');
                this.snackbar.open(message, undefined, {
                    duration: RequestProperties.DEFAULT.showExceptionDurationSucc,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassSucc
                });
            });
        }
    }
}
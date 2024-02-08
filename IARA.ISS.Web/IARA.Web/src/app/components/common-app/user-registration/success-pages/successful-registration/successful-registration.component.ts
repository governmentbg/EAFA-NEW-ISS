import { Component, ViewEncapsulation } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";
import { fuseAnimations } from "@fuse/animations";
import { FuseTranslationLoaderService } from "@fuse/services/translation-loader.service";
import { Environment } from "../../../../../../environments/environment";
import { RequestProperties } from "@app/shared/services/request-properties";
import { CommonUtils } from "@app/shared/utils/common.utils";
import { UsersService } from '@app/services/common-app/users.service';



@Component({
    selector: 'successful-registration',
    templateUrl: './successful-registration.component.html',
    styleUrls: ['../success-pages.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations
})
export class SuccessfulRegistrationComponent {
    public homeUrl: string;
    public userEmail: string = '';

    private authService: UsersService;
    private snackbar: MatSnackBar;
    private translationService: FuseTranslationLoaderService;

    public constructor(userService: UsersService,
        snackbar: MatSnackBar,
        translationService: FuseTranslationLoaderService) {

        if (window.history.state !== undefined && window.history.state.email !== undefined) {
            this.userEmail = window.history.state.email;
        }
        this.authService = userService;
        this.snackbar = snackbar;
        this.translationService = translationService;
        this.homeUrl = Environment.Instance.frontendBaseUrl;
    }

    public resendConfirmationEmail(): void {
        if (!CommonUtils.isNullOrUndefined(this.userEmail)) {
            this.authService.resendConfirmationEmail(this.userEmail).subscribe(() => {
                const message: string = this.translationService.getValue('successful-registration.confirmation-email-resent');
                this.snackbar.open(message, undefined, {
                    duration: RequestProperties.DEFAULT.showExceptionDurationSucc,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassSucc
                });
            });
        }
    }
}

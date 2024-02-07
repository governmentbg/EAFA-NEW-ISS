import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { ActivatedRoute, Params, Router } from "@angular/router";
import { fuseAnimations } from "@fuse/animations";
import { FuseConfigService } from "@fuse/services/config.service";
import { AccountActivationStatusesEnum } from "@app/enums/account-activation-statuses.enum";
import { AuthService } from "@app/shared/services/auth.service";
import { CommonUtils } from "@app/shared/utils/common.utils";

const TIME_TO_WAIT = 5000; // ms

@Component({
    selector: 'successful-email-confirmation',
    templateUrl: './successful-email-confirmation.component.html',
    styleUrls: ['../success-pages.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations
})
export class SuccessfulEmailConfirmationComponent implements OnInit {
    public isAccountActivationSuccessful: boolean | undefined;

    private router: Router;
    private route: ActivatedRoute;
    private fuseConfigService: FuseConfigService;
    private token!: string;
    private authService: AuthService;

    public constructor(router: Router,
        route: ActivatedRoute,
        fuseConfigService: FuseConfigService,
        authService: AuthService
    ) {
        this.router = router;
        this.route = route;
        this.fuseConfigService = fuseConfigService;
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
    }

    public ngOnInit(): void {
        this.route.queryParams.subscribe({
            next: (params: Params) => {
                this.token = params['token'] as string;

                if (CommonUtils.isNullOrEmpty(this.token)) {
                    this.navigateToRedirect();
                    return;
                }

                this.authService.activateUserAccount(this.token!).subscribe({
                    next: (status: AccountActivationStatusesEnum) => {
                        switch (status) {
                            case AccountActivationStatusesEnum.Successful: {
                                this.isAccountActivationSuccessful = true;
                                setTimeout(() => {
                                    this.redirectToLogin();
                                }, TIME_TO_WAIT);
                            } break;
                            case AccountActivationStatusesEnum.TokenExpired: {
                                this.isAccountActivationSuccessful = false;
                            } break;
                            case AccountActivationStatusesEnum.TokenNonExistent: {
                                this.navigateToRedirect();
                            } break;
                            default: {
                                this.navigateToRedirect();
                            } break;
                        }
                    }
                });
            }
        });
    }

    public redirectToLogin(): void {
        this.navigateToRedirect();
    }

    public resendConfirmationEmail(): void {
        this.authService.resendConfirmationEmailForToken(this.token).subscribe(() => {
            this.navigateToSuccessfulRegistrationPage();
        });
    }

    private navigateToRedirect(): void {
        this.router.navigateByUrl('/redirect');
    }

    private navigateToSuccessfulRegistrationPage(): void {
        this.router.navigateByUrl('/successful-registration');
    }
}
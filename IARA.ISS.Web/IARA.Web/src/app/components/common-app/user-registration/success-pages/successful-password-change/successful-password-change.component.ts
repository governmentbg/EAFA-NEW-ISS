import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { fuseAnimations } from "@fuse/animations";
import { FuseConfigService } from "@fuse/services/config.service";
import { CommonUtils } from "@app/shared/utils/common.utils";

const TIME_TO_WAIT = 5000; // ms

@Component({
    selector: 'successful-password-change',
    templateUrl: './successful-password-change.component.html',
    styleUrls: ['../success-pages.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations
})
export class SuccessfulPasswordChangeComponent implements OnInit {

    private router: Router;
    private route: ActivatedRoute;
    private fuseConfigService: FuseConfigService;
    private token!: string;

    public constructor(router: Router,
        route: ActivatedRoute,
        fuseConfigService: FuseConfigService
    ) {
        this.router = router;
        this.route = route;
        this.fuseConfigService = fuseConfigService;

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
        const token: string | null | undefined = window.history?.state?.token;

        if (CommonUtils.isNullOrEmpty(token)) {
            this.navigateToRedirect();
            return;
        }

        setTimeout(() => {
            this.redirectToLogin();
        }, TIME_TO_WAIT);
    }

    public redirectToLogin(): void {
        this.navigateToRedirect();
    }

    private navigateToRedirect(): void {
        this.router.navigateByUrl('/redirect');
    }
}
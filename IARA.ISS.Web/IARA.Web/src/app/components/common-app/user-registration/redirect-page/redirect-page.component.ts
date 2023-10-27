import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Params } from '@angular/router';
import { AuthService } from "@app/shared/services/auth.service";
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';


@Component({
    selector: 'redirect-page',
    templateUrl: './redirect-page.component.html'
})
export class RedirectPageComponent implements OnInit {

    constructor(private authService: AuthService, private route: ActivatedRoute) { }

    ngOnInit(): void {
        this.route.queryParams.subscribe({
            next: (params: Params) => {
                if (Object.keys(params).length == 0) {
                    this.authService.checkAuthAndLogin().then(isAuthenticated => {
                        if (isAuthenticated) {
                            this.authService.redirectBasedOnUser();
                        } else if (IS_PUBLIC_APP) {
                            this.authService.redirectToHomePagePublicAppNotAuthenticated();
                        }
                    });
                } else {
                    // this.authService.checkAuthentication();
                    this.authService.isAuthenticatedEvent.subscribe(isAuthenticated => {
                        if (isAuthenticated) {
                            this.authService.redirectBasedOnUser();
                        }
                    });
                }
            }
        });


    }

}
import { Component } from "@angular/core";
import { AuthService } from '../../shared/services/auth.service';

@Component({
    template: '<h1>Unauthorized</h1>',
    selector: 'unauthorized'
})
export class UnauthorizedComponent {

    constructor(private authService: AuthService) { }

    ngOnInit(): void {
        if (this.authService.isAuthenticatedEvent.value) {
            this.checkUserInfoAndRedirect();
        } else {
            this.authService.isAuthenticatedEvent.subscribe(result => {
                if (result) {
                    this.checkUserInfoAndRedirect();
                }
            });
        }
    }

    private checkUserInfoAndRedirect() {
        if (this.authService.userRegistrationInfo == undefined) {
            this.authService.getUserAuthInfo().subscribe(result => {
                if (result) {
                    this.authService.redirectBasedOnUser();
                }
            });
        } else {
            this.authService.redirectBasedOnUser();
        }
    }


}
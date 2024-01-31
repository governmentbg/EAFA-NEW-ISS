import { AfterViewInit, Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UsersService } from '@app/services/common-app/users.service';

@Component({
    selector: 'successful-email-confirmation',
    templateUrl: './successful-email-confirmation.component.html',
})
export class SuccessfulEmailConfirmationComponent implements AfterViewInit{
    private readonly userService!: UsersService;
    private readonly route: ActivatedRoute;

    public constructor(userService: UsersService, route: ActivatedRoute) {
        this.userService = userService;
        this.route = route;
    }

    public ngAfterViewInit(): void {
        this.route.queryParams.subscribe(params => {
            console.log(params['token']);
            this.userService.confirmEmail(params['token']).subscribe();
        });
    }
}

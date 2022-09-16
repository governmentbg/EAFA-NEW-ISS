import { Component } from "@angular/core";
import { MyProfilePublicService } from "@app/services/public-app/my-profile-public.service";

@Component({
    selector: 'my-profile',
    templateUrl: './my-profile-public.component.html'
})
export class MyProfilePublicComponent {
    public service: MyProfilePublicService;

    public constructor(service: MyProfilePublicService) {
        this.service = service;
    }
}
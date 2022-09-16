import { Component } from '@angular/core';
import { MyProfileAdministrationService } from '@app/services/administration-app/my-profile-administration.service';

@Component({
    selector: 'my-profile',
    templateUrl: './my-profile-administration.component.html'
})
export class MyProfileAdministrationComponent {
    public service: MyProfileAdministrationService;

    public constructor(service: MyProfileAdministrationService) {
        this.service = service;
    }
}
import { Component } from '@angular/core';
import { ApplicationsAdministrationService } from '@app/services/administration-app/applications-administration.service';
import { RecreationalFishingAdministrationService } from '@app/services/administration-app/recreational-fishing-administration.service';

@Component({
    selector: 'recreational-fishing-applications',
    templateUrl: './recreational-fishing-applications.component.html'
})
export class RecreationalFishingApplicationsComponent {
    public service: RecreationalFishingAdministrationService;
    public applicationsService: ApplicationsAdministrationService;

    public constructor(service: RecreationalFishingAdministrationService, applicationsService: ApplicationsAdministrationService) {
        this.service = service;
        this.applicationsService = applicationsService;
    }
}
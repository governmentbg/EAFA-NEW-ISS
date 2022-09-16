import { Component } from '@angular/core';
import { ApplicationsAdministrationService } from '@app/services/administration-app/applications-administration.service';
import { DeliveryAdministrationService } from '@app/services/administration-app/delivery-administration.service';
import { ScientificFishingAdministrationService } from '@app/services/administration-app/scientific-fishing-administration.service';

@Component({
    selector: 'scientific-fishing',
    templateUrl: './scientific-fishing.component.html'
})
export class ScientificFishingComponent {
    public service: ScientificFishingAdministrationService;
    public applicationsService: ApplicationsAdministrationService;
    public deliveryService: DeliveryAdministrationService;

    public constructor(service: ScientificFishingAdministrationService,
        applicationsService: ApplicationsAdministrationService,
        deliveryService: DeliveryAdministrationService
    ) {
        this.service = service;
        this.applicationsService = applicationsService;
        this.deliveryService = deliveryService;
    }
}
import { Component } from '@angular/core';
import { ApplicationsPublicService } from '@app/services/public-app/applications-public.service';
import { DeliveryPublicService } from '@app/services/public-app/delivery-public.service';
import { ScientificFishingPublicService } from '@app/services/public-app/scientific-fishing-public.service';

@Component({
    selector: 'scientific-fishing',
    templateUrl: './scientific-fishing.component.html'
})
export class ScientificFishingComponent {
    public service: ScientificFishingPublicService;
    public applicationsService: ApplicationsPublicService;
    public deliveryService: DeliveryPublicService;

    public constructor(service: ScientificFishingPublicService,
        applicationsService: ApplicationsPublicService,
        deliveryService: DeliveryPublicService
    ) {
        this.service = service;
        this.applicationsService = applicationsService;
        this.deliveryService = deliveryService;
    }
}
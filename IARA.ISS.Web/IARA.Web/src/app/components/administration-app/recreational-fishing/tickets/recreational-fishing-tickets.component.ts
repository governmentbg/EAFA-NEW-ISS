import { Component } from '@angular/core';
import { RecreationalFishingAdministrationService } from '@app/services/administration-app/recreational-fishing-administration.service';

@Component({
    selector: 'recreational-fishing-tickets',
    templateUrl: './recreational-fishing-tickets.component.html'
})
export class RecreationalFishingTicketsComponent {
    public service: RecreationalFishingAdministrationService;

    public constructor(service: RecreationalFishingAdministrationService) {
        this.service = service;
    }
}
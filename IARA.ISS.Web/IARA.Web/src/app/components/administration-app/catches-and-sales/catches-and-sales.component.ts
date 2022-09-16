import { Component } from '@angular/core';

import { CatchesAndSalesAdministrationService } from '@app/services/administration-app/catches-and-sales-administration.service';

@Component({
    selector: 'catches-and-sales',
    templateUrl: './catches-and-sales.component.html'
})
export class CatchesAndSalesComponent {
    public service: CatchesAndSalesAdministrationService;

    public constructor(service: CatchesAndSalesAdministrationService) {
        this.service = service;
    }
}
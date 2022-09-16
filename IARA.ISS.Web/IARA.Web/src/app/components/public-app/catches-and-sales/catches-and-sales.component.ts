import { Component } from '@angular/core';

import { CatchesAndSalesPublicService } from '@app/services/public-app/catches-and-sales-public.service';

@Component({
    selector: 'catches-and-sales',
    templateUrl: './catches-and-sales.component.html'
})
export class CatchesAndSalesPublicComponent {
    public service: CatchesAndSalesPublicService;

    public constructor(service: CatchesAndSalesPublicService) {
        this.service = service;
    }
}
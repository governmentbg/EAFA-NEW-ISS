import { Component } from '@angular/core';
import { StatisticalFormsPublicService } from '@app/services/public-app/statistical-forms-public.service';

@Component({
    selector: 'statistical-forms',
    templateUrl: './statistical-forms.component.html'
})
export class StatisticalFormsComponent {
    public service: StatisticalFormsPublicService;

    public constructor(service: StatisticalFormsPublicService) {
        this.service = service;
    }
}
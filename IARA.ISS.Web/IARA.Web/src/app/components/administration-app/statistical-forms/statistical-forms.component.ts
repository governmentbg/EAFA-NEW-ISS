import { Component } from '@angular/core';
import { StatisticalFormsAdministrationService } from '@app/services/administration-app/statistical-forms-administration.service';

@Component({
    selector: 'statistical-forms',
    templateUrl: './statistical-forms.component.html'
})
export class StatisticalFormsComponent {
    public service: StatisticalFormsAdministrationService;

    public constructor(service: StatisticalFormsAdministrationService) {
        this.service = service;
    }
}
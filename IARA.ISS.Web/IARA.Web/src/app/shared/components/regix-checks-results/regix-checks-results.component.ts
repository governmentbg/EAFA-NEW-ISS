import { Component, Input } from "@angular/core";
import { ApplicationRegiXCheckDTO } from "@app/models/generated/dtos/ApplicationRegiXCheckDTO";


@Component({
    selector: 'regix-checks-results',
    templateUrl: './regix-checks-results.component.html'
})
export class RegixChecksResultsComponent {
    @Input()
    public regixChecksCollection: ApplicationRegiXCheckDTO[] = [];

    @Input()
    public recordsPerPage: number = 5;
}
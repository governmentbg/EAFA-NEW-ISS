import { Component } from '@angular/core';
import { ReportAdministrationService } from '@app/services/administration-app/report-administration.service';

@Component({
    selector: 'report-view',
    templateUrl: './report-view.component.html'
})
export class ReportViewComponent {
    public readonly reportService: ReportAdministrationService;

    public constructor(reportService: ReportAdministrationService) {
        this.reportService = reportService;
    }
}
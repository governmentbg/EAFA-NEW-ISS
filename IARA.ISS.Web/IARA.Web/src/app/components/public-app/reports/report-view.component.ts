import { Component } from '@angular/core';
import { ReportPublicService } from '@app/services/public-app/report-public.service';

@Component({
    selector: 'report-view',
    templateUrl: './report-view.component.html'
})
export class ReportViewComponent {
    public readonly reportService: ReportPublicService;

    public constructor(reportService: ReportPublicService) {
        this.reportService = reportService;
    }
}
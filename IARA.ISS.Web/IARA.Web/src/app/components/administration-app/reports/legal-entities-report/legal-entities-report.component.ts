import { Component } from '@angular/core';
import { ReportTypeEnum } from '@app/enums/report-type.enum';

@Component({
    selector: 'legal-entities-report',
    templateUrl: './legal-entities-report.component.html'
})
export class LegalEntitiesReportComponent {
    public reportTypeEnum: typeof ReportTypeEnum = ReportTypeEnum;
}
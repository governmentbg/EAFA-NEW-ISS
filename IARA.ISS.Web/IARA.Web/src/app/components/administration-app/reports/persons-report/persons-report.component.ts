import { Component } from '@angular/core';
import { ReportTypeEnum } from '@app/enums/report-type.enum';

@Component({
    selector: 'persons-report',
    templateUrl: './persons-report.component.html'
})
export class PersonsReportComponent {
    public reportTypeEnum: typeof ReportTypeEnum = ReportTypeEnum;
}
import { Component, Input } from '@angular/core';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TypesCountReportDTO } from '@app/models/generated/dtos/TypesCountReportDTO';

@Component({
    selector: 'dashboard-card',
    templateUrl: './dashboard-card.component.html',
    styleUrls: ['./dashboard.component.scss']
})
export class DashboardCardComponent {
    @Input()
    public titleSize: number = 1.1;
    @Input()
    public cardFlex: string = '100';


    @Input()
    public data!: TypesCountReportDTO;

    private tlTranslationService: FuseTranslationLoaderService;

    public constructor(tlTranslationService: FuseTranslationLoaderService) {
        this.tlTranslationService = tlTranslationService;
    }
}


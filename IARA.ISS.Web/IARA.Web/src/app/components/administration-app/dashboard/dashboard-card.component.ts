import { Component, Input } from '@angular/core';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TypesCountReportDTO } from '@app/models/generated/dtos/TypesCountReportDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { Router } from '@angular/router';
import { TicketTypesCountReportDTO } from '@app/models/generated/dtos/TicketTypesCountReportDTO';

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
    public data!: TypesCountReportDTO | TicketTypesCountReportDTO;

    @Input()
    public isTicket: boolean = false;

    private tlTranslationService: FuseTranslationLoaderService;
    private router: Router;

    public constructor(
        tlTranslationService: FuseTranslationLoaderService,
        router: Router
    ) {
        this.tlTranslationService = tlTranslationService;
        this.router = router;
    }

    public navigateToApplicationPageByType(): void {
        if (!this.isTicket) {
            switch ((this.data as TypesCountReportDTO).pageCode) {
                case PageCodeEnum.SciFi:
                    this.navigateByUrl('/scientific-fishing-applications'); break;
                case PageCodeEnum.ChangeFirstSaleBuyer:
                case PageCodeEnum.ChangeFirstSaleCenter:
                case PageCodeEnum.RegFirstSaleBuyer:
                case PageCodeEnum.RegFirstSaleCenter:
                case PageCodeEnum.TermFirstSaleBuyer:
                case PageCodeEnum.TermFirstSaleCenter:
                case PageCodeEnum.DupFirstSaleBuyer:
                case PageCodeEnum.DupFirstSaleCenter:
                    this.navigateByUrl('/sales-centers-applications'); break;
                case PageCodeEnum.DeregShip:
                case PageCodeEnum.RegVessel:
                case PageCodeEnum.ShipRegChange:
                    this.navigateByUrl('/fishing-vessels-applications'); break;
                case PageCodeEnum.LE:
                case PageCodeEnum.Assocs:
                    this.navigateByUrl('/legal-entities-applications'); break;
                case PageCodeEnum.AptitudeCourceExam:
                case PageCodeEnum.CommFishLicense:
                case PageCodeEnum.CompetencyDup:
                    this.navigateByUrl('/qualified-fishers-applications'); break;
                case PageCodeEnum.CommFish:
                case PageCodeEnum.RightToFishResource:
                case PageCodeEnum.RightToFishThirdCountry:
                case PageCodeEnum.PoundnetCommFish:
                case PageCodeEnum.PoundnetCommFishLic:
                case PageCodeEnum.CatchQuataSpecies:
                case PageCodeEnum.DupCommFish:
                case PageCodeEnum.DupRightToFishThirdCountry:
                case PageCodeEnum.DupPoundnetCommFish:
                case PageCodeEnum.DupRightToFishResource:
                case PageCodeEnum.DupPoundnetCommFishLic:
                case PageCodeEnum.DupCatchQuataSpecies:
                case PageCodeEnum.FishingGearsCommFish:
                    this.navigateByUrl('/commercial-fishing-applications'); break;
                case PageCodeEnum.AquaFarmReg:
                case PageCodeEnum.AquaFarmChange:
                case PageCodeEnum.AquaFarmDereg:
                    this.navigateByUrl('/aquaculture-farms-applications'); break;
                case PageCodeEnum.ReduceFishCap:
                case PageCodeEnum.IncreaseFishCap:
                case PageCodeEnum.TransferFishCap:
                case PageCodeEnum.CapacityCertDup:
                    this.navigateByUrl('/fishing-capacity-applications'); break;
                case PageCodeEnum.StatFormAquaFarm:
                case PageCodeEnum.StatFormRework:
                case PageCodeEnum.StatFormFishVessel:
                    this.navigateByUrl('/statistical-forms-applications'); break;
            default:
                    this.navigateByUrl('/application_processing'); break;
            }
        }
        else {
            this.router.navigateByUrl('/ticket_applications', { state: { ticketTypeId: (this.data as TicketTypesCountReportDTO).typeId } });
        }
    }

    private navigateByUrl(url: string): void {
        this.router.navigateByUrl(url, { state: { typeId: (this.data as TypesCountReportDTO).typeId } });
    }
}


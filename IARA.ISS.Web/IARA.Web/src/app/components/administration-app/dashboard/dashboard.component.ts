import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import {
    ApexAxisChartSeries,
    ApexChart,
    ApexDataLabels,
    ApexPlotOptions,
    ApexResponsive,
    ApexXAxis,
    ApexLegend,
    ApexFill
} from 'ng-apexcharts';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IApplicationsRegisterService } from '@app/interfaces/administration-app/applications-register.interface';
import { IDashboardService } from '@app/interfaces/administration-app/dashboard.interface';
import { ApplicationRegisterDTO } from '@app/models/generated/dtos/ApplicationRegisterDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ApplicationsRegisterFilters } from '@app/models/generated/filters/ApplicationsRegisterFilters';
import { ApplicationsProcessingService } from '@app/services/administration-app/applications-processing.service';
import { DashboardService } from '@app/services/administration-app/dashboard.service';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { ApplicationsTableComponent, ApplicationTablePageType } from '@app/components/common-app/applications/applications-table/applications-table.component';
import { ApplicationProcessingHasPermissions } from '@app/components/common-app/applications/models/application-processing-has-permissions.model';
import { StatusCountReportDataDTO } from '@app/models/generated/dtos/StatusCountReportDataDTO';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { TypesCountReportDTO } from '@app/models/generated/dtos/TypesCountReportDTO';
import { IRecreationalFishingService } from '@app/interfaces/common-app/recreational-fishing.interface';
import { RecreationalFishingAdministrationService } from '@app/services/administration-app/recreational-fishing-administration.service';
import { TicketTypesCountReportDTO } from '@app/models/generated/dtos/TicketTypesCountReportDTO';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export type ChartOptions = {
    series: ApexAxisChartSeries;
    chart: ApexChart;
    dataLabels: ApexDataLabels;
    plotOptions: ApexPlotOptions;
    responsive: ApexResponsive[];
    xaxis: ApexXAxis;
    legend: ApexLegend;
    fill: ApexFill;
};

type ThreeState = 'yes' | 'no' | 'both';

@Component({
    selector: 'dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent<T extends IDialogComponent> implements AfterViewInit {

    public pageType: ApplicationTablePageType = 'DashboardPage';

    public service!: IDashboardService;
    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public statuses!: NomenclatureDTO<number>[];
    public types!: NomenclatureDTO<number>[];
    public sources!: NomenclatureDTO<number>[];
    public showOnlyAssignedOptions!: NomenclatureDTO<ThreeState>[];

    public recreationalFishingService: IRecreationalFishingService;

    public applicationProcessingHasPermissions: Map<PageCodeEnum, ApplicationProcessingHasPermissions>;
    public applicationRegisterService!: IApplicationsRegisterService;

    public hasApplicationsProcessingPermission: boolean = false;
    public hasTicketsReadPermission: boolean = false;
    public hasAnyApplicationsReadPermission: boolean = false;

    public applicationChartOptions!: Partial<ChartOptions>;

    public ticketChartOptions!: Partial<ChartOptions>;

    public typesCountReports!: TypesCountReportDTO[];
    public ticketTypesCountReports!: TicketTypesCountReportDTO[];

    private applicationsStatusCountReports!: StatusCountReportDataDTO;
    private ticketsStatusCountReports!: StatusCountReportDataDTO;

    private grid!: DataTableManager<ApplicationRegisterDTO, ApplicationsRegisterFilters>;
    private permissionsService: PermissionsService;
    private getAllServiceMethod: (request: GridRequestModel<ApplicationsRegisterFilters>) => Observable<GridResultModel<ApplicationRegisterDTO>>;

    @ViewChild(ApplicationsTableComponent)
    private applicationsTable!: ApplicationsTableComponent<T>;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    public constructor(translate: FuseTranslationLoaderService,
        service: DashboardService,
        applicationRegisterService: ApplicationsProcessingService,
        permissionsService: PermissionsService,
        recreationalFishingAdministrationService: RecreationalFishingAdministrationService
    ) {
        this.translate = translate;
        this.service = service;
        this.applicationRegisterService = applicationRegisterService;
        this.permissionsService = permissionsService;
        this.recreationalFishingService = recreationalFishingAdministrationService;

        this.hasApplicationsProcessingPermission = this.permissionsService.has(PermissionsEnum.ApplicationsRead);
        this.hasTicketsReadPermission = this.permissionsService.has(PermissionsEnum.TicketApplicationsRead);

        this.hasAnyApplicationsReadPermission = this.permissionsService.has(PermissionsEnum.QualifiedFishersApplicationsRead)
            || this.permissionsService.has(PermissionsEnum.ScientificFishingApplicationsRead)
            || this.permissionsService.has(PermissionsEnum.LegalEntitiesApplicationsRead)
            || this.permissionsService.has(PermissionsEnum.BuyersApplicationsRead)
            || this.permissionsService.has(PermissionsEnum.ShipsRegisterApplicationsRead)
            || this.permissionsService.has(PermissionsEnum.CommercialFishingPermitApplicationsRead)
            || this.permissionsService.has(PermissionsEnum.AquacultureFacilitiesApplicationsRead)
            || this.permissionsService.has(PermissionsEnum.FishingCapacityApplicationsRead)
            || this.permissionsService.has(PermissionsEnum.StatisticalFormsAquaFarmApplicationsRead)
            || this.permissionsService.has(PermissionsEnum.StatisticalFormsFishVesselsApplicationsRead)
            || this.permissionsService.has(PermissionsEnum.StatisticalFormsReworkApplicationsRead);

        if (this.hasApplicationsProcessingPermission === true) {
            this.getAllServiceMethod = this.service.getAllApplications.bind(this.service);
        }
        else {
            this.getAllServiceMethod = this.service.getAllApplicationsByUserId.bind(this.service);
        }

        this.applicationProcessingHasPermissions = new Map<PageCodeEnum, ApplicationProcessingHasPermissions>();

        this.buildForm();
    }

    public ngOnInit(): void {
        if (this.hasAnyApplicationsReadPermission || this.hasApplicationsProcessingPermission) {
            this.applicationRegisterService.getApplicationStatuses().subscribe({
                next: (statuses: NomenclatureDTO<number>[]) => {
                    this.statuses = statuses;
                }
            });

            this.applicationRegisterService.getApplicationTypes().subscribe({
                next: (types: NomenclatureDTO<number>[]) => {
                    this.types = types;
                }
            });

            this.applicationRegisterService.getApplicationSources().subscribe({
                next: (sources: NomenclatureDTO<number>[]) => {
                    this.sources = sources;
                }
            });

            this.showOnlyAssignedOptions = [
                new NomenclatureDTO<ThreeState>({
                    value: 'yes',
                    displayName: this.translate.getValue('applications-register.assigned-only'),
                    isActive: true
                }),
                new NomenclatureDTO<ThreeState>({
                    value: 'no',
                    displayName: this.translate.getValue('applications-register.not-assigned-only'),
                    isActive: true
                }),
                new NomenclatureDTO<ThreeState>({
                    value: 'both',
                    displayName: this.translate.getValue('applications-register.assigned-show-all'),
                    isActive: true
                }),
            ];

            this.service.getStatusCountReportData(!this.hasApplicationsProcessingPermission, false).subscribe({
                next: (result: StatusCountReportDataDTO) => {
                    this.applicationsStatusCountReports = result;
                    this.applicationChartOptions = this.buildChart(this.applicationsStatusCountReports);
                }
            });

            this.service.getTypesCountReport(!this.hasApplicationsProcessingPermission).subscribe({
                next: (result: TypesCountReportDTO[]) => {
                    this.typesCountReports = result;
                }
            });
        }

        if (this.hasTicketsReadPermission) {
            this.service.getStatusCountReportData(false, true).subscribe({
                next: (result: StatusCountReportDataDTO) => {
                    this.ticketsStatusCountReports = result;
                    this.ticketChartOptions = this.buildChart(this.ticketsStatusCountReports);
                }
            });

            this.service.getTicketTypesCountReport().subscribe({
                next: (result: TicketTypesCountReportDTO[]) => {
                    this.ticketTypesCountReports = result;
                }
            });
        }
    }

    public ngAfterViewInit(): void {
        if (this.hasAnyApplicationsReadPermission || this.hasApplicationsProcessingPermission) {
            this.grid = new DataTableManager<ApplicationRegisterDTO, ApplicationsRegisterFilters>({
                tlDataTable: this.applicationsTable.datatable,
                searchPanel: this.searchpanel,
                requestServiceMethod: this.getAllServiceMethod,
                filtersMapper: this.mapFilters.bind(this)
            });

            this.grid.refreshData();
        }
    }

    private buildForm(): void {
        this.form = new FormGroup({
            eventisNumControl: new FormControl(),
            applicationDateControl: new FormControl(),
            applicationTypeControl: new FormControl(),
            applicationStatusControl: new FormControl(),
            applicationSourceControl: new FormControl(),
            submittedForControl: new FormControl(),
            submittedForEgnLncControl: new FormControl(),
            showOnlyAssignedControl: new FormControl(),
            assignedToControl: new FormControl()
        });
    }

    private buildChart(statusCountReports: StatusCountReportDataDTO): Partial<ChartOptions> {
        return {
            series: statusCountReports.series as ApexAxisChartSeries,
            chart: {
                type: 'bar',
                height: 400,
                stacked: true,
                toolbar: {
                    show: true
                },
                zoom: {
                    enabled: true
                },
                animations: {
                    enabled: false
                }
            },
            responsive: [
                {
                    breakpoint: 480,
                    options: {
                        legend: {
                            position: 'bottom',
                            offsetX: -10,
                            offsetY: 0
                        }
                    }
                }
            ],
            plotOptions: {
                bar: {
                    horizontal: false
                }
            },
            xaxis: {
                type: 'category',
                categories: statusCountReports.categories
            },
            legend: {
                position: 'bottom',
                offsetX: 40
            },
            fill: {
                opacity: 1
            }
        };
    }

    private mapFilters(filters: FilterEventArgs): ApplicationsRegisterFilters {
        const result = new ApplicationsRegisterFilters({
            freeTextSearch: filters.searchText,
            showOnlyNotFinished: true,

            eventisNum: filters.getValue('eventisNumControl'),
            applicationTypeId: filters.getValue('applicationTypeControl'),
            applicationStatusId: filters.getValue('applicationStatusControl'),
            dateFrom: filters.getValue<DateRangeData>('applicationDateControl')?.start,
            dateTo: filters.getValue<DateRangeData>('applicationDateControl')?.end,
            submittedFor: filters.getValue('submittedForControl'),
            submittedForEgnLnc: filters.getValue('submittedForEgnLncControl'),
            applicationSourceId: filters.getValue('applicationSourceControl'),
        });

        const showAssigned = filters.getValue<ThreeState>('showOnlyAssignedControl');
        if (showAssigned !== undefined && showAssigned !== null) {
            switch (showAssigned) {
                case 'yes':
                    result.showAssignedApplications = true;
                    break;
                case 'no':
                    result.showAssignedApplications = false;
                    break;
                case 'both':
                    result.showAssignedApplications = undefined;
                    break;
            }
        }
        result.assignedTo = filters.getValue('assignedToControl');

        return result;
    }
}
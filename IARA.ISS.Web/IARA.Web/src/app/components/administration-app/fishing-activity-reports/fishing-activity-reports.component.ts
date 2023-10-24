import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { FishingActivityReportDTO } from '@app/models/generated/dtos/FishingActivityReportDTO';
import { FishingActivityReportItemDTO } from '@app/models/generated/dtos/FishingActivityReportItemDTO';
import { FishingActivityReportPageDTO } from '@app/models/generated/dtos/FishingActivityReportPageDTO';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { FishingActivityReportsFilters } from '@app/models/generated/filters/FishingActivityReportsFilters';
import { CatchesAndSalesAdministrationService } from '@app/services/administration-app/catches-and-sales-administration.service';
import { FishingActivityReportsService } from '@app/services/administration-app/fishing-activity-reports.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { EditShipLogBookPageComponent } from '../../common-app/catches-and-sales/components/ship-log-book/edit-ship-log-book-page.component';
import { EditShipLogBookPageDialogParams } from '../../common-app/catches-and-sales/components/ship-log-book/models/edit-ship-log-book-page-dialog-params.model';
import { ViewFluxVmsRequestsDialogParams } from '../flux-vms-requests/models/view-flux-vms-requests-dialog-params.model';
import { ViewFluxVmsRequestsComponent } from '../flux-vms-requests/view-flux-vms-requests.component';

@Component({
    selector: 'fishing-activity-reports',
    templateUrl: './fishing-activity-reports.component.html'
})
export class FishingActivityReportsComponent implements OnInit, AfterViewInit {
    public translate: FuseTranslationLoaderService;
    public form: FormGroup;

    public ships: ShipNomenclatureDTO[] = [];

    public readonly hasFishLogBookPageReadPermission: boolean;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private grid!: DataTableManager<FishingActivityReportDTO, FishingActivityReportsFilters>;

    private readonly service: FishingActivityReportsService;
    private readonly catchSalesService: CatchesAndSalesAdministrationService;
    private readonly nomenclatures: CommonNomenclatures;
    private readonly viewDialog: TLMatDialog<ViewFluxVmsRequestsComponent>;
    private readonly pageDialog: TLMatDialog<EditShipLogBookPageComponent>;

    public constructor(
        translate: FuseTranslationLoaderService,
        service: FishingActivityReportsService,
        catchSalesService: CatchesAndSalesAdministrationService,
        nomenclatures: CommonNomenclatures,
        permissions: PermissionsService,
        viewDialog: TLMatDialog<ViewFluxVmsRequestsComponent>,
        pageDialog: TLMatDialog<EditShipLogBookPageComponent>
    ) {
        this.translate = translate;
        this.service = service;
        this.catchSalesService = catchSalesService;
        this.nomenclatures = nomenclatures;
        this.viewDialog = viewDialog;
        this.pageDialog = pageDialog;

        this.hasFishLogBookPageReadPermission = permissions.hasAny(PermissionsEnum.FishLogBookPageReadAll, PermissionsEnum.FishLogBookRead);

        this.form = this.buildForm();
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false
        ).subscribe({
            next: (ships: ShipNomenclatureDTO[]) => {
                this.ships = ships;
            }
        });
    }

    public ngAfterViewInit(): void {
        this.grid = new DataTableManager<FishingActivityReportDTO, FishingActivityReportsFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllFishingActivityReports.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        this.grid.refreshData();
    }

    public openViewDialog(report: FishingActivityReportItemDTO): void {
        const data: ViewFluxVmsRequestsDialogParams = new ViewFluxVmsRequestsDialogParams({
            id: report.requestId
        });

        this.viewDialog.open({
            title: this.translate.getValue('flux-vms-requests.request-dialog-title'),
            TCtor: ViewFluxVmsRequestsComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction): void => {
                    closeFn();
                }
            },
            componentData: data,
            translteService: this.translate,
            viewMode: true,
            disableDialogClose: false
        }).subscribe({
            next: () => {
                // nothing to do
            }
        });
    }

    public openPageDialog(page: FishingActivityReportPageDTO): void {
        if (!this.hasFishLogBookPageReadPermission) {
            return;
        }

        const data: EditShipLogBookPageDialogParams = new EditShipLogBookPageDialogParams({
            id: page.id,
            model: undefined,
            service: this.catchSalesService,
            viewMode: true
        });

        this.pageDialog.openWithTwoButtons({
            title: this.translate.getValue('catches-and-sales.view-fishing-log-book-page-dialog-title'),
            TCtor: EditShipLogBookPageComponent,
            translteService: this.translate,
            viewMode: true,
            headerAuditButton: {
                id: page.id!,
                tableName: 'ShipLogBookPage',
                tooltip: '',
                getAuditRecordData: this.catchSalesService.getShipLogBookPageSimpleAudit.bind(this.catchSalesService)
            },
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction): void => {
                    closeFn();
                }
            },
            componentData: data,
            disableDialogClose: false
        }, '1500px').subscribe({
            next: () => {
                // nothing to do
            }
        });
    }

    private buildForm(): FormGroup {
        return new FormGroup({
            tripIdentifierControl: new FormControl(),
            shipControl: new FormControl(),
            startTimeControl: new FormControl(),
            endTimeControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): FishingActivityReportsFilters {
        const result: FishingActivityReportsFilters = new FishingActivityReportsFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            tripIdentifier: filters.getValue('tripIdentifierControl'),
            shipId: filters.getValue('shipControl'),
            startTime: filters.getValue('startTimeControl'),
            endTime: filters.getValue('endTimeControl')
        });

        return result;
    }
}
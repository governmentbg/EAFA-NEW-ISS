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
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { EditShipLogBookPageComponent } from '../../common-app/catches-and-sales/components/ship-log-book/edit-ship-log-book-page.component';
import { EditShipLogBookPageDialogParams } from '../../common-app/catches-and-sales/components/ship-log-book/models/edit-ship-log-book-page-dialog-params.model';
import { ViewFluxVmsRequestsDialogParams } from '../flux-vms-requests/models/view-flux-vms-requests-dialog-params.model';
import { ViewFluxVmsRequestsComponent } from '../flux-vms-requests/view-flux-vms-requests.component';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

type ThreeState = 'yes' | 'no' | 'both';

@Component({
    selector: 'fishing-activity-reports',
    templateUrl: './fishing-activity-reports.component.html'
})
export class FishingActivityReportsComponent implements OnInit, AfterViewInit {
    public translate: FuseTranslationLoaderService;
    public form: FormGroup;

    public ships: ShipNomenclatureDTO[] = [];
    public hasErrorsOptions: NomenclatureDTO<ThreeState>[] = [];
    public hasLandingOptions: NomenclatureDTO<ThreeState>[] = [];

    public readonly hasFishLogBookPageReadPermission: boolean;
    public readonly hasReplayMessagesPermission: boolean;

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
    private readonly confirmDialog: TLConfirmDialog;

    public constructor(
        translate: FuseTranslationLoaderService,
        service: FishingActivityReportsService,
        catchSalesService: CatchesAndSalesAdministrationService,
        nomenclatures: CommonNomenclatures,
        permissions: PermissionsService,
        viewDialog: TLMatDialog<ViewFluxVmsRequestsComponent>,
        pageDialog: TLMatDialog<EditShipLogBookPageComponent>,
        confirmDialog: TLConfirmDialog
    ) {
        this.translate = translate;
        this.service = service;
        this.catchSalesService = catchSalesService;
        this.nomenclatures = nomenclatures;
        this.viewDialog = viewDialog;
        this.pageDialog = pageDialog;
        this.confirmDialog = confirmDialog;

        this.hasFishLogBookPageReadPermission = permissions.hasAny(PermissionsEnum.FishLogBookPageReadAll, PermissionsEnum.FishLogBookRead);
        this.hasReplayMessagesPermission = permissions.has(PermissionsEnum.FishingActivityReportsReplay);

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

        this.hasErrorsOptions = [
            new NomenclatureDTO<ThreeState>({
                value: 'yes',
                displayName: this.translate.getValue('fishing-activities.has-errors-yes'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'no',
                displayName: this.translate.getValue('fishing-activities.has-errors-no'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'both',
                displayName: this.translate.getValue('fishing-activities.has-errors-both'),
                isActive: true
            })
        ];

        this.hasLandingOptions = [
            new NomenclatureDTO<ThreeState>({
                value: 'yes',
                displayName: this.translate.getValue('fishing-activities.has-landing-yes'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'no',
                displayName: this.translate.getValue('fishing-activities.has-landing-no'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'both',
                displayName: this.translate.getValue('fishing-activities.has-landing-both'),
                isActive: true
            })
        ];
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

    public replayTrip(trip: FishingActivityReportDTO): void {
        this.confirmDialog.open({
            title: this.translate.getValue('fishing-activities.replay-trip-title'),
            message: this.translate.getValue('fishing-activities.replay-trip-message'),
            okBtnLabel: this.translate.getValue('fishing-activities.replay-trip-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.fishingActivityReportReplayTrip(trip.tripIdentifier!).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
            }
        });
    }

    public replayMessage(report: FishingActivityReportItemDTO): void {
        this.confirmDialog.open({
            title: this.translate.getValue('fishing-activities.replay-message-title'),
            message: this.translate.getValue('fishing-activities.replay-message-message'),
            okBtnLabel: this.translate.getValue('fishing-activities.replay-message-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.fishingActivityReportReplayMessage(report.id!).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
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
            endTimeControl: new FormControl(),
            requestUuidControl: new FormControl(),
            errorsControl: new FormControl(),
            hasErrorsControl: new FormControl(),
            hasLandingControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): FishingActivityReportsFilters {
        const result: FishingActivityReportsFilters = new FishingActivityReportsFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            tripIdentifier: filters.getValue('tripIdentifierControl'),
            shipId: filters.getValue('shipControl'),
            startTime: filters.getValue('startTimeControl'),
            endTime: filters.getValue('endTimeControl'),
            requestUuid: filters.getValue('requestUuidControl'),
            errors: filters.getValue('errorsControl')
        });

        const hasErrors: ThreeState | undefined = filters.getValue<ThreeState>('hasErrorsControl');
        switch (hasErrors) {
            case 'yes':
                result.hasErrors = true;
                break;
            case 'no':
                result.hasErrors = false;
                break;
            default:
            case 'both':
                result.hasErrors = undefined;
                break;
        }

        const hasLanding: ThreeState | undefined = filters.getValue<ThreeState>('hasLandingControl');
        switch (hasLanding) {
            case 'yes':
                result.hasLanding = true;
                break;
            case 'no':
                result.hasLanding = false;
                break;
            default:
            case 'both':
                result.hasLanding = undefined;
                break;
        }

        return result;
    }
}
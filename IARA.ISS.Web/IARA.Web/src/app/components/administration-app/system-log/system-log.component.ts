import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { ISystemLogService } from '@app/interfaces/administration-app/system-log.interface';
import { RequestStatistics } from '@app/models/common/request-statistics.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { SystemLogDTO } from '@app/models/generated/dtos/SystemLogDTO';
import { SystemLogViewDTO } from '@app/models/generated/dtos/SystemLogViewDTO';
import { SystemLogFilters } from '@app/models/generated/filters/SystemLogFilters';
import { SystemLogService } from '@app/services/administration-app/system-log.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { IRemoteTLDatatableComponent } from '@app/shared/components/data-table/interfaces/tl-remote-datatable.interface';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { SystemLogDialogParams } from './models/system-log-dialog-params.model';
import { ViewSystemLogComponent } from './view-system-log.component';

@Component({
    selector: 'system-log',
    templateUrl: './system-log.component.html'
})
export class SystemLogComponent implements AfterViewInit, OnInit {

    public actionTypeCategories!: NomenclatureDTO<number>[];
    public users!: NomenclatureDTO<number>[];
    public formGroup!: FormGroup;
    public statistics: RequestStatistics[] = [];
    public tracingEnabled = false;
    public listeningForStatistics = false;

    private viewDialog: TLMatDialog<ViewSystemLogComponent>;
    private systemLogService: ISystemLogService;
    private translateService: FuseTranslationLoaderService;
    private commonNomenclaturesService: CommonNomenclatures;
    private snackbar: MatSnackBar;


    @ViewChild(SearchPanelComponent)
    public set searchPanel(searchpanel: SearchPanelComponent) {
        this.searchpanel = searchpanel;
    }

    @ViewChild(TLDataTableComponent)
    public set dataTable(datatable: IRemoteTLDatatableComponent) {
        this.datatable = datatable;
    }

    private datatable!: IRemoteTLDatatableComponent;
    private searchpanel!: SearchPanelComponent;
    private gridManager!: DataTableManager<SystemLogDTO, SystemLogFilters>;


    public constructor(systemLogService: SystemLogService,
        translateService: FuseTranslationLoaderService,
        viewDialog: TLMatDialog<ViewSystemLogComponent>,
        commonNomenclaturesService: CommonNomenclatures,
        snackbar: MatSnackBar) {
        this.systemLogService = systemLogService;
        this.translateService = translateService;
        this.viewDialog = viewDialog;
        this.commonNomenclaturesService = commonNomenclaturesService;
        this.snackbar = snackbar;

        this.formGroup = new FormGroup({
            actionTypeControl: new FormControl(),
            dateRangeControl: new FormControl(),
            userControl: new FormControl()
        });
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature<number>(NomenclatureTypes.AuditLogActionTypes, this.systemLogService.getActionTypeCategories.bind(this.systemLogService)).subscribe((result: NomenclatureDTO<number>[]) => {
            this.actionTypeCategories = result;
        });

        this.commonNomenclaturesService.getUserNames().subscribe((result: NomenclatureDTO<number>[]) => {
            this.users = result;
        });
    }

    private successSnackbar(message: string) {
        const config = new MatSnackBarConfig();
        config.horizontalPosition = 'center';
        config.verticalPosition = 'bottom';
        config.duration = 3000;
        config.panelClass = 'snack-bar-success-color';
        this.snackbar.open(message, 'X', config);
    }

    public openDialog(row: SystemLogDTO): void {
        this.systemLogService.get(row.id!).subscribe({
            next: (result: SystemLogViewDTO) => {
                const data: SystemLogDialogParams = new SystemLogDialogParams({
                    systemLog: row,
                    systemLogView: result
                });
                if (row.id !== undefined) {
                    const headerTitle = this.translateService.getValue('system-log.dialog-title');

                    this.viewDialog.open({
                        title: headerTitle,
                        TCtor: ViewSystemLogComponent,
                        headerCancelButton: {
                            cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
                        },
                        rightSideActionsCollection: [
                            {
                                id: 'close-btn',
                                color: 'primary',
                                translateValue: this.translateService.getValue('system-log.dialog-close-button')
                            }
                        ],
                        componentData: data,
                        translteService: this.translateService
                    });
                }
            }
        });
    }

    public closeDialogBtnClicked(closeFn: () => void): void {
        closeFn();
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<SystemLogDTO, SystemLogFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.systemLogService.getAll.bind(this.systemLogService),
            filtersMapper: this.mapFilters
        });

        const id: string | undefined = window.history.state?.tableId;
        const tableName: string | undefined = window.history.state?.tableName;

        if (!CommonUtils.isNullOrEmpty(id) && !CommonUtils.isNullOrEmpty(tableName)) {
            this.gridManager.advancedFilters = new SystemLogFilters({ tableId: id, tableName: tableName });
        }

        this.gridManager.refreshData();
    }

    private mapFilters(inputArgs: FilterEventArgs): SystemLogFilters {
        const filter: SystemLogFilters = new SystemLogFilters({
            freeTextSearch: inputArgs.searchText,
            showInactiveRecords: inputArgs.showInactiveRecords
        });

        filter.actionTypeId = inputArgs.getValue('actionTypeControl');
        filter.registeredDateFrom = inputArgs.getValue<DateRangeData>('dateRangeControl')?.start;
        filter.registeredDateTo = inputArgs.getValue<DateRangeData>('dateRangeControl')?.end;
        filter.userId = inputArgs.getValue('userControl');

        return filter;
    }
}
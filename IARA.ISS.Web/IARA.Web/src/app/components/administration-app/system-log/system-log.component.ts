import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
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
import { BaseSystemLogDTO } from '@app/models/generated/dtos/BaseSystemLogDTO';

const DATE_RANGE_CONTROL_NAME = 'dateRangeControl';

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
    private gridManager!: DataTableManager<BaseSystemLogDTO, SystemLogFilters>;
    private viewDialog: TLMatDialog<ViewSystemLogComponent>;
    private systemLogService: ISystemLogService;
    private translateService: FuseTranslationLoaderService;
    private commonNomenclaturesService: CommonNomenclatures;

    public constructor(
        systemLogService: SystemLogService,
        translateService: FuseTranslationLoaderService,
        viewDialog: TLMatDialog<ViewSystemLogComponent>,
        commonNomenclaturesService: CommonNomenclatures
    ) {
        this.systemLogService = systemLogService;
        this.translateService = translateService;
        this.viewDialog = viewDialog;
        this.commonNomenclaturesService = commonNomenclaturesService;

        this.buildForm();
    }

    public ngOnInit(): void {
        this.setDateRangeControlValue();

        NomenclatureStore.instance.getNomenclature<number>(NomenclatureTypes.AuditLogActionTypes, this.systemLogService.getActionTypeCategories.bind(this.systemLogService)).subscribe((result: NomenclatureDTO<number>[]) => {
            this.actionTypeCategories = result;
        });

        this.commonNomenclaturesService.getUserNames().subscribe((result: NomenclatureDTO<number>[]) => {
            this.users = result;
        });
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
        this.gridManager = new DataTableManager<BaseSystemLogDTO, SystemLogFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.systemLogService.getAll.bind(this.systemLogService),
            filtersMapper: this.mapFilters
        });

        if (this.searchpanel !== null && this.searchpanel !== undefined) {
            this.searchpanel.filtersChanged.subscribe({
                next: () => {
                    const dateRange: DateRangeData | null = this.searchPanel?.appliedFilters.find(x => CommonUtils.getFormControlName(x.control) === DATE_RANGE_CONTROL_NAME)?.value;

                    if (dateRange === undefined || dateRange === null) {
                        this.setDateRangeControlValue();
                    }
                }
            })
        }

        if (this.gridManager.advancedFilters === undefined || this.gridManager.advancedFilters === null) {
            const today: Date = new Date();
            const startDate: Date = new Date(today.getFullYear(), today.getMonth(), today.getDate() - 7);

            this.gridManager.advancedFilters = new SystemLogFilters({ registeredDateFrom: startDate, registeredDateTo: today });
        }

        const id: string | undefined = window.history.state?.tableId;
        const tableName: string | undefined = window.history.state?.tableName;

        if (!CommonUtils.isNullOrEmpty(id) && !CommonUtils.isNullOrEmpty(tableName)) {
            this.gridManager.advancedFilters = new SystemLogFilters({ tableId: id, tableName: tableName, showRelatedLogs: true });
        }

        this.gridManager.refreshData();
    }

    private mapFilters(filters: FilterEventArgs): SystemLogFilters {
        const result: SystemLogFilters = new SystemLogFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            actionTypeId: filters.getValue('actionTypeControl'),
            registeredDateFrom: filters.getValue<DateRangeData>('dateRangeControl')?.start,
            registeredDateTo: filters.getValue<DateRangeData>('dateRangeControl')?.end,
            application: filters.getValue('applicationControl'),
            action: filters.getValue('actionControl'),
            tableName: filters.getValue('tableNameControl'),
            tableId: filters.getValue('tableIdControl'),
            oldValue: filters.getValue('oldValueControl'),
            newValue: filters.getValue('newValueControl'),
            showRelatedLogs: filters.getValue('showRelatedLogsControl') ?? false
        });

        const user: number | string | undefined = filters.getValue('userControl');
        if (user !== undefined && user !== null) {
            if (typeof user === 'number') {
                result.userId = user;
            }
            else {
                result.username = user;
            }
        }

        if ((result.registeredDateFrom === undefined || result.registeredDateFrom === null) && (result.registeredDateTo === undefined || result.registeredDateTo === null)) {
            const today: Date = new Date();
            const startDate: Date = new Date(today.getFullYear(), today.getMonth(), today.getDate() - 7);
            const dateRange = new DateRangeData({ start: startDate, end: today });

            this.formGroup?.get('dateRangeControl')?.setValue(dateRange);
            result.registeredDateFrom = startDate;
            result.registeredDateTo = today;
        }

        return result;
    }

    private buildForm(): void {
        this.formGroup = new FormGroup({
            actionTypeControl: new FormControl(),
            dateRangeControl: new FormControl(undefined, Validators.required),
            userControl: new FormControl(),
            applicationControl: new FormControl(),
            actionControl: new FormControl(),
            tableNameControl: new FormControl(),
            tableIdControl: new FormControl(),
            oldValueControl: new FormControl(),
            newValueControl: new FormControl(),
            showRelatedLogsControl: new FormControl(false)
        });

        setTimeout(() => {
            this.formGroup.get('dateRangeControl')!.markAsTouched();
            this.formGroup.get('dateRangeControl')!.updateValueAndValidity();
        });
    }

    private setDateRangeControlValue(): void {
        const today: Date = new Date();
        const startDate: Date = new Date(today.getFullYear(), today.getMonth(), today.getDate() - 7);
        const dateRange = new DateRangeData({ start: startDate, end: today });

        this.formGroup?.get('dateRangeControl')?.setValue(dateRange);
        this.formGroup?.get('dateRangeControl')?.updateValueAndValidity();
    }
}
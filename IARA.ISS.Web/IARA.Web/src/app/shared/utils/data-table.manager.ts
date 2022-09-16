import { BaseRequestModel } from '@app/models/common/BaseRequestModel';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { Observable, Subscription } from 'rxjs';
import { IRemoteTLDatatableComponent } from '../components/data-table/interfaces/tl-remote-datatable.interface';
import { ExcelExporterHeaderNames, ExcelExporterRequestModel } from '../components/data-table/models/excel-exporter-request-model.model';
import { FilterEventArgs } from '../components/data-table/models/filter-event-args.model';
import { SearchPanelComponent } from '../components/search-panel/search-panel.component';
import { IBaseDatatableManagerParams } from '../interfaces/base-datatable-manager-params.interface';
import { IDatatableManagerParams } from '../interfaces/datatable-manager-params.interface';
import { BaseDataTableManager } from './base-data-table.manager';
import { CommonUtils } from './common.utils';

export class DataTableManager<TDataModel, TFilters extends BaseRequestModel> extends BaseDataTableManager<TDataModel> {

    public constructor(params: IDatatableManagerParams<TDataModel, TFilters>) {
        super({
            tlDataTable: params.tlDataTable
        } as IBaseDatatableManagerParams<TDataModel>, new GridRequestModel<TFilters>());
        this.searchPanel = params.searchPanel;
        this.filtersMapper = params.filtersMapper;
        this.requestServiceMethod = (request) => { return params.requestServiceMethod(request as GridRequestModel<TFilters>); }

        this.tlDataTable.showInactiveChanged.subscribe(this.onInactiveChanged.bind(this));

        if (this.searchPanel !== null && this.searchPanel !== undefined) {
            this.searchPanel.filtersChanged.subscribe(this.onFilteringChanged.bind(this));
        }

        this.childDataTables = params.childTlDataTables ?? [];
        this.excelRequestServiceMethod = params.excelRequestServiceMethod;
        this.excelFilename = params.excelFilename;
        this.tlDataTable.excelExported.subscribe(this.onExcelExported.bind(this));
    }

    private filtersMapper: (filters: FilterEventArgs) => TFilters;
    private searchPanel: SearchPanelComponent | undefined;
    private childDataTables: IRemoteTLDatatableComponent[] = [];

    private excelRequestServiceMethod: ((request: ExcelExporterRequestModel<TFilters>) => Observable<boolean>) | undefined;
    private excelFilename: string | undefined;
    private excelRequestMethodSub: Subscription | undefined;

    public get GridRequest(): GridRequestModel<TFilters> {
        return this.gridRequest as GridRequestModel<TFilters>;
    }

    public set advancedFilters(value: TFilters) {
        if (value !== null && value !== undefined) {
            this.GridRequest.filters = value;
        }
    }

    public onInactiveChanged(showInactive: boolean): void {
        this.gridRequest.pageNumber = 1;
        this.tlDataTable!.pageNumber = 0;

        if (this.GridRequest.filters != undefined) {
            this.GridRequest.filters.showInactiveRecords = showInactive;
        } else {
            const filters = new FilterEventArgs('', showInactive);
            this.GridRequest.filters = this.filtersMapper(filters);
            this.GridRequest.filters = CommonUtils.sanitizeModelStrings(this.GridRequest.filters);
        }
        this.refreshData();
    }

    public onFilteringChanged(filters: FilterEventArgs): void {
        const filterObj = this.filtersMapper(filters);
        filterObj.freeTextSearch = filters.searchText;

        if (this.GridRequest.filters != undefined) {
            filterObj.showInactiveRecords = this.GridRequest.filters.showInactiveRecords;
        } else {
            filterObj.showInactiveRecords = filters.showInactiveRecords;
        }

        this.GridRequest.filters = filterObj;
        this.GridRequest.filters = CommonUtils.sanitizeModelStrings(this.GridRequest.filters);
        this.gridRequest.pageNumber = 1;
        this.tlDataTable!.pageNumber = 0;
        this.refreshData();
    }

    private onExcelExported(): void {
        const request = new ExcelExporterRequestModel<TFilters>(
            this.excelFilename ?? '',
            this.GridRequest.filters,
            this.GridRequest.sortColumns,
            this.getHeaderNames(this.tlDataTable),
            this.getChildHeaderNames()
        );

        if (this.excelRequestServiceMethod) {
            this.excelRequestMethodSub?.unsubscribe();
            this.excelRequestMethodSub = this.excelRequestServiceMethod(request).subscribe({
                next: (success: boolean) => {
                    // nothing
                }
            });
        }
    }

    private getHeaderNames(datatable: IRemoteTLDatatableComponent): ExcelExporterHeaderNames {
        const headerNames: ExcelExporterHeaderNames = {};

        for (const column of datatable.columns) {
            if (column.prop && column.name && typeof column.prop === 'string') {
                headerNames[column.prop] = column.name;
            }
        }

        return headerNames;
    }

    private getChildHeaderNames(): ExcelExporterHeaderNames[] {
        const result: ExcelExporterHeaderNames[] = [];

        if (this.childDataTables) {
            for (const table of this.childDataTables) {
                result.push(this.getHeaderNames(table));
            }
        }

        return result;
    }
}
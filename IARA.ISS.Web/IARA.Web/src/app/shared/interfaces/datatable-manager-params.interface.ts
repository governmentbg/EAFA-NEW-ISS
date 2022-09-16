import { BaseRequestModel } from '@app/models/common/BaseRequestModel';
import { Observable } from 'rxjs';
import { GridRequestModel } from '../../models/common/grid-request.model';
import { GridResultModel } from '../../models/common/grid-result.model';
import { IRemoteTLDatatableComponent } from '../components/data-table/interfaces/tl-remote-datatable.interface';
import { ExcelExporterRequestModel } from '../components/data-table/models/excel-exporter-request-model.model';
import { FilterEventArgs } from '../components/data-table/models/filter-event-args.model';
import { SearchPanelComponent } from '../components/search-panel/search-panel.component';

export interface IDatatableManagerParams<TDataModel, TFilters extends BaseRequestModel> {
    tlDataTable: IRemoteTLDatatableComponent;
    childTlDataTables?: IRemoteTLDatatableComponent[];
    searchPanel: SearchPanelComponent | undefined;
    requestServiceMethod: (request: GridRequestModel<TFilters>) => Observable<GridResultModel<TDataModel>>;
    filtersMapper: (filters: FilterEventArgs) => TFilters;

    excelRequestServiceMethod?: (request: ExcelExporterRequestModel<TFilters>) => Observable<boolean>;
    excelFilename?: string;
}
import { GridResultModel } from '@app/models/common/grid-result.model';
import { Observable } from 'rxjs';
import { BaseGridRequestModel } from '../../models/common/base-grid-request.model';
import { IRemoteTLDatatableComponent } from '../components/data-table/interfaces/tl-remote-datatable.interface';

export interface IBaseDatatableManagerParams<TDataModel> {
    tlDataTable: IRemoteTLDatatableComponent;
    requestServiceMethod: (request: BaseGridRequestModel) => Observable<GridResultModel<TDataModel>>;
}
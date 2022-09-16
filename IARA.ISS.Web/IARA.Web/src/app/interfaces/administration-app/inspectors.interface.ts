import { Observable } from 'rxjs';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { InspectorsRegisterDTO } from '@app/models/generated/dtos/InspectorsRegisterDTO';
import { InspectorsRegisterEditDTO } from '@app/models/generated/dtos/InspectorsRegisterEditDTO';
import { UnregisteredPersonEditDTO } from '@app/models/generated/dtos/UnregisteredPersonEditDTO';
import { InspectorsFilters } from '@app/models/generated/filters/InspectorsFilters';

export interface IInspectorsService {
    getAllRegistered(request: GridRequestModel<InspectorsFilters>): Observable<GridResultModel<InspectorsRegisterDTO>>;
    getAllUnregistered(request: GridRequestModel<InspectorsFilters>): Observable<GridResultModel<InspectorsRegisterDTO>>;
    addInspector(inspectors: InspectorsRegisterEditDTO): Observable<number>;
    addUnregisteredInspector(inspectors: UnregisteredPersonEditDTO): Observable<number>;
    editInspector(inspector: InspectorsRegisterEditDTO): Observable<void>;
    editUnregisteredInspector(inspector: UnregisteredPersonEditDTO): Observable<void>;
    deleteInspector(id: number): Observable<void>;
    undoDeleteInspector(id: number): Observable<void>;
}
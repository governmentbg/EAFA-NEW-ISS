import { QualifiedFisherDTO } from '@app/models/generated/dtos/QualifiedFisherDTO';
import { QualifiedFishersFilters } from '@app/models/generated/filters/QualifiedFishersFilters';
import { Observable } from 'rxjs';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { IApplicationsActionsService } from '../common-app/application-actions.interface';
import { QualifiedFisherEditDTO } from '@app/models/generated/dtos/QualifiedFisherEditDTO';
import { PrintConfigurationParameters } from '@app/components/common-app/applications/models/print-configuration-parameters.model';

export interface IQualifiedFishersService extends IApplicationsActionsService {
    getAll(request: GridRequestModel<QualifiedFishersFilters>): Observable<GridResultModel<QualifiedFisherDTO>>;
    get(id: number): Observable<QualifiedFisherEditDTO>;

    downloadRegister(id: number, configurations: PrintConfigurationParameters): Observable<boolean>;

    add(model: QualifiedFisherEditDTO): Observable<number>;
    addAndDownloadRegister(model: QualifiedFisherEditDTO, configurations: PrintConfigurationParameters): Observable<boolean>;
    edit(model: QualifiedFisherEditDTO): Observable<number>;
    editAndDownloadRegister(model: QualifiedFisherDTO, configurations: PrintConfigurationParameters): Observable<boolean>;
    delete(id: number): Observable<void>;
    undoDelete(id: number): Observable<void>;

}
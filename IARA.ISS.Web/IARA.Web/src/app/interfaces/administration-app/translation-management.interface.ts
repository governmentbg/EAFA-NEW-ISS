import { Observable } from 'rxjs';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { TranslationManagementDTO } from '@app/models/generated/dtos/TranslationManagementDTO';
import { TranslationManagementFilters } from '@app/models/generated/filters/TranslationManagementFilters';
import { TranslationManagementEditDTO } from '@app/models/generated/dtos/TranslationManagementEditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { IBaseAuditService } from '../base-audit.interface';

export interface ITranslationManagementService extends IBaseAuditService {
    getAllLabelTranslations(request: GridRequestModel<TranslationManagementFilters>): Observable<GridResultModel<TranslationManagementDTO>>;
    getAllHelpTranslations(request: GridRequestModel<TranslationManagementFilters>): Observable<GridResultModel<TranslationManagementDTO>>;
    get(id: number): Observable<TranslationManagementEditDTO>;
    getByKey(key: string): Observable<TranslationManagementEditDTO>;
    add(request: TranslationManagementEditDTO): Observable<number>;
    edit(request: TranslationManagementEditDTO): Observable<void>;
    getGroups(): Observable<NomenclatureDTO<number>[]>;
}
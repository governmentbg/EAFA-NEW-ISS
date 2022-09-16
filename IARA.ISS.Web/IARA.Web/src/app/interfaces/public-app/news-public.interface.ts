import { Observable } from 'rxjs';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NewsFilters } from '@app/models/generated/filters/NewsFilters';
import { NewsDetailsDTO } from '../../models/generated/dtos/NewsDetailsDTO';
import { NewsDTO } from '../../models/generated/dtos/NewsDTO';
import { NomenclatureDTO } from '../../models/generated/dtos/GenericNomenclatureDTO';

export interface INewsPublicService {
    getPublishedNews(id: number): Observable<NewsDetailsDTO>;
    downloadFile(fileId: number, fileName: string): Observable<boolean>;
    getAll(gridRequestModel: GridRequestModel<NewsFilters>): Observable<GridResultModel<NewsDTO>>;
    getDistricts(): Observable<NomenclatureDTO<number>[]>;
}
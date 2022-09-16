import { Observable } from 'rxjs';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NewsManagementDTO } from '@app/models/generated/dtos/NewsManagementDTO';
import { NewsManagementEditDTO } from '@app/models/generated/dtos/NewsManagementEditDTO';
import { NewsManagementFilters } from '@app/models/generated/filters/NewsManagmentFilters';
import { IBaseAuditService } from '@app/interfaces/base-audit.interface';

export interface INewsManagementService extends IBaseAuditService {
    add(news: NewsManagementEditDTO): Observable<number>
    edit(news: NewsManagementEditDTO): Observable<void>
    get(id: number): Observable<NewsManagementEditDTO>
    getAll(gridRequestModel: GridRequestModel<NewsManagementFilters>): Observable<GridResultModel<NewsManagementDTO>>;
    deleteNews(id: number): Observable<void>;
    undoDeletedNews(id: number): Observable<void>;
    downloadFile(fileId: number, fileName: string): Observable<boolean>;
    getMainImage(newsId: number): Observable<string>
}
import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { INewsManagementService } from '@app/interfaces/administration-app/news-management.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NewsManagementDTO } from '@app/models/generated/dtos/NewsManagementDTO';
import { NewsManagementEditDTO } from '@app/models/generated/dtos/NewsManagementEditDTO';
import { NewsManagementFilters } from '@app/models/generated/filters/NewsManagmentFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '@app/services/common-app/base-audit.service';

@Injectable({
    providedIn: 'root'
})
export class NewsManagementService extends BaseAuditService implements INewsManagementService {
    protected controller: string = 'NewsManagement';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public getMainImage(newsId: number): Observable<string> {
        const params = new HttpParams().append('id', newsId.toString())

        return this.requestService.get(this.area, this.controller, 'GetMainImage', {
            httpParams: params,
            responseType: 'text',
            properties: RequestProperties.NO_SPINNER
        });
    }

    public downloadFile(fileId: number, fileName: string): Observable<boolean> {
        return this.requestService.download(this.area, this.controller, 'DownloadFile', fileName, {
            httpParams: new HttpParams().append('id', fileId.toString())
        })
    }

    public add(news: NewsManagementEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'Add', news, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public edit(news: NewsManagementEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'Edit', news, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public get(id: number): Observable<NewsManagementEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'Get', {
            httpParams: params,
            responseTypeCtr: NewsManagementEditDTO
        });
    }

    public getAll(gridRequestModel: GridRequestModel<NewsManagementFilters>): Observable<GridResultModel<NewsManagementDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAll', gridRequestModel, {
            responseTypeCtr: GridResultModel
        });
    }

    public deleteNews(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'DeleteNews', { httpParams: params });
    }

    public undoDeletedNews(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDeletedNews', null, { httpParams: params });
    }
}
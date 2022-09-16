import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { INewsPublicService } from '../../interfaces/public-app/news-public.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NewsDTO } from '@app/models/generated/dtos/NewsDTO';
import { NewsImageDTO } from '@app/models/generated/dtos/NewsImageDTO';
import { NewsFilters } from '@app/models/generated/filters/NewsFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';
import { NewsDetailsDTO } from '../../models/generated/dtos/NewsDetailsDTO';
import { HttpParams } from '@angular/common/http';
import { RequestProperties } from '../../shared/services/request-properties';
import { NomenclatureDTO } from '../../models/generated/dtos/GenericNomenclatureDTO';

@Injectable({
    providedIn: 'root'
})
export class NewsPublicService extends BaseAuditService implements INewsPublicService {
    protected controller: string = 'NewsPublic';

    constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Public);
    }

    public getPublishedNews(id: number): Observable<NewsDetailsDTO> {
        const httpParameters: HttpParams = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetPublishedNews', {
            httpParams: httpParameters,
            responseTypeCtr: NewsDetailsDTO,
            properties: new RequestProperties({ rethrowException: true, showException: false })
        });
    }

    public getDistricts(): Observable<NomenclatureDTO<number>[]> {

        return this.requestService.get(this.area, this.controller, 'GetDistricts', {
            responseTypeCtr: NomenclatureDTO,
            properties: new RequestProperties({ rethrowException: true, showException: false })
        });
    }

    public downloadFile(fileId: number, fileName: string): Observable<boolean> {
        const httpParameters: HttpParams = new HttpParams().append('id', fileId.toString());

        return this.requestService.download(this.area, this.controller, 'DownloadFile', fileName, {
            httpParams: httpParameters
        });
    }

    public getAll(gridRequestModel: GridRequestModel<NewsFilters>): Observable<GridResultModel<NewsDTO>> {
        type Result = GridResultModel<NewsDTO>;
        type Body = GridRequestModel<NewsFilters>;

        return this.requestService.post<Result, Body>(this.area, this.controller, 'GetAll', gridRequestModel, {
            responseTypeCtr: NewsDTO
        }).pipe(switchMap((entries: Result) => {
            const newsIds: number[] = entries.records.map((news: NewsDTO) => {
                return news.id!;
            });

            if (newsIds.length === 0) {
                return of(entries);
            }

            return this.getNewsImages(newsIds).pipe(map((newsImages: NewsImageDTO[]) => {
                for (const newsImage of newsImages) {
                    const found: NewsDTO | undefined = entries.records.find((entry: NewsDTO) => {
                        return entry.id === newsImage.newsId;
                    });

                    if (found !== undefined) {
                        found.mainImage = newsImage.image;
                    }
                }
                return entries;
            }));
        }));
    }

    private getNewsImages(newsIds: number[]): Observable<NewsImageDTO[]> {
        return this.requestService.post(this.area, this.controller, 'GetNewsImages', newsIds, {
            responseTypeCtr: NewsImageDTO
        });
    }
}
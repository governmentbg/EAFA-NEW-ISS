import { Observable } from "rxjs";
import { GridRequestModel } from "@app/models/common/grid-request.model";
import { GridResultModel } from "@app/models/common/grid-result.model";
import { YearlyQuotaDTO } from "@app/models/generated/dtos/YearlyQuotaDTO";
import { YearlyQuotasFilters } from "@app/models/generated/filters/YearlyQuotasFilters";
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { YearlyQuotaEditDTO } from '@app/models/generated/dtos/YearlyQuotaEditDTO';
import { QuotaHistDTO } from '@app/models/generated/dtos/QuotaHistDTO';

export interface IYearlyQuotasService {
    getAll(request: GridRequestModel<YearlyQuotasFilters>): Observable<GridResultModel<YearlyQuotaDTO>>;

    get(id: number): Observable<YearlyQuotaEditDTO>;
    add(item: YearlyQuotaEditDTO): Observable<number>;
    edit(item: YearlyQuotaEditDTO): Observable<void>;
    delete(id: number): Observable<void>;
    undoDelete(id: number): Observable<void>;

    transfer(newQuotaId: number, oldQuotaId: number, transferValue: number, basis: string): Observable<void>;
    downloadFile(fileId: number, fileName: string): Observable<boolean>;

    getYearlyQuotaHistory(ids: number[]): Observable<QuotaHistDTO[]>;
    getLastYearsQuota(newQuotaId: number): Observable<YearlyQuotaEditDTO>;

    getYearlyQuotasForList(): Observable<NomenclatureDTO<number>[]>;
}
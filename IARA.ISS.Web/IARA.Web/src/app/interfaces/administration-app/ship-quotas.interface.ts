import { Observable } from "rxjs";
import { GridRequestModel } from "@app/models/common/grid-request.model";
import { GridResultModel } from "@app/models/common/grid-result.model";
import { ShipQuotaDTO } from "@app/models/generated/dtos/ShipQuotaDTO";
import { ShipQuotasFilters } from "@app/models/generated/filters/ShipQuotasFilters";
import { ShipQuotaEditDTO } from '@app/models/generated/dtos/ShipQuotaEditDTO';
import { QuotaHistDTO } from '@app/models/generated/dtos/QuotaHistDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

export interface IShipQuotasService {
    getAll(request: GridRequestModel<ShipQuotasFilters>): Observable<GridResultModel<ShipQuotaDTO>>;

    get(id: number): Observable<ShipQuotaEditDTO>;
    add(item: ShipQuotaEditDTO): Observable<number>;
    edit(item: ShipQuotaEditDTO): Observable<void>;
    delete(id: number): Observable<void>;
    undoDelete(id: number): Observable<void>;

    transfer(newQuotaId: number, oldQuotaId: number, transferValue: number, basis: string): Observable<void>;
    downloadFile(fileId: number, fileName: string): Observable<boolean>;

    getShipQuotaHistory(ids: number[]): Observable<QuotaHistDTO[]>;

    getShipQuotasForList(id: number): Observable<NomenclatureDTO<number>[]>;
    getYearlyQuotasForList(): Observable<NomenclatureDTO<number>[]>;
}
import { Observable } from 'rxjs';
import { PrintUserNomenclatureDTO } from '@app/models/generated/dtos/PrintUserNomenclatureDTO';

export interface IPrintConfigurationsService {
    getUsersNomenclature(): Observable<PrintUserNomenclatureDTO[]>;
    getMyTerritoryUnitUsersNomenclature(): Observable<PrintUserNomenclatureDTO[]>;
    getCentralTerritoryUnitUsersNomenclature(): Observable<PrintUserNomenclatureDTO[]>;
}
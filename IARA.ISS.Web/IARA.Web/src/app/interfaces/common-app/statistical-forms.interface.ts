import { Observable } from 'rxjs';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { StatisticalFormAquaFarmEditDTO } from '@app/models/generated/dtos/StatisticalFormAquaFarmEditDTO';
import { StatisticalFormDTO } from '@app/models/generated/dtos/StatisticalFormDTO';
import { StatisticalFormFishVesselEditDTO } from '@app/models/generated/dtos/StatisticalFormFishVesselEditDTO';
import { StatisticalFormReworkEditDTO } from '@app/models/generated/dtos/StatisticalFormReworkEditDTO';
import { StatisticalFormsFilters } from '@app/models/generated/filters/StatisticalFormsFilters';
import { IBaseAuditService } from '../base-audit.interface';
import { IApplicationsActionsService } from './application-actions.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { StatisticalFormShipDTO } from '@app/models/generated/dtos/StatisticalFormShipDTO';
import { StatisticalFormAquacultureDTO } from '@app/models/generated/dtos/StatisticalFormAquacultureDTO';
import { StatisticalFormAquacultureNomenclatureDTO } from '@app/models/generated/dtos/StatisticalFormAquacultureNomenclatureDTO';

export interface IStatisticalFormsService extends IApplicationsActionsService, IBaseAuditService {
    getAllStatisticalForms(request: GridRequestModel<StatisticalFormsFilters>): Observable<GridResultModel<StatisticalFormDTO>>;
    getStatisticalFormAquaFarm(id: number): Observable<StatisticalFormAquaFarmEditDTO>;
    getStatisticalFormRework(id: number): Observable<StatisticalFormReworkEditDTO>;
    getStatisticalFormFishVessel(id: number): Observable<StatisticalFormFishVesselEditDTO>;

    addStatisticalFormAquaFarm(form: StatisticalFormAquaFarmEditDTO): Observable<number>;
    addStatisticalFormRework(form: StatisticalFormReworkEditDTO): Observable<number>;
    addStatisticalFormFishVessel(form: StatisticalFormFishVesselEditDTO): Observable<number>;

    editStatisticalFormAquaFarm(form: StatisticalFormAquaFarmEditDTO): Observable<void>;
    editStatisticalFormRework(form: StatisticalFormReworkEditDTO): Observable<void>;
    editStatisticalFormFishVessel(form: StatisticalFormFishVesselEditDTO): Observable<void>;

    deleteStatisticalForm(id: number): Observable<void>;
    undoDeleteStatisticalForm(id: number): Observable<void>;

    getStatisticalFormShip(shipId: number): Observable<StatisticalFormShipDTO>;
    getStatisticalFormAquaculture(aquacultureId: number): Observable<StatisticalFormAquacultureDTO>;
    getShipFishingGearsForYear(shipId: number, year: number): Observable<NomenclatureDTO<number>[]>;

    getGrossTonnageIntervals(): Observable<NomenclatureDTO<number>[]>;
    getVesselLengthIntervals(): Observable<NomenclatureDTO<number>[]>;
    getFuelTypes(): Observable<NomenclatureDTO<number>[]>;
    getReworkProductTypes(): Observable<NomenclatureDTO<number>[]>;
    getAllAquacultureNomenclatures(): Observable<StatisticalFormAquacultureNomenclatureDTO[]>;

    vesselStatFormAlreadyExists(shipId: number, year: number, formId: number | undefined): Observable<boolean>;
    aquaFarmStatFormAlreadyExists(aquacultureId: number, year: number, formId: number | undefined): Observable<boolean>;
}
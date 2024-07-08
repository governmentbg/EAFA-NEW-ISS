import { Observable } from 'rxjs';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { SailAreaNomenclatureDTO } from '@app/models/generated/dtos/SailAreaNomenclatureDTO';
import { ShipRegisterEditDTO } from '@app/models/generated/dtos/ShipRegisterEditDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { VesselTypeNomenclatureDTO } from '@app/models/generated/dtos/VesselTypeNomenclatureDTO';
import { IApplicationsActionsService } from '@app/interfaces/common-app/application-actions.interface';
import { IBaseAuditService } from '../base-audit.interface';
import { FleetTypeNomenclatureDTO } from '@app/models/generated/dtos/FleetTypeNomenclatureDTO';
import { ShipsRegisterFilters } from '@app/models/generated/filters/ShipsRegisterFilters';
import { ExcelExporterRequestModel } from '@app/shared/components/data-table/models/excel-exporter-request-model.model';
import { PaymentTariffDTO } from '@app/models/generated/dtos/PaymentTariffDTO';
import { FreedCapacityTariffCalculationParameters } from '@app/components/common-app/ships-register/models/freed-capacity-tariff-calculation-parameters.model';

export interface IShipsRegisterService extends IApplicationsActionsService, IBaseAuditService {
    getShip(id: number): Observable<ShipRegisterEditDTO>;

    addShip(ship: ShipRegisterEditDTO): Observable<number>;
    editShip(ship: ShipRegisterEditDTO): Observable<void>;

    addThirdPartyShip(ship: ShipRegisterEditDTO): Observable<number>;
    editThirdPartyShip(ship: ShipRegisterEditDTO): Observable<void>;

    downloadFile(fileId: number, fileName: string): Observable<boolean>;
    downloadShipRegisterExcel(request: ExcelExporterRequestModel<ShipsRegisterFilters>): Observable<boolean>;

    calculateFreedCapacityAppliedTariffs(parameters: FreedCapacityTariffCalculationParameters): Observable<PaymentTariffDTO[]>;

    getShipOwnerAudit(id: number): Observable<SimpleAuditDTO>;
    getShipLogBookPageSimpleAudit(id: number): Observable<SimpleAuditDTO>;

    getEventTypes(): Observable<NomenclatureDTO<number>[]>;
    getFleetTypes(): Observable<FleetTypeNomenclatureDTO[]>;
    getPublicAidTypes(): Observable<NomenclatureDTO<number>[]>;
    getPublicAidSegments(): Observable<NomenclatureDTO<number>[]>;
    getSailAreas(): Observable<SailAreaNomenclatureDTO[]>;
    getVesselTypes(): Observable<VesselTypeNomenclatureDTO[]>;
    getHullMaterials(): Observable<NomenclatureDTO<number>[]>;
    getFuelTypes(): Observable<NomenclatureDTO<number>[]>;
}

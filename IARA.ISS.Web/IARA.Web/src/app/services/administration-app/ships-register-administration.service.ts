import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { IShipsRegisterService } from '@app/interfaces/common-app/ships-register.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { SailAreaNomenclatureDTO } from '@app/models/generated/dtos/SailAreaNomenclatureDTO';
import { ShipEventTypeDTO } from '@app/models/generated/dtos/ShipEventTypeDTO';
import { ShipRegisterApplicationEditDTO } from '@app/models/generated/dtos/ShipRegisterApplicationEditDTO';
import { ShipRegisterDTO } from '@app/models/generated/dtos/ShipRegisterDTO';
import { ShipRegisterEditDTO } from '@app/models/generated/dtos/ShipRegisterEditDTO';
import { ShipRegisterEventDTO } from '@app/models/generated/dtos/ShipRegisterEventDTO';
import { ShipRegisterRegixDataDTO } from '@app/models/generated/dtos/ShipRegisterRegixDataDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { VesselTypeNomenclatureDTO } from '@app/models/generated/dtos/VesselTypeNomenclatureDTO';
import { ShipsRegisterFilters } from '@app/models/generated/filters/ShipsRegisterFilters';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { ApplicationsProcessingService } from './applications-processing.service';
import { ApplicationsRegisterAdministrativeBaseService } from '@app/services/administration-app/applications-register-administrative-base.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ShipChangeOfCircumstancesApplicationDTO } from '@app/models/generated/dtos/ShipChangeOfCircumstancesApplicationDTO';
import { ShipChangeOfCircumstancesRegixDataDTO } from '@app/models/generated/dtos/ShipChangeOfCircumstancesRegixDataDTO';
import { ShipRegisterChangeOfCircumstancesDTO } from '@app/models/generated/dtos/ShipRegisterChangeOfCircumstancesDTO';
import { ShipDeregistrationRegixDataDTO } from '@app/models/generated/dtos/ShipDeregistrationRegixDataDTO';
import { ShipDeregistrationApplicationDTO } from '@app/models/generated/dtos/ShipDeregistrationApplicationDTO';
import { ShipRegisterDeregistrationDTO } from '@app/models/generated/dtos/ShipRegisterDeregistrationDTO';
import { FleetTypeNomenclatureDTO } from '@app/models/generated/dtos/FleetTypeNomenclatureDTO';
import { FishingGearDTO } from '@app/models/generated/dtos/FishingGearDTO';
import { ShipRegisterYearlyQuotaDTO } from '@app/models/generated/dtos/ShipRegisterYearlyQuotaDTO';
import { ShipRegisterIncreaseCapacityDTO } from '@app/models/generated/dtos/ShipRegisterIncreaseCapacityDTO';
import { ShipRegisterReduceCapacityDTO } from '@app/models/generated/dtos/ShipRegisterReduceCapacityDTO';
import { ExcelExporterRequestModel } from '@app/shared/components/data-table/models/excel-exporter-request-model.model';
import { ShipRegisterLogBookPagesFilters } from '@app/models/generated/filters/ShipRegisterLogBookPagesFilters';
import { ShipRegisterLogBookPageDTO } from '@app/models/generated/dtos/ShipRegisterLogBookPageDTO';

@Injectable({
    providedIn: 'root'
})
export class ShipsRegisterAdministrationService extends ApplicationsRegisterAdministrativeBaseService implements IShipsRegisterService {
    protected controller: string = 'ShipsRegisterAdministration';

    public constructor(requestService: RequestService, applicationsProcessingService: ApplicationsProcessingService) {
        super(requestService, applicationsProcessingService);
    }

    // register
    public getAllShips(request: GridRequestModel<ShipsRegisterFilters>): Observable<GridResultModel<ShipRegisterDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllShips', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getShip(id: number): Observable<ShipRegisterEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetShip', {
            httpParams: params,
            responseTypeCtr: ShipRegisterEditDTO
        });
    }

    public getRegisterByApplicationId(applicationId: number, pageCode: PageCodeEnum): Observable<ShipRegisterEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        if (pageCode === PageCodeEnum.RegVessel) {
            return this.requestService.get(this.area, this.controller, 'GetRegisterByApplicationId', {
                httpParams: params,
                responseTypeCtr: ShipRegisterEditDTO
            });
        }
        else if (pageCode === PageCodeEnum.ShipRegChange || pageCode === PageCodeEnum.DeregShip) {
            return this.requestService.get(this.area, this.controller, 'GetRegisterByChangeOfCircumstancesApplicationId', {
                httpParams: params,
                responseTypeCtr: ShipRegisterEditDTO
            });
        }
        else if (pageCode === PageCodeEnum.IncreaseFishCap || pageCode === PageCodeEnum.ReduceFishCap) {
            return this.requestService.get(this.area, this.controller, 'GetRegisterByChangeCapacityApplicationId', {
                httpParams: params,
                responseTypeCtr: ShipRegisterEditDTO
            });
        }

        throw new Error('Invalid pageCode for getRegisterByApplicationId for ship: ' + PageCodeEnum[pageCode]);
    }

    public getShipFromChangeOfCircumstancesApplication(applicationId: number): Observable<ShipRegisterEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetShipFromChangeOfCircumstancesApplication', {
            httpParams: params,
            responseTypeCtr: ShipRegisterEditDTO
        });
    }

    public getShipFromDeregistrationApplication(applicationId: number): Observable<ShipRegisterEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetShipFromDeregistrationApplication', {
            httpParams: params,
            responseTypeCtr: ShipRegisterEditDTO
        });
    }

    public getShipFromIncreaseCapacityApplication(applicationId: number): Observable<ShipRegisterEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetShipFromIncreaseCapacityApplication', {
            httpParams: params,
            responseTypeCtr: ShipRegisterEditDTO
        });
    }

    public getShipFromReduceCapacityApplication(applicationId: number): Observable<ShipRegisterEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetShipFromReduceCapacityApplication', {
            httpParams: params,
            responseTypeCtr: ShipRegisterEditDTO
        });
    }

    public getRegixData(applicationId: number, pageCode: PageCodeEnum): Observable<RegixChecksWrapperDTO<IApplicationRegister>> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        if (pageCode === PageCodeEnum.RegVessel) {
            return this.requestService.get<RegixChecksWrapperDTO<ShipRegisterRegixDataDTO>>(this.area, this.controller, 'GetShipRegixData', {
                httpParams: params,
                responseTypeCtr: RegixChecksWrapperDTO
            }).pipe(map((result: RegixChecksWrapperDTO<ShipRegisterRegixDataDTO>) => {
                result.dialogDataModel = new ShipRegisterRegixDataDTO(result.dialogDataModel);
                result.regiXDataModel = new ShipRegisterRegixDataDTO(result.regiXDataModel);

                return result;
            }));
        }
        else if (pageCode === PageCodeEnum.ShipRegChange) {
            return this.requestService.get<RegixChecksWrapperDTO<ShipChangeOfCircumstancesRegixDataDTO>>(this.area, this.controller, 'GetShipChangeOfCircumstancesRegixData', {
                httpParams: params,
                responseTypeCtr: RegixChecksWrapperDTO
            }).pipe(map((result: RegixChecksWrapperDTO<ShipChangeOfCircumstancesRegixDataDTO>) => {
                result.dialogDataModel = new ShipChangeOfCircumstancesRegixDataDTO(result.dialogDataModel);
                result.regiXDataModel = new ShipChangeOfCircumstancesRegixDataDTO(result.regiXDataModel);

                return result;
            }));
        }
        else if (pageCode === PageCodeEnum.DeregShip) {
            return this.requestService.get<RegixChecksWrapperDTO<ShipDeregistrationRegixDataDTO>>(this.area, this.controller, 'GetShipDeregistrationRegixData', {
                httpParams: params,
                responseTypeCtr: RegixChecksWrapperDTO
            }).pipe(map((result: RegixChecksWrapperDTO<ShipDeregistrationRegixDataDTO>) => {
                result.dialogDataModel = new ShipDeregistrationRegixDataDTO(result.dialogDataModel);
                result.regiXDataModel = new ShipDeregistrationRegixDataDTO(result.regiXDataModel);

                return result;
            }));
        }

        throw new Error('Invalid page code for getRegixData: ' + PageCodeEnum[pageCode]);
    }

    public getApplicationDataForRegister(applicationId: number): Observable<ShipRegisterEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetApplicationDataForRegister', {
            httpParams: params,
            responseTypeCtr: ShipRegisterEditDTO
        });
    }

    public getShipEventHistory(shipId: number): Observable<ShipRegisterEventDTO[]> {
        const params = new HttpParams().append('shipId', shipId.toString());

        return this.requestService.get(this.area, this.controller, 'GetShipEventHistory', {
            httpParams: params,
            responseTypeCtr: ShipRegisterEventDTO
        });
    }

    public downloadShipRegisterExcel(request: ExcelExporterRequestModel<ShipsRegisterFilters>): Observable<boolean> {
        return this.requestService.downloadPost(this.area, this.controller, 'DownloadShipRegisterExcel', request.filename, request, {
            properties: new RequestProperties({ showException: true, rethrowException: true }),
        });
    }

    public addShip(ship: ShipRegisterEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddShip', ship, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editShip(ship: ShipRegisterEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditShip', ship, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public addThirdPartyShip(ship: ShipRegisterEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddThirdPartyShip', ship, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editThirdPartyShip(ship: ShipRegisterEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditThirdPartyShip', ship, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public getShipOwnerAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetShipOwnerSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getShipLogBookPageSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetShipLogBookPageSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getShipFishingGears(shipUId: number): Observable<FishingGearDTO[]> {
        const params = new HttpParams().append('shipUId', shipUId.toString());

        return this.requestService.get(this.area, this.controller, 'GetShipFishingGears', {
            httpParams: params,
            responseTypeCtr: FishingGearDTO
        });
    }

    public getShipCatchQuotaNomenclatures(shipUId: number): Observable<NomenclatureDTO<number>[]> {
        const params = new HttpParams().append('shipUId', shipUId.toString());

        return this.requestService.get(this.area, this.controller, 'GetShipCatchQuotaNomenclatures', {
            httpParams: params,
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getShipYearlyQuota(shipCatchQuotaId: number): Observable<ShipRegisterYearlyQuotaDTO> {
        const params = new HttpParams().append('shipCatchQuotaId', shipCatchQuotaId.toString());

        return this.requestService.get(this.area, this.controller, 'GetShipYearlyQuota', {
            httpParams: params,
            responseTypeCtr: ShipRegisterYearlyQuotaDTO
        });
    }

    public getShipLogBookPages(request: GridRequestModel<ShipRegisterLogBookPagesFilters>): Observable<GridResultModel<ShipRegisterLogBookPageDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetShipLogBookPages', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    // nomenclatures
    public getEventTypes(): Observable<ShipEventTypeDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetEventTypes', { responseTypeCtr: ShipEventTypeDTO });
    }

    public getFleetTypes(): Observable<FleetTypeNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetFleetTypes', { responseTypeCtr: FleetTypeNomenclatureDTO });
    }

    public getPublicAidTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetPublicAidTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getPublicAidSegments(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetPublicAidSegments', { responseTypeCtr: NomenclatureDTO });
    }

    public getSailAreas(): Observable<SailAreaNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetSailAreas', { responseTypeCtr: SailAreaNomenclatureDTO });
    }

    public getVesselTypes(): Observable<VesselTypeNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetVesselTypes', { responseTypeCtr: VesselTypeNomenclatureDTO });
    }

    public getPorts(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetPorts', { responseTypeCtr: NomenclatureDTO });
    }

    public getHullMaterials(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetHullMaterials', { responseTypeCtr: NomenclatureDTO });
    }

    public getFuelTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetFuelTypes', { responseTypeCtr: NomenclatureDTO });
    }

    // applications
    public getApplication(id: number, getRegiXData: boolean, pageCode: PageCodeEnum): Observable<IApplicationRegister> {
        const params = new HttpParams()
            .append('id', id.toString())
            .append('getRegiXData', getRegiXData.toString());

        if (pageCode === PageCodeEnum.RegVessel) {
            return this.requestService.get(this.area, this.controller, 'GetShipApplication', {
                httpParams: params,
                responseTypeCtr: ShipRegisterApplicationEditDTO
            });
        }
        else if (pageCode === PageCodeEnum.ShipRegChange) {
            return this.requestService.get(this.area, this.controller, 'GetShipChangeOfCircumstancesApplication', {
                httpParams: params,
                responseTypeCtr: ShipChangeOfCircumstancesApplicationDTO
            });
        }
        else if (pageCode === PageCodeEnum.DeregShip) {
            return this.requestService.get(this.area, this.controller, 'GetShipDeregistrationApplication', {
                httpParams: params,
                responseTypeCtr: ShipDeregistrationApplicationDTO
            });
        }

        throw new Error('Invalid page code for getApplication: ' + PageCodeEnum[pageCode]);
    }

    public addApplication(application: IApplicationRegister, pageCode: PageCodeEnum): Observable<number> {
        if (pageCode === PageCodeEnum.RegVessel) {
            return this.requestService.post(this.area, this.controller, 'AddShipApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (pageCode === PageCodeEnum.ShipRegChange) {
            return this.requestService.post(this.area, this.controller, 'AddShipChangeOfCircumstancesApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (pageCode === PageCodeEnum.DeregShip) {
            return this.requestService.post(this.area, this.controller, 'AddShipDeregistrationApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }

        throw new Error('Invalid page code for addApplication: ' + PageCodeEnum[pageCode]);
    }

    public editApplication(application: IApplicationRegister, pageCode: PageCodeEnum, saveAsDraft: boolean = false): Observable<number> {
        const params = new HttpParams().append('saveAsDraft', saveAsDraft.toString());

        if (pageCode === PageCodeEnum.RegVessel) {
            return this.requestService.post(this.area, this.controller, 'EditShipApplication', application, {
                httpParams: params,
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (pageCode === PageCodeEnum.ShipRegChange) {
            return this.requestService.post(this.area, this.controller, 'EditShipChangeOfCircumstancesApplication', application, {
                httpParams: params,
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (pageCode === PageCodeEnum.DeregShip) {
            return this.requestService.post(this.area, this.controller, 'EditShipDeregistrationApplication', application, {
                httpParams: params,
                properties: new RequestProperties({ asFormData: true })
            });
        }

        throw new Error('Invalid page code for editApplication: ' + PageCodeEnum[pageCode]);
    }

    public editApplicationDataAndStartRegixChecks(model: IApplicationRegister): Observable<void> {
        if (model.pageCode === PageCodeEnum.RegVessel) {
            return this.requestService.post(this.area, this.controller, 'EditShipApplicationAndStartRegixChecks', model, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (model.pageCode === PageCodeEnum.ShipRegChange) {
            return this.requestService.post(this.area, this.controller, 'EditShipChangeOfCircumstancesApplicationAndStartRegixChecks', model, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (model.pageCode === PageCodeEnum.DeregShip) {
            return this.requestService.post(this.area, this.controller, 'EditShipDeregistrationApplicationAndStartRegixChecks', model, {
                properties: new RequestProperties({ asFormData: true })
            });
        }

        throw new Error('Invalid page code for editApplicationDataAndStartRegixChecks: ' + PageCodeEnum[model.pageCode!]);
    }

    public completeChangeOfCircumstancesApplication(ships: ShipRegisterChangeOfCircumstancesDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'CompleteShipChangeOfCircumstancesApplication', ships, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public completeShipDeregistrationApplication(ships: ShipRegisterDeregistrationDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'CompleteShipDeregistrationApplication', ships, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public completeShipIncreaseCapacityApplication(ships: ShipRegisterIncreaseCapacityDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'CompleteShipIncreaseCapacityApplication', ships, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public completeShipReduceCapacityApplication(ships: ShipRegisterReduceCapacityDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'CompleteShipReduceCapacityApplication', ships, {
            properties: new RequestProperties({ asFormData: true })
        });
    }
}
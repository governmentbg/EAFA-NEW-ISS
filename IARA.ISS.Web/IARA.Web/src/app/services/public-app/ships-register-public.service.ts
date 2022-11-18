import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { IShipsRegisterService } from '@app/interfaces/common-app/ships-register.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { SailAreaNomenclatureDTO } from '@app/models/generated/dtos/SailAreaNomenclatureDTO';
import { ShipRegisterApplicationEditDTO } from '@app/models/generated/dtos/ShipRegisterApplicationEditDTO';
import { ShipRegisterEditDTO } from '@app/models/generated/dtos/ShipRegisterEditDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { VesselTypeNomenclatureDTO } from '@app/models/generated/dtos/VesselTypeNomenclatureDTO';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { ApplicationsRegisterPublicBaseService } from '@app/services/public-app/applications-register-public-base.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ShipChangeOfCircumstancesApplicationDTO } from '@app/models/generated/dtos/ShipChangeOfCircumstancesApplicationDTO';
import { ShipEventTypeDTO } from '@app/models/generated/dtos/ShipEventTypeDTO';
import { ShipDeregistrationApplicationDTO } from '@app/models/generated/dtos/ShipDeregistrationApplicationDTO';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { FleetTypeNomenclatureDTO } from '@app/models/generated/dtos/FleetTypeNomenclatureDTO';
import { ExcelExporterRequestModel } from '@app/shared/components/data-table/models/excel-exporter-request-model.model';
import { ShipsRegisterFilters } from '@app/models/generated/filters/ShipsRegisterFilters';

@Injectable({
    providedIn: 'root'
})
export class ShipsRegisterPublicService extends ApplicationsRegisterPublicBaseService implements IShipsRegisterService {
    protected controller: string = 'ShipsRegisterPublic';

    public constructor(requestService: RequestService) {
        super(requestService);
    }

    // register
    public downloadShipRegisterExcel(request: ExcelExporterRequestModel<ShipsRegisterFilters>): Observable<boolean> {
        throw new Error('This method should not be called from the public app.');
    }

    public getShip(id: number): Observable<ShipRegisterEditDTO> {
        throw new Error('This method should not be called from the public app.');
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

    public addShip(ship: ShipRegisterEditDTO): Observable<number> {
        throw new Error('This method should not be called from the public app.');
    }

    public editShip(ship: ShipRegisterEditDTO): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public addThirdPartyShip(ship: ShipRegisterEditDTO): Observable<number> {
        throw new Error('This method should not be called from the public app.');
    }

    public editThirdPartyShip(ship: ShipRegisterEditDTO): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public getShipOwnerAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getShipLogBookPageSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    // applications
    public getApplication(id: number, getRegiXData: boolean, pageCode: PageCodeEnum): Observable<IApplicationRegister> {
        const params = new HttpParams()
            .append('id', id.toString());

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

    public addApplication(application: ShipRegisterApplicationEditDTO, pageCode: PageCodeEnum): Observable<number> {
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

    public editApplication(application: ShipRegisterApplicationEditDTO, pageCode: PageCodeEnum): Observable<number> {
        if (pageCode === PageCodeEnum.RegVessel) {
            return this.requestService.post(this.area, this.controller, 'EditShipApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (pageCode === PageCodeEnum.ShipRegChange) {
            return this.requestService.post(this.area, this.controller, 'EditShipChangeOfCircumstancesApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (pageCode === PageCodeEnum.DeregShip) {
            return this.requestService.post(this.area, this.controller, 'EditShipDeregistrationApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }

        throw new Error('Invalid page code for editApplication: ' + PageCodeEnum[pageCode]);
    }

    // nomenclatures
    public getEventTypes(): Observable<ShipEventTypeDTO[]> {
        throw new Error('This method should not be called from the public app');
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
}

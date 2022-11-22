import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http';

import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { IFishingCapacityService } from '@app/interfaces/common-app/fishing-capacity.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { MaximumFishingCapacityDTO } from '@app/models/generated/dtos/MaximumFishingCapacityDTO';
import { MaximumFishingCapacityEditDTO } from '@app/models/generated/dtos/MaximumFishingCapacityEditDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { MaximumFishingCapacityFilters } from '@app/models/generated/filters/MaximumFishingCapacityFilters';
import { RequestService } from '@app/shared/services/request.service';
import { ApplicationsRegisterPublicBaseService } from '@app/services/public-app/applications-register-public-base.service';
import { IncreaseFishingCapacityApplicationDTO } from '@app/models/generated/dtos/IncreaseFishingCapacityApplicationDTO';
import { ReduceFishingCapacityApplicationDTO } from '@app/models/generated/dtos/ReduceFishingCapacityApplicationDTO';
import { TransferFishingCapacityApplicationDTO } from '@app/models/generated/dtos/TransferFishingCapacityApplicationDTO';
import { RequestProperties } from '@app/shared/services/request-properties';
import { FishingCapacityCertificateDTO } from '@app/models/generated/dtos/FishingCapacityCertificateDTO';
import { FishingCapacityCertificateEditDTO } from '@app/models/generated/dtos/FishingCapacityCertificateEditDTO';
import { LatestMaximumCapacityDTO } from '@app/models/generated/dtos/LatestMaximumCapacityDTO';
import { FishingCapacityCertificateNomenclatureDTO } from '@app/models/generated/dtos/FishingCapacityCertificateNomenclatureDTO';
import { FishingCapacityCertificatesFilters } from '@app/models/generated/filters/FishingCapacityCertificatesFilters';
import { FishingCapacityFilters } from '@app/models/generated/filters/FishingCapacityFilters';
import { ShipFishingCapacityDTO } from '@app/models/generated/dtos/ShipFishingCapacityDTO';
import { FishingCapacityStatisticsDTO } from '@app/models/generated/dtos/FishingCapacityStatistics';
import { CapacityCertificateDuplicateApplicationDTO } from '@app/models/generated/dtos/CapacityCertificateDuplicateApplicationDTO';
import { ExcelExporterRequestModel } from '@app/shared/components/data-table/models/excel-exporter-request-model.model';
import { PrintConfigurationParameters } from '@app/components/common-app/applications/models/print-configuration-parameters.model';

@Injectable({
    providedIn: 'root'
})
export class FishingCapacityPublicService extends ApplicationsRegisterPublicBaseService implements IFishingCapacityService {
    protected controller: string = 'FishingCapacityPublic';

    public constructor(requestService: RequestService) {
        super(requestService);
    }

    // register
    public getAllShipCapacities(request: GridRequestModel<FishingCapacityFilters>): Observable<GridResultModel<ShipFishingCapacityDTO>> {
        throw new Error('This method should not be called from the public app.');
    }

    // capacity certificates
    public getAllCapacityCertificates(request: GridRequestModel<FishingCapacityCertificatesFilters>): Observable<GridResultModel<FishingCapacityCertificateDTO>> {
        throw new Error('This method should not be called from the public app.');
    }

    public getCapacityCertificate(id: number): Observable<FishingCapacityCertificateEditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getRegisterByApplicationId(applicationId: number): Observable<unknown> {
        throw new Error('Method not applicable');
    }

    public editCapacityCertificate(certificate: FishingCapacityCertificateEditDTO): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public deleteCapacityCertificate(id: number): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public undoDeleteCapacityCertificate(id: number): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public getFishingCapacityCertificateSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getFishingCapacityHolderAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getAllCapacityCertificateNomenclatures(): Observable<FishingCapacityCertificateNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllCapacityCertificateNomenclatures', {
            responseTypeCtr: FishingCapacityCertificateNomenclatureDTO
        });
    }

    public downloadFishingCapacityCertificate(certificateId: number, configurations: PrintConfigurationParameters): Observable<boolean> {
        throw new Error('This method should not be called from the public app.');
    }

    public downloadFishingCapacityCertificateExcel(request: ExcelExporterRequestModel<FishingCapacityCertificatesFilters>): Observable<boolean> {
        throw new Error('This method should not be called from the public app.');
    }

    // applications
    public getApplication(id: number, getRegiXData: boolean, pageCode: PageCodeEnum): Observable<IApplicationRegister> {
        const params = new HttpParams()
            .append('id', id.toString());

        if (pageCode === PageCodeEnum.IncreaseFishCap) {
            return this.requestService.get(this.area, this.controller, 'GetIncreaseFishingCapacityApplication', {
                httpParams: params,
                responseTypeCtr: IncreaseFishingCapacityApplicationDTO
            });
        }
        else if (pageCode === PageCodeEnum.ReduceFishCap) {
            return this.requestService.get(this.area, this.controller, 'GetReduceFishingCapacityApplication', {
                httpParams: params,
                responseTypeCtr: ReduceFishingCapacityApplicationDTO
            });
        }
        else if (pageCode === PageCodeEnum.TransferFishCap) {
            return this.requestService.get(this.area, this.controller, 'GetTransferFishingCapacityApplication', {
                httpParams: params,
                responseTypeCtr: TransferFishingCapacityApplicationDTO
            });
        }
        else if (pageCode === PageCodeEnum.CapacityCertDup) {
            return this.requestService.get(this.area, this.controller, 'GetCapacityCertificateDuplicateApplication', {
                httpParams: params,
                responseTypeCtr: CapacityCertificateDuplicateApplicationDTO
            });
        }

        throw new Error('Invalid page code for getApplication: ' + PageCodeEnum[pageCode]);
    }

    public addApplication(application: IApplicationRegister, pageCode: PageCodeEnum): Observable<number> {
        if (pageCode === PageCodeEnum.IncreaseFishCap) {
            return this.requestService.post(this.area, this.controller, 'AddIncreaseFishingCapacityApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (pageCode === PageCodeEnum.ReduceFishCap) {
            return this.requestService.post(this.area, this.controller, 'AddReduceFishingCapacityApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (pageCode === PageCodeEnum.TransferFishCap) {
            return this.requestService.post(this.area, this.controller, 'AddTransferFishingCapacityApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (pageCode === PageCodeEnum.CapacityCertDup) {
            return this.requestService.post(this.area, this.controller, 'AddCapacityCertificateDuplicateApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }

        throw new Error('Invalid page code for addApplication: ' + PageCodeEnum[pageCode]);
    }

    public editApplication(application: IApplicationRegister,pageCode: PageCodeEnum): Observable<number> {
        if (pageCode === PageCodeEnum.IncreaseFishCap) {
            return this.requestService.post(this.area, this.controller, 'EditIncreaseFishingCapacityApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (pageCode === PageCodeEnum.ReduceFishCap) {
            return this.requestService.post(this.area, this.controller, 'EditReduceFishingCapacityApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (pageCode === PageCodeEnum.TransferFishCap) {
            return this.requestService.post(this.area, this.controller, 'EditTransferFishingCapacityApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (pageCode === PageCodeEnum.CapacityCertDup) {
            return this.requestService.post(this.area, this.controller, 'EditCapacityCertificateDuplicateApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }

        throw new Error('Invalid page code for editApplication: ' + PageCodeEnum[pageCode]);
    }

    public completeTransferFishingCapacityApplication(model: TransferFishingCapacityApplicationDTO): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public completeCapacityCertificateDuplicateApplication(model: CapacityCertificateDuplicateApplicationDTO): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    // maximum capacity
    public getAllMaximumCapacities(request: GridRequestModel<MaximumFishingCapacityFilters>): Observable<GridResultModel<MaximumFishingCapacityDTO>> {
        throw new Error('This method should not be called from the public app.');
    }

    public getMaximumCapacity(id: number): Observable<MaximumFishingCapacityEditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getLatestMaximumCapacities(): Observable<LatestMaximumCapacityDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public addMaximumCapacity(capacity: MaximumFishingCapacityEditDTO): Observable<number> {
        throw new Error('This method should not be called from the public app.');
    }

    public editMaximumCapacity(capacity: MaximumFishingCapacityEditDTO): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public getMaximumCapacitySimpleAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    // analysis
    public getFishingCapacityStatistics(date: Date): Observable<FishingCapacityStatisticsDTO> {
        throw new Error('This method should not be called from the public app.');
    }
}
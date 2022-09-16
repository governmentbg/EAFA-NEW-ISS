import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { IAquacultureFacilitiesService } from '@app/interfaces/common-app/aquaculture-facilities.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { AquacultureApplicationEditDTO } from '@app/models/generated/dtos/AquacultureApplicationEditDTO';
import { AquacultureChangeOfCircumstancesApplicationDTO } from '@app/models/generated/dtos/AquacultureChangeOfCircumstancesApplicationDTO';
import { AquacultureDeregistrationApplicationDTO } from '@app/models/generated/dtos/AquacultureDeregistrationApplicationDTO';
import { AquacultureFacilityDTO } from '@app/models/generated/dtos/AquacultureFacilityDTO';
import { AquacultureFacilityEditDTO } from '@app/models/generated/dtos/AquacultureFacilityEditDTO';
import { AquacultureFacilitiesFilters } from '@app/models/generated/filters/AquacultureFacilitiesFilters';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { ApplicationsRegisterPublicBaseService } from './applications-register-public-base.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { CancellationHistoryEntryDTO } from '@app/models/generated/dtos/CancellationHistoryEntryDTO';
import { ExcelExporterRequestModel } from '../../shared/components/data-table/models/excel-exporter-request-model.model';

@Injectable({
    providedIn: 'root'
})
export class AquacultureFacilitiesPublicService extends ApplicationsRegisterPublicBaseService implements IAquacultureFacilitiesService {
    protected controller: string = 'AquacultureFacilitiesPublic';

    public constructor(requestService: RequestService) {
        super(requestService);
    }

    // register
    public getAllAquacultures(request: GridRequestModel<AquacultureFacilitiesFilters>): Observable<GridResultModel<AquacultureFacilityDTO>> {
        throw new Error('This method should not be called from the public app.');
    }

    public getAquaculture(id: number): Observable<AquacultureFacilityEditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public addAquaculture(aquaculture: AquacultureFacilityEditDTO, ignoreLogBookConflicts: boolean): Observable<number> {
        throw new Error('This method should not be called from the public app.');
    }

    public editAquaculture(aquaculture: AquacultureFacilityEditDTO, ignoreLogBookConflicts: boolean): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public editAndDownloadAquaculture(aquaculture: AquacultureFacilityEditDTO, ignoreLogBookConflicts: boolean): Observable<boolean> {
        throw new Error('This method should not be called from the public app.');
    }

    public updateAquacultureStatus(aquacultureId: number, status: CancellationHistoryEntryDTO): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public deleteAquaculture(id: number): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public undoDeleteAquaculture(id: number): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public downloadAquacultureFacility(aquacultureId: number): Observable<boolean> {
        throw new Error('This method should not be called from the public app.');
    }

    public getAquacultureFromChangeOfCircumstancesApplication(applicationId: number): Observable<AquacultureFacilityEditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getAquacultureFromDeregistrationApplication(applicationId: number): Observable<AquacultureFacilityEditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public completeChangeOfCircumstancesApplication(aquaculture: AquacultureFacilityEditDTO, ignoreLogBookConflicts: boolean): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public completeDeregistrationApplication(aquaculture: AquacultureFacilityEditDTO): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public downloadAquacultureFacilitiesExcel(request: ExcelExporterRequestModel<AquacultureFacilitiesFilters>): Observable<boolean> {
        throw new Error('This method should not be called from the public app.');
    }

    public getInstallationAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getInstallationNetCageAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getUsageDocumentAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getWaterLawCertificateAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getOvosCertificateAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getBabhCertificateAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getLogBookAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    // applications
    public getRegisterByApplicationId(applicationId: number, pageCode: PageCodeEnum): Observable<AquacultureFacilityEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        if (pageCode === PageCodeEnum.AquaFarmReg) {
            return this.requestService.get(this.area, this.controller, 'GetRegisterByApplicationId', {
                httpParams: params,
                responseTypeCtr: AquacultureFacilityEditDTO
            });
        }
        else if (pageCode === PageCodeEnum.AquaFarmChange || pageCode === PageCodeEnum.AquaFarmDereg) {
            return this.requestService.get(this.area, this.controller, 'GetRegisterByChangeOfCircumstancesApplicationId', {
                httpParams: params,
                responseTypeCtr: AquacultureFacilityEditDTO
            });
        }

        throw new Error('Invalid pageCode for getRegisterByApplicationId for aquaculture: ' + PageCodeEnum[pageCode]);
    }

    public getApplication(applicationId: number, pageCode: PageCodeEnum): Observable<IApplicationRegister> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        if (pageCode === PageCodeEnum.AquaFarmReg) {
            return this.requestService.get(this.area, this.controller, 'GetAquacultureApplication', {
                httpParams: params,
                responseTypeCtr: AquacultureApplicationEditDTO
            });
        }
        else if (pageCode === PageCodeEnum.AquaFarmChange) {
            return this.requestService.get(this.area, this.controller, 'GetAquacultureChangeOfCircumstancesApplication', {
                httpParams: params,
                responseTypeCtr: AquacultureChangeOfCircumstancesApplicationDTO
            });
        }
        else if (pageCode === PageCodeEnum.AquaFarmDereg) {
            return this.requestService.get(this.area, this.controller, 'GetAquacultureDeregistrationApplication', {
                httpParams: params,
                responseTypeCtr: AquacultureDeregistrationApplicationDTO
            });
        }

        throw new Error('Invalid page code for getApplication: ' + PageCodeEnum[pageCode]);
    }

    public addApplication(application: IApplicationRegister, pageCode: PageCodeEnum): Observable<number> {
        if (pageCode === PageCodeEnum.AquaFarmReg) {
            return this.requestService.post(this.area, this.controller, 'AddAquacultureApplication', application, {
                properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: false })
            });
        }
        else if (pageCode === PageCodeEnum.AquaFarmChange) {
            return this.requestService.post(this.area, this.controller, 'AddAquacultureChangeOfCircumstancesApplication', application, {
                properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: false })
            });
        }
        else if (pageCode === PageCodeEnum.AquaFarmDereg) {
            return this.requestService.post(this.area, this.controller, 'AddAquacultureDeregistrationApplication', application, {
                properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: false })
            });
        }

        throw new Error('Invalid page code for addApplication: ' + PageCodeEnum[pageCode]);
    }

    public editApplication(application: IApplicationRegister, pageCode: PageCodeEnum): Observable<number> {

        if (pageCode === PageCodeEnum.AquaFarmReg) {
            return this.requestService.post(this.area, this.controller, 'EditAquacultureApplication', application, {
                properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: false })
            });
        }
        else if (pageCode === PageCodeEnum.AquaFarmChange) {
            return this.requestService.post(this.area, this.controller, 'EditAquacultureChangeOfCircumstancesApplication', application, {
                properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: false })
            });
        }
        else if (pageCode === PageCodeEnum.AquaFarmDereg) {
            return this.requestService.post(this.area, this.controller, 'EditAquacultureDeregistrationApplication', application, {
                properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: false })
            });
        }

        throw new Error('Invalid page code for editApplication: ' + PageCodeEnum[pageCode]);
    }

    // Nomenclatures
    public getAllAquacultureNomenclatures(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllAquacultureNomenclatures', { responseTypeCtr: NomenclatureDTO });
    }

    public getAquaculturePowerSupplyTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAquaculturePowerSupplyTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getAquacultureWaterAreaTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAquacultureWaterAreaTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getWaterLawCertificateTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetWaterLawCertificateTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getInstallationTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAquacultureInstallationTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getInstallationBasinPurposeTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetInstallationBasinPurposeTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getInstallationBasinMaterialTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetInstallationBasinMaterialTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getHatcheryEquipmentTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetHatcheryEquipmentTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getInstallationNetCageTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetInstallationNetCageTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getInstallationCollectorTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetInstallationCollectorTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getAquacultureStatusTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAquacultureStatusTypes', { responseTypeCtr: NomenclatureDTO });
    }
}
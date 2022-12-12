import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { IAquacultureFacilitiesService } from '@app/interfaces/common-app/aquaculture-facilities.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { AquacultureApplicationEditDTO } from '@app/models/generated/dtos/AquacultureApplicationEditDTO';
import { AquacultureChangeOfCircumstancesApplicationDTO } from '@app/models/generated/dtos/AquacultureChangeOfCircumstancesApplicationDTO';
import { AquacultureChangeOfCircumstancesRegixDataDTO } from '@app/models/generated/dtos/AquacultureChangeOfCircumstancesRegixDataDTO';
import { AquacultureDeregistrationApplicationDTO } from '@app/models/generated/dtos/AquacultureDeregistrationApplicationDTO';
import { AquacultureDeregistrationRegixDataDTO } from '@app/models/generated/dtos/AquacultureDeregistrationRegixDataDTO';
import { AquacultureFacilityDTO } from '@app/models/generated/dtos/AquacultureFacilityDTO';
import { AquacultureFacilityEditDTO } from '@app/models/generated/dtos/AquacultureFacilityEditDTO';
import { AquacultureRegixDataDTO } from '@app/models/generated/dtos/AquacultureRegixDataDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { AquacultureFacilitiesFilters } from '@app/models/generated/filters/AquacultureFacilitiesFilters';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { ApplicationsProcessingService } from './applications-processing.service';
import { ApplicationsRegisterAdministrativeBaseService } from './applications-register-administrative-base.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { CancellationHistoryEntryDTO } from '@app/models/generated/dtos/CancellationHistoryEntryDTO';
import { ExcelExporterRequestModel } from '@app/shared/components/data-table/models/excel-exporter-request-model.model';
import { RegisterDTO } from '@app/models/generated/dtos/RegisterDTO';
import { ILogBookService } from '@app/components/common-app/commercial-fishing/components/edit-log-book/interfaces/log-book.interface';
import { CommercialFishingLogBookEditDTO } from '@app/models/generated/dtos/CommercialFishingLogBookEditDTO';
import { LogBookEditDTO } from '@app/models/generated/dtos/LogBookEditDTO';
import { LogBookForRenewalDTO } from '@app/models/generated/dtos/LogBookForRenewalDTO';
import { RangeOverlappingLogBooksDTO } from '@app/models/generated/dtos/RangeOverlappingLogBooksDTO';
import { OverlappingLogBooksParameters } from '@app/shared/components/overlapping-log-books/models/overlapping-log-books-parameters.model';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { LogBookDetailDTO } from '@app/models/generated/dtos/LogBookDetailDTO';
import { PermissionsService } from '@app/shared/services/permissions.service';

@Injectable({
    providedIn: 'root'
})
export class AquacultureFacilitiesAdministrationService extends ApplicationsRegisterAdministrativeBaseService implements IAquacultureFacilitiesService, ILogBookService {
    protected controller: string = 'AquacultureFacilitiesAdministration';
    private readonly permissions: PermissionsService;

    public constructor(
        requestService: RequestService,
        applicationProcessingService: ApplicationsProcessingService,
        permissions: PermissionsService
    ) {
        super(requestService, applicationProcessingService);
        this.permissions = permissions;
    }

    // register
    public getAllAquacultures(request: GridRequestModel<AquacultureFacilitiesFilters>): Observable<GridResultModel<AquacultureFacilityDTO>> {
        type Result = GridResultModel<AquacultureFacilityDTO>;
        type Body = GridRequestModel<AquacultureFacilitiesFilters>;

        return this.requestService.post<Result, Body>(this.area, this.controller, 'GetAllAquacultures', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        }).pipe(switchMap((entries: Result) => {
            const aquacultureIds: number[] = entries.records.map((aquacultureFacility: AquacultureFacilityDTO) => {
                return aquacultureFacility.id!;
            });

            if (aquacultureIds.length === 0) {
                return of(entries);
            }

            if (this.permissions.has(PermissionsEnum.AquacultureLogBook1Read)) {
                return this.getLogBooksForTable(this.controller, aquacultureIds, request.filters).pipe(map((logBooks: LogBookDetailDTO[]) => {
                    for (const logBook of logBooks) {
                        const found = entries.records.find((entry: AquacultureFacilityDTO) => {
                            return entry.id === logBook.registerId;
                        });

                        if (found !== undefined) {
                            if (found.logBooks !== undefined && found.logBooks !== null) {
                                found.logBooks.push(new LogBookDetailDTO(logBook));
                            }
                            else {
                                found.logBooks = [logBook];
                            }
                        }
                    }

                    return entries;
                }))
            }
            else {
                return of(entries);
            }
        }));
    }

    public getAquaculture(id: number): Observable<AquacultureFacilityEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetAquaculture', {
            httpParams: params,
            responseTypeCtr: AquacultureFacilityEditDTO
        });
    }

    public addAquaculture(aquaculture: AquacultureFacilityEditDTO, ignoreLogBookConflicts: boolean): Observable<number> {
        const params: HttpParams = new HttpParams().append('ignoreLogBookConflicts', ignoreLogBookConflicts.toString());

        return this.requestService.post(this.area, this.controller, 'AddAquaculture', aquaculture, {
            properties: new RequestProperties({ asFormData: true }),
            httpParams: params
        });
    }

    public editAquaculture(aquaculture: AquacultureFacilityEditDTO, ignoreLogBookConflicts: boolean): Observable<void> {
        const params: HttpParams = new HttpParams().append('ignoreLogBookConflicts', ignoreLogBookConflicts.toString());

        return this.requestService.post(this.area, this.controller, 'EditAquaculture', aquaculture, {
            properties: new RequestProperties({ asFormData: true }),
            httpParams: params
        });
    }

    public editAndDownloadAquaculture(aquaculture: AquacultureFacilityEditDTO, ignoreLogBookConflicts: boolean): Observable<boolean> {
        const params: HttpParams = new HttpParams().append('ignoreLogBookConflicts', ignoreLogBookConflicts.toString());

        const registerDto: RegisterDTO<AquacultureFacilityEditDTO> = new RegisterDTO<AquacultureFacilityEditDTO>({
            dto: aquaculture
        });

        return this.requestService.downloadPost(this.area, this.controller, 'EditAndDownloadAquaculture', 'aquaculture', registerDto, {
            properties: new RequestProperties({ asFormData: true }),
            httpParams: params
        });
    }

    public updateAquacultureStatus(aquacultureId: number, status: CancellationHistoryEntryDTO, applicationId?: number): Observable<void> {
        let params = new HttpParams().append('aquacultureId', aquacultureId.toString());

        if (applicationId !== undefined && applicationId !== null) {
            params = params.append('applicationId', applicationId.toString());
        }

        return this.requestService.put(this.area, this.controller, 'UpdateAquacultureStatus', status, {
            httpParams: params
        });
    }

    public deleteAquaculture(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'DeleteAquaculture', { httpParams: params });
    }

    public undoDeleteAquaculture(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDeleteAquaculture', null, { httpParams: params });
    }

    public downloadAquacultureFacility(aquacultureId: number): Observable<boolean> {
        const params = new HttpParams().append('aquacultureId', aquacultureId.toString());
        return this.requestService.downloadPost(this.area, this.controller, 'DownloadAquacultureFacility', 'aquaculture', undefined, {
            httpParams: params
        });
    }

    public getAquacultureFromChangeOfCircumstancesApplication(applicationId: number): Observable<AquacultureFacilityEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetAquacultureFromChangeOfCircumstancesApplication', {
            httpParams: params,
            responseTypeCtr: AquacultureFacilityEditDTO
        });
    }

    public getAquacultureFromDeregistrationApplication(applicationId: number): Observable<AquacultureFacilityEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetAquacultureFromDeregistrationApplication', {
            httpParams: params,
            responseTypeCtr: AquacultureFacilityEditDTO
        });
    }

    public completeChangeOfCircumstancesApplication(aquaculture: AquacultureFacilityEditDTO, ignoreLogBookConflicts: boolean): Observable<void> {
        const params: HttpParams = new HttpParams().append('ignoreLogBookConflicts', ignoreLogBookConflicts.toString());

        return this.requestService.post(this.area, this.controller, 'CompleteChangeOfCircumstancesApplication', aquaculture, {
            properties: new RequestProperties({ asFormData: true }),
            httpParams: params
        });
    }

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

    public downloadAquacultureFacilitiesExcel(request: ExcelExporterRequestModel<AquacultureFacilitiesFilters>): Observable<boolean> {
        return this.requestService.downloadPost(this.area, this.controller, 'DownloadAquacultureFacilitiesExcel', request.filename, request, {
            properties: new RequestProperties({ showException: true, rethrowException: true }),
        });
    }

    // log books

    public getLogBook(logBookId: number): Observable<LogBookEditDTO> {
        const params: HttpParams = new HttpParams().append('logBookId', logBookId.toString());

        return this.requestService.get(this.area, this.controller, 'GetLogBook', {
            httpParams: params,
            responseTypeCtr: LogBookEditDTO
        });
    }

    public getPermitLicenseLogBook(logBookPermitLicenseId: number): Observable<CommercialFishingLogBookEditDTO> {
        throw new Error('Should call getLogBook method');
    }

    public addLogBook(model: LogBookEditDTO, registerId: number, ignoreLogBookConflicts: boolean): Observable<void> {
        const params: HttpParams = new HttpParams()
            .append('registerId', registerId.toString())
            .append('ignoreLogBookConflicts', ignoreLogBookConflicts.toString());

        return this.requestService.post(this.area, this.controller, 'AddLogBook', model, {
            httpParams: params
        });
    }

    public editLogBook(model: LogBookEditDTO, registerId: number, ignoreLogBookConflicts: boolean): Observable<void> {
        const params: HttpParams = new HttpParams()
            .append('registerId', registerId.toString())
            .append('ignoreLogBookConflicts', ignoreLogBookConflicts.toString());

        return this.requestService.put(this.area, this.controller, 'EditLogBook', model, {
            httpParams: params
        });
    }

    public getOverlappedLogBooks(parameters: OverlappingLogBooksParameters[]): Observable<RangeOverlappingLogBooksDTO[]> {
        return this.requestService.post(this.area, this.controller, 'GetOverlappedLogBooks', parameters, {
            responseTypeCtr: RangeOverlappingLogBooksDTO
        });
    }

    public getLogBooksForRenewal(permitLicenseRegisterId: number, showFinished: boolean): Observable<LogBookForRenewalDTO[]> {
        throw new Error('Not implemented funcionality for aquaculture log book pages');
    }

    public getLogBooksForRenewalByIds(permitLicenseRegisterIds: number[]): Observable<LogBookEditDTO[]> {
        throw new Error('Not implemented funcionality for aquaculture log book pages');
    }

    public deleteLogBook(logBookId: number): Observable<void> {
        const params: HttpParams = new HttpParams().append('id', logBookId.toString());

        return this.requestService.delete(this.area, this.controller, 'DeleteLogBook', {
            httpParams: params
        });
    }

    public undoDeleteLogBook(logBookId: number): Observable<void> {
        const params: HttpParams = new HttpParams().append('id', logBookId.toString());

        return this.requestService.patch(this.area, this.controller, 'UndoDeleteLogBook', undefined, {
            httpParams: params
        });
    }

    // audits

    public getInstallationAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetAquacultureInstallationSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getInstallationNetCageAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetAquacultureInstallationNetCageAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getUsageDocumentAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetAquacultureUsageDocumentSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getWaterLawCertificateAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetAquacultureWaterLawCertificateSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getOvosCertificateAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetAquacultureOvosCertificateSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getBabhCertificateAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetAquacultureBabhCertificateSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getLogBookAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetAquacultureLogBookSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    // applications
    public getApplication(applicationId: number, getRegiXData: boolean, pageCode: PageCodeEnum): Observable<IApplicationRegister> {
        const params = new HttpParams()
            .append('id', applicationId.toString())
            .append('getRegiXData', getRegiXData.toString());

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

    public getRegixData(applicationId: number, pageCode: PageCodeEnum): Observable<RegixChecksWrapperDTO<IApplicationRegister>> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        if (pageCode === PageCodeEnum.AquaFarmReg) {
            return this.requestService.get<RegixChecksWrapperDTO<AquacultureRegixDataDTO>>(this.area, this.controller, 'GetAquacultureRegixData', {
                httpParams: params,
                responseTypeCtr: RegixChecksWrapperDTO
            }).pipe(map((result: RegixChecksWrapperDTO<AquacultureRegixDataDTO>) => {
                result.dialogDataModel = new AquacultureRegixDataDTO(result.dialogDataModel);
                result.regiXDataModel = new AquacultureRegixDataDTO(result.regiXDataModel);

                return result;
            }));
        }
        else if (pageCode === PageCodeEnum.AquaFarmChange) {
            return this.requestService.get<RegixChecksWrapperDTO<AquacultureChangeOfCircumstancesRegixDataDTO>>(this.area, this.controller, 'GetAquacultureChangeOfCircumstancesRegixData', {
                httpParams: params,
                responseTypeCtr: RegixChecksWrapperDTO
            }).pipe(map((result: RegixChecksWrapperDTO<AquacultureChangeOfCircumstancesRegixDataDTO>) => {
                result.dialogDataModel = new AquacultureChangeOfCircumstancesRegixDataDTO(result.dialogDataModel);
                result.regiXDataModel = new AquacultureChangeOfCircumstancesRegixDataDTO(result.regiXDataModel);

                return result;
            }));
        }
        else if (pageCode === PageCodeEnum.AquaFarmDereg) {
            return this.requestService.get<RegixChecksWrapperDTO<AquacultureDeregistrationRegixDataDTO>>(this.area, this.controller, 'GetAquacultureDeregistrationRegixData', {
                httpParams: params,
                responseTypeCtr: RegixChecksWrapperDTO
            }).pipe(map((result: RegixChecksWrapperDTO<AquacultureDeregistrationRegixDataDTO>) => {
                result.dialogDataModel = new AquacultureDeregistrationRegixDataDTO(result.dialogDataModel);
                result.regiXDataModel = new AquacultureDeregistrationRegixDataDTO(result.regiXDataModel);

                return result;
            }));
        }

        throw new Error('Invalid page code for getRegixData: ' + PageCodeEnum[pageCode]);
    }

    public getApplicationDataForRegister(applicationId: number, pageCode: PageCodeEnum): Observable<IApplicationRegister> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetApplicationDataForRegister', {
            httpParams: params,
            responseTypeCtr: AquacultureFacilityEditDTO
        });
    }

    public addApplication(application: IApplicationRegister, pageCode: PageCodeEnum): Observable<number> {
        if (pageCode === PageCodeEnum.AquaFarmReg) {
            return this.requestService.post(this.area, this.controller, 'AddAquacultureApplication', application, {
                properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: true })
            });
        }
        else if (pageCode === PageCodeEnum.AquaFarmChange) {
            return this.requestService.post(this.area, this.controller, 'AddAquacultureChangeOfCircumstancesApplication', application, {
                properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: true })
            });
        }
        else if (pageCode === PageCodeEnum.AquaFarmDereg) {
            return this.requestService.post(this.area, this.controller, 'AddAquacultureDeregistrationApplication', application, {
                properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: true })
            });
        }

        throw new Error('Invalid page code for addApplication: ' + PageCodeEnum[pageCode]);
    }

    public editApplication(application: IApplicationRegister, pageCode: PageCodeEnum, saveAsDraft: boolean): Observable<number> {
        const params = new HttpParams().append('saveAsDraft', saveAsDraft.toString());

        if (pageCode === PageCodeEnum.AquaFarmReg) {
            return this.requestService.post(this.area, this.controller, 'EditAquacultureApplication', application, {
                httpParams: params,
                properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: true })
            });
        }
        else if (pageCode === PageCodeEnum.AquaFarmChange) {
            return this.requestService.post(this.area, this.controller, 'EditAquacultureChangeOfCircumstancesApplication', application, {
                httpParams: params,
                properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: true })
            });
        }
        else if (pageCode === PageCodeEnum.AquaFarmDereg) {
            return this.requestService.post(this.area, this.controller, 'EditAquacultureDeregistrationApplication', application, {
                httpParams: params,
                properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: true })
            });
        }

        throw new Error('Invalid page code for editApplication: ' + PageCodeEnum[pageCode]);
    }

    public editApplicationDataAndStartRegixChecks(model: IApplicationRegister): Observable<void> {
        if (model.pageCode === PageCodeEnum.AquaFarmReg) {
            return this.requestService.post(this.area, this.controller, 'EditAquacultureApplicationAndStartRegixChecks', model, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (model.pageCode === PageCodeEnum.AquaFarmChange) {
            return this.requestService.post(this.area, this.controller, 'EditAquacultureChangeOfCircumstancesApplicationAndStartRegixChecks', model, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (model.pageCode === PageCodeEnum.AquaFarmDereg) {
            return this.requestService.post(this.area, this.controller, 'EditAquacultureDeregistrationApplicationAndStartRegixChecks', model, {
                properties: new RequestProperties({ asFormData: true })
            });
        }

        throw new Error('Invalid page code for editApplicationDataAndStartRegixChecks: ' + PageCodeEnum[model.pageCode!]);
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

    // helpers

    private getLogBooksForTable(controller: string, aquacultureIds: number[], filters: AquacultureFacilitiesFilters | undefined): Observable<LogBookDetailDTO[]> {
        return this.requestService.post(this.area, controller, 'GetLogBooksForTable', aquacultureIds, {
            responseTypeCtr: LogBookDetailDTO
        });
    }
}
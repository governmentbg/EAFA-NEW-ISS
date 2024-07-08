import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { IFishingCapacityService } from '@app/interfaces/common-app/fishing-capacity.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { MaximumFishingCapacityDTO } from '@app/models/generated/dtos/MaximumFishingCapacityDTO';
import { MaximumFishingCapacityEditDTO } from '@app/models/generated/dtos/MaximumFishingCapacityEditDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { MaximumFishingCapacityFilters } from '@app/models/generated/filters/MaximumFishingCapacityFilters';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { ApplicationsProcessingService } from '@app/services/administration-app/applications-processing.service';
import { ApplicationsRegisterAdministrativeBaseService } from '@app/services/administration-app/applications-register-administrative-base.service';
import { IncreaseFishingCapacityApplicationDTO } from '@app/models/generated/dtos/IncreaseFishingCapacityApplicationDTO';
import { ReduceFishingCapacityApplicationDTO } from '@app/models/generated/dtos/ReduceFishingCapacityApplicationDTO';
import { TransferFishingCapacityApplicationDTO } from '@app/models/generated/dtos/TransferFishingCapacityApplicationDTO';
import { IncreaseFishingCapacityRegixDataDTO } from '@app/models/generated/dtos/IncreaseFishingCapacityRegixDataDTO';
import { ReduceFishingCapacityRegixDataDTO } from '@app/models/generated/dtos/ReduceFishingCapacityRegixDataDTO';
import { TransferFishingCapacityRegixDataDTO } from '@app/models/generated/dtos/TransferFishingCapacityRegixDataDTO';
import { FishingCapacityCertificateDTO } from '@app/models/generated/dtos/FishingCapacityCertificateDTO';
import { FishingCapacityCertificateEditDTO } from '@app/models/generated/dtos/FishingCapacityCertificateEditDTO';
import { LatestMaximumCapacityDTO } from '@app/models/generated/dtos/LatestMaximumCapacityDTO';
import { FishingCapacityCertificateNomenclatureDTO } from '@app/models/generated/dtos/FishingCapacityCertificateNomenclatureDTO';
import { FishingCapacityCertificatesFilters } from '@app/models/generated/filters/FishingCapacityCertificatesFilters';
import { FishingCapacityFilters } from '@app/models/generated/filters/FishingCapacityFilters';
import { ShipFishingCapacityDTO } from '@app/models/generated/dtos/ShipFishingCapacityDTO';
import { ShipFishingCapacityHistoryDTO } from '@app/models/generated/dtos/ShipFishingCapacityHistoryDTO';
import { IncreaseFishingCapacityDataDTO } from '@app/models/generated/dtos/IncreaseFishingCapacityDataDTO';
import { ReduceFishingCapacityDataDTO } from '@app/models/generated/dtos/ReduceFishingCapacityDataDTO';
import { CapacityCertificateHistoryDTO } from '@app/models/generated/dtos/CapacityCertificateHistoryDTO';
import { FishingCapacityStatisticsDTO } from '@app/models/generated/dtos/FishingCapacityStatistics';
import { CapacityCertificateDuplicateApplicationDTO } from '@app/models/generated/dtos/CapacityCertificateDuplicateApplicationDTO';
import { CapacityCertificateDuplicateRegixDataDTO } from '@app/models/generated/dtos/CapacityCertificateDuplicateRegixDataDTO';
import { ExcelExporterRequestModel } from '@app/shared/components/data-table/models/excel-exporter-request-model.model';
import { PaymentTariffDTO } from '@app/models/generated/dtos/PaymentTariffDTO';
import { FreedCapacityTariffCalculationParameters } from '@app/components/common-app/ships-register/models/freed-capacity-tariff-calculation-parameters.model';

@Injectable({
    providedIn: 'root'
})
export class FishingCapacityAdministrationService extends ApplicationsRegisterAdministrativeBaseService implements IFishingCapacityService {
    protected controller: string = 'FishingCapacityAdministration';

    private translate: FuseTranslationLoaderService;

    public constructor(
        requestService: RequestService,
        applicationProcessingService: ApplicationsProcessingService,
        translate: FuseTranslationLoaderService
    ) {
        super(requestService, applicationProcessingService);

        this.translate = translate;
    }

    // register
    public getAllShipCapacities(request: GridRequestModel<FishingCapacityFilters>): Observable<GridResultModel<ShipFishingCapacityDTO>> {
        type Result = GridResultModel<ShipFishingCapacityDTO>;
        type Body = GridRequestModel<FishingCapacityFilters>;

        return this.requestService.post<Result, Body>(this.area, this.controller, 'GetAllShipCapacities', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        }).pipe(map((entries: Result) => {
            for (const entry of entries.records) {
                if (entry.history !== undefined && entry.history !== null) {
                    for (const hist of entry.history) {
                        if (hist.applicationId === undefined || hist.applicationId === null) {
                            hist.reason = this.translate.getValue('fishing-capacity.manually-deregistered-ship');
                        }
                    }
                }
            }
            return entries;
        }));
    }

    // capacity certificates
    public getAllCapacityCertificates(request: GridRequestModel<FishingCapacityCertificatesFilters>): Observable<GridResultModel<FishingCapacityCertificateDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllCapacityCertificates', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getRegisterByApplicationId(applicationId: number): Observable<unknown> {
        throw new Error('Method not applicable.');
    }

    public getCapacityCertificate(id: number): Observable<FishingCapacityCertificateEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetCapacityCertificate', {
            httpParams: params,
            responseTypeCtr: FishingCapacityCertificateEditDTO
        });
    }

    public editCapacityCertificate(certificate: FishingCapacityCertificateEditDTO): Observable<void> {
        return this.requestService.put(this.area, this.controller, 'EditCapacityCertificate', certificate);
    }

    public deleteCapacityCertificate(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'DeleteCapacityCertificate', { httpParams: params });
    }

    public undoDeleteCapacityCertificate(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDeleteCapacityCertificate', null, { httpParams: params });
    }

    public getFishingCapacityCertificateSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.get(this.area, this.controller, 'GetFishingCapacityCertificateSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getFishingCapacityHolderAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetFishingCapacityHolderSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getAllCapacityCertificateNomenclatures(): Observable<FishingCapacityCertificateNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllCapacityCertificateNomenclatures', {
            responseTypeCtr: FishingCapacityCertificateNomenclatureDTO
        });
    }

    public downloadFishingCapacityCertificate(certificateId: number): Observable<boolean> {
        const params = new HttpParams().append('certificateId', certificateId.toString());
        return this.requestService.downloadPost(this.area, this.controller, 'DownloadFishingCapacityCertificate', 'certificate', undefined, {
            httpParams: params
        });
    }

    public downloadFishingCapacityCertificateExcel(request: ExcelExporterRequestModel<FishingCapacityCertificatesFilters>): Observable<boolean> {
        return this.requestService.downloadPost(this.area, this.controller, 'DownloadFishingCapacityCertificateExcel', request.filename, request, {
            properties: new RequestProperties({ showException: true, rethrowException: true }),
        });
    }

    // applications
    public getApplication(id: number, getRegiXData: boolean, pageCode: PageCodeEnum): Observable<IApplicationRegister> {
        const params = new HttpParams()
            .append('id', id.toString())
            .append('getRegiXData', getRegiXData.toString());

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

    public getRegixData(applicationId: number, pageCode: PageCodeEnum): Observable<RegixChecksWrapperDTO<IApplicationRegister>> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        if (pageCode === PageCodeEnum.IncreaseFishCap) {
            type ResultType = RegixChecksWrapperDTO<IncreaseFishingCapacityRegixDataDTO>;

            return this.requestService.get<ResultType>(this.area, this.controller, 'GetIncreaseFishingCapacityRegixData', {
                httpParams: params,
                responseTypeCtr: RegixChecksWrapperDTO
            }).pipe(map((result: ResultType) => {
                result.dialogDataModel = new IncreaseFishingCapacityRegixDataDTO(result.dialogDataModel);
                result.regiXDataModel = new IncreaseFishingCapacityRegixDataDTO(result.regiXDataModel);

                return result;
            }));
        }
        else if (pageCode === PageCodeEnum.ReduceFishCap) {
            type ResultType = RegixChecksWrapperDTO<ReduceFishingCapacityRegixDataDTO>;

            return this.requestService.get<ResultType>(this.area, this.controller, 'GetReduceFishingCapacityRegixData', {
                httpParams: params,
                responseTypeCtr: RegixChecksWrapperDTO
            }).pipe(map((result: ResultType) => {
                result.dialogDataModel = new ReduceFishingCapacityRegixDataDTO(result.dialogDataModel);
                result.regiXDataModel = new ReduceFishingCapacityRegixDataDTO(result.regiXDataModel);

                return result;
            }));
        }
        else if (pageCode === PageCodeEnum.TransferFishCap) {
            type ResultType = RegixChecksWrapperDTO<TransferFishingCapacityRegixDataDTO>;

            return this.requestService.get<ResultType>(this.area, this.controller, 'GetTransferFishingCapacityRegixData', {
                httpParams: params,
                responseTypeCtr: RegixChecksWrapperDTO
            }).pipe(map((result: ResultType) => {
                result.dialogDataModel = new TransferFishingCapacityRegixDataDTO(result.dialogDataModel);
                result.regiXDataModel = new TransferFishingCapacityRegixDataDTO(result.regiXDataModel);

                return result;
            }));
        }
        else if (pageCode === PageCodeEnum.CapacityCertDup) {
            type ResultType = RegixChecksWrapperDTO<CapacityCertificateDuplicateRegixDataDTO>;

            return this.requestService.get<ResultType>(this.area, this.controller, 'GetCapacityCertificateDuplicateRegixData', {
                httpParams: params,
                responseTypeCtr: RegixChecksWrapperDTO
            }).pipe(map((result: ResultType) => {
                result.dialogDataModel = new CapacityCertificateDuplicateRegixDataDTO(result.dialogDataModel);
                result.regiXDataModel = new CapacityCertificateDuplicateRegixDataDTO(result.regiXDataModel);

                return result;
            }));
        }

        throw new Error('Invalid page code for getRegixData: ' + PageCodeEnum[pageCode]);
    }

    public getApplicationDataForRegister(applicationId: number, pageCode: PageCodeEnum): Observable<IApplicationRegister> {
        throw new Error('Method not implemented.');
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

    public editApplication(application: IApplicationRegister, pageCode: PageCodeEnum, saveAsDraft: boolean): Observable<number> {
        const params = new HttpParams().append('saveAsDraft', saveAsDraft.toString());

        if (pageCode === PageCodeEnum.IncreaseFishCap) {
            return this.requestService.post(this.area, this.controller, 'EditIncreaseFishingCapacityApplication', application, {
                httpParams: params,
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (pageCode === PageCodeEnum.ReduceFishCap) {
            return this.requestService.post(this.area, this.controller, 'EditReduceFishingCapacityApplication', application, {
                httpParams: params,
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (pageCode === PageCodeEnum.TransferFishCap) {
            return this.requestService.post(this.area, this.controller, 'EditTransferFishingCapacityApplication', application, {
                httpParams: params,
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (pageCode === PageCodeEnum.CapacityCertDup) {
            return this.requestService.post(this.area, this.controller, 'EditCapacityCertificateDuplicateApplication', application, {
                httpParams: params,
                properties: new RequestProperties({ asFormData: true })
            });
        }

        throw new Error('Invalid page code for editApplication: ' + PageCodeEnum[pageCode]);
    }

    public editApplicationDataAndStartRegixChecks(model: IApplicationRegister): Observable<void> {
        if (model.pageCode === PageCodeEnum.IncreaseFishCap) {
            return this.requestService.post(this.area, this.controller, 'EditIncreaseFishingCapacityApplicationAndStartRegixChecks', model, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (model.pageCode === PageCodeEnum.ReduceFishCap) {
            return this.requestService.post(this.area, this.controller, 'EditReduceFishingCapacityApplicationAndStartRegixChecks', model, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (model.pageCode === PageCodeEnum.TransferFishCap) {
            return this.requestService.post(this.area, this.controller, 'EditTransferFishingCapacityApplicationAndStartRegixChecks', model, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (model.pageCode === PageCodeEnum.CapacityCertDup) {
            return this.requestService.post(this.area, this.controller, 'EditCapacityCertificateDuplicateApplicationAndStartRegixChecks', model, {
                properties: new RequestProperties({ asFormData: true })
            });
        }

        throw new Error('Invalid page code for editApplicationDataAndStartRegixChecks: ' + PageCodeEnum[model.pageCode!]);
    }

    public getCapacityDataFromApplication(applicationId: number, pageCode: PageCodeEnum): Observable<IncreaseFishingCapacityDataDTO | ReduceFishingCapacityDataDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        if (pageCode === PageCodeEnum.IncreaseFishCap) {
            return this.requestService.get(this.area, this.controller, 'GetCapacityDataFromIncreaseCapacityApplication', {
                httpParams: params,
                responseTypeCtr: IncreaseFishingCapacityDataDTO
            });
        }
        else if (pageCode === PageCodeEnum.ReduceFishCap) {
            return this.requestService.get(this.area, this.controller, 'GetCapacityDataFromReduceCapacityApplication', {
                httpParams: params,
                responseTypeCtr: ReduceFishingCapacityDataDTO
            });
        }

        throw new Error('Invalid page code for getCapacityDataFromApplication: ' + PageCodeEnum[pageCode]);
    }

    public completeTransferFishingCapacityApplication(model: TransferFishingCapacityApplicationDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'CompleteTransferFishingCapacityApplication', model, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public completeCapacityCertificateDuplicateApplication(model: CapacityCertificateDuplicateApplicationDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'CompleteCapacityCertificateDuplicateApplication', model, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    // maximum capacity
    public getAllMaximumCapacities(request: GridRequestModel<MaximumFishingCapacityFilters>): Observable<GridResultModel<MaximumFishingCapacityDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllMaximumCapacities', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getMaximumCapacity(id: number): Observable<MaximumFishingCapacityEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetMaximumCapacity', {
            httpParams: params,
            responseTypeCtr: MaximumFishingCapacityEditDTO
        });
    }

    public getLatestMaximumCapacities(): Observable<LatestMaximumCapacityDTO> {
        return this.requestService.get(this.area, this.controller, 'GetLatestMaximumCapacities', {
            responseTypeCtr: LatestMaximumCapacityDTO
        });
    }

    public addMaximumCapacity(capacity: MaximumFishingCapacityEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddMaximumCapacity', capacity);
    }

    public editMaximumCapacity(capacity: MaximumFishingCapacityEditDTO): Observable<void> {
        return this.requestService.put(this.area, this.controller, 'EditMaximumCapacity', capacity);
    }

    public getMaximumCapacitySimpleAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetMaximumCapacitySimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    // analysis
    public getFishingCapacityStatistics(date: Date): Observable<FishingCapacityStatisticsDTO> {
        const params = new HttpParams()
            .append('year', date.getFullYear().toString())
            .append('month', (date.getMonth() + 1).toString())
            .append('day', date.getDate().toString());

        return this.requestService.get(this.area, this.controller, 'GetFishingCapacityStatistics', {
            httpParams: params,
            responseTypeCtr: FishingCapacityStatisticsDTO
        });
    }

    //tariffs
    public calculateFreedCapacityAppliedTariffs(parameters: FreedCapacityTariffCalculationParameters): Observable<PaymentTariffDTO[]> {
        return this.requestService.post(this.area, this.controller, 'CalculateFreedCapacityAppliedTariffs', parameters, {
            responseTypeCtr: PaymentTariffDTO
        });
    }

    // helpers
    private getShipCapacityHistory(shipIds: number[]): Observable<ShipFishingCapacityHistoryDTO[]> {
        return this.requestService.post(this.area, this.controller, 'GetShipCapacityHistory', shipIds, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: ShipFishingCapacityHistoryDTO
        });
    }

    private getCapacityCertificatesHistory(certificateIds: number[]): Observable<CapacityCertificateHistoryDTO[]> {
        return this.requestService.post(this.area, this.controller, 'GetCapacityCertificateHistory', certificateIds, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: CapacityCertificateHistoryDTO
        });
    }
}
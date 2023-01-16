import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { StatisticalFormDTO } from '@app/models/generated/dtos/StatisticalFormDTO';
import { StatisticalFormsFilters } from '@app/models/generated/filters/StatisticalFormsFilters';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { ApplicationsRegisterAdministrativeBaseService } from './applications-register-administrative-base.service';
import { IStatisticalFormsService } from '@app/interfaces/common-app/statistical-forms.interface';
import { ApplicationsProcessingService } from './applications-processing.service';
import { StatisticalFormAquaFarmEditDTO } from '@app/models/generated/dtos/StatisticalFormAquaFarmEditDTO';
import { StatisticalFormFishVesselEditDTO } from '@app/models/generated/dtos/StatisticalFormFishVesselEditDTO';
import { StatisticalFormReworkEditDTO } from '@app/models/generated/dtos/StatisticalFormReworkEditDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { StatisticalFormAquaFarmApplicationEditDTO } from '@app/models/generated/dtos/StatisticalFormAquaFarmApplicationEditDTO';
import { StatisticalFormReworkApplicationEditDTO } from '@app/models/generated/dtos/StatisticalFormReworkApplicationEditDTO';
import { StatisticalFormFishVesselApplicationEditDTO } from '@app/models/generated/dtos/StatisticalFormFishVesselApplicationEditDTO';
import { StatisticalFormAquaFarmRegixDataDTO } from '@app/models/generated/dtos/StatisticalFormAquaFarmRegixDataDTO';
import { StatisticalFormReworkRegixDataDTO } from '@app/models/generated/dtos/StatisticalFormReworkRegixDataDTO';
import { StatisticalFormFishVesselRegixDataDTO } from '@app/models/generated/dtos/StatisticalFormFishVesselRegixDataDTO';
import { StatisticalFormShipDTO } from '@app/models/generated/dtos/StatisticalFormShipDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { StatisticalFormAquacultureDTO } from '@app/models/generated/dtos/StatisticalFormAquacultureDTO';
import { StatisticalFormAquacultureNomenclatureDTO } from '@app/models/generated/dtos/StatisticalFormAquacultureNomenclatureDTO';
import { StatisticalFormTypesEnum } from '../../enums/statistical-form-types.enum';

@Injectable({
    providedIn: 'root'
})
export class StatisticalFormsAdministrationService extends ApplicationsRegisterAdministrativeBaseService implements IStatisticalFormsService {
    protected controller: string = 'StatisticalFormsAdministration';

    private translate: FuseTranslationLoaderService;

    public constructor(
        requestService: RequestService,
        applicationProcessingService: ApplicationsProcessingService,
        translate: FuseTranslationLoaderService
    ) {
        super(requestService, applicationProcessingService);

        this.translate = translate;
    }

    // Register
    public getAllStatisticalForms(request: GridRequestModel<StatisticalFormsFilters>): Observable<GridResultModel<StatisticalFormDTO>> {
        type Request = GridRequestModel<StatisticalFormsFilters>;
        type Response = GridResultModel<StatisticalFormDTO>;

        return this.requestService.post<Response, Request>(this.area, this.controller, 'GetAllStatisticalForms', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        }).pipe(map((result: GridResultModel<StatisticalFormDTO>) => {
            for (const entry of result.records) {
                switch (entry.formType) {
                    case StatisticalFormTypesEnum.AquaFarm:
                        entry.formObject = entry.formObject!.replace('{{URORR}}', this.translate.getValue('statistical-forms.aqua-farm-uror'));
                        entry.formObject = entry.formObject!.replace('{{REG}}', this.translate.getValue('statistical-forms.aqua-farm-reg-num'));
                        break;
                    case StatisticalFormTypesEnum.FishVessel:
                        entry.formObject = entry.formObject!.replace('{{CFR}}', this.translate.getValue('common.ship-cfr'));
                        entry.formObject = entry.formObject!.replace('{{EXT}}', this.translate.getValue('common.ship-external-mark'));
                        break;
                }
            }

            return result;
        }));
    }

    public getStatisticalFormAquaFarm(id: number): Observable<StatisticalFormAquaFarmEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetStatisticalFormAquaFarm', {
            httpParams: params,
            responseTypeCtr: StatisticalFormAquaFarmEditDTO
        });
    }

    public getStatisticalFormRework(id: number): Observable<StatisticalFormReworkEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetStatisticalFormRework', {
            httpParams: params,
            responseTypeCtr: StatisticalFormReworkEditDTO
        });
    }

    public getStatisticalFormFishVessel(id: number): Observable<StatisticalFormFishVesselEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetStatisticalFormFishVessel', {
            httpParams: params,
            responseTypeCtr: StatisticalFormFishVesselEditDTO
        });
    }

    public addStatisticalFormAquaFarm(form: StatisticalFormAquaFarmEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddStatisticalFormAquaFarm', form, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public addStatisticalFormRework(form: StatisticalFormReworkEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddStatisticalFormRework', form, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public addStatisticalFormFishVessel(form: StatisticalFormFishVesselEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddStatisticalFormFishVessel', form, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editStatisticalFormAquaFarm(form: StatisticalFormAquaFarmEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditStatisticalFormAquaFarm', form, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editStatisticalFormRework(form: StatisticalFormReworkEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditStatisticalFormRework', form, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editStatisticalFormFishVessel(form: StatisticalFormFishVesselEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditStatisticalFormFishVessel', form, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public deleteStatisticalForm(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'DeleteStatisticalForm', { httpParams: params });
    }

    public undoDeleteStatisticalForm(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDeleteStatisticalForm', null, { httpParams: params });
    }

    public getStatisticalFormShip(shipId: number): Observable<StatisticalFormShipDTO> {
        const params = new HttpParams().append('shipId', shipId.toString());

        return this.requestService.get(this.area, this.controller, 'GetStatisticalFormShip', {
            httpParams: params,
            responseTypeCtr: StatisticalFormShipDTO
        });
    }

    public getStatisticalFormAquaculture(aquacultureId: number): Observable<StatisticalFormAquacultureDTO> {
        const params = new HttpParams().append('aquacultureId', aquacultureId.toString());

        return this.requestService.get(this.area, this.controller, 'GetStatisticalFormAquaculture', {
            httpParams: params,
            responseTypeCtr: StatisticalFormAquacultureDTO
        });
    }

    public getShipFishingGearsForYear(shipId: number, year: number): Observable<NomenclatureDTO<number>[]> {
        const params = new HttpParams()
            .append('shipId', shipId.toString())
            .append('year', year.toString());

        return this.requestService.get(this.area, this.controller, 'GetShipFishingGearsForYear', {
            httpParams: params,
            responseTypeCtr: NomenclatureDTO
        });
    }

    // applications
    public getApplication(id: number, getRegiXData: boolean, pageCode: PageCodeEnum): Observable<IApplicationRegister> {
        const params = new HttpParams()
            .append('id', id.toString())
            .append('getRegiXData', getRegiXData.toString());

        if (pageCode === PageCodeEnum.StatFormAquaFarm) {
            return this.requestService.get(this.area, this.controller, 'GetStatisticalFormAquaFarmApplication', {
                httpParams: params,
                responseTypeCtr: StatisticalFormAquaFarmApplicationEditDTO
            });
        }
        else if (pageCode === PageCodeEnum.StatFormRework) {
            return this.requestService.get(this.area, this.controller, 'GetStatisticalFormReworkApplication', {
                httpParams: params,
                responseTypeCtr: StatisticalFormReworkApplicationEditDTO
            });
        }
        else if (pageCode === PageCodeEnum.StatFormFishVessel) {
            return this.requestService.get(this.area, this.controller, 'GetStatisticalFormFishVesselApplication', {
                httpParams: params,
                responseTypeCtr: StatisticalFormFishVesselApplicationEditDTO
            });
        }

        throw new Error('Invalid page code for getApplication: ' + PageCodeEnum[pageCode]);
    }

    public getRegixData(id: number, pageCode: PageCodeEnum): Observable<RegixChecksWrapperDTO<IApplicationRegister>> {
        const params = new HttpParams().append('applicationId', id.toString());

        if (pageCode === PageCodeEnum.StatFormAquaFarm) {
            return this.requestService.get<RegixChecksWrapperDTO<StatisticalFormAquaFarmRegixDataDTO>>(this.area, this.controller, 'GetStatisticalFormAquaFarmRegixData', {
                httpParams: params,
                responseTypeCtr: RegixChecksWrapperDTO
            }).pipe(map((result: RegixChecksWrapperDTO<StatisticalFormAquaFarmRegixDataDTO>) => {
                result.dialogDataModel = new StatisticalFormAquaFarmRegixDataDTO(result.dialogDataModel);
                result.regiXDataModel = new StatisticalFormAquaFarmRegixDataDTO(result.regiXDataModel);

                return result;
            }));
        }
        else if (pageCode === PageCodeEnum.StatFormRework) {
            return this.requestService.get<RegixChecksWrapperDTO<StatisticalFormReworkRegixDataDTO>>(this.area, this.controller, 'GetStatisticalFormReworkRegixData', {
                httpParams: params,
                responseTypeCtr: RegixChecksWrapperDTO
            }).pipe(map((result: RegixChecksWrapperDTO<StatisticalFormReworkRegixDataDTO>) => {
                result.dialogDataModel = new StatisticalFormReworkRegixDataDTO(result.dialogDataModel);
                result.regiXDataModel = new StatisticalFormReworkRegixDataDTO(result.regiXDataModel);

                return result;
            }));
        }
        else if (pageCode === PageCodeEnum.StatFormFishVessel) {
            return this.requestService.get<RegixChecksWrapperDTO<StatisticalFormFishVesselRegixDataDTO>>(this.area, this.controller, 'GetStatisticalFormFishVesselRegixData', {
                httpParams: params,
                responseTypeCtr: RegixChecksWrapperDTO
            }).pipe(map((result: RegixChecksWrapperDTO<StatisticalFormFishVesselRegixDataDTO>) => {
                result.dialogDataModel = new StatisticalFormFishVesselRegixDataDTO(result.dialogDataModel);
                result.regiXDataModel = new StatisticalFormFishVesselRegixDataDTO(result.regiXDataModel);

                return result;
            }));
        }

        throw new Error('Invalid page code for getRegixData: ' + PageCodeEnum[pageCode]);
    }

    public getApplicationDataForRegister(applicationId: number, pageCode: PageCodeEnum): Observable<IApplicationRegister> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        if (pageCode === PageCodeEnum.StatFormAquaFarm) {
            return this.requestService.get(this.area, this.controller, 'GetApplicationAquaFarmDataForRegister', {
                httpParams: params,
                responseTypeCtr: StatisticalFormAquaFarmEditDTO
            });
        }
        else if (pageCode === PageCodeEnum.StatFormRework) {
            return this.requestService.get(this.area, this.controller, 'GetApplicationReworkDataForRegister', {
                httpParams: params,
                responseTypeCtr: StatisticalFormReworkEditDTO
            });
        }
        else if (pageCode === PageCodeEnum.StatFormFishVessel) {
            return this.requestService.get(this.area, this.controller, 'GetApplicationFishVesselDataForRegister', {
                httpParams: params,
                responseTypeCtr: StatisticalFormFishVesselEditDTO
            });
        }

        throw new Error('Invalid page code for getApplicationDataForRegister: ' + PageCodeEnum[pageCode]);
    }

    public getRegisterByApplicationId(applicationId: number, pageCode: PageCodeEnum): Observable<unknown> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        switch (pageCode) {
            case PageCodeEnum.StatFormAquaFarm:
                return this.requestService.get(this.area, this.controller, 'GetAquaFarmRegisterByApplicationId', {
                    httpParams: params,
                    responseTypeCtr: StatisticalFormAquaFarmEditDTO
                });
            case PageCodeEnum.StatFormFishVessel:
                return this.requestService.get(this.area, this.controller, 'GetFishVesselRegisterByApplicationId', {
                    httpParams: params,
                    responseTypeCtr: StatisticalFormFishVesselEditDTO
                });
            case PageCodeEnum.StatFormRework:
                return this.requestService.get(this.area, this.controller, 'GetReworkRegisterByApplicationId', {
                    httpParams: params,
                    responseTypeCtr: StatisticalFormReworkEditDTO
                });
        }

        throw new Error('Invalid page code for getRegisterByApplicationId: ' + PageCodeEnum[pageCode]);
    }

    public addApplication(application: IApplicationRegister, pageCode: PageCodeEnum): Observable<number> {
        if (pageCode === PageCodeEnum.StatFormAquaFarm) {
            return this.requestService.post(this.area, this.controller, 'AddStatisticalFormAquaFarmApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (pageCode === PageCodeEnum.StatFormRework) {
            return this.requestService.post(this.area, this.controller, 'AddStatisticalFormReworkApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (pageCode === PageCodeEnum.StatFormFishVessel) {
            return this.requestService.post(this.area, this.controller, 'AddStatisticalFormFishVesselApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }

        throw new Error('Invalid page code for addApplication: ' + PageCodeEnum[pageCode]);
    }

    public editApplication(application: IApplicationRegister, pageCode: PageCodeEnum, saveAsDraft: boolean): Observable<number> {
        const params = new HttpParams().append('saveAsDraft', saveAsDraft.toString());

        if (pageCode === PageCodeEnum.StatFormAquaFarm) {
            return this.requestService.post(this.area, this.controller, 'EditStatisticalFormAquaFarmApplication', application, {
                httpParams: params,
                properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: true })
            });
        }
        else if (pageCode === PageCodeEnum.StatFormRework) {
            return this.requestService.post(this.area, this.controller, 'EditStatisticalFormReworkApplication', application, {
                httpParams: params,
                properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: true })
            });
        }
        else if (pageCode === PageCodeEnum.StatFormFishVessel) {
            return this.requestService.post(this.area, this.controller, 'EditStatisticalFormFishVesselApplication', application, {
                httpParams: params,
                properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: true })
            });
        }

        throw new Error('Invalid page code for editApplication: ' + PageCodeEnum[pageCode]);
    }

    public editApplicationDataAndStartRegixChecks(model: IApplicationRegister): Observable<void> {
        if (model.pageCode === PageCodeEnum.AquaFarmReg) {
            return this.requestService.post(this.area, this.controller, 'EditStatisticalFormAquaFarmApplicationRegixData', model, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (model.pageCode === PageCodeEnum.AquaFarmChange) {
            return this.requestService.post(this.area, this.controller, 'EditStatisticalFormReworkApplicationRegixData', model, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (model.pageCode === PageCodeEnum.AquaFarmDereg) {
            return this.requestService.post(this.area, this.controller, 'EditStatisticalFormFishVesselApplicationRegixData', model, {
                properties: new RequestProperties({ asFormData: true })
            });
        }

        throw new Error('Invalid page code for editApplicationDataAndStartRegixChecks: ' + PageCodeEnum[model.pageCode!]);
    }

    //Nomenclatures
    public getGrossTonnageIntervals(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetGrossTonnageIntervals', { responseTypeCtr: NomenclatureDTO });
    }

    public getVesselLengthIntervals(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetVesselLengthIntervals', { responseTypeCtr: NomenclatureDTO });
    }

    public getFuelTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetFuelTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getReworkProductTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetReworkProductTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getAllAquacultureNomenclatures(): Observable<StatisticalFormAquacultureNomenclatureDTO[]> {
        return this.requestService.get<StatisticalFormAquacultureNomenclatureDTO[]>(this.area, this.controller, 'GetAllAquacultureNomenclatures', {
            responseTypeCtr: StatisticalFormAquacultureNomenclatureDTO
        }).pipe(map((result: StatisticalFormAquacultureNomenclatureDTO[]) => {
            for (const aqua of result) {
                aqua.displayName = `${aqua.displayName} | ${this.translate.getValue('statistical-forms.aqua-farm-uror')}: ${aqua.urorNum}`;
                aqua.displayName = `${aqua.displayName} | ${this.translate.getValue('statistical-forms.aqua-farm-reg-num')}: ${aqua.regNum}`;
            }
            return result;
        }));
    }

    public vesselStatFormAlreadyExists(shipId: number, year: number, formId: number | undefined): Observable<boolean> {
        let params = new HttpParams()
            .append('shipId', shipId.toString())
            .append('year', year.toString());

        if (formId !== undefined && formId !== null) {
            params = params.append('formId', formId.toString());
        }

        return this.requestService.get(this.area, this.controller, 'VesselStatFormAlreadyExists', {
            httpParams: params
        });
    }

    public aquaFarmStatFormAlreadyExists(aquacultureId: number, year: number, formId: number | undefined): Observable<boolean> {
        let params = new HttpParams()
            .append('aquacultureId', aquacultureId.toString())
            .append('year', year.toString());

        if (formId !== undefined && formId !== null) {
            params = params.append('formId', formId.toString());
        }

        return this.requestService.get(this.area, this.controller, 'AquaFarmStatFormAlreadyExists', {
            httpParams: params
        });
    }
}
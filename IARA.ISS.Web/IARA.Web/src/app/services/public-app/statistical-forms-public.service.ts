import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { IStatisticalFormsService } from '@app/interfaces/common-app/statistical-forms.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { StatisticalFormAquaFarmApplicationEditDTO } from '@app/models/generated/dtos/StatisticalFormAquaFarmApplicationEditDTO';
import { StatisticalFormAquaFarmEditDTO } from '@app/models/generated/dtos/StatisticalFormAquaFarmEditDTO';
import { StatisticalFormDTO } from '@app/models/generated/dtos/StatisticalFormDTO';
import { StatisticalFormFishVesselApplicationEditDTO } from '@app/models/generated/dtos/StatisticalFormFishVesselApplicationEditDTO';
import { StatisticalFormFishVesselEditDTO } from '@app/models/generated/dtos/StatisticalFormFishVesselEditDTO';
import { StatisticalFormReworkApplicationEditDTO } from '@app/models/generated/dtos/StatisticalFormReworkApplicationEditDTO';
import { StatisticalFormReworkEditDTO } from '@app/models/generated/dtos/StatisticalFormReworkEditDTO';
import { StatisticalFormsFilters } from '@app/models/generated/filters/StatisticalFormsFilters';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { ApplicationsRegisterPublicBaseService } from './applications-register-public-base.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { StatisticalFormShipDTO } from '@app/models/generated/dtos/StatisticalFormShipDTO';
import { StatisticalFormAquacultureDTO } from '@app/models/generated/dtos/StatisticalFormAquacultureDTO';
import { StatisticalFormAquacultureNomenclatureDTO } from '@app/models/generated/dtos/StatisticalFormAquacultureNomenclatureDTO';
import { StatisticalFormTypesEnum } from '@app/enums/statistical-form-types.enum';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';

@Injectable({
    providedIn: 'root'
})
export class StatisticalFormsPublicService extends ApplicationsRegisterPublicBaseService implements IStatisticalFormsService {
    protected controller: string = 'StatisticalFormsPublic';

    private translate: FuseTranslationLoaderService;

    public constructor(requestService: RequestService, translate: FuseTranslationLoaderService) {
        super(requestService);

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

    public getCurrentUserAsSubmittedBy(type: StatisticalFormTypesEnum): Observable<ApplicationSubmittedByDTO> {
        const params = new HttpParams().append('type', type.toString());

        return this.requestService.get(this.area, this.controller, 'GetCurrentUserAsSubmittedBy', {
            httpParams: params,
            responseTypeCtr: ApplicationSubmittedByDTO
        });
    }

    public addStatisticalFormAquaFarm(form: StatisticalFormAquaFarmEditDTO): Observable<number> {
        throw new Error('This method should not be called from the public app.');
    }

    public addStatisticalFormRework(form: StatisticalFormReworkEditDTO): Observable<number> {
        throw new Error('This method should not be called from the public app.');
    }

    public addStatisticalFormFishVessel(form: StatisticalFormFishVesselEditDTO): Observable<number> {
        throw new Error('This method should not be called from the public app.');
    }

    public editStatisticalFormAquaFarm(form: StatisticalFormAquaFarmEditDTO): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public editStatisticalFormRework(form: StatisticalFormReworkEditDTO): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public editStatisticalFormFishVessel(form: StatisticalFormFishVesselEditDTO): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public deleteStatisticalForm(id: number): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public undoDeleteStatisticalForm(id: number): Observable<void> {
        throw new Error('This method should not be called from the public app.');
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

    public getApplication(id: number, pageCode: PageCodeEnum): Observable<IApplicationRegister> {
        const params = new HttpParams().append('id', id.toString());

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

    public editApplication(application: IApplicationRegister, pageCode: PageCodeEnum): Observable<number> {
        if (pageCode === PageCodeEnum.StatFormAquaFarm) {
            return this.requestService.post(this.area, this.controller, 'EditStatisticalFormAquaFarmApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (pageCode === PageCodeEnum.StatFormRework) {
            return this.requestService.post(this.area, this.controller, 'EditStatisticalFormReworkApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }
        else if (pageCode === PageCodeEnum.StatFormFishVessel) {
            return this.requestService.post(this.area, this.controller, 'EditStatisticalFormFishVesselApplication', application, {
                properties: new RequestProperties({ asFormData: true })
            });
        }

        throw new Error('Invalid page code for editApplication: ' + PageCodeEnum[pageCode]);
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
}
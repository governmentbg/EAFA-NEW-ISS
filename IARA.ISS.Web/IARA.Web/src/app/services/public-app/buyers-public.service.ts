import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { IBuyersService } from '@app/interfaces/common-app/buyers.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { BuyerDTO } from '@app/models/generated/dtos/BuyerDTO';
import { BuyerEditDTO } from '@app/models/generated/dtos/BuyerEditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { BuyersFilters } from '@app/models/generated/filters/BuyersFilters';
import { ApplicationsRegisterPublicBaseService } from './applications-register-public-base.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { BuyerApplicationEditDTO } from '@app/models/generated/dtos/BuyerApplicationEditDTO';
import { BuyerChangeOfCircumstancesApplicationDTO } from '@app/models/generated/dtos/BuyerChangeOfCircumstancesApplicationDTO';
import { CancellationHistoryEntryDTO } from '@app/models/generated/dtos/CancellationHistoryEntryDTO';
import { BuyerTerminationApplicationDTO } from '@app/models/generated/dtos/BuyerTerminationApplicationDTO';
import { BuyerTypesEnum } from '@app/enums/buyer-types.enum';

@Injectable({
    providedIn: 'root'
})
export class BuyersPublicService extends ApplicationsRegisterPublicBaseService implements IBuyersService {
    protected controller: string = 'BuyersPublic';

    public constructor(requestService: RequestService) {
        super(requestService);
    }

    // Register
    public getAll(request: GridRequestModel<BuyersFilters>): Observable<GridResultModel<BuyerDTO>> {
        throw new Error('This method should not be called from the public app.');
    }

    public get(id: number): Observable<BuyerEditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public edit(item: BuyerDTO): Observable<number> {
        throw new Error('This method should not be called from the public app.');
    }

    public editAndDownloadRegister(model: BuyerDTO): Observable<boolean> {
        throw new Error('This method should not be called from the public app.');
    }

    public add(item: BuyerDTO): Observable<number> {
        throw new Error('This method should not be called from the public app.');
    }

    public addAndDownloadRegister(model: BuyerDTO): Observable<boolean> {
        throw new Error('This method should not be called from the public app.');
    }

    public downloadRegister(id: number, buyerType: BuyerTypesEnum): Observable<boolean> {
        throw new Error('This method should not be called from the public app.');
    }

    public downloadFile(fileId: number, fileName: string): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadFile', fileName, { httpParams: params });
    }

    public getRegisterByApplicationId(applicationId: number, pageCode: PageCodeEnum): Observable<BuyerEditDTO> {
        let serviceName: string = '';
        const params = new HttpParams().append('applicationId', applicationId.toString());

        switch (pageCode) {
            case PageCodeEnum.RegFirstSaleBuyer:
            case PageCodeEnum.RegFirstSaleCenter:
                serviceName = 'GetRegisterByApplicationId'; break;
            case PageCodeEnum.ChangeFirstSaleBuyer:
            case PageCodeEnum.ChangeFirstSaleCenter:
            case PageCodeEnum.TermFirstSaleBuyer:
            case PageCodeEnum.TermFirstSaleCenter:
                serviceName = 'GetRegisterByChangeOfCircumstancesApplicationId'; break;
        }

        return this.requestService.get(this.area, this.controller, serviceName, {
            httpParams: params,
            responseTypeCtr: BuyerEditDTO
        });
    }

    public updateBuyerStatus(buyerId: number, status: CancellationHistoryEntryDTO): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    // audits
    public getPremiseUsageDocumentAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getLogBookAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getBuyerLicensesAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    // Applications
    public getApplication(id: number, getRegiXData: boolean, pageCode: PageCodeEnum): Observable<IApplicationRegister> {
        const params = new HttpParams()
            .append('id', id.toString());

        switch (pageCode) {
            case PageCodeEnum.RegFirstSaleBuyer:
            case PageCodeEnum.RegFirstSaleCenter: {
                return this.requestService.get(this.area, this.controller, 'GetApplication', {
                    httpParams: params,
                    responseTypeCtr: BuyerApplicationEditDTO
                });
            }
            case PageCodeEnum.ChangeFirstSaleBuyer:
            case PageCodeEnum.ChangeFirstSaleCenter: {
                return this.requestService.get(this.area, this.controller, 'GetBuyerChangeOfCircumstancesApplication', {
                    httpParams: params,
                    responseTypeCtr: BuyerChangeOfCircumstancesApplicationDTO
                });
            }
            case PageCodeEnum.TermFirstSaleBuyer:
            case PageCodeEnum.TermFirstSaleCenter: {
                return this.requestService.get(this.area, this.controller, 'GetBuyerTerminationApplication', {
                    httpParams: params,
                    responseTypeCtr: BuyerTerminationApplicationDTO
                });
            }
            default: {
                throw new Error('Invalid page code for getApplication: ' + PageCodeEnum[pageCode]);
            }
        }
    }

    public addApplication(application: IApplicationRegister, pageCode: PageCodeEnum): Observable<number> {
        let method: string = '';

        switch (pageCode) {
            case PageCodeEnum.RegFirstSaleBuyer:
            case PageCodeEnum.RegFirstSaleCenter: {
                method = 'AddApplication';
            } break;
            case PageCodeEnum.ChangeFirstSaleBuyer:
            case PageCodeEnum.ChangeFirstSaleCenter: {
                method = 'AddBuyerChangeOfCircumstancesApplication';
            } break;
            case PageCodeEnum.TermFirstSaleBuyer:
            case PageCodeEnum.TermFirstSaleCenter: {
                method = 'AddBuyerTerminationApplication'
            } break;
        }

        return this.requestService.post(this.area, this.controller, method, application, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editApplication(application: IApplicationRegister, pageCode: PageCodeEnum): Observable<number> {
        let method: string = '';

        switch (pageCode) {
            case PageCodeEnum.RegFirstSaleBuyer:
            case PageCodeEnum.RegFirstSaleCenter: {
                method = 'EditApplication';
            } break;
            case PageCodeEnum.ChangeFirstSaleBuyer:
            case PageCodeEnum.ChangeFirstSaleCenter: {
                method = 'EditBuyerChangeOfCircumstancesApplication';
            } break;
            case PageCodeEnum.TermFirstSaleBuyer:
            case PageCodeEnum.TermFirstSaleCenter: {
                method = 'EditBuyerTerminationApplication'
            } break;
        }

        return this.requestService.post(this.area, this.controller, method, application, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    // change of circumstances
    public getBuyerFromChangeOfCircumstancesApplication(applicationId: number): Observable<BuyerEditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public completeBuyerChangeOfCircumstancesApplication(buyer: BuyerEditDTO): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public getBuyerFromTerminationApplication(applicationId: number): Observable<BuyerEditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    // Nomenclatures
    public getBuyerTypes(): Observable<NomenclatureDTO<number>[]> {
        throw new Error('This method should not be called from the public app.');
    }

    public getAllBuyersNomenclatures(): Observable<NomenclatureDTO<number>[]> {
        throw new Error('This method should not be called from the public app.');
    }

    public getAllFirstSaleCentersNomenclatures(): Observable<NomenclatureDTO<number>[]> {
        throw new Error('This method should not be called from the public app.');
    }

    public getBuyerStatuses(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetBuyerStatuses', {
            responseTypeCtr: NomenclatureDTO
        });
    }
}
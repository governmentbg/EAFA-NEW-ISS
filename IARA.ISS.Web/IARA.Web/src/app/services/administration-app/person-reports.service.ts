import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { IPersonReportsService } from '../../interfaces/administration-app/person-reports.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { LegalEntityReportDTO } from '@app/models/generated/dtos/LegalEntityReportDTO';
import { PersonReportDTO } from '@app/models/generated/dtos/PersonReportDTO';
import { ReportHistoryDTO } from '@app/models/generated/dtos/ReportHistoryDTO';
import { LegalEntitiesReportFilters } from '@app/models/generated/filters/LegalEntitiesReportFilters';
import { PersonsReportFilters } from '@app/models/generated/filters/PersonsReportFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';

@Injectable({
    providedIn: 'root'
})
export class PersonReportsService extends BaseAuditService implements IPersonReportsService {
    protected controller: string = 'PersonReports';

    private readonly translationService: FuseTranslationLoaderService;

    public constructor(requestService: RequestService, translationService: FuseTranslationLoaderService) {
        super(requestService, AreaTypes.Administrative);
        this.translationService = translationService;
    }

    public getAllLegalEntitiesReport(request: GridRequestModel<LegalEntitiesReportFilters>): Observable<GridResultModel<LegalEntityReportDTO>> {
        type Result = GridResultModel<LegalEntityReportDTO>;
        type Body = GridRequestModel<LegalEntitiesReportFilters>;

        return this.requestService.post<Result, Body>(this.area, this.controller, 'GetAllLegalEntitiesReport', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        }).pipe(switchMap((entries: Result) => {
            const ids: number[] = entries.records.map((legalReport: LegalEntityReportDTO) => {
                return legalReport.id!;
            });

            if (ids.length === 0) {
                return of(entries);
            }

            return this.getLegalEntitiesHistory(ids).pipe(map((history: ReportHistoryDTO[]) => {
                for (const h of history) {
                    const found = entries.records.find((entry: LegalEntityReportDTO) => {
                        return entry.eik === h.eik;
                    });

                    if (h.pageCode !== undefined && h.pageCode !== null) {
                        h.documentsName = this.getHistoryDocumentName(h.pageCode, h.isApplication);
                    }

                    if (found !== undefined) {
                        if (found.history !== undefined && found.history !== null) {
                            found.history.push(h);
                        }
                        else {
                            found.history = [h];
                        }
                    }
                }

                return entries;
            }));
        }));
    }

    public getLegalEntitiesHistory(legalIds: number[]): Observable<ReportHistoryDTO[]> {
        return this.requestService.post(this.area, this.controller, 'GetLegalEntitiesHistory', legalIds, {
            responseTypeCtr: ReportHistoryDTO
        });
    }

    public getAllPersonsReport(request: GridRequestModel<PersonsReportFilters>): Observable<GridResultModel<PersonReportDTO>> {
        type Result = GridResultModel<PersonReportDTO>;
        type Body = GridRequestModel<PersonsReportFilters>;

        return this.requestService.post<Result, Body>(this.area, this.controller, 'GetAllPersonsReport', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        }).pipe(switchMap((entries: GridResultModel<PersonReportDTO>) => {
            const validPeopleIds: number[] = entries.records.map((personReport: PersonReportDTO) => {
                return personReport.id!;
            });

            if (validPeopleIds.length === 0) {
                return of(entries);
            }
            
            return this.getPeopleHistory(validPeopleIds).pipe(map((history: ReportHistoryDTO[]) => {
                for (const h of history) {
                    const found = entries.records.find((entry: PersonReportDTO) => {
                        return entry.egn === h.egn;
                    });

                    if (h.pageCode !== undefined && h.pageCode !== null) {
                        h.documentsName = this.getHistoryDocumentName(h.pageCode, h.isApplication);
                    }

                    if (found !== undefined) {
                        if (found.history !== undefined && found.history !== null) {
                            found.history.push(h);
                        }
                        else {
                            found.history = [h];
                        }
                    }
                }

                return entries;
            }));
        }));
    }

    public getPeopleHistory(validPeopleIds: number[]): Observable<ReportHistoryDTO[]> {
        return this.requestService.post(this.area, this.controller, 'GetPeopleHistory', validPeopleIds, {
            responseTypeCtr: ReportHistoryDTO
        });
    }

    public getLegalEntityReport(id: number): Observable<LegalEntityReportDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetLegalEntityReport', {
            httpParams: params,
            responseTypeCtr: LegalEntityReportDTO
        });
    }

    public getPersonReport(id: number): Observable<PersonReportDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetPersonReport', {
            httpParams: params,
            responseTypeCtr: PersonReportDTO
        });
    }

    private getHistoryDocumentName(pageCode: PageCodeEnum, isApplication: boolean = false): string {
        let documentName: string;
        
        switch (pageCode) {
            case PageCodeEnum.RegVessel:
            case PageCodeEnum.DeregShip:
            case PageCodeEnum.ShipRegChange:
                documentName = isApplication
                    ? this.translationService.getValue('persons-report-page.ship-appl')
                    : this.translationService.getValue('persons-report-page.ship-owner');
                break;
            case PageCodeEnum.CommFish:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishThirdCountry:
                documentName = isApplication
                    ? this.translationService.getValue('persons-report-page.permit-appl')
                    : this.translationService.getValue('persons-report-page.permit');
                break;
            case PageCodeEnum.DupCommFish:
            case PageCodeEnum.DupRightToFishThirdCountry:
            case PageCodeEnum.DupPoundnetCommFish:
                documentName = isApplication
                    ? this.translationService.getValue('persons-report-page.permit-dup-appl')
                    : this.translationService.getValue('persons-report-page.permit-dup');
                break;
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
                documentName = isApplication
                    ? this.translationService.getValue('persons-report-page.permit-licenses-appl')
                    : this.translationService.getValue('persons-report-page.permit-licenses');
                break;
            case PageCodeEnum.DupRightToFishResource:
            case PageCodeEnum.DupPoundnetCommFishLic:
            case PageCodeEnum.DupCatchQuataSpecies:
                documentName = isApplication
                    ? this.translationService.getValue('persons-report-page.permit-licenses-dup-appl')
                    : this.translationService.getValue('persons-report-page.permit-licenses-dup');
                break;
            case PageCodeEnum.AptitudeCourceExam:
            case PageCodeEnum.CommFishLicense:
                documentName = isApplication
                    ? this.translationService.getValue('persons-report-page.qualified-fishers-appl')
                    : this.translationService.getValue('persons-report-page.qualified-fishers');
                break;
            case PageCodeEnum.CompetencyDup:
                documentName = isApplication
                    ? this.translationService.getValue('persons-report-page.qualified-fishers-dup-appl')
                    : this.translationService.getValue('persons-report-page.qualified-fishers-dup');
                break;
            case PageCodeEnum.AquaFarmReg:
            case PageCodeEnum.AquaFarmChange:
            case PageCodeEnum.AquaFarmDereg:
                documentName = isApplication
                    ? this.translationService.getValue('persons-report-page.aquaculture-facility-appl')
                    : this.translationService.getValue('persons-report-page.aquaculture-facility');
                break;
            case PageCodeEnum.AquacultureLogBookPage:
                documentName = this.translationService.getValue('persons-report-page.aquaculture-log-book-pages');
                break;
            case PageCodeEnum.AuanRegister:
                documentName = this.translationService.getValue('persons-report-page.auan');
                break;
            case PageCodeEnum.ChangeFirstSaleBuyer:
            case PageCodeEnum.ChangeFirstSaleCenter:
            case PageCodeEnum.RegFirstSaleBuyer:
            case PageCodeEnum.RegFirstSaleCenter:
            case PageCodeEnum.TermFirstSaleBuyer:
            case PageCodeEnum.TermFirstSaleCenter:
            case PageCodeEnum.Buyers:
                documentName = isApplication
                    ? this.translationService.getValue('persons-report-page.buyer-appl')
                    : this.translationService.getValue('persons-report-page.buyer');
                break;
            case PageCodeEnum.DupFirstSaleBuyer:
            case PageCodeEnum.DupFirstSaleCenter:
                documentName = isApplication
                    ? this.translationService.getValue('persons-report-page.buyer-dup-appl')
                    : this.translationService.getValue('persons-report-page.buyer-dup');
                break;
            case PageCodeEnum.IncreaseFishCap:
            case PageCodeEnum.ReduceFishCap:
            case PageCodeEnum.TransferFishCap:
            case PageCodeEnum.CapacityCertDup:
                documentName = isApplication
                    ? this.translationService.getValue('persons-report-page.capacity-certificates-appl')
                    : this.translationService.getValue('persons-report-page.capacity-certificates');
                break;
            case PageCodeEnum.Inspections:
                documentName = this.translationService.getValue('persons-report-page.inspected-people');
                break;
            case PageCodeEnum.RecFishDup:
            case PageCodeEnum.RecFish:
                documentName = this.translationService.getValue('persons-report-page.fishing-tickets');
                break;
            case PageCodeEnum.Assocs:
            case PageCodeEnum.LE:
                documentName = isApplication
                    ? this.translationService.getValue('persons-report-page.assoc-appl')
                    : this.translationService.getValue('persons-report-page.fishing-associations');
                break;
            case PageCodeEnum.StatFormAquaFarm:
            case PageCodeEnum.StatFormFishVessel:
            case PageCodeEnum.StatFormRework:
                documentName = isApplication
                    ? this.translationService.getValue('persons-report-page.statistical-forms-appl')
                    : this.translationService.getValue('persons-report-page.statistical-forms');
                break;
            case PageCodeEnum.AdmissionLogBookPage:
            case PageCodeEnum.FirstSaleLogBookPage:
            case PageCodeEnum.ShipLogBookPage:
            case PageCodeEnum.TransportationLogBookPage:
                documentName = this.translationService.getValue('persons-report-page.log-books');
                break;
            case PageCodeEnum.SciFi:
                documentName = isApplication
                    ? this.translationService.getValue('persons-report-page.scientific-fishing-appl')
                    : this.translationService.getValue('persons-report-page.scientific-fishing');
                break;
            case PageCodeEnum.PenalDecrees:
                documentName = this.translationService.getValue('persons-report-page.penal-points');
                break;
            case PageCodeEnum.NewsManagement:
                documentName = this.translationService.getValue('persons-report-page.user-legals');
                break;
            default:
                documentName = '';
                break;
        }

        return documentName;
    }
}
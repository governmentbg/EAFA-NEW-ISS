import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { ReportTypeEnum } from '@app/enums/report-type.enum';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { LegalEntityReportDTO } from '@app/models/generated/dtos/LegalEntityReportDTO';
import { PersonReportDTO } from '@app/models/generated/dtos/PersonReportDTO';
import { ReportHistoryDTO } from '@app/models/generated/dtos/ReportHistoryDTO';
import { LegalEntitiesReportFilters } from '@app/models/generated/filters/LegalEntitiesReportFilters';
import { PersonsReportFilters } from '@app/models/generated/filters/PersonsReportFilters';
import { PersonReportsService } from '@app/services/administration-app/person-reports.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { LegalEntityReportInfoComponent } from '../../legal-entities-report/components/show-legal-entity-report-info/legal-entity-report-info.component';
import { PersonsReportInfoComponent } from '../../persons-report/components/show-persons-report-info/persons-report-info.component';
import { PageCodeEnum } from '@app/enums/page-code.enum';

@Component({
    selector: 'reports-content',
    templateUrl: './reports-content.component.html',
    styleUrls: ['./reports-content.component.scss']
})
export class ReportsContentComponent implements OnInit, AfterViewInit {
    @Input() public reportType!: string;

    public translationService: FuseTranslationLoaderService;
    public permissions: PermissionsService;
    public formGroup!: FormGroup;
    public reportTypeEnum: typeof ReportTypeEnum = ReportTypeEnum;
    public countries: NomenclatureDTO<number>[] = [];
    public populatedAreas: NomenclatureDTO<number>[] = [];

    public readonly pageCodes: typeof PageCodeEnum = PageCodeEnum;
    public readonly canReadSciFiRegisterRecords: boolean;
    public readonly canReadQuaFiRegisterRecords: boolean;
    public readonly canReadAquacultureRegisterRecords: boolean;
    public readonly canReadBuyerRegisterRecords: boolean;
    public readonly canReadAuanRegisterRecords: boolean;
    public readonly canReadCapacityRegisterRecords: boolean;
    public readonly canReadInspectedPeopleRegisterRecords: boolean;
    public readonly canReadLogBookRegisterRecords: boolean;
    public readonly canReadPermitLicensesRegisterRecords: boolean;
    public readonly canReadPermitRegisterRecords: boolean;
    public readonly canReadShipOwnerRegisterRecords: boolean;
    public readonly canReadStatFormsRegisterRecords: boolean;
    public readonly canReadAssocationsRecords: boolean;
    public readonly canReadAssociationTicketRecords: boolean;
    public readonly canReadTicketRecords: boolean;
    public readonly canReadUserRecords: boolean;
    public readonly canReadPenalPointsRecords: boolean;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    public searchpanel!: SearchPanelComponent;

    private service: PersonReportsService;
    private router: Router;
    private nomenclaturesService: CommonNomenclatures;

    private gridManager!: DataTableManager<any, any>;

    private personReportInfoDialog: TLMatDialog<PersonsReportInfoComponent>;
    private legalEntityReportInfoDialog: TLMatDialog<LegalEntityReportInfoComponent>;

    public constructor(
        translationService: FuseTranslationLoaderService,
        permissions: PermissionsService,
        service: PersonReportsService,
        nomenclaturesService: CommonNomenclatures,
        personReportInfoDialog: TLMatDialog<PersonsReportInfoComponent>,
        legalEntityReportInfoDialog: TLMatDialog<LegalEntityReportInfoComponent>,
        router: Router
    ) {
        this.translationService = translationService;
        this.permissions = permissions;
        this.service = service;
        this.nomenclaturesService = nomenclaturesService;
        this.personReportInfoDialog = personReportInfoDialog;
        this.legalEntityReportInfoDialog = legalEntityReportInfoDialog;
        this.router = router;

        this.canReadSciFiRegisterRecords = permissions.has(PermissionsEnum.ScientificFishingRead);
        this.canReadQuaFiRegisterRecords = permissions.has(PermissionsEnum.QualifiedFishersRead);
        this.canReadAquacultureRegisterRecords = this.permissions.has(PermissionsEnum.AquacultureFacilitiesRead);
        this.canReadAuanRegisterRecords = this.permissions.has(PermissionsEnum.AuanRegisterRead)
        this.canReadBuyerRegisterRecords = this.permissions.has(PermissionsEnum.BuyersRead);
        this.canReadLogBookRegisterRecords = this.permissions.has(PermissionsEnum.FishLogBookRead)
            || this.permissions.has(PermissionsEnum.TransportationLogBookRead)
            || this.permissions.has(PermissionsEnum.ApplicationsRead)
            || this.permissions.has(PermissionsEnum.AuanRegisterRead)
            || this.permissions.has(PermissionsEnum.FirstSaleLogBookRead)
            || this.permissions.has(PermissionsEnum.AdmissionLogBookRead)
            || this.permissions.has(PermissionsEnum.AquacultureLogBookRead);

        this.canReadPermitLicensesRegisterRecords = this.permissions.has(PermissionsEnum.CommercialFishingPermitLicenseRegisterRead);
        this.canReadPermitRegisterRecords = this.permissions.has(PermissionsEnum.CommercialFishingPermitRegisterRead);
        this.canReadInspectedPeopleRegisterRecords = this.permissions.has(PermissionsEnum.InspectionsRead);
        this.canReadAssocationsRecords = this.permissions.has(PermissionsEnum.AssociationsRead);
        this.canReadAssociationTicketRecords = this.permissions.has(PermissionsEnum.AssociationsTicketsRead);
        this.canReadTicketRecords = this.permissions.has(PermissionsEnum.TicketsRead);
        this.canReadCapacityRegisterRecords = this.permissions.has(PermissionsEnum.FishingCapacityCertificatesRead);
        this.canReadShipOwnerRegisterRecords = this.permissions.has(PermissionsEnum.ShipsRegisterRead);

        this.canReadStatFormsRegisterRecords = this.permissions.has(PermissionsEnum.StatisticalFormsAquaFarmRead)
            || this.permissions.has(PermissionsEnum.StatisticalFormsReworkRead)
            || this.permissions.has(PermissionsEnum.StatisticalFormsFishVesselRead);

        this.canReadUserRecords = this.permissions.has(PermissionsEnum.InternalUsersRead)
            || this.permissions.has(PermissionsEnum.ExternalUsersRead);

        this, this.canReadPenalPointsRecords = this.permissions.has(PermissionsEnum.AwardedPointsRead);
    }

    public ngOnInit(): void {
        this.buildForm();

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Countries, this.nomenclaturesService.getCountries.bind(this.nomenclaturesService), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.countries = result;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.PopulatedAreas, this.nomenclaturesService.getPopulatedAreas.bind(this.nomenclaturesService), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.populatedAreas = result;
            }
        });
    }

    public ngAfterViewInit(): void {
        if (this.reportType === this.reportTypeEnum.LEGAL_ENTITIES) {
            this.gridManager = new DataTableManager<LegalEntityReportDTO, LegalEntitiesReportFilters>({
                tlDataTable: this.datatable,
                searchPanel: this.searchpanel,
                requestServiceMethod: this.service.getAllLegalEntitiesReport.bind(this.service),
                filtersMapper: this.mapLegalEntitiesReportFilters
            });

            this.datatable.activeRecordChanged.subscribe({
                next: (result: LegalEntityReportDTO) => {
                    this.showReportInfo(result.id);
                }
            });
        }
        else if (this.reportType === this.reportTypeEnum.PERSONS) {
            this.gridManager = new DataTableManager<PersonReportDTO, PersonsReportFilters>({
                tlDataTable: this.datatable,
                searchPanel: this.searchpanel,
                requestServiceMethod: this.service.getAllPersonsReport.bind(this.service),
                filtersMapper: this.mapPersonsReportFilters
            });

            this.datatable.activeRecordChanged.subscribe({
                next: (result: PersonReportDTO) => {
                    this.showReportInfo(result.id);
                }
            });
        }

        this.gridManager.refreshData();
    }

    public showReportInfo(id?: number): void {
        let data: DialogParamsModel | undefined;
        let headerAuditBtn: IHeaderAuditButton | undefined;

        if (id !== undefined) {
            data = new DialogParamsModel({ id: id, isReadonly: true });
        }

        if (this.reportType === this.reportTypeEnum.LEGAL_ENTITIES) {
            this.legalEntityReportInfoDialog.open({
                title: this.translationService.getValue('legal-entities-report-page.legal-entity-report-info-dialog-title'),
                TCtor: LegalEntityReportInfoComponent,
                headerAuditButton: headerAuditBtn,
                headerCancelButton: {
                    cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
                },
                componentData: data,
                translteService: this.translationService,
            });
        } else if (this.reportType === this.reportTypeEnum.PERSONS) {
            this.personReportInfoDialog.open({
                title: this.translationService.getValue('persons-report-page.person-report-info-dialog-title'),
                TCtor: PersonsReportInfoComponent,
                headerAuditButton: headerAuditBtn,
                headerCancelButton: {
                    cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
                },
                componentData: data,
                translteService: this.translationService,
            });
        }
    }

    public canReadRegister(row: GridRow<ReportHistoryDTO>): boolean {
        switch (row.data.pageCode) {
            case PageCodeEnum.RegVessel:
            case PageCodeEnum.ShipRegChange:
            case PageCodeEnum.DeregShip:
                return this.canReadShipOwnerRegisterRecords;
            case PageCodeEnum.CommFish:
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.RightToFishThirdCountry:
            case PageCodeEnum.DupCommFish:
            case PageCodeEnum.DupRightToFishThirdCountry:
            case PageCodeEnum.DupPoundnetCommFish:
                return this.canReadPermitRegisterRecords;
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
            case PageCodeEnum.DupRightToFishResource:
            case PageCodeEnum.DupPoundnetCommFishLic:
            case PageCodeEnum.DupCatchQuataSpecies:
                return this.canReadPermitLicensesRegisterRecords;
            case PageCodeEnum.AptitudeCourceExam:
            case PageCodeEnum.CommFishLicense:
            case PageCodeEnum.CompetencyDup:
                return this.canReadQuaFiRegisterRecords;
            case PageCodeEnum.AquaFarmReg:
            case PageCodeEnum.AquaFarmChange:
            case PageCodeEnum.AquaFarmDereg:
                return this.canReadAquacultureRegisterRecords;
            case PageCodeEnum.AuanRegister:
                return this.canReadAuanRegisterRecords;
            case PageCodeEnum.ChangeFirstSaleBuyer:
            case PageCodeEnum.ChangeFirstSaleCenter:
            case PageCodeEnum.RegFirstSaleBuyer:
            case PageCodeEnum.RegFirstSaleCenter:
            case PageCodeEnum.TermFirstSaleBuyer:
            case PageCodeEnum.TermFirstSaleCenter:
            case PageCodeEnum.DupFirstSaleBuyer:
            case PageCodeEnum.DupFirstSaleCenter:
            case PageCodeEnum.Buyers:
                return this.canReadBuyerRegisterRecords;
            case PageCodeEnum.IncreaseFishCap:
            case PageCodeEnum.ReduceFishCap:
            case PageCodeEnum.TransferFishCap:
            case PageCodeEnum.CapacityCertDup:
                return this.canReadCapacityRegisterRecords;
            case PageCodeEnum.Inspections:
                return this.canReadInspectedPeopleRegisterRecords;
            case PageCodeEnum.RecFish:
                return this.canReadTicketRecords;
            case PageCodeEnum.Assocs:
                return this.canReadAssociationTicketRecords;
            case PageCodeEnum.StatFormAquaFarm:
            case PageCodeEnum.StatFormFishVessel:
            case PageCodeEnum.StatFormRework:
                return this.canReadStatFormsRegisterRecords;
            case PageCodeEnum.AdmissionLogBookPage:
            case PageCodeEnum.AquacultureLogBookPage:
            case PageCodeEnum.FirstSaleLogBookPage:
            case PageCodeEnum.ShipLogBookPage:
            case PageCodeEnum.TransportationLogBookPage:
                return this.canReadLogBookRegisterRecords;
            case PageCodeEnum.SciFi:
                return this.canReadSciFiRegisterRecords;
            case PageCodeEnum.PenalDecrees:
                return this.canReadPenalPointsRecords;
            default:
                return false;
        }
    }

    public openRegister(row: GridRow<ReportHistoryDTO>): void {
        const id: number = row.data.id!;
        const isPerson: boolean = row.data.isPerson!;

        switch (row.data.pageCode) {
            case PageCodeEnum.RegVessel:
            case PageCodeEnum.ShipRegChange:
            case PageCodeEnum.DeregShip:
                this.navigateByUrl('/fishing-vessels', id, isPerson);
                break;
            case PageCodeEnum.CommFish:
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.RightToFishThirdCountry:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
                this.navigateByUrl('/commercial-fishing-permits-and-licenses', id, isPerson);
                break;
            case PageCodeEnum.AptitudeCourceExam:
            case PageCodeEnum.CommFishLicense:
                this.navigateByUrl('/qualified-fishers-register', id, isPerson);
                break;
            case PageCodeEnum.AquaFarmReg:
            case PageCodeEnum.AquaFarmChange:
            case PageCodeEnum.AquaFarmDereg:
                this.navigateByUrl('/aquaculture-farms', id, isPerson);
                break;
            case PageCodeEnum.AuanRegister:
                this.navigateByUrl('/auan-register', id, isPerson);
                break;
            case PageCodeEnum.ChangeFirstSaleBuyer:
            case PageCodeEnum.ChangeFirstSaleCenter:
            case PageCodeEnum.RegFirstSaleBuyer:
            case PageCodeEnum.RegFirstSaleCenter:
            case PageCodeEnum.TermFirstSaleBuyer:
            case PageCodeEnum.TermFirstSaleCenter:
            case PageCodeEnum.Buyers:
                this.navigateByUrl('/sales-centers-register', id, isPerson);
                break;
            case PageCodeEnum.IncreaseFishCap:
            case PageCodeEnum.ReduceFishCap:
            case PageCodeEnum.TransferFishCap:
            case PageCodeEnum.CapacityCertDup:
                this.navigateByUrl('/fishing-capacity-certificates-register', id, isPerson);
                break;
            case PageCodeEnum.Inspections:
                this.navigateByUrl('/inspections-register', id, isPerson);
                break;
            case PageCodeEnum.RecFish:
            case PageCodeEnum.Assocs:
                this.navigateByUrl('/ticket_applications', id, isPerson);
                break;
            case PageCodeEnum.StatFormAquaFarm:
            case PageCodeEnum.StatFormFishVessel:
            case PageCodeEnum.StatFormRework:
                this.navigateByUrl('/statistical-forms-register', id, isPerson);
                break;
            case PageCodeEnum.AdmissionLogBookPage:
            case PageCodeEnum.AquacultureLogBookPage:
            case PageCodeEnum.FirstSaleLogBookPage:
            case PageCodeEnum.ShipLogBookPage:
            case PageCodeEnum.TransportationLogBookPage:
                this.navigateByUrl('/log-books-and-declarations', id, isPerson);
                break;
            case PageCodeEnum.SciFi:
                this.navigateByUrl('/scientific-fishing', id, isPerson);
                break;
            case PageCodeEnum.PenalDecrees:
                this.navigateByUrl('/awarded-points', id, isPerson);
                break;
            default:
                this.navigateByUrl('/application_processing', id, isPerson);
                break;
        }
    }

    private buildForm(): void {
        if (this.reportType === this.reportTypeEnum.LEGAL_ENTITIES) {
            this.formGroup = new FormGroup({
                legalNameControl: new FormControl(),
                eikControl: new FormControl(),
                countryControl: new FormControl(),
                populatedAreaControl: new FormControl()
            });
        }
        else if (this.reportType === this.reportTypeEnum.PERSONS) {
            this.formGroup = new FormGroup({
                firstNameControl: new FormControl(),
                middleNameControl: new FormControl(),
                lastNameControl: new FormControl(),
                egnControl: new FormControl(),
                countryControl: new FormControl(),
                populatedAreaControl: new FormControl()
            });
        }
    }

    private mapPersonsReportFilters(filters: FilterEventArgs): PersonsReportFilters {
        return new PersonsReportFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,
            middleName: filters.getValue('middleNameControl'),
            firstName: filters.getValue('firstNameControl'),
            lastName: filters.getValue('lastNameControl'),
            egn: filters.getValue('egnControl'),
            countryId: filters.getValue('countryControl'),
            populatedAreaId: filters.getValue('populatedAreaControl')
        });
    }

    private mapLegalEntitiesReportFilters(filters: FilterEventArgs): LegalEntitiesReportFilters {
        return new LegalEntitiesReportFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,
            legalName: filters.getValue('legalNameControl'),
            eik: filters.getValue('eikControl'),
            countryId: filters.getValue('countryControl'),
            populatedAreaId: filters.getValue('populatedAreaControl')
        });
    }

    private closeDialogBtnClicked(closeFn: () => void): void {
        closeFn();
    }

    private navigateByUrl(url: string, id: number, isPerson: boolean): void {
        this.router.navigateByUrl(url, { state: { id: id, isPerson: isPerson } });
    }
}
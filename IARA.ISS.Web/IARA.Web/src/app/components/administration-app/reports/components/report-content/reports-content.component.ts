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
    public readonly canReadSciFiApplicationRecords: boolean;
    public readonly canReadQuaFiRegisterRecords: boolean;
    public readonly canReadQuaFiApplicationRecords: boolean;
    public readonly canReadAquacultureRegisterRecords: boolean;
    public readonly canReadAquacultureApplicationRecords: boolean;
    public readonly canReadBuyerRegisterRecords: boolean;
    public readonly canReadBuyerApplicationRecords: boolean;
    public readonly canReadAuanRegisterRecords: boolean;
    public readonly canReadCapacityRegisterRecords: boolean;
    public readonly canReadCapacityApplicationRecords: boolean;
    public readonly canReadInspectedPeopleRegisterRecords: boolean;
    public readonly canReadLogBookRegisterRecords: boolean;
    public readonly canReadPermitLicensesRegisterRecords: boolean;
    public readonly canReadPermitLicensesApplicationRecords: boolean;
    public readonly canReadPermitRegisterRecords: boolean;
    public readonly canReadPermitApplicationRecords: boolean;
    public readonly canReadShipOwnerRegisterRecords: boolean;
    public readonly canReadShipOwnerApplicationRecords: boolean;
    public readonly canReadStatFormsRegisterRecords: boolean;
    public readonly canReadStatFormsApplicationRecords: boolean;
    public readonly canReadAssocationsRecords: boolean;
    public readonly canReadAssocationsApplicationRecords: boolean;
    public readonly canReadAssociationTicketRecords: boolean;
    public readonly canReadTicketRecords: boolean;
    public readonly canReadExternalUserRecords: boolean;
    public readonly canReadInternalUserRecords: boolean;
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
        this.canReadSciFiApplicationRecords = permissions.has(PermissionsEnum.ScientificFishingApplicationsRead);
        this.canReadQuaFiRegisterRecords = permissions.has(PermissionsEnum.QualifiedFishersRead);
        this.canReadQuaFiApplicationRecords = permissions.has(PermissionsEnum.QualifiedFishersApplicationsRead);
        this.canReadAquacultureRegisterRecords = this.permissions.has(PermissionsEnum.AquacultureFacilitiesRead);
        this.canReadAquacultureApplicationRecords = this.permissions.has(PermissionsEnum.AquacultureFacilitiesApplicationsRead);
        this.canReadAuanRegisterRecords = this.permissions.has(PermissionsEnum.AuanRegisterRead)
        this.canReadBuyerRegisterRecords = this.permissions.has(PermissionsEnum.BuyersRead);
        this.canReadBuyerApplicationRecords = this.permissions.has(PermissionsEnum.BuyersApplicationsRead);
        this.canReadLogBookRegisterRecords = this.permissions.has(PermissionsEnum.FishLogBookRead)
            || this.permissions.has(PermissionsEnum.TransportationLogBookRead)
            || this.permissions.has(PermissionsEnum.ApplicationsRead)
            || this.permissions.has(PermissionsEnum.AuanRegisterRead)
            || this.permissions.has(PermissionsEnum.FirstSaleLogBookRead)
            || this.permissions.has(PermissionsEnum.AdmissionLogBookRead)
            || this.permissions.has(PermissionsEnum.AquacultureLogBookRead);

        this.canReadPermitLicensesRegisterRecords = this.permissions.has(PermissionsEnum.CommercialFishingPermitLicenseRegisterRead);
        this.canReadPermitLicensesApplicationRecords = this.permissions.has(PermissionsEnum.CommercialFishingPermitLicenseApplicationsRead);
        this.canReadPermitRegisterRecords = this.permissions.has(PermissionsEnum.CommercialFishingPermitRegisterRead);
        this.canReadPermitApplicationRecords = this.permissions.has(PermissionsEnum.CommercialFishingPermitApplicationsRead);
        this.canReadInspectedPeopleRegisterRecords = this.permissions.has(PermissionsEnum.InspectionsRead);
        this.canReadAssocationsRecords = this.permissions.has(PermissionsEnum.AssociationsRead);
        this.canReadAssocationsApplicationRecords = this.permissions.has(PermissionsEnum.LegalEntitiesApplicationsRead);
        this.canReadAssociationTicketRecords = this.permissions.has(PermissionsEnum.AssociationsTicketsRead);
        this.canReadTicketRecords = this.permissions.has(PermissionsEnum.TicketsRead);
        this.canReadCapacityRegisterRecords = this.permissions.has(PermissionsEnum.FishingCapacityCertificatesRead);
        this.canReadCapacityApplicationRecords = this.permissions.has(PermissionsEnum.FishingCapacityApplicationsRead);
        this.canReadShipOwnerRegisterRecords = this.permissions.has(PermissionsEnum.ShipsRegisterRead);
        this.canReadShipOwnerApplicationRecords = this.permissions.has(PermissionsEnum.ShipsRegisterApplicationsRead);

        this.canReadStatFormsRegisterRecords = this.permissions.has(PermissionsEnum.StatisticalFormsAquaFarmRead)
            || this.permissions.has(PermissionsEnum.StatisticalFormsReworkRead)
            || this.permissions.has(PermissionsEnum.StatisticalFormsFishVesselRead);

        this.canReadStatFormsApplicationRecords = this.permissions.has(PermissionsEnum.StatisticalFormsAquaFarmApplicationsRead)
            || this.permissions.has(PermissionsEnum.StatisticalFormsReworkApplicationsRead)
            || this.permissions.has(PermissionsEnum.StatisticalFormsFishVesselsApplicationsRead);

        this.canReadExternalUserRecords = this.permissions.has(PermissionsEnum.ExternalUsersRead);
        this.canReadInternalUserRecords = this.permissions.has(PermissionsEnum.InternalUsersRead);

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
        }
        else if (this.reportType === this.reportTypeEnum.PERSONS) {
            this.gridManager = new DataTableManager<PersonReportDTO, PersonsReportFilters>({
                tlDataTable: this.datatable,
                searchPanel: this.searchpanel,
                requestServiceMethod: this.service.getAllPersonsReport.bind(this.service),
                filtersMapper: this.mapPersonsReportFilters
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
        const isApplication: boolean = row.data.isApplication ?? false;

        switch (row.data.pageCode) {
            case PageCodeEnum.RegVessel:
            case PageCodeEnum.ShipRegChange:
            case PageCodeEnum.DeregShip:
                return isApplication ? this.canReadShipOwnerApplicationRecords : this.canReadShipOwnerRegisterRecords;
            case PageCodeEnum.CommFish:
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.RightToFishThirdCountry:
            case PageCodeEnum.DupCommFish:
            case PageCodeEnum.DupRightToFishThirdCountry:
            case PageCodeEnum.DupPoundnetCommFish:
                return isApplication ? this.canReadPermitApplicationRecords : this.canReadPermitRegisterRecords;
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
            case PageCodeEnum.DupRightToFishResource:
            case PageCodeEnum.DupPoundnetCommFishLic:
            case PageCodeEnum.DupCatchQuataSpecies:
            case PageCodeEnum.FishingGearsCommFish:
                return isApplication ? this.canReadPermitLicensesApplicationRecords : this.canReadPermitLicensesRegisterRecords;
            case PageCodeEnum.AptitudeCourceExam:
            case PageCodeEnum.CommFishLicense:
            case PageCodeEnum.CompetencyDup:
                return isApplication ? this.canReadQuaFiApplicationRecords : this.canReadQuaFiRegisterRecords;
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
                return isApplication ? this.canReadBuyerApplicationRecords : this.canReadBuyerRegisterRecords;
            case PageCodeEnum.IncreaseFishCap:
            case PageCodeEnum.ReduceFishCap:
            case PageCodeEnum.TransferFishCap:
            case PageCodeEnum.CapacityCertDup:
                return isApplication ? this.canReadCapacityApplicationRecords : this.canReadCapacityRegisterRecords;
            case PageCodeEnum.Inspections:
                return this.canReadInspectedPeopleRegisterRecords;
            case PageCodeEnum.RecFish:
                return this.canReadTicketRecords;
            case PageCodeEnum.Assocs:
            case PageCodeEnum.LE:
                return isApplication ? this.canReadAssocationsApplicationRecords : this.canReadAssociationTicketRecords;
            case PageCodeEnum.StatFormAquaFarm:
            case PageCodeEnum.StatFormFishVessel:
            case PageCodeEnum.StatFormRework:
                return isApplication ? this.canReadStatFormsApplicationRecords : this.canReadStatFormsRegisterRecords;
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
            case PageCodeEnum.NewsManagement:
                return row.data.isInternal ? this.canReadInternalUserRecords : this.canReadExternalUserRecords;
            default:
                return false;
        }
    }

    public openRegister(row: GridRow<ReportHistoryDTO>): void {
        const id: number = row.data.id!;
        const isPerson: boolean = row.data.isPerson!;
        const isApplication: boolean = row.data.isApplication ?? false;
        const isSubmittedFor: boolean = row.data.isSubmittedFor ?? false;
        const isInternal: boolean = row.data.isInternal ?? false;

        switch (row.data.pageCode) {
            case PageCodeEnum.RegVessel:
            case PageCodeEnum.DeregShip:
            case PageCodeEnum.ShipRegChange:
                if (isApplication) {
                    this.navigateByUrl('/fishing-vessels-applications', id, isPerson);
                }
                else {
                    this.navigateByUrl('/fishing-vessels', id, isPerson);
                }
                break;
            case PageCodeEnum.CommFish:
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.RightToFishThirdCountry:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
            case PageCodeEnum.FishingGearsCommFish:
                if (isApplication) {
                    this.navigateByUrl('/commercial-fishing-applications', id, isPerson);
                }
                else {
                    this.navigateByUrl('/commercial-fishing-permits-and-licenses', id, isPerson);
                }
                break;
            case PageCodeEnum.DupCommFish:
            case PageCodeEnum.DupRightToFishThirdCountry:
            case PageCodeEnum.DupPoundnetCommFish:
            case PageCodeEnum.DupRightToFishResource:
            case PageCodeEnum.DupPoundnetCommFishLic:
            case PageCodeEnum.DupCatchQuataSpecies:
                if (isApplication) {
                    this.navigateByUrl('/commercial-fishing-applications', id, isPerson, isSubmittedFor, false);
                }
                else {
                    this.navigateByUrl('/commercial-fishing-permits-and-licenses', id, isPerson, false, true);
                }
                break;
            case PageCodeEnum.AptitudeCourceExam:
            case PageCodeEnum.CommFishLicense:
            case PageCodeEnum.CompetencyDup:
                if (isApplication) {
                    this.navigateByUrl('/qualified-fishers-applications', id, isPerson, isSubmittedFor);
                }
                else {
                    this.navigateByUrl('/qualified-fishers-register', id, isPerson);
                }
                break;
            case PageCodeEnum.AquaFarmReg:
            case PageCodeEnum.AquaFarmDereg:
            case PageCodeEnum.AquaFarmChange:
                if (isApplication) {
                    this.navigateByUrl('/aquaculture-farms-applications', id, isPerson, isSubmittedFor);
                }
                else {
                    this.navigateByUrl('/aquaculture-farms', id, isPerson);
                }
                break;
            case PageCodeEnum.AuanRegister:
                this.navigateByUrl('/auan-register', id, isPerson);
                break;
            case PageCodeEnum.RegFirstSaleBuyer:
            case PageCodeEnum.RegFirstSaleCenter:
            case PageCodeEnum.TermFirstSaleBuyer:
            case PageCodeEnum.TermFirstSaleCenter:
            case PageCodeEnum.Buyers:
            case PageCodeEnum.ChangeFirstSaleBuyer:
            case PageCodeEnum.ChangeFirstSaleCenter:
            case PageCodeEnum.DupFirstSaleBuyer:
            case PageCodeEnum.DupFirstSaleCenter:
                if (isApplication) {
                    this.navigateByUrl('/sales-centers-applications', id, isPerson, isSubmittedFor);
                }
                else {
                    this.navigateByUrl('/sales-centers-register', id, isPerson);
                }
                break;
            case PageCodeEnum.IncreaseFishCap:
            case PageCodeEnum.ReduceFishCap:
            case PageCodeEnum.TransferFishCap:
            case PageCodeEnum.CapacityCertDup:
                if (isApplication) {
                    this.navigateByUrl('/fishing-capacity-applications', id, isPerson, isSubmittedFor);
                }
                else {
                    this.navigateByUrl('/fishing-capacity-certificates-register', id, isPerson);
                }
                break;
            case PageCodeEnum.Inspections:
                this.navigateByUrl('/inspections-register', id, isPerson);
                break;
            case PageCodeEnum.RecFish:
                this.navigateByUrl('/ticket_applications', id, isPerson);
                break;
            case PageCodeEnum.Assocs:
            case PageCodeEnum.LE:
                if (isApplication) {
                    this.navigateByUrl('/legal-entities-applications', id, isPerson, isSubmittedFor);
                }
                else {
                    this.navigateByUrl('/ticket_applications', id, isPerson);
                }
                break;
            case PageCodeEnum.StatFormAquaFarm:
            case PageCodeEnum.StatFormFishVessel:
            case PageCodeEnum.StatFormRework:
                if (isApplication) {
                    this.navigateByUrl('/statistical-forms-applications', id, isPerson, isSubmittedFor);
                }
                else {
                    this.navigateByUrl('/statistical-forms-register', id, isPerson);
                }
                break;
            case PageCodeEnum.AdmissionLogBookPage:
            case PageCodeEnum.AquacultureLogBookPage:
            case PageCodeEnum.FirstSaleLogBookPage:
            case PageCodeEnum.ShipLogBookPage:
            case PageCodeEnum.TransportationLogBookPage:
                this.navigateByUrl('/log-books-and-declarations', id, isPerson);
                break;
            case PageCodeEnum.SciFi:
                if (isApplication) {
                    this.navigateByUrl('/scientific-fishing-applications', id, isPerson, isSubmittedFor);
                }
                else {
                    this.navigateByUrl('/scientific-fishing', id, isPerson);
                }
                break;
            case PageCodeEnum.PenalDecrees:
                this.navigateByUrl('/awarded-points', id, isPerson);
                break;
            case PageCodeEnum.NewsManagement:
                if (isInternal) {
                    this.navigateByUrl('/internal-users', id, isPerson);
                }
                else {
                    this.navigateByUrl('/external-users', id, isPerson);
                }
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

    private navigateByUrl(url: string, id: number, isPerson: boolean, isSubmittedFor: boolean = false, isDuplicate: boolean = false): void {
        this.router.navigateByUrl(url, {
            state: {
                id: id,
                isPerson: isPerson,
                isSubmittedFor: isSubmittedFor,
                isDuplicate: isDuplicate
            }
        });
    }
}
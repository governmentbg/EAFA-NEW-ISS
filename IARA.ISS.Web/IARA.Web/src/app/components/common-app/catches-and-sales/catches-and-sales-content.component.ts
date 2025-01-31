import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { CatchesAndSalesDialogParamsModel } from './models/catches-and-sales-dialog-params.model';
import { EditShipLogBookPageComponent } from './components/ship-log-book/edit-ship-log-book-page.component';
import { JustifiedCancellationComponent } from './components/justified-cancellation/justified-cancellation.component';
import { JustifiedCancellationDialogParams } from './components/justified-cancellation/models/justified-cancellation-dialog-params.model';
import { EditTransportationLogBookPageComponent } from './components/edit-transporation-log-book/edit-transportation-log-book-page.component';
import { EditAquacultureLogBookPageComponent } from './components/edit-aquaculture-log-book-page/edit-aquaculture-log-book-page.component';
import { EditAdmissionLogBookPageComponent } from './components/edit-admission-log-book/edit-admission-log-book-page.component';
import { EditFirstSaleLogBookPageComponent } from './components/edit-first-sale-log-book/edit-first-sale-log-book-page.component';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { LogBookRegisterDTO } from '@app/models/generated/dtos/LogBookRegisterDTO';
import { CatchesAndSalesPublicFilters } from '@app/models/generated/filters/CatchesAndSalesPublicFilters';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ShipLogBookPageRegisterDTO } from '@app/models/generated/dtos/ShipLogBookPageRegisterDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { AdmissionLogBookPageRegisterDTO } from '@app/models/generated/dtos/AdmissionLogBookPageRegisterDTO';
import { TransportationLogBookPageRegisterDTO } from '@app/models/generated/dtos/TransportationLogBookPageRegisterDTO';
import { FirstSaleLogBookPageRegisterDTO } from '@app/models/generated/dtos/FirstSaleLogBookPageRegisterDTO';
import { AquacultureLogBookPageRegisterDTO } from '@app/models/generated/dtos/AquacultureLogBookPageRegisterDTO';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { ShipLogBookPageEditDTO } from '@app/models/generated/dtos/ShipLogBookPageEditDTO';
import { FirstSaleLogBookPageEditDTO } from '@app/models/generated/dtos/FirstSaleLogBookPageEditDTO';
import { AdmissionLogBookPageEditDTO } from '@app/models/generated/dtos/AdmissionLogBookPageEditDTO';
import { TransportationLogBookPageEditDTO } from '@app/models/generated/dtos/TransportationLogBookPageEditDTO';
import { AquacultureLogBookPageEditDTO } from '@app/models/generated/dtos/AquacultureLogBookPageEditDTO';
import { LogBookPageCancellationReasonDTO } from '@app/models/generated/dtos/LogBookPageCancellationReasonDTO';
import { AddLogBookPageWizardComponent } from './components/add-log-book-page-wizard/add-log-book-page-wizard.component';
import { AddLogBookPageDialogParams } from './components/add-log-book-page-wizard/models/add-log-book-page-wizard-dialog-params.model';
import { CatchesAndSalesAdministrationFilters } from '@app/models/generated/filters/CatchesAndSalesAdministrationFilters';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';
import { AddShipPageWizardComponent } from './components/add-ship-page-wizard/add-ship-page-wizard.component';
import { AddShipWizardDialogParams } from './components/add-ship-page-wizard/models/add-ship-wizard-dialog-params.model';
import { EditShipLogBookPageDialogParams } from './components/ship-log-book/models/edit-ship-log-book-page-dialog-params.model';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { EditLogBookComponent } from '../commercial-fishing/components/edit-log-book/edit-log-book.component';
import { EditLogBookDialogParamsModel } from '../commercial-fishing/components/log-books/models/edit-log-book-dialog-params.model';
import { LogBookGroupsEnum } from '@app/enums/log-book-groups.enum';
import { LogBookEditDTO } from '@app/models/generated/dtos/LogBookEditDTO';
import { CommercialFishingLogBookEditDTO } from '@app/models/generated/dtos/CommercialFishingLogBookEditDTO';
import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum';
import { AddShipPageDocumentWizardComponent } from './components/add-ship-page-document-wizard/add-ship-page-document-wizard.component';
import { AddShipPageDocumentDialogParamsModel } from './models/add-ship-page-document-dialog-params.model';
import { LogBookPageDocumentTypesEnum } from './enums/log-book-page-document-types.enum';
import { PagesPermissions } from './components/ship-pages-and-declarations-table/models/pages-permissions.model';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { SystemPropertiesService } from '@app/services/common-app/system-properties.service';
import { SystemPropertiesDTO } from '@app/models/generated/dtos/SystemPropertiesDTO';
import { LogBookStatusesEnum } from '@app/enums/log-book-statuses.enum';
import { AddRelatedDeclarationComponent } from './components/add-related-declaration/add-related-declaration.component';
import { AddRelatedDeclarationDialogParams } from './models/add-related-declaration-dialog-params.model';
import { EditLogBookPageNumberComponent } from './components/edit-log-book-page-number/edit-log-book-page-number.component';
import { EditPageNumberDilogParamsModel } from './models/edit-page-number-dialog-params.model';
import { LogBookPageFilesDialogParamsModel } from './models/log-book-page-files-dialog-params.model';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { LogBookPageFilesComponent } from './components/log-book-page-files/log-book-page-files.component';
import { LogBookPageFilesDTO } from '@app/models/generated/dtos/LogBookPageFilesDTO';
import { LogBookPageEditExceptionDTO } from '@app/models/generated/dtos/LogBookPageEditExceptionDTO';
import { CatchesAndSalesUtils } from './utils/catches-and-sales.utils';
import { SecurityService } from '@app/services/common-app/security.service';

const PAGE_NUMBER_CONTROL_NAME: string = 'pageNumberControl';
type ThreeState = 'yes' | 'no' | 'both';

@Component({
    selector: 'catches-and-sales-content',
    templateUrl: './catches-and-sales-content.component.html'
})
export class CatchesAndSalesContent implements OnInit, AfterViewInit {
    @Input()
    public isPublicApp!: boolean;

    @Input()
    private service!: ICatchesAndSalesService;

    public translationService: FuseTranslationLoaderService;
    public formGroup!: FormGroup;

    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;
    public readonly logBookTypesEnum: typeof LogBookTypesEnum = LogBookTypesEnum;
    public readonly logBookStatusesEnum: typeof LogBookStatusesEnum = LogBookStatusesEnum;
    public readonly logBookPageStatusesEnum: typeof LogBookPageStatusesEnum = LogBookPageStatusesEnum;

    public readonly canReadShipLogBookRecords: boolean;
    public readonly canAddShipLogBookRecords: boolean;
    public readonly canEditShipLogBookRecords: boolean;
    public readonly canCancelShipLogBookRecords: boolean;
    public readonly canEditNumberShipLogBookRecords: boolean;
    public readonly canAddEditFilesShipLogBookRecords: boolean;
    public readonly canEditOnlineShipLogBookRecords: boolean;

    public readonly canReadFirstSaleLogBookRecords: boolean;
    public readonly canAddFirstSaleLogBookRecords: boolean;
    public readonly canEditFirstSaleLogBookRecords: boolean;
    public readonly canCancelFirstSaleLogBookRecords: boolean;
    public readonly canEditNumberFirstSaleLogBookRecords: boolean;
    public readonly canEditCommonDataFirstSaleLogBookRecords: boolean;
    public readonly canAddEditFilesFirstSaleLogBookRecords: boolean;

    public readonly canReadAdmissionLogBookRecords: boolean;
    public readonly canAddAdmissionLogBookRecords: boolean;
    public readonly canEditAdmissionLogBookRecords: boolean;
    public readonly canCancelAdmissionLogBookRecords: boolean;
    public readonly canEditNumberAdmissionLogBookRecords: boolean;
    public readonly canEditCommonDataAdmissionLogBookRecords: boolean;
    public readonly canAddEditFilesAdmissionLogBookRecords: boolean;

    public readonly canReadTransportationLogBookRecords: boolean;
    public readonly canAddTransportationLogBookRecords: boolean;
    public readonly canEditTransportationLogBookRecords: boolean;
    public readonly canCancelTransportationLogBookRecords: boolean;
    public readonly canEditNumberTransportationLogBookRecords: boolean;
    public readonly canEditCommonDataTransportationLogBookRecords: boolean;
    public readonly canAddEditFilesTransportationLogBookRecords: boolean;

    public readonly canReadAquacultureLogBookRecords: boolean;
    public readonly canAddAquacultureLogBookRecords: boolean;
    public readonly canEditAquacultureLogBookRecords: boolean;
    public readonly canCancelAquacultureLogBookRecords: boolean;
    public readonly canEditNumberAquacultureLogBookRecords: boolean;
    public readonly canAddEditFilesAquacultureLogBookRecords: boolean;

    public readonly canReadInspections: boolean;

    public readonly shipForbiddenForPagesTooltip: string;
    public readonly addLogBookPageTooltip: string;
    public readonly logBookFishinedTooltip: string;
    public readonly logBookHasSuspendedLicenseTooltip: string;

    public logBookTypes: NomenclatureDTO<number>[] = [];
    public ships: ShipNomenclatureDTO[] = [];
    public aquacultureFacilities: NomenclatureDTO<number>[] = [];
    public registeredBuyers: NomenclatureDTO<number>[] = [];
    public logBookStatuses: NomenclatureDTO<number>[] = [];
    public showExistingPages: NomenclatureDTO<ThreeState>[] = [];
    public allowAddDeclarationsAfterFinished: boolean = false;

    public pagesPermissions!: PagesPermissions;

    @ViewChild('mainTable')
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private systemProperties!: SystemPropertiesDTO;
    private currentUserId: number;

    private gridManager!: DataTableManager<LogBookRegisterDTO, CatchesAndSalesAdministrationFilters | CatchesAndSalesPublicFilters>;

    private logBookExceptions: LogBookPageEditExceptionDTO[] = [];

    private readonly nomenclatures: CommonNomenclatures;
    private readonly addShipLogBookPageWizardDialog: TLMatDialog<AddShipPageWizardComponent>;
    private readonly editShipLogBookPageDialog: TLMatDialog<EditShipLogBookPageComponent>;
    private readonly addRelatedDeclarationDialog: TLMatDialog<AddRelatedDeclarationComponent>;
    private readonly addLogBookPageWizardDialog: TLMatDialog<AddLogBookPageWizardComponent>;
    private readonly editFirstSaleLogBookPageDialog: TLMatDialog<EditFirstSaleLogBookPageComponent>;
    private readonly editAdmissionLogBookPageDialog: TLMatDialog<EditAdmissionLogBookPageComponent>;
    private readonly editTransportationLogBookPageDialog: TLMatDialog<EditTransportationLogBookPageComponent>;
    private readonly editAquacultureLogBookPageDialog: TLMatDialog<EditAquacultureLogBookPageComponent>;
    private readonly cancellationDialog: TLMatDialog<JustifiedCancellationComponent>;
    private readonly viewLogBookDialog: TLMatDialog<EditLogBookComponent>;
    private readonly addShipPageDocumentWizardDialog: TLMatDialog<AddShipPageDocumentWizardComponent>;
    private readonly editPageNumberDialog: TLMatDialog<EditLogBookPageNumberComponent>;
    private readonly editPageFilesDialog: TLMatDialog<LogBookPageFilesComponent>;
    private readonly confirmationDialog: TLConfirmDialog;
    private readonly systemPropertiesService: SystemPropertiesService;

    public constructor(
        translationService: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        permissions: PermissionsService,
        addShipLogBookPageWizardDialog: TLMatDialog<AddShipPageWizardComponent>,
        editShipLogBookPageDialog: TLMatDialog<EditShipLogBookPageComponent>,
        addRelatedDeclarationDialog: TLMatDialog<AddRelatedDeclarationComponent>,
        addLogBookPageWizardDialog: TLMatDialog<AddLogBookPageWizardComponent>,
        editFirstSaleLogBookPageDialog: TLMatDialog<EditFirstSaleLogBookPageComponent>,
        editAdmissionLogBookPageDialog: TLMatDialog<EditAdmissionLogBookPageComponent>,
        editTransportationLogBookPageDialog: TLMatDialog<EditTransportationLogBookPageComponent>,
        editAquacultureLogBookPageDialog: TLMatDialog<EditAquacultureLogBookPageComponent>,
        cancellationDialog: TLMatDialog<JustifiedCancellationComponent>,
        viewLogBookDialog: TLMatDialog<EditLogBookComponent>,
        addShipPageDocumentWizardDialog: TLMatDialog<AddShipPageDocumentWizardComponent>,
        editPageNumberDialog: TLMatDialog<EditLogBookPageNumberComponent>,
        editPageFilesDialog: TLMatDialog<LogBookPageFilesComponent>,
        confirmationDialog: TLConfirmDialog,
        systemPropertiesService: SystemPropertiesService,
        authService: SecurityService
    ) {
        this.translationService = translationService;
        this.nomenclatures = nomenclatures;

        this.addShipLogBookPageWizardDialog = addShipLogBookPageWizardDialog;
        this.editShipLogBookPageDialog = editShipLogBookPageDialog;
        this.addRelatedDeclarationDialog = addRelatedDeclarationDialog;
        this.addLogBookPageWizardDialog = addLogBookPageWizardDialog;
        this.editFirstSaleLogBookPageDialog = editFirstSaleLogBookPageDialog;
        this.editAdmissionLogBookPageDialog = editAdmissionLogBookPageDialog;
        this.editTransportationLogBookPageDialog = editTransportationLogBookPageDialog;
        this.editAquacultureLogBookPageDialog = editAquacultureLogBookPageDialog;
        this.cancellationDialog = cancellationDialog;
        this.viewLogBookDialog = viewLogBookDialog;
        this.addShipPageDocumentWizardDialog = addShipPageDocumentWizardDialog;
        this.editPageNumberDialog = editPageNumberDialog;
        this.editPageFilesDialog = editPageFilesDialog;
        this.confirmationDialog = confirmationDialog;
        this.systemPropertiesService = systemPropertiesService;

        this.canReadShipLogBookRecords = permissions.has(PermissionsEnum.FishLogBookRead) || permissions.has(PermissionsEnum.FishLogBookPageReadAll);
        this.canAddShipLogBookRecords = permissions.has(PermissionsEnum.FishLogBookPageAdd);
        this.canEditShipLogBookRecords = permissions.has(PermissionsEnum.FishLogBookPageEdit);
        this.canCancelShipLogBookRecords = permissions.has(PermissionsEnum.FishLogBookPageCancel);
        this.canEditNumberShipLogBookRecords = permissions.has(PermissionsEnum.FishLogBookPageEditNumber);
        this.canAddEditFilesShipLogBookRecords = permissions.has(PermissionsEnum.FishLogBookPageEditFiles);
        this.canEditOnlineShipLogBookRecords = permissions.has(PermissionsEnum.FishLogBookPageOnlineEdit);

        this.canReadFirstSaleLogBookRecords = permissions.has(PermissionsEnum.FirstSaleLogBookRead) || permissions.has(PermissionsEnum.FirstSaleLogBookPageReadAll);
        this.canAddFirstSaleLogBookRecords = permissions.has(PermissionsEnum.FirstSaleLogBookPageAdd);
        this.canEditFirstSaleLogBookRecords = permissions.has(PermissionsEnum.FirstSaleLogBookPageEdit);
        this.canCancelFirstSaleLogBookRecords = permissions.has(PermissionsEnum.FirstSaleLogBookPageCancel);
        this.canEditNumberFirstSaleLogBookRecords = permissions.has(PermissionsEnum.FirstSaleLogBookPageEditNumber);
        this.canEditCommonDataFirstSaleLogBookRecords = permissions.has(PermissionsEnum.FirstSaleLogBookPageEditCommonData);
        this.canAddEditFilesFirstSaleLogBookRecords = permissions.has(PermissionsEnum.FirstSaleLogBookPageEditFiles);

        this.canReadAdmissionLogBookRecords = permissions.has(PermissionsEnum.AdmissionLogBookRead) || permissions.has(PermissionsEnum.AdmissionLogBookPageReadAll);
        this.canAddAdmissionLogBookRecords = permissions.has(PermissionsEnum.AdmissionLogBookPageAdd);
        this.canEditAdmissionLogBookRecords = permissions.has(PermissionsEnum.AdmissionLogBookPageEdit);
        this.canCancelAdmissionLogBookRecords = permissions.has(PermissionsEnum.AdmissionLogBookPageCancel);
        this.canEditNumberAdmissionLogBookRecords = permissions.has(PermissionsEnum.AdmissionLogBookPageEditNumber);
        this.canEditCommonDataAdmissionLogBookRecords = permissions.has(PermissionsEnum.AdmissionLogBookPageEditCommonData);
        this.canAddEditFilesAdmissionLogBookRecords = permissions.has(PermissionsEnum.AdmissionLogBookPageEditFiles);

        this.canReadTransportationLogBookRecords = permissions.has(PermissionsEnum.TransportationLogBookRead) || permissions.has(PermissionsEnum.TransportationLogBookPageReadAll);
        this.canAddTransportationLogBookRecords = permissions.has(PermissionsEnum.TransportationLogBookPageAdd);
        this.canEditTransportationLogBookRecords = permissions.has(PermissionsEnum.TransportationLogBookPageEdit);
        this.canCancelTransportationLogBookRecords = permissions.has(PermissionsEnum.TransportationLogBookPageCancel);
        this.canEditNumberTransportationLogBookRecords = permissions.has(PermissionsEnum.TransportationLogBookPageEditNumber);
        this.canEditCommonDataTransportationLogBookRecords = permissions.has(PermissionsEnum.TransportationLogBookPageEditCommonData);
        this.canAddEditFilesTransportationLogBookRecords = permissions.has(PermissionsEnum.TransportationLogBookPageEditFiles);

        this.canReadAquacultureLogBookRecords = permissions.has(PermissionsEnum.AquacultureLogBookRead) || permissions.has(PermissionsEnum.AquacultureLogBookPageReadAll);
        this.canAddAquacultureLogBookRecords = permissions.has(PermissionsEnum.AquacultureLogBookPageAdd);
        this.canEditAquacultureLogBookRecords = permissions.has(PermissionsEnum.AquacultureLogBookPageEdit);
        this.canCancelAquacultureLogBookRecords = permissions.has(PermissionsEnum.AquacultureLogBookPageCancel);
        this.canEditNumberAquacultureLogBookRecords = permissions.has(PermissionsEnum.AquacultureLogBookPageEditNumber);
        this.canAddEditFilesAquacultureLogBookRecords = permissions.has(PermissionsEnum.AquacultureLogBookPageEditFiles);

        this.canReadInspections = permissions.has(PermissionsEnum.InspectionsRead) || permissions.has(PermissionsEnum.InspectionsReadAll);

        this.pagesPermissions = new PagesPermissions({
            canAddAdmissionLogBookRecords: this.canAddAdmissionLogBookRecords,
            canAddFirstSaleLogBookRecords: this.canAddFirstSaleLogBookRecords,
            canAddTransportationLogBookRecords: this.canAddTransportationLogBookRecords,
            canCancelShipLogBookRecords: this.canCancelShipLogBookRecords,
            canEditAdmissionLogBookRecords: this.canEditAdmissionLogBookRecords,
            canEditFirstSaleLogBookRecords: this.canEditFirstSaleLogBookRecords,
            canEditShipLogBookRecords: this.canEditShipLogBookRecords,
            canEditTransportationLogBookRecords: this.canEditTransportationLogBookRecords,
            canEditNumberShipLogBookRecords: this.canEditNumberShipLogBookRecords,
            canAddEditFilesShipLogBookRecords: this.canAddEditFilesShipLogBookRecords,
            canEditOnlineShipLogBookRecords: this.canEditOnlineShipLogBookRecords
        });

        this.shipForbiddenForPagesTooltip = this.translationService.getValue('catches-and-sales.ship-is-forbidden-for-page-add');
        this.addLogBookPageTooltip = this.translationService.getValue('catches-and-sales.add-log-book-page');
        this.logBookFishinedTooltip = this.translationService.getValue('catches-and-sales.no-page-add-because-log-book-is-finished');
        this.logBookHasSuspendedLicenseTooltip = this.translationService.getValue('catches-and-sales.no-page-add-because-log-book-is-suspended');

        this.showExistingPages = [
            new NomenclatureDTO<ThreeState>({
                value: 'yes',
                displayName: this.translationService.getValue('catches-and-sales.show-only-existing-pages-yes'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'no',
                displayName: this.translationService.getValue('catches-and-sales.show-only-existing-pages-no'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'both',
                displayName: this.translationService.getValue('catches-and-sales.show-only-existing-pages-all'),
                isActive: true
            })
        ];

        this.currentUserId = authService.User!.userId;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        this.systemProperties = await this.systemPropertiesService.properties.toPromise();

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.LogBookTypes, this.service.getLogBookTypes.bind(this.service)
        ).subscribe({
            next: (results: NomenclatureDTO<number>[]) => {
                this.logBookTypes = results;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.LogBookStatuses, this.nomenclatures.getLogBookStatuses.bind(this.nomenclatures), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.logBookStatuses = result;

                const defaultStatuses: NomenclatureDTO<number>[] = result.filter(x => x.code === LogBookStatusesEnum[LogBookStatusesEnum.New] || x.code === LogBookStatusesEnum[LogBookStatusesEnum.Renewed])!;
                this.formGroup.get('logBookStatusesControl')!.setValue(defaultStatuses);
            }
        });

        this.service.getLogBookPageEditExceptions().subscribe({
            next: (exceptions: LogBookPageEditExceptionDTO[]) => {
                this.logBookExceptions = exceptions;
            }
        })

        if (!this.isPublicApp && this.canReadShipLogBookRecords) {
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false
            ).subscribe({
                next: (ships: ShipNomenclatureDTO[]) => {
                    this.ships = ships;
                }
            });
        }

        if (!this.isPublicApp && this.canReadAquacultureLogBookRecords) {
            this.service.getAquacultureFacilities().subscribe({
                next: (result: NomenclatureDTO<number>[]) => {
                    this.aquacultureFacilities = result;
                }
            });
        }

        if (!this.isPublicApp && (this.canReadFirstSaleLogBookRecords || this.canReadAdmissionLogBookRecords || this.canReadTransportationLogBookRecords)) {
            this.service.getRegisteredBuyersNomenclature().subscribe({
                next: (result: NomenclatureDTO<number>[]) => {
                    this.registeredBuyers = result;
                }
            });
        }
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<LogBookRegisterDTO, CatchesAndSalesAdministrationFilters | CatchesAndSalesPublicFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllCatchesAndSales.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        const ownerId: number | undefined = window.history.state?.id;
        const isPerson: boolean | undefined = window.history.state?.isPerson;
        const tableId: number | undefined = window.history.state?.tableId;
        const pageCode: string | undefined = window.history.state?.pageCode;

        if (!this.isPublicApp && !CommonUtils.isNullOrEmpty(ownerId)) {
            if (isPerson === true) {
                this.gridManager.advancedFilters = new CatchesAndSalesAdministrationFilters({ personId: ownerId });
            }
            else if (isPerson === false) {
                this.gridManager.advancedFilters = new CatchesAndSalesAdministrationFilters({ legalId: ownerId });
            }
        }

        if (!CommonUtils.isNullOrEmpty(tableId) && !CommonUtils.isNullOrEmpty(pageCode)) {
            switch (pageCode) {
                case 'ShipLogBookPage':
                case 'ShipLogBookPages':
                    this.gridManager.advancedFilters = new CatchesAndSalesAdministrationFilters({ shipLogBookPageId: tableId });
                    break;
                case 'FirstSaleLogBookPage':
                    this.gridManager.advancedFilters = new CatchesAndSalesAdministrationFilters({ firstSaleLogBookPageId: tableId });
                    break;
                case 'AdmissionLogBookPage':
                    this.gridManager.advancedFilters = new CatchesAndSalesAdministrationFilters({ admissionLogBookPageId: tableId });
                    break;
                case 'TransportationLogBookPage':
                    this.gridManager.advancedFilters = new CatchesAndSalesAdministrationFilters({ transportationLogBookPageId: tableId });
                    break;
                case 'AquacultureLogBookPage':
                    this.gridManager.advancedFilters = new CatchesAndSalesAdministrationFilters({ aquacultureLogBookPageId: tableId });
                    break;
            }
        }

        this.gridManager.onRequestServiceMethodCalled.subscribe({
            next: (rows: LogBookRegisterDTO[] | undefined) => {
                if (rows !== null && rows !== undefined && rows.length > 0) {
                    const now: Date = new Date();
                    if (this.systemProperties.addLogBookPagesDaysTolerance) {
                        for (const row of rows) {
                            const isUnlocked: boolean = CatchesAndSalesUtils.checkIfLogBookIsUnlocked(this.logBookExceptions, this.currentUserId, row.typeId!, row.id!, now);

                            //Страниците от електронните дневници може да се редактират, ако потребителят има такова право или има изключение за дневника
                            if (row.typeCode === LogBookTypesEnum.Ship) {
                                if (row.isOnline && !this.canEditOnlineShipLogBookRecords) {
                                    row.allowEditLogBookPages = isUnlocked;
                                }
                                else {
                                    row.allowEditLogBookPages = true;
                                }
                            }

                            if (row.isLogBookFinished || row.isLogBookSuspended) {
                                if (!isUnlocked) {
                                    if (row.suspendedPermitLicenseValidTo !== undefined && row.suspendedPermitLicenseValidTo !== null) {
                                        const now: Date = new Date();
                                        const days: number = Math.round((now.getTime() - new Date(row.suspendedPermitLicenseValidTo).getTime()) / (1000 * 3600 * 24));
                                        row.allowNewLogBookPages = days < this.systemProperties.addLogBookPagesDaysTolerance;
                                    }
                                    else if (row.finishDate !== undefined && row.finishDate !== null) {
                                        const now: Date = new Date();
                                        const days: number = Math.round((now.getTime() - new Date(row.finishDate).getTime()) / (1000 * 3600 * 24));
                                        row.allowNewLogBookPages = days < this.systemProperties.addLogBookPagesDaysTolerance;
                                    }
                                    else {
                                        row.allowNewLogBookPages = false;
                                    }
                                }
                                else {
                                    row.allowNewLogBookPages = true;
                                }
                            }
                            else {
                                row.allowNewLogBookPages = false;
                            }
                        }
                    }

                    setTimeout(() => {
                        const iconButtons = this.getMainTableButtons();
                        for (const iconButton of iconButtons) {
                            const row = rows!.find(x => x.id!.toString() === iconButton.getAttribute('data-logbook-id'));

                            if (row !== null && row !== undefined) {
                                if (iconButton.getAttribute('no-enable') === 'true' && (!row.allowNewLogBookPages && (row.isLogBookFinished || row.isLogBookSuspended))) {
                                    const button = iconButton.getElementsByTagName('button')[0];

                                    button.disabled = true;
                                    button.style.opacity = '0.26';
                                }
                                else {
                                    const button = iconButton.getElementsByTagName('button')[0];

                                    button.disabled = false;
                                    button.style.opacity = '1';
                                    button.style.cursor = 'pointer';
                                }
                            }
                        }
                    });
                }
            }
        });

        this.gridManager.refreshData();
    }

    public async viewLogBook(logBookRegister: LogBookRegisterDTO): Promise<void> {
        const title: string = this.translationService.getValue('catches-and-sales.view-log-book-dialog-title');
        let headerAuditBtn: IHeaderAuditButton | undefined;
        let logBookGroup: LogBookGroupsEnum;
        let logBook: LogBookEditDTO | CommercialFishingLogBookEditDTO;

        switch (logBookRegister.typeCode) {
            case LogBookTypesEnum.Ship: {
                logBookGroup = LogBookGroupsEnum.Ship;
                logBook = await this.service.getCommercialFishingLogBook(logBookRegister.id!).toPromise();
            } break;
            case LogBookTypesEnum.FirstSale: {
                logBookGroup = LogBookGroupsEnum.DeclarationsAndDocuments;
            } break;
            case LogBookTypesEnum.Admission: {
                if (logBookRegister.ownerType === LogBookPagePersonTypesEnum.RegisteredBuyer) {
                    logBookGroup = LogBookGroupsEnum.DeclarationsAndDocuments;
                }
                else {
                    logBookGroup = LogBookGroupsEnum.Ship;
                }
            } break;
            case LogBookTypesEnum.Transportation: {
                if (logBookRegister.ownerType === LogBookPagePersonTypesEnum.RegisteredBuyer) {
                    logBookGroup = LogBookGroupsEnum.DeclarationsAndDocuments;
                }
                else {
                    logBookGroup = LogBookGroupsEnum.Ship;
                }
            } break;
            case LogBookTypesEnum.Aquaculture: {
                logBookGroup = LogBookGroupsEnum.Aquaculture;
                logBook = await this.service.getLogBook(logBookRegister.id!).toPromise();
            } break;
        }

        switch (logBookRegister.ownerType) {
            case LogBookPagePersonTypesEnum.Person:
            case LogBookPagePersonTypesEnum.LegalPerson: {
                logBook = await this.service.getCommercialFishingLogBook(logBookRegister.id!).toPromise();
            } break;
            case LogBookPagePersonTypesEnum.RegisteredBuyer: {
                logBook = await this.service.getLogBook(logBookRegister.id!).toPromise();
            } break;
        }

        const data: EditLogBookDialogParamsModel = new EditLogBookDialogParamsModel({
            model: logBook!,
            readOnly: true,
            logBookGroup: logBookGroup!,
            ownerType: logBook!.ownerType,
            pagesRangeError: false,
            isOnline: logBook!.isOnline
        });

        if (!this.isPublicApp) {
            headerAuditBtn = {
                id: logBookRegister.id!,
                getAuditRecordData: this.service.getLogBookAudit.bind(this.service),
                tableName: 'CatchSales.LogBooks'
            }
        }

        const dialog = this.viewLogBookDialog.openWithTwoButtons({
            title: title,
            TCtor: EditLogBookComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditLogBookDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translationService,
            disableDialogClose: true,
            viewMode: true
        }, '1500px');
    }

    public addLogBookPage(logBook: LogBookRegisterDTO): void {
        switch (logBook.typeCode) {
            case LogBookTypesEnum.Ship: {
                this.addShipLogBookPage(logBook);
            } break;
            case LogBookTypesEnum.FirstSale:
            case LogBookTypesEnum.Admission:
            case LogBookTypesEnum.Transportation: {
                this.openAddLogBookPageWizardDialog(logBook);
            } break;
            case LogBookTypesEnum.Aquaculture: {
                this.openAquacultureLogBookPageDialog(logBook);
            } break;
            default: {
                throw new Error(`Invalid log book type: ${LogBookTypesEnum[logBook.typeCode!]}`);
            }
        }
    }

    public editShipLogBookPage(shipPage: ShipLogBookPageRegisterDTO, viewMode: boolean = false): void {
        let title: string = '';
        let headerAuditBtn: IHeaderAuditButton | undefined;
        const data: EditShipLogBookPageDialogParams = new EditShipLogBookPageDialogParams({
            id: shipPage.id,
            model: undefined,
            service: this.service,
            viewMode: viewMode
        });

        if (viewMode === true) {
            title = this.translationService.getValue('catches-and-sales.view-fishing-log-book-page-dialog-title');
        }
        else {
            title = this.translationService.getValue('catches-and-sales.edit-fishing-log-book-page-dialog-title');
        }

        if (!IS_PUBLIC_APP) {
            headerAuditBtn = {
                id: shipPage.id!,
                tableName: 'CatchSales.ShipLogBookPages',
                tooltip: '',
                getAuditRecordData: this.service.getShipLogBookPageSimpleAudit.bind(this.service)
            };
        }

        this.editShipLogBookPageDialog.openWithTwoButtons({
            title: title,
            TCtor: EditShipLogBookPageComponent,
            translteService: this.translationService,
            viewMode: viewMode,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditShipLogBookPageDialogBtnClicked.bind(this)
            },
            componentData: data,
            disableDialogClose: !viewMode
        }, '1500px').subscribe({
            next: (result: ShipLogBookPageEditDTO | undefined) => {
                if (result !== undefined) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    public editAdmissionLogBookPage(logBookPage: AdmissionLogBookPageRegisterDTO, viewMode: boolean = false): void {
        if (logBookPage.status === LogBookPageStatusesEnum.Missing) {
            this.openAddLogBookPageWizard(LogBookTypesEnum.Admission, logBookPage.logBookTypeId!, logBookPage.logBookId!, logBookPage.pageNumber);
        }
        else {
            this.openEditAdmissionLogBookPageDialog(logBookPage, viewMode);
        }
    }

    public editTransportationLogBookPage(logBookPage: TransportationLogBookPageRegisterDTO, viewMode: boolean = false): void {
        if (logBookPage.status === LogBookPageStatusesEnum.Missing) {
            this.openAddLogBookPageWizard(LogBookTypesEnum.Transportation, logBookPage.logBookTypeId!, logBookPage.logBookId!, logBookPage.pageNumber);
        }
        else {
            const data: CatchesAndSalesDialogParamsModel = new CatchesAndSalesDialogParamsModel({
                id: logBookPage.id,
                logBookId: logBookPage.logBookId,
                logBookTypeId: logBookPage.logBookTypeId,
                pageNumber: logBookPage.pageNumber,
                pageStatus: logBookPage.status,
                service: this.service,
                viewMode: viewMode,
                canEditCommonDataPermission: this.canEditCommonDataTransportationLogBookRecords
            });

            this.openEditTransportationLogBookPageDialog(data, viewMode);
        }
    }

    public editFirstSaleLogBookPage(logBookPage: FirstSaleLogBookPageRegisterDTO, viewMode: boolean = false): void {
        if (logBookPage.status === LogBookPageStatusesEnum.Missing) {
            this.openAddLogBookPageWizard(LogBookTypesEnum.FirstSale, logBookPage.logBookTypeId!, logBookPage.logBookId!, logBookPage.pageNumber);
        }
        else {
            this.openEditFirstSaleLogBookPageDialog(logBookPage, viewMode);
        }
    }

    public editAquacultureLogBookPage(logBookPage: AquacultureLogBookPageRegisterDTO, viewMode: boolean = false): void {
        let title: string = '';
        let headerAuditBtn: IHeaderAuditButton | undefined;
        const data: CatchesAndSalesDialogParamsModel = new CatchesAndSalesDialogParamsModel({
            id: logBookPage.id,
            logBookId: logBookPage.logBookId,
            logBookTypeId: logBookPage.logBookTypeId,
            service: this.service,
            viewMode: viewMode
        });

        if (viewMode === true) {
            title = this.translationService.getValue('catches-and-sales.view-aquaculture-log-book-page-dialog-title');
        }
        else {
            title = this.translationService.getValue('catches-and-sales.edit-aquaculture-log-book-page-dialog-title');
        }

        if (!IS_PUBLIC_APP) {
            headerAuditBtn = {
                id: logBookPage.id!,
                tableName: 'CatchSales.AquacultureLogBookPages',
                tooltip: '',
                getAuditRecordData: this.service.getAquacultureLogBookPageSimpleAudit.bind(this.service)
            };
        }

        this.editAquacultureLogBookPageDialog.openWithTwoButtons({
            title: title,
            TCtor: EditAquacultureLogBookPageComponent,
            translteService: this.translationService,
            viewMode: viewMode,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditAquacultureLogBookPageDialogBtnClicked.bind(this)
            },
            componentData: data,
            disableDialogClose: !viewMode
        }, '1500px').subscribe({
            next: (result: AquacultureLogBookPageEditDTO | undefined) => {
                if (result !== undefined) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    public onAnnulShipLogBookPageBtnClicked(logBookPage: ShipLogBookPageRegisterDTO, viewMode: boolean = false): void {
        const title: string = this.translationService.getValue('catches-and-sales.ship-page-cancellation-dialog-title');
        const reasonControlLabel: string = this.translationService.getValue('catches-and-sales.ship-page-cancellation-reason-label');
        const model: LogBookPageCancellationReasonDTO = new LogBookPageCancellationReasonDTO({
            logBookId: logBookPage.logBookId,
            logBookPageNumber: logBookPage.pageNumber,
            reason: logBookPage.cancellationReason
        });

        this.openJustifiedCancellationReasonDialog(title, model, LogBookTypesEnum.Ship, reasonControlLabel, viewMode);
    }

    public onRestoreAnnulledShipLogBookPageBtnClicked(logBookPage: ShipLogBookPageRegisterDTO): void {
        const messageTranslate: string = this.translationService.getValue('catches-and-sales.ship-page-restore-annulled-page-confirm-dialog-message');
        const messageText: string = `${messageTranslate}: ${logBookPage.pageNumber}`;

        this.confirmationDialog.open({
            title: this.translationService.getValue('catches-and-sales.ship-page-restore-annulled-page-confirm-dialog-title'),
            message: messageText,
            cancelBtnLabel: this.translationService.getValue('catches-and-sales.ship-page-restore-annulled-page-confirm-dialog-cancel'),
            okBtnLabel: this.translationService.getValue('catches-and-sales.ship-page-restore-annulled-page-confirm-dialog-ok')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.restoreAnnulledShipLogBookPage(logBookPage.id!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        }
                    });
                }
            }
        });
    }

    public onAnnulAdmissionLogBookPageBtnClicked(logBookPage: AdmissionLogBookPageRegisterDTO, viewMode: boolean = false): void {
        const title: string = this.translationService.getValue('catches-and-sales.admission-page-cancellation-dialog-title');
        const reasonControlLabel: string = this.translationService.getValue('catches-and-sales.admission-page-cancellation-reason-label');
        const model: LogBookPageCancellationReasonDTO = new LogBookPageCancellationReasonDTO({
            logBookId: logBookPage.logBookId,
            logBookPageNumber: logBookPage.pageNumber!.toString(),
            reason: logBookPage.cancellationReason
        });

        this.openJustifiedCancellationReasonDialog(title, model, LogBookTypesEnum.Admission, reasonControlLabel, viewMode);
    }

    public onRestoreAnnulledAdmissionLogBookPageBtnClicked(logBookPage: AdmissionLogBookPageRegisterDTO): void {
        const messageTranslate: string = this.translationService.getValue('catches-and-sales.admission-page-restore-annulled-page-confirm-dialog-message');
        const messageText: string = `${messageTranslate}: ${logBookPage.pageNumber}`;

        this.confirmationDialog.open({
            title: this.translationService.getValue('catches-and-sales.admission-page-restore-annulled-page-confirm-dialog-title'),
            message: messageText,
            cancelBtnLabel: this.translationService.getValue('catches-and-sales.admission-page-restore-annulled-page-confirm-dialog-cancel'),
            okBtnLabel: this.translationService.getValue('catches-and-sales.admission-page-restore-annulled-page-confirm-dialog-ok')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.restoreAnnulledAdmissionLogBookPage(logBookPage.id!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        }
                    });
                }
            }
        });
    }

    public onAnnulTransportationLogBookPageBtnClicked(logBookPage: TransportationLogBookPageRegisterDTO, viewMode: boolean = false): void {
        const title: string = this.translationService.getValue('catches-and-sales.transportation-page-cancellation-dialog-title');
        const reasonControlLabel: string = this.translationService.getValue('catches-and-sales.transportation-page-cancellation-reason-label');
        const model: LogBookPageCancellationReasonDTO = new LogBookPageCancellationReasonDTO({
            logBookId: logBookPage.logBookId,
            logBookPageNumber: logBookPage.pageNumber!.toString(),
            reason: logBookPage.cancellationReason
        });

        this.openJustifiedCancellationReasonDialog(title, model, LogBookTypesEnum.Transportation, reasonControlLabel, viewMode);
    }

    public onRestoreAnnulledTransportationLogBookPageBtnClicked(logBookPage: TransportationLogBookPageRegisterDTO): void {
        const messageTranslate: string = this.translationService.getValue('catches-and-sales.transportation-page-restore-annulled-page-confirm-dialog-message');
        const messageText: string = `${messageTranslate}: ${logBookPage.pageNumber}`;

        this.confirmationDialog.open({
            title: this.translationService.getValue('catches-and-sales.transportation-page-restore-annulled-page-confirm-dialog-title'),
            message: messageText,
            cancelBtnLabel: this.translationService.getValue('catches-and-sales.transportation-page-restore-annulled-page-confirm-dialog-cancel'),
            okBtnLabel: this.translationService.getValue('catches-and-sales.transportation-page-restore-annulled-page-confirm-dialog-ok')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.restoreAnnulledTransportationLogBookPage(logBookPage.id!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        }
                    });
                }
            }
        });
    }

    public onAnnulFirstSaleLogBookPageBtnClicked(logBookPage: FirstSaleLogBookPageRegisterDTO, viewMode: boolean = false): void {
        const title: string = this.translationService.getValue('catches-and-sales.first-sale-page-cancellation-dialog-title');
        const reasonControlLabel: string = this.translationService.getValue('catches-and-sales.first-sale-page-cancellation-reason-label');
        const model: LogBookPageCancellationReasonDTO = new LogBookPageCancellationReasonDTO({
            logBookId: logBookPage.logBookId,
            logBookPageNumber: logBookPage.pageNumber!.toString(),
            reason: logBookPage.cancellationReason
        });

        this.openJustifiedCancellationReasonDialog(title, model, LogBookTypesEnum.FirstSale, reasonControlLabel, viewMode);
    }

    public onRestoreAnnulledFirstSaleLogBookPageBtnClicked(logBookPage: FirstSaleLogBookPageRegisterDTO): void {
        const messageTranslate: string = this.translationService.getValue('catches-and-sales.first-sale-page-restore-annulled-page-confirm-dialog-message');
        const messageText: string = `${messageTranslate}: ${logBookPage.pageNumber}`;

        this.confirmationDialog.open({
            title: this.translationService.getValue('catches-and-sales.first-sale-page-restore-annulled-page-confirm-dialog-title'),
            message: messageText,
            cancelBtnLabel: this.translationService.getValue('catches-and-sales.first-sale-page-restore-annulled-page-confirm-dialog-cancel'),
            okBtnLabel: this.translationService.getValue('catches-and-sales.first-sale-page-restore-annulled-page-confirm-dialog-ok')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.restoreAnnulledFirstSaleLogBookPage(logBookPage.id!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        }
                    });
                }
            }
        });
    }

    public onAnnulAquacultureLogBookPageBtnClicked(logBookPage: AquacultureLogBookPageRegisterDTO, viewMode: boolean = false): void {
        const title: string = this.translationService.getValue('catches-and-sales.aquaculture-page-cancellation-dialog-title');
        const reasonControlLabel: string = this.translationService.getValue('catches-and-sales.aquaculture-page-cancellation-reason-label');
        const model: LogBookPageCancellationReasonDTO = new LogBookPageCancellationReasonDTO({
            logBookId: logBookPage.logBookId,
            logBookPageNumber: logBookPage.pageNumber!.toString(),
            reason: logBookPage.cancellationReason
        });

        this.openJustifiedCancellationReasonDialog(title, model, LogBookTypesEnum.Aquaculture, reasonControlLabel, viewMode);
    }

    public onRestoreAnnulledAquacultureLogBookPageBtnClicked(logBookPage: AquacultureLogBookPageRegisterDTO): void {
        const messageTranslate: string = this.translationService.getValue('catches-and-sales.aquaculture-page-restore-annulled-page-confirm-dialog-message');
        const messageText: string = `${messageTranslate}: ${logBookPage.pageNumber}`;

        this.confirmationDialog.open({
            title: this.translationService.getValue('catches-and-sales.aquaculture-page-restore-annulled-page-confirm-dialog-title'),
            message: messageText,
            cancelBtnLabel: this.translationService.getValue('catches-and-sales.aquaculture-page-restore-annulled-page-confirm-dialog-cancel'),
            okBtnLabel: this.translationService.getValue('catches-and-sales.aquaculture-page-restore-annulled-page-confirm-dialog-ok')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.restoreAnnulledAquacultureLogBookPage(logBookPage.id!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        }
                    });
                }
            }
        });
    }

    public onAddAdmissionDeclarationBtnClicked(shipLogBookPage: ShipLogBookPageRegisterDTO): void {
        const title: string = this.translationService.getValue('catches-and-sales.add-ship-admission-document-title');
        this.openAddShipPageDocumentWizardDialog(
            shipLogBookPage.id!,
            title,
            LogBookPageDocumentTypesEnum.AdmissionDocument,
            LogBookTypesEnum.Ship,
            undefined);
    }

    public onAddTransportationDocumentBtnClicked(shipLogBookPage: ShipLogBookPageRegisterDTO): void {
        const title: string = this.translationService.getValue('catches-and-sales.add-ship-transportation-document-title');
        this.openAddShipPageDocumentWizardDialog(
            shipLogBookPage.id!,
            title,
            LogBookPageDocumentTypesEnum.TransportationDocument,
            LogBookTypesEnum.Ship,
            undefined);
    }

    public onAddFirstSaleDocumentBtnClicked(shipLogBookPage: ShipLogBookPageRegisterDTO): void {
        const title: string = this.translationService.getValue('catches-and-sales.add-ship-first-sale-document-title');
        this.openAddShipPageDocumentWizardDialog(shipLogBookPage.id!, title, LogBookPageDocumentTypesEnum.FirstSaleDocument, LogBookTypesEnum.Ship, undefined);
    }

    public onAddRelatedDeclarationBtnClicked(shipLogBookPage: ShipLogBookPageEditDTO): void {
        this.addRelatedDeclarationDialog.openWithTwoButtons({
            title: this.translationService.getValue('catches-and-sales.add-related-declaration-title'),
            TCtor: AddRelatedDeclarationComponent,
            translteService: this.translationService,
            viewMode: false,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => {
                    closeFn();
                }
            },
            componentData: new AddRelatedDeclarationDialogParams(this.service, shipLogBookPage),
            disableDialogClose: false
        }, '500px').subscribe({
            next: (success: boolean | undefined) => {
                if (success === true) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    public onAddAdmissionDocumentFromTransportationBtnClicked(transportationLogBookPage: TransportationLogBookPageRegisterDTO): void {
        const title: string = this.translationService.getValue('catches-and-sales.add-transportation-admission-document-title');
        this.openAddShipPageDocumentWizardDialog(
            transportationLogBookPage.id!,
            title,
            LogBookPageDocumentTypesEnum.AdmissionDocument,
            LogBookTypesEnum.Transportation,
            transportationLogBookPage.pageNumber);
    }

    public onAddFirstSaleDocumentFromTransportationBtnClicked(transportationLogBookPage: TransportationLogBookPageRegisterDTO): void {
        const title: string = this.translationService.getValue('catches-and-sales.add-transportation-first-sale-document-title');
        this.openAddShipPageDocumentWizardDialog(
            transportationLogBookPage.id!,
            title,
            LogBookPageDocumentTypesEnum.FirstSaleDocument,
            LogBookTypesEnum.Transportation,
            transportationLogBookPage.pageNumber);
    }

    public onAddFirstSaleDocumentFromAdmissionBtnClicked(admissionLogBookPage: AdmissionLogBookPageRegisterDTO): void {
        const title: string = this.translationService.getValue('catches-and-sales.add-admission-first-sale-document-title');
        this.openAddShipPageDocumentWizardDialog(
            admissionLogBookPage.id!,
            title,
            LogBookPageDocumentTypesEnum.FirstSaleDocument,
            LogBookTypesEnum.Admission,
            admissionLogBookPage.pageNumber);
    }

    public onEditShipLogBookPageNumber(shipLogBookPage: ShipLogBookPageRegisterDTO): void {
        const title: string = this.translationService.getValue('catches-and-sales.edit-ship-log-book-page-number-title');
        const saveBtnLabel: string = this.translationService.getValue('catches-and-sales.edit-page-number-save-btn');

        const componentData: AddShipWizardDialogParams = new AddShipWizardDialogParams({
            logBookId: shipLogBookPage.logBookId,
            service: this.service,
            pageNumber: shipLogBookPage.pageNumber,
            pageId: shipLogBookPage.id,
            isEdit: true
        });

        this.openAddShipLogBookPageDialog(componentData, title, saveBtnLabel);
    }

    public onEditAdmissionLogBookPageNumber(admissionLogBookPage: AdmissionLogBookPageRegisterDTO): void {
        const title: string = this.translationService.getValue('catches-and-sales.edit-admission-log-book-page-number-title');

        const componentData: EditPageNumberDilogParamsModel = new EditPageNumberDilogParamsModel({
            pageId: admissionLogBookPage.id,
            pageNumber: admissionLogBookPage.pageNumber,
            logBookType: LogBookTypesEnum.Admission,
            logBookId: admissionLogBookPage.logBookId,
            service: this.service,
            ships: this.ships
        });

        this.openEditPageNumberDialog(title, componentData);
    }

    public onEditFirstSaleLogBookPageNumber(firstSaleLogBookPage: FirstSaleLogBookPageRegisterDTO): void {
        const title: string = this.translationService.getValue('catches-and-sales.edit-first-sale-log-book-page-number-title');

        const componentData: EditPageNumberDilogParamsModel = new EditPageNumberDilogParamsModel({
            pageId: firstSaleLogBookPage.id,
            pageNumber: firstSaleLogBookPage.pageNumber,
            logBookType: LogBookTypesEnum.FirstSale,
            logBookId: firstSaleLogBookPage.logBookId,
            service: this.service,
            ships: this.ships
        });

        this.openEditPageNumberDialog(title, componentData);
    }

    public onEditTransportationLogBookPageNumber(transportationLogBookPage: TransportationLogBookPageRegisterDTO): void {
        const title: string = this.translationService.getValue('catches-and-sales.edit-transportation-log-book-page-number-title');

        const componentData: EditPageNumberDilogParamsModel = new EditPageNumberDilogParamsModel({
            pageId: transportationLogBookPage.id,
            pageNumber: transportationLogBookPage.pageNumber,
            logBookType: LogBookTypesEnum.Transportation,
            logBookId: transportationLogBookPage.logBookId,
            service: this.service,
            ships: this.ships
        });

        this.openEditPageNumberDialog(title, componentData);
    }

    public onEditAquacultureLogBookPageNumber(aquacultureLogBookPage: AquacultureLogBookPageRegisterDTO): void {
        const title: string = this.translationService.getValue('catches-and-sales.edit-aquaculture-log-book-page-number-title');

        const componentData: EditPageNumberDilogParamsModel = new EditPageNumberDilogParamsModel({
            pageId: aquacultureLogBookPage.id,
            pageNumber: aquacultureLogBookPage.pageNumber,
            logBookType: LogBookTypesEnum.Aquaculture,
            logBookId: aquacultureLogBookPage.logBookId,
            service: this.service
        });

        this.openEditPageNumberDialog(title, componentData);
    }

    public onAddEditShipLogBookPageFiles(shipLogBookPage: ShipLogBookPageRegisterDTO): void {
        const title: string = this.translationService.getValue('catches-and-sales.edit-ship-log-book-page-files-title');

        const componentData: LogBookPageFilesDialogParamsModel = new LogBookPageFilesDialogParamsModel({
            logBookPageId: shipLogBookPage.id,
            pageCode: PageCodeEnum.ShipLogBookPage,
            logBookType: LogBookTypesEnum.Ship,
            service: this.service
        });

        this.openEditPageFlesDialog(title, componentData);
    }

    public onAddEditAdmissionLogBookPageFiles(admissionLogBookPage: AdmissionLogBookPageRegisterDTO): void {
        const title: string = this.translationService.getValue('catches-and-sales.edit-admission-log-book-page-files-title');

        const componentData: LogBookPageFilesDialogParamsModel = new LogBookPageFilesDialogParamsModel({
            logBookPageId: admissionLogBookPage.id,
            pageCode: PageCodeEnum.AdmissionLogBookPage,
            logBookType: LogBookTypesEnum.Admission,
            service: this.service
        });

        this.openEditPageFlesDialog(title, componentData);
    }

    public onAddEditFirstSaleLogBookPageFiles(firstSaleLogBookPage: FirstSaleLogBookPageRegisterDTO): void {
        const title: string = this.translationService.getValue('catches-and-sales.edit-first-sale-log-book-page-files-title');

        const componentData: LogBookPageFilesDialogParamsModel = new LogBookPageFilesDialogParamsModel({
            logBookPageId: firstSaleLogBookPage.id,
            pageCode: PageCodeEnum.FirstSaleLogBookPage,
            logBookType: LogBookTypesEnum.FirstSale,
            service: this.service
        });

        this.openEditPageFlesDialog(title, componentData);
    }

    public onAddEditTransportationLogBookPageFiles(transportationLogBookPage: TransportationLogBookPageRegisterDTO): void {
        const title: string = this.translationService.getValue('catches-and-sales.edit-transportation-log-book-page-files-title');

        const componentData: LogBookPageFilesDialogParamsModel = new LogBookPageFilesDialogParamsModel({
            logBookPageId: transportationLogBookPage.id,
            pageCode: PageCodeEnum.TransportationLogBookPage,
            logBookType: LogBookTypesEnum.Transportation,
            service: this.service
        });

        this.openEditPageFlesDialog(title, componentData);
    }

    public onAddEditAquacultureLogBookPageFiles(aquacultureLogBookPage: AquacultureLogBookPageRegisterDTO): void {
        const title: string = this.translationService.getValue('catches-and-sales.edit-aquaculture-log-book-page-files-title');

        const componentData: LogBookPageFilesDialogParamsModel = new LogBookPageFilesDialogParamsModel({
            logBookPageId: aquacultureLogBookPage.id,
            pageCode: PageCodeEnum.AquacultureLogBookPage,
            logBookType: LogBookTypesEnum.Aquaculture,
            service: this.service
        });

        this.openEditPageFlesDialog(title, componentData);
    }

    private openAddShipPageDocumentWizardDialog(logBookPageId: number, title: string, selectedDocumentType: LogBookPageDocumentTypesEnum, logBookType: LogBookTypesEnum, logBookPageNum: number | undefined): void {
        this.addShipPageDocumentWizardDialog.open({
            title: title,
            TCtor: AddShipPageDocumentWizardComponent,
            translteService: this.translationService,
            viewMode: false,
            headerCancelButton: {
                cancelBtnClicked: this.closeAddLogBookPageDialogBtnClicked.bind(this)
            },
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translationService.getValue('catches-and-sales.add-ship-page-document-wizard-confirm-and-go-to-add-dialog')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translationService.getValue('common.cancel'),
            },
            componentData: new AddShipPageDocumentDialogParamsModel({
                service: this.service,
                documentType: selectedDocumentType,
                logBookPageId: logBookPageId,
                logBookType: logBookType,
                documentNumber: logBookPageNum
            }),
            disableDialogClose: true
        }, '1500px').subscribe({
            next: (result: unknown | undefined) => { // TODO types
                if (result !== undefined) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    private addShipLogBookPage(shipLogBook: LogBookRegisterDTO): void {
        const selectedPageNumber: string | undefined = this.searchpanel.appliedFilters.find(x => CommonUtils.getFormControlName(x.control) === PAGE_NUMBER_CONTROL_NAME)?.value;
        const title: string = this.translationService.getValue('catches-and-sales.add-ship-log-book-page-wizard-dialog-title');
        const saveBtnLabel: string = this.translationService.getValue('catches-and-sales.add-log-book-page-wizard-confirm-and-go-to-add-dialog');

        const componentData: AddShipWizardDialogParams = new AddShipWizardDialogParams({
            logBookId: shipLogBook.id,
            service: this.service,
            pageNumber: selectedPageNumber
        });

        this.openAddShipLogBookPageDialog(componentData, title, saveBtnLabel);
    }

    private openAddShipLogBookPageDialog(componentData: AddShipWizardDialogParams, title: string, saveBtnLabel: string): void {
        this.addShipLogBookPageWizardDialog.open({
            title: title,
            TCtor: AddShipPageWizardComponent,
            translteService: this.translationService,
            headerCancelButton: {
                cancelBtnClicked: this.closeAddShipLogBookPageDialogBtnClicked.bind(this)
            },
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: saveBtnLabel
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translationService.getValue('common.cancel'),
            },
            componentData: componentData,
            disableDialogClose: true,
            viewMode: false
        }, '1500px').subscribe({
            next: (page: ShipLogBookPageEditDTO | undefined) => {
                if (page !== undefined && page !== null) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    private openAddLogBookPageWizardDialog(logBook: LogBookRegisterDTO): void {
        let selectedPageNumber: number | undefined = Number(this.searchpanel.appliedFilters.find(x => CommonUtils.getFormControlName(x.control) === PAGE_NUMBER_CONTROL_NAME)?.value);

        if (isNaN(selectedPageNumber)) {
            selectedPageNumber = undefined;
        }

        this.openAddLogBookPageWizard(logBook.typeCode!, logBook.typeId!, logBook.id!, selectedPageNumber);
    }

    private openAddLogBookPageWizard(logBookType: LogBookTypesEnum, logBookTypeId: number, logBookId: number, pageNumber: number | undefined): void {
        let title: string = '';
        switch (logBookType) {
            case LogBookTypesEnum.FirstSale: {
                title = this.translationService.getValue('catches-and-sales.add-first-sale-log-book-page-wizard-dialog-title');
            } break;
            case LogBookTypesEnum.Admission: {
                title = this.translationService.getValue('catches-and-sales.add-admission-log-book-page-wizard-dialog-title');
            } break;
            case LogBookTypesEnum.Transportation: {
                title = this.translationService.getValue('catches-and-sales.add-transportation-log-book-page-wizard-dialog-title');
            }
        }

        this.addLogBookPageWizardDialog.open({
            title: title,
            TCtor: AddLogBookPageWizardComponent,
            translteService: this.translationService,
            viewMode: false,
            headerCancelButton: {
                cancelBtnClicked: this.closeAddLogBookPageDialogBtnClicked.bind(this)
            },
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translationService.getValue('catches-and-sales.add-log-book-page-wizard-confirm-and-go-to-add-dialog')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translationService.getValue('common.cancel'),
            },
            componentData: new AddLogBookPageDialogParams({
                service: this.service,
                logBookType: logBookType,
                logBookId: logBookId,
                logBookTypeId: logBookTypeId,
                pageNumber: pageNumber,
                pageStatus: pageNumber !== undefined && pageNumber !== null ? LogBookPageStatusesEnum.Missing : undefined
            }),
            disableDialogClose: true
        }, '1500px').subscribe({
            next: (result: unknown | undefined) => { // TODO types
                if (result !== undefined) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    private openAquacultureLogBookPageDialog(aquacultureLogBook: LogBookRegisterDTO): void {
        let selectedPageNumber: number | undefined = Number(this.searchpanel.appliedFilters.find(x => CommonUtils.getFormControlName(x.control) === PAGE_NUMBER_CONTROL_NAME)?.value);

        if (isNaN(selectedPageNumber)) {
            selectedPageNumber = undefined;
        }

        this.editAquacultureLogBookPageDialog.openWithTwoButtons({
            title: this.translationService.getValue('catches-and-sales.add-aquaculture-log-book-page-dialog-title'),
            TCtor: EditAquacultureLogBookPageComponent,
            translteService: this.translationService,
            viewMode: false,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditAquacultureLogBookPageDialogBtnClicked.bind(this)
            },
            componentData: new CatchesAndSalesDialogParamsModel({
                logBookId: aquacultureLogBook.id,
                logBookTypeId: aquacultureLogBook.typeId,
                service: this.service,
                pageNumber: selectedPageNumber,
                viewMode: false
            }),
            disableDialogClose: true
        }, '1500px').subscribe({
            next: (result: AquacultureLogBookPageEditDTO | undefined) => {
                if (result !== undefined) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    private openEditTransportationLogBookPageDialog(data: CatchesAndSalesDialogParamsModel, viewMode: boolean): void {
        let title: string = '';
        let headerAuditBtn: IHeaderAuditButton | undefined;

        if (data.id === null || data.id === undefined) {
            title = this.translationService.getValue('catches-and-sales.add-transportation-log-book-page-dialog-title');
        }
        else if (viewMode === true) {
            title = this.translationService.getValue('catches-and-sales.view-transportation-log-book-page-dialog-title');
        }
        else {
            title = this.translationService.getValue('catches-and-sales.edit-transportation-log-book-page-dialog-title');
        }

        if (!IS_PUBLIC_APP && data.id !== null && data.id !== undefined) {
            headerAuditBtn = {
                id: data.id!,
                tableName: 'CatchSales.TransportationLogBookPages',
                tooltip: '',
                getAuditRecordData: this.service.getTransportationLogBookPageSimpleAudit.bind(this.service)
            };
        }

        this.editTransportationLogBookPageDialog.openWithTwoButtons({
            title: title,
            TCtor: EditTransportationLogBookPageComponent,
            translteService: this.translationService,
            viewMode: viewMode,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditTransportationLogBookPageDialogBtnClicked.bind(this)
            },
            componentData: data,
            disableDialogClose: !viewMode
        }, '1500px').subscribe({
            next: (result: TransportationLogBookPageEditDTO | undefined) => {
                if (result !== undefined) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    private openEditAdmissionLogBookPageDialog(logBookPage: AdmissionLogBookPageRegisterDTO, viewMode: boolean): void {
        let title: string = '';
        let headerAuditBtn: IHeaderAuditButton | undefined;
        const data: CatchesAndSalesDialogParamsModel = new CatchesAndSalesDialogParamsModel({
            id: logBookPage.id,
            logBookId: logBookPage.logBookId,
            logBookTypeId: logBookPage.logBookTypeId,
            pageNumber: logBookPage.pageNumber,
            pageStatus: logBookPage.status,
            service: this.service,
            viewMode: viewMode,
            canEditCommonDataPermission: this.canEditCommonDataAdmissionLogBookRecords
        });

        if (viewMode === true) {
            title = this.translationService.getValue('catches-and-sales.view-admission-log-book-page-dialog-title');
        }
        else {
            title = this.translationService.getValue('catches-and-sales.edit-admission-log-book-page-dialog-title');
        }

        if (!IS_PUBLIC_APP) {
            headerAuditBtn = {
                id: logBookPage.id!,
                tableName: 'CatchSales.AdmissionLogBookPages',
                tooltip: '',
                getAuditRecordData: this.service.getAdmissionLogBookPageSimpleAudit.bind(this.service)
            };
        }

        this.editAdmissionLogBookPageDialog.openWithTwoButtons({
            title: title,
            TCtor: EditAdmissionLogBookPageComponent,
            translteService: this.translationService,
            viewMode: viewMode,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditAdmissionLogBookPageDialogBtnClicked.bind(this)
            },
            componentData: data,
            disableDialogClose: !viewMode
        }, '1500px').subscribe({
            next: (result: AdmissionLogBookPageEditDTO | undefined) => {
                if (result !== undefined) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    private openEditFirstSaleLogBookPageDialog(logBookPage: FirstSaleLogBookPageRegisterDTO, viewMode: boolean): void {
        let title: string = '';
        let headerAuditBtn: IHeaderAuditButton | undefined;
        const data: CatchesAndSalesDialogParamsModel = new CatchesAndSalesDialogParamsModel({
            id: logBookPage.id,
            logBookId: logBookPage.logBookId,
            logBookTypeId: logBookPage.logBookTypeId,
            pageNumber: logBookPage.pageNumber,
            pageStatus: logBookPage.status,
            service: this.service,
            viewMode: viewMode,
            canEditCommonDataPermission: this.canEditCommonDataFirstSaleLogBookRecords
        });

        if (viewMode === true) {
            title = this.translationService.getValue('catches-and-sales.view-first-sale-log-book-page-dialog-title');
        }
        else {
            title = this.translationService.getValue('catches-and-sales.edit-first-sale-log-book-page-dialog-title');
        }

        if (!IS_PUBLIC_APP) {
            headerAuditBtn = {
                id: logBookPage.id!,
                tableName: 'CatchSales.FirstSaleLogBookPages',
                tooltip: '',
                getAuditRecordData: this.service.getFirstSaleLogBookPageSimpleAudit.bind(this.service)
            };
        }

        this.editFirstSaleLogBookPageDialog.openWithTwoButtons({
            title: title,
            TCtor: EditFirstSaleLogBookPageComponent,
            translteService: this.translationService,
            viewMode: viewMode,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditFirstSaleLogBookPageDialogBtnClicked.bind(this)
            },
            componentData: data,
            disableDialogClose: !viewMode
        }, '1500px').subscribe({
            next: (result: FirstSaleLogBookPageEditDTO | undefined) => {
                if (result !== undefined) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    private openJustifiedCancellationReasonDialog(
        title: string,
        model: LogBookPageCancellationReasonDTO | undefined,
        logBookType: LogBookTypesEnum,
        reasonControlLabel: string | undefined,
        viewMode: boolean = false
    ): void {
        this.cancellationDialog.openWithTwoButtons({
            title: title,
            TCtor: JustifiedCancellationComponent,
            translteService: this.translationService,
            viewMode: viewMode,
            headerCancelButton: {
                cancelBtnClicked: this.closeJustifiedCancellationDialogBtnClicked.bind(this)
            },
            componentData: new JustifiedCancellationDialogParams({
                model: model,
                logBookType: logBookType,
                reasonControlLabel: reasonControlLabel,
                cancellationServiceMethod: this.service.annulLogBookPage.bind(this.service)
            })
        }, '800px').subscribe({
            next: (reasonData: LogBookPageCancellationReasonDTO) => {
                if (!CommonUtils.isNullOrEmpty(reasonData?.reason)) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    private openEditPageNumberDialog(title: string, data: EditPageNumberDilogParamsModel): void {
        this.editPageNumberDialog.openWithTwoButtons({
            title: title,
            TCtor: EditLogBookPageNumberComponent,
            translteService: this.translationService,
            viewMode: false,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditLogPageNumberBookDialogBtnClicked.bind(this)
            },
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translationService.getValue('catches-and-sales.edit-page-number-save-btn')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translationService.getValue('common.cancel')
            },
            componentData: data,
            disableDialogClose: true
        }, '1500px').subscribe({
            next: (pageNumber: number) => {
                if (pageNumber !== undefined && pageNumber !== null) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    private openEditPageFlesDialog(title: string, data: LogBookPageFilesDialogParamsModel): void {
        this.editPageFilesDialog.openWithTwoButtons({
            title: title,
            TCtor: LogBookPageFilesComponent,
            translteService: this.translationService,
            viewMode: false,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditLogPageNumberBookDialogBtnClicked.bind(this)
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translationService.getValue('common.cancel')
            },
            componentData: data,
            disableDialogClose: true
        }, '1200px').subscribe({
            next: (result: LogBookPageFilesDTO | undefined) => {
                if (result !== undefined && result !== null) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    private buildForm(): void {
        this.formGroup = new FormGroup({
            logBookNumberControl: new FormControl(),
            documentNumberControl: new FormControl(null, TLValidators.number(0)),
            showExistingPagesControl: new FormControl(),
            logBookTypeControl: new FormControl(),
            logBookStatusesControl: new FormControl(),
            logBookValidityRangeControl: new FormControl(),
            onlinePageNumberControl: new FormControl()
        });

        this.formGroup.addControl(PAGE_NUMBER_CONTROL_NAME, new FormControl(undefined, TLValidators.number(0)));

        if (!this.isPublicApp) {
            this.formGroup.addControl('shipControl', new FormControl());
            this.formGroup.addControl('aquacultureFacilityControl', new FormControl());
            this.formGroup.addControl('aquacultureHolderNameControl', new FormControl());
            this.formGroup.addControl('registeredBuyerControl', new FormControl());
            this.formGroup.addControl('ownerEgnEikControl', new FormControl());
        }
    }

    private mapFilters(filters: FilterEventArgs): CatchesAndSalesAdministrationFilters | CatchesAndSalesPublicFilters {
        let result: CatchesAndSalesAdministrationFilters | CatchesAndSalesPublicFilters;

        if (this.isPublicApp === true) {
            result = new CatchesAndSalesPublicFilters({
                freeTextSearch: filters.searchText,
                showInactiveRecords: filters.showInactiveRecords,

                pageNumber: filters.getValue(PAGE_NUMBER_CONTROL_NAME),
                logBookNumber: filters.getValue('logBookNumberControl'),
                onlinePageNumber: filters.getValue('onlinePageNumberControl'),
                logBookTypeId: filters.getValue('logBookTypeControl'),
                logBookStatusIds: filters.getValue('logBookStatusesControl'),
                documentNumber: filters.getValue('documentNumberControl'),
                logBookValidityStartDate: filters.getValue<DateRangeData>('logBookValidityRangeControl')?.start,
                logBookValidityEndDate: filters.getValue<DateRangeData>('logBookValidityRangeControl')?.end
            });
        }
        else {
            result = new CatchesAndSalesAdministrationFilters({
                freeTextSearch: filters.searchText,
                showInactiveRecords: filters.showInactiveRecords,

                pageNumber: filters.getValue(PAGE_NUMBER_CONTROL_NAME),
                logBookNumber: filters.getValue('logBookNumberControl'),
                logBookTypeId: filters.getValue('logBookTypeControl'),
                documentNumber: filters.getValue('documentNumberControl'),
                ownerEngEik: filters.getValue('ownerEgnEikControl'),
                logBookStatusIds: filters.getValue('logBookStatusesControl'),
                logBookValidityStartDate: filters.getValue<DateRangeData>('logBookValidityRangeControl')?.start,
                logBookValidityEndDate: filters.getValue<DateRangeData>('logBookValidityRangeControl')?.end,
                onlinePageNumber: filters.getValue('onlinePageNumberControl')
            });

            if (result instanceof CatchesAndSalesAdministrationFilters) {
                if (this.canReadShipLogBookRecords) {
                    result.shipId = filters.getValue('shipControl');
                }

                if (this.canReadAquacultureLogBookRecords) {
                    result.aquacultureId = filters.getValue('aquacultureFacilityControl');
                    result.aquacultureHolderName = filters.getValue('aquacultureHolderNameControl');
                }

                if (this.canReadFirstSaleLogBookRecords || this.canReadAdmissionLogBookRecords || this.canReadTransportationLogBookRecords) {
                    result.registeredBuyerId = filters.getValue('registeredBuyerControl');
                }
            }
        }

        if (result.pageNumber !== undefined && result.pageNumber !== null) {
            result.pageNumber = Number(result.pageNumber);

            if (isNaN(result.pageNumber)) {
                result.pageNumber = undefined;
            }
        }

        if (result.documentNumber !== undefined && result.documentNumber !== null) {
            result.documentNumber = Number(result.documentNumber);

            if (isNaN(result.documentNumber)) {
                result.documentNumber = undefined;
            }
        }

        const showOnlyExistingPages = filters.getValue<ThreeState>('showExistingPagesControl');
        if (showOnlyExistingPages !== undefined && showOnlyExistingPages !== null) {
            switch (showOnlyExistingPages) {
                case 'yes':
                    result.showOnlyExistingPages = true;
                    break;
                case 'no':
                    result.showOnlyExistingPages = false;
                    break;
                case 'both':
                    result.showOnlyExistingPages = undefined;
                    break;
            }
        }

        return result;
    }

    private getMainTableButtons(): Element[] {
        return Array.from(document.querySelectorAll('[data-logbook-id]'));
    }

    private closeAddShipLogBookPageDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeEditShipLogBookPageDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeAddLogBookPageDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeEditFirstSaleLogBookPageDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeEditAdmissionLogBookPageDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeEditTransportationLogBookPageDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeEditAquacultureLogBookPageDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeJustifiedCancellationDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeEditLogBookDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeEditLogPageNumberBookDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}

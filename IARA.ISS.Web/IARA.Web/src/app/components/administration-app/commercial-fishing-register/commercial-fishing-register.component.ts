
import { IRemoteTLDatatableComponent } from '@app/shared/components/data-table/interfaces/tl-remote-datatable.interface';
import { AfterViewInit, Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ICommercialFishingService } from '@app/interfaces/common-app/commercial-fishing.interface';
import { IDeliveryService } from '@app/interfaces/common-app/delivery.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ApplicationDeliveryDTO } from '@app/models/generated/dtos/ApplicationDeliveryDTO';
import { ApplicationForChoiceDTO } from '@app/models/generated/dtos/ApplicationForChoiceDTO';
import { CommercialFishingPermitRegisterDTO } from '@app/models/generated/dtos/CommercialFishingPermitRegisterDTO';
import { CommercialFishingRegisterFilters } from '@app/models/generated/filters/CommercialFishingRegisterFilters';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { RegisterDeliveryDialogParams } from '@app/shared/components/register-delivery/models/register-delivery-dialog-params.model';
import { RegisterDeliveryComponent } from '@app/shared/components/register-delivery/register-delivery.component';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { ChooseApplicationComponent } from '@app/components/common-app/applications/components/choose-application/choose-application.component';
import { ChooseApplicationDialogParams } from '@app/components/common-app/applications/components/choose-application/models/choose-application-dialog-params.model';
import { EditCommercialFishingComponent } from '@app/components/common-app/commercial-fishing/components/edit-commercial-fishing/edit-commercial-fishing.component';
import { CommercialFishingAdministrationService } from '@app/services/administration-app/commercial-fishing-administration.service';
import { DeliveryAdministrationService } from '@app/services/administration-app/delivery-administration.service';
import { CommercialFishingEditDTO } from '@app/models/generated/dtos/CommercialFishingEditDTO';
import { CommercialFishingTypesEnum } from '@app/enums/commercial-fishing-types.enum';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { CommercialFishingPermitLicenseRegisterDTO } from '@app/models/generated/dtos/CommercialFishingPermitLicenseRegisterDTO';
import { FishingGearNomenclatureDTO } from '@app/models/generated/dtos/FishingGearNomenclatureDTO';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { LogbookDTO } from '@app/models/generated/dtos/LogbookDTO';
import { EditLogBookComponent } from '@app/components/common-app/commercial-fishing/components/edit-log-book/edit-log-book.component';
import { LogBookEditDTO } from '@app/models/generated/dtos/LogBookEditDTO';
import { EditLogBookDialogParamsModel } from '@app/components/common-app/commercial-fishing/components/log-books/models/edit-log-book-dialog-params.model';
import { LogBookGroupsEnum } from '@app/enums/log-book-groups.enum';
import { CommercialFishingLogbookRegisterDTO } from '@app/models/generated/dtos/CommercialFishingLogbookRegisterDTO';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum';
import { ChooseLogBookForRenewalComponent } from '@app/components/common-app/commercial-fishing/components/log-books/components/choose-log-book-for-renewal/choose-log-book-for-renewal.component';
import { ChooseLogBookForRenewalDialogParams } from '@app/components/common-app/commercial-fishing/components/log-books/models/choose-log-book-for-renewal-dialog-params.model';
import { LogBookForRenewalDTO } from '@app/models/generated/dtos/LogBookForRenewalDTO';
import { OnActionEndedType, SimpleAuditMethod } from '@app/components/common-app/commercial-fishing/components/log-books/log-books.component';
import { CommercialFishingRegisterCacheService } from './services/commercial-fishing-register-cache.service';
import { SuspensionsComponent } from '@app/components/common-app/commercial-fishing/components/suspensions/suspensions.component';
import { SuspensionsDialogParams } from '@app/components/common-app/commercial-fishing/components/suspensions/models/suspensions-dialog-params.model';
import { SuspensionDataDTO } from '@app/models/generated/dtos/SuspensionDataDTO';

type ThreeState = 'yes' | 'no' | 'both';

@Component({
    selector: 'commercial-fishing-register',
    templateUrl: './commercial-fishing-register.component.html',
    providers: [CommercialFishingRegisterCacheService]
})
export class CommercialFishingRegisterComponent implements OnInit, AfterViewInit, OnChanges {
    @Input()
    public shipId: number | undefined;

    @Input()
    public reloadData: boolean = false;

    public translationService: FuseTranslationLoaderService;
    public formGroup!: FormGroup;
    public logBooksPerPage: number = 5;

    public permitTypes: NomenclatureDTO<number>[] = [];
    public permitLicenseTypes: NomenclatureDTO<number>[] = [];
    public fishingGearTypes: FishingGearNomenclatureDTO[] = [];
    public territoryUnits: NomenclatureDTO<number>[] = [];
    public permitIsSuspendedOptions: NomenclatureDTO<ThreeState>[] = [];
    public permitIsExpiredOptions: NomenclatureDTO<ThreeState>[] = [];
    public permitLicenseIsSuspendedOptions: NomenclatureDTO<ThreeState>[] = [];
    public permitLicenseIsExpiredOptions: NomenclatureDTO<ThreeState>[] = [];

    public readonly disabledLogBookAddButtonsTooltipText: string;
    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    public readonly canReadPermitRecords: boolean;
    public readonly canAddPermitRecords: boolean;
    public readonly canEditPermitRecords: boolean;
    public readonly canRestorePermitRecords: boolean;
    public readonly canDeletePermitRecords: boolean;

    public readonly canReadPermitApplications: boolean;
    public readonly canReadPermitLicenseApplications: boolean;

    public readonly canReadPermitLicenseRecords: boolean;
    public readonly canAddPermitLicenseRecords: boolean;
    public readonly canEditPermitLicenseRecords: boolean;
    public readonly canDeletePermitLicenseRecords: boolean;
    public readonly canRestorePermitLicenseRecords: boolean;

    public readonly hasPermitsReadAllPermission: boolean;
    public readonly hasPermitLicenseReadAllPermission: boolean;

    public readonly canReadLogBooks: boolean;
    public readonly canAddLogBooks: boolean;
    public readonly canEditLogBooks: boolean;
    public readonly canDeleteLogBooks: boolean;
    public readonly canRestoreLogBooks: boolean;

    public readonly canReadPermitSuspensionsRecords: boolean;
    private readonly canAddPermitSuspensionsRecords: boolean;
    private readonly canEditPermitSuspensionsRecords: boolean;
    private readonly canDeletePermitSuspensionsRecords: boolean;
    private readonly canRestorePermitSuspensionsRecords: boolean;

    public readonly canReadPermitLicenseSuspensionsRecords: boolean;
    private readonly canAddPermitLicenseSuspensionsRecords: boolean;
    private readonly canEditPermitLicenseSuspensionsRecords: boolean;
    private readonly canDeletePermitLicenseSuspensionsRecords: boolean;
    private readonly canRestorePermitLicenseSuspensionsRecords: boolean;

    public readonly getLogBookAuditMethod!: SimpleAuditMethod;
    public readonly logBookGroup: LogBookGroupsEnum = LogBookGroupsEnum.Ship;
    public readonly logBookPagePersonTypesEnum: typeof LogBookPagePersonTypesEnum = LogBookPagePersonTypesEnum;
    public readonly cacheService: CommercialFishingRegisterCacheService;

    @ViewChild(SearchPanelComponent)
    private searchpanel: SearchPanelComponent | undefined;

    @ViewChild(TLDataTableComponent)
    private datatable!: IRemoteTLDatatableComponent;

    private readonly service!: ICommercialFishingService;
    private readonly nomenclatures: CommonNomenclatures;
    private readonly deliveryService!: IDeliveryService;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly editDialog: TLMatDialog<EditCommercialFishingComponent>;
    private readonly logBookDialog: TLMatDialog<EditLogBookComponent>;
    private readonly chooseApplicationDialog: TLMatDialog<ChooseApplicationComponent>;
    private readonly deliveryDialog: TLMatDialog<RegisterDeliveryComponent>;
    private readonly snackbar: MatSnackBar;
    private readonly chooseLogBookForRenewalDialog: TLMatDialog<ChooseLogBookForRenewalComponent>;
    private readonly suspensionsDialog: TLMatDialog<SuspensionsComponent>;
    private readonly router: Router;

    private gridManager!: DataTableManager<CommercialFishingPermitRegisterDTO, CommercialFishingRegisterFilters>;

    public constructor(
        translationService: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editDialog: TLMatDialog<EditCommercialFishingComponent>,
        chooseApplicationDialog: TLMatDialog<ChooseApplicationComponent>,
        deliveryDialog: TLMatDialog<RegisterDeliveryComponent>,
        permissions: PermissionsService,
        commercialFishingService: CommercialFishingAdministrationService,
        deliveryService: DeliveryAdministrationService,
        commonNomenclatures: CommonNomenclatures,
        logBookDialog: TLMatDialog<EditLogBookComponent>,
        snackbar: MatSnackBar,
        chooseLogBookForRenewalDialog: TLMatDialog<ChooseLogBookForRenewalComponent>,
        cacheService: CommercialFishingRegisterCacheService,
        router: Router,
        suspensionsDialog: TLMatDialog<SuspensionsComponent>
    ) {
        this.translationService = translationService;
        this.confirmDialog = confirmDialog;
        this.editDialog = editDialog;
        this.chooseApplicationDialog = chooseApplicationDialog;
        this.deliveryDialog = deliveryDialog;
        this.service = commercialFishingService;
        this.deliveryService = deliveryService;
        this.nomenclatures = commonNomenclatures;
        this.logBookDialog = logBookDialog;
        this.snackbar = snackbar;
        this.chooseLogBookForRenewalDialog = chooseLogBookForRenewalDialog;
        this.cacheService = cacheService;
        this.router = router;
        this.suspensionsDialog = suspensionsDialog;

        this.getLogBookAuditMethod = this.service.getLogBookAudit.bind(this.service);
        this.disabledLogBookAddButtonsTooltipText = this.translationService.getValue('commercial-fishing.cannot-add-log-books-to-suspended-permit-license');

        this.canReadPermitRecords = permissions.has(PermissionsEnum.CommercialFishingPermitRegisterRead);
        this.canAddPermitRecords = permissions.has(PermissionsEnum.CommercialFishingPermitRegisterAddRecords);
        this.canEditPermitRecords = permissions.has(PermissionsEnum.CommercialFishingPermitRegisterEditRecords);
        this.canDeletePermitRecords = permissions.hasAny(PermissionsEnum.CommercialFishingPermitRegisterDeleteRecords);
        this.canRestorePermitRecords = permissions.has(PermissionsEnum.CommercialFishingPermitRegisterRestoreRecords);

        this.canReadPermitApplications = permissions.has(PermissionsEnum.CommercialFishingPermitApplicationsRead) || permissions.has(PermissionsEnum.CommercialFishingPermitApplicationsReadAll);
        this.canReadPermitLicenseApplications = permissions.has(PermissionsEnum.CommercialFishingPermitLicenseApplicationsRead) || permissions.has(PermissionsEnum.CommercialFishingPermitLicenseApplicationsReadAll);

        this.canReadPermitLicenseRecords = permissions.has(PermissionsEnum.CommercialFishingPermitLicenseRegisterRead);
        this.canAddPermitLicenseRecords = permissions.has(PermissionsEnum.CommercialFishingPermitLicenseRegisterAddRecords);
        this.canEditPermitLicenseRecords = permissions.has(PermissionsEnum.CommercialFishingPermitLicenseRegisterEditRecords);
        this.canDeletePermitLicenseRecords = permissions.has(PermissionsEnum.CommercialFishingPermitLicenseRegisterDeleteRecords);
        this.canRestorePermitLicenseRecords = permissions.has(PermissionsEnum.CommercialFishingPermitLicenseRegisterRestoreRecords);

        this.hasPermitsReadAllPermission = permissions.has(PermissionsEnum.CommercialFishingPermitRegisterReadAll);
        this.hasPermitLicenseReadAllPermission = permissions.has(PermissionsEnum.CommercialFishingPermitLicenseRegisterReadAll);

        this.canReadLogBooks = permissions.has(PermissionsEnum.PermitLicenseLogBookRead);
        this.canAddLogBooks = permissions.has(PermissionsEnum.PermitLicenseLogBookAdd);
        this.canEditLogBooks = permissions.has(PermissionsEnum.PermitLicenseLogBookEdit);
        this.canDeleteLogBooks = permissions.has(PermissionsEnum.PermitLicenseLogBookDelete);
        this.canRestoreLogBooks = permissions.has(PermissionsEnum.PermitLicenseLogBookRestore);

        this.canReadPermitSuspensionsRecords = permissions.has(PermissionsEnum.PermitSuspensionRead);
        this.canAddPermitSuspensionsRecords = permissions.has(PermissionsEnum.PermitSuspensionAdd);
        this.canEditPermitSuspensionsRecords = permissions.has(PermissionsEnum.PermitSuspensionEdit);
        this.canDeletePermitSuspensionsRecords = permissions.has(PermissionsEnum.PermitSuspensionDelete);
        this.canRestorePermitSuspensionsRecords = permissions.has(PermissionsEnum.PermitSuspensionRestore);

        this.canReadPermitLicenseSuspensionsRecords = permissions.has(PermissionsEnum.PermitLicenseSuspensionRead);
        this.canAddPermitLicenseSuspensionsRecords = permissions.has(PermissionsEnum.PermitLicenseSuspensionAdd);
        this.canEditPermitLicenseSuspensionsRecords = permissions.has(PermissionsEnum.PermitLicenseSuspensionEdit);
        this.canDeletePermitLicenseSuspensionsRecords = permissions.has(PermissionsEnum.PermitLicenseSuspensionDelete);
        this.canRestorePermitLicenseSuspensionsRecords = permissions.has(PermissionsEnum.PermitLicenseSuspensionRestore);

        this.buildForm();
    }

    public ngOnInit(): void {
        if (this.shipId === null || this.shipId === undefined) {
            if (this.canReadPermitRecords) {
                NomenclatureStore.instance.getNomenclature<number>(
                    NomenclatureTypes.CommercialFishingPermitTypes, this.service.getCommercialFishingPermitTypes.bind(this.service), true
                ).subscribe({
                    next: (results: NomenclatureDTO<number>[]) => {
                        this.permitTypes = results;
                    }
                });
            }

            if (this.canReadPermitLicenseRecords) {
                NomenclatureStore.instance.getNomenclature<number>(
                    NomenclatureTypes.CommercialFishingPermitLicenseTypes, this.service.getCommercialFishingPermitLicenseTypes.bind(this.service), true
                ).subscribe({
                    next: (results: NomenclatureDTO<number>[]) => {
                        this.permitLicenseTypes = results;
                    }
                });
            }

            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.FishingGear, this.nomenclatures.getFishingGear.bind(this.nomenclatures), false
            ).subscribe({
                next: (results: FishingGearNomenclatureDTO[]) => {
                    this.fishingGearTypes = results;
                }
            });

            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false
            ).subscribe({
                next: (results: NomenclatureDTO<number>[]) => {
                    this.territoryUnits = results;
                }
            });

            this.permitIsSuspendedOptions = [
                new NomenclatureDTO<ThreeState>({
                    value: 'yes',
                    displayName: this.translationService.getValue('commercial-fishing.permit-is-suspended-yes'),
                    isActive: true
                }),
                new NomenclatureDTO<ThreeState>({
                    value: 'no',
                    displayName: this.translationService.getValue('commercial-fishing.permit-is-suspended-no'),
                    isActive: true
                }),
                new NomenclatureDTO<ThreeState>({
                    value: 'both',
                    displayName: this.translationService.getValue('commercial-fishing.permit-is-suspended-all'),
                    isActive: true
                })
            ];

            this.permitIsExpiredOptions = [
                new NomenclatureDTO<ThreeState>({
                    value: 'yes',
                    displayName: this.translationService.getValue('commercial-fishing.permit-is-expired-yes'),
                    isActive: true
                }),
                new NomenclatureDTO<ThreeState>({
                    value: 'no',
                    displayName: this.translationService.getValue('commercial-fishing.permit-is-expired-no'),
                    isActive: true
                }),
                new NomenclatureDTO<ThreeState>({
                    value: 'both',
                    displayName: this.translationService.getValue('commercial-fishing.permit-is-expired-all'),
                    isActive: true
                })
            ];

            this.permitLicenseIsSuspendedOptions = [
                new NomenclatureDTO<ThreeState>({
                    value: 'yes',
                    displayName: this.translationService.getValue('commercial-fishing.permit-license-is-suspended-yes'),
                    isActive: true
                }),
                new NomenclatureDTO<ThreeState>({
                    value: 'no',
                    displayName: this.translationService.getValue('commercial-fishing.permit-license-is-suspended-no'),
                    isActive: true
                }),
                new NomenclatureDTO<ThreeState>({
                    value: 'both',
                    displayName: this.translationService.getValue('commercial-fishing.permit-license-is-suspended-all'),
                    isActive: true
                })
            ];

            this.permitLicenseIsExpiredOptions = [
                new NomenclatureDTO<ThreeState>({
                    value: 'yes',
                    displayName: this.translationService.getValue('commercial-fishing.permit-license-is-expired-yes'),
                    isActive: true
                }),
                new NomenclatureDTO<ThreeState>({
                    value: 'no',
                    displayName: this.translationService.getValue('commercial-fishing.permit-license-is-expired-no'),
                    isActive: true
                }),
                new NomenclatureDTO<ThreeState>({
                    value: 'both',
                    displayName: this.translationService.getValue('commercial-fishing.permit-license-is-expired-all'),
                    isActive: true
                })
            ];
        }
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const reloadData: boolean | undefined = changes['reloadData']?.currentValue;

        if (reloadData === true) {
            this.gridManager?.refreshData();
        }
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<CommercialFishingPermitRegisterDTO, CommercialFishingRegisterFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.shipId === null || this.shipId === undefined ? this.searchpanel : undefined,
            requestServiceMethod: this.service.getAllPermits.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        if (this.searchpanel !== null && this.searchpanel !== undefined) {
            this.searchpanel.filtersChanged.subscribe({
                next: () => {
                    this.cacheService.clearPermitLicenseLogBooksCache(); // clear all cache
                }
            });
        }

        const isPerson: boolean | undefined = window.history.state?.isPerson;
        let legalId: number | undefined;
        let personId: number | undefined;
        if (isPerson === true) {
            personId = window.history.state?.id;
        }

        if (isPerson === false) {
            legalId = window.history.state?.id;
        }

        this.gridManager.advancedFilters = new CommercialFishingRegisterFilters({
            personId: personId ?? undefined,
            legalId: legalId ?? undefined,
            shipId: this.shipId ?? undefined
        });

        if (this.shipId === null || this.shipId === undefined) {
            this.gridManager.refreshData();
            this.cacheService.clearPermitLicenseLogBooksCache(); // clear all cache
        }
    }

    public createRegister(): void {
        let data: DialogParamsModel | undefined;
        let auditButton: IHeaderAuditButton | undefined;
        let title: string = '';

        this.chooseApplicationDialog.open({
            TCtor: ChooseApplicationComponent,
            title: this.translationService.getValue('applications-register.choose-application-for-register-creation'),
            translteService: this.translationService,
            componentData: new ChooseApplicationDialogParams({
                pageCodes: [
                    PageCodeEnum.CommFish,
                    PageCodeEnum.PoundnetCommFish,
                    PageCodeEnum.RightToFishThirdCountry,
                    PageCodeEnum.RightToFishResource,
                    PageCodeEnum.PoundnetCommFishLic,
                    PageCodeEnum.CatchQuataSpecies
                ]
            }),
            disableDialogClose: true,
            headerCancelButton: {
                cancelBtnClicked: this.closeApplicationChooseDialogBtnClicked.bind(this)
            },
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translationService.getValue('applications-register.choose')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translationService.getValue('common.cancel'),
            }
        }).subscribe((dialogData: { selectedApplication: ApplicationForChoiceDTO }) => {
            if (dialogData !== null && dialogData !== undefined) {
                let isPermit!: boolean;
                data = new DialogParamsModel({
                    id: undefined,
                    pageCode: dialogData.selectedApplication.pageCode,
                    service: this.service,
                    isThirdCountry: dialogData.selectedApplication.pageCode === PageCodeEnum.RightToFishThirdCountry,
                    isApplication: false,
                    isApplicationHistoryMode: false,
                    showOnlyRegiXData: false,
                    isReadonly: false,
                    viewMode: false,
                    applicationId: dialogData.selectedApplication.id
                });

                switch (dialogData.selectedApplication.pageCode) {
                    case PageCodeEnum.CommFish: {
                        title = this.translationService.getValue('commercial-fishing.add-permit-dialog-title');
                        isPermit = true;
                    }
                        break;
                    case PageCodeEnum.PoundnetCommFish: {
                        title = this.translationService.getValue('commercial-fishing.add-poundnet-permit-dialog-title');
                        isPermit = true;
                    }
                        break;
                    case PageCodeEnum.RightToFishThirdCountry: {
                        title = this.translationService.getValue('commercial-fishing.add-3rd-country-permit-dialog-title');
                        isPermit = true;
                    }
                        break;
                    case PageCodeEnum.RightToFishResource: {
                        title = this.translationService.getValue('commercial-fishing.add-permit-license-dialog-title');
                        isPermit = false;
                    }
                        break;
                    case PageCodeEnum.PoundnetCommFishLic: {
                        title = this.translationService.getValue('commercial-fishing.add-poundnet-permit-license-dialog-title');
                        isPermit = false;
                    }
                        break;
                    case PageCodeEnum.CatchQuataSpecies: {
                        title = this.translationService.getValue('commercial-fishing.add-quata-species-permit-license-dialog-title');
                        isPermit = false;
                    }
                        break;
                    default: throw new Error(`Unknown page code for creating commercial fishing permit/license: ${PageCodeEnum[dialogData.selectedApplication.pageCode!]}`);
                }

                this.openPermitDialog(isPermit, true, data, title, auditButton, false, false);
            }
        });
    }

    public editRegister(permit: CommercialFishingPermitRegisterDTO | CommercialFishingPermitLicenseRegisterDTO, viewMode?: boolean): void {
        let title: string = '';
        let tableName: string = '';
        let simpleAuditMethod: ((id: number) => Observable<SimpleAuditDTO>) | undefined = undefined;
        let isPermit!: boolean;

        const data: DialogParamsModel = new DialogParamsModel({
            id: permit.id,
            pageCode: permit.pageCode,
            service: this.service,
            applicationId: permit.applicationId,
            isThirdCountry: permit.typeCode === CommercialFishingTypesEnum.ThirdCountryPermit,
            isApplication: false,
            isApplicationHistoryMode: false,
            showOnlyRegiXData: false,
            isReadonly: false,
            viewMode: viewMode ?? false
        });

        if (permit.typeCode === CommercialFishingTypesEnum.Permit
            || permit.typeCode === CommercialFishingTypesEnum.PoundNetPermit
            || permit.typeCode === CommercialFishingTypesEnum.ThirdCountryPermit) {

            tableName = 'PermitRegister';
            isPermit = true;
            simpleAuditMethod = this.service.getSimpleAudit.bind(this.service);
        }
        else {
            tableName = 'PermitLicensesRegister';
            isPermit = false;
            simpleAuditMethod = this.service.getPermitLicenseSimpleAudit.bind(this.service);
        }

        const auditButton: IHeaderAuditButton = {
            id: permit.id!,
            getAuditRecordData: simpleAuditMethod!,
            tableName: tableName
        };

        switch (permit.typeCode) {
            case CommercialFishingTypesEnum.Permit: {
                if (viewMode) {
                    title = this.translationService.getValue('commercial-fishing.view-permit-dialog-title');
                }
                else {
                    title = this.translationService.getValue('commercial-fishing.edit-permit-dialog-title');
                }
            }
                break;
            case CommercialFishingTypesEnum.PoundNetPermit: {
                if (viewMode) {
                    title = this.translationService.getValue('commercial-fishing.view-poundnet-permit-dialog-title');
                }
                else {
                    title = this.translationService.getValue('commercial-fishing.edit-poundnet-permit-dialog-title');
                }
            }
                break;
            case CommercialFishingTypesEnum.ThirdCountryPermit: {
                if (viewMode) {
                    title = this.translationService.getValue('commercial-fishing.view-3rd-country-permit-dialog-title');
                }
                else {
                    title = this.translationService.getValue('commercial-fishing.edit-3rd-country-permit-dialog-title');
                }
            }
                break;

            case CommercialFishingTypesEnum.PermitLicense: {
                if (viewMode) {
                    title = this.translationService.getValue('commercial-fishing.view-permit-license-dialog-title');
                }
                else {
                    title = this.translationService.getValue('commercial-fishing.edit-permit-license-dialog-title');
                }
            }
                break;
            case CommercialFishingTypesEnum.PoundNetPermitLicense: {
                if (viewMode) {
                    title = this.translationService.getValue('commercial-fishing.view-poundnet-permit-license-dialog-title');
                }
                else {
                    title = this.translationService.getValue('commercial-fishing.edit-poundnet-permit-license-diloag-title');
                }
            }
                break;
            case CommercialFishingTypesEnum.QuataSpeciesPermitLicense: {
                if (viewMode) {
                    title = this.translationService.getValue('commercial-fishing.view-quata-species-permit-license-dialog-title');
                }
                else {
                    title = this.translationService.getValue('commercial-fishing.edit-quata-species-permit-license-dialog-title');
                }
            }
                break;
            case CommercialFishingTypesEnum.oldInternalPermit: {
                if (viewMode) {
                    title = this.translationService.getValue('commercial-fishing.view-old-internal-permit-dialog-title');
                }
                else {
                    title = this.translationService.getValue('commercial-fishing.edit-old-internal-permit-dialog-title');
                }
            } break;
            case CommercialFishingTypesEnum.oldSpecialPermit: {
                if (viewMode) {
                    title = this.translationService.getValue('commercial-fishing.view-old-special-permit-dialog-title');
                }
                else {
                    title = this.translationService.getValue('commercial-fishing.edit-old-special-permit-dialog-title');
                }
            } break;
        }

        this.openPermitDialog(isPermit, false, data, title, auditButton, viewMode ?? false, permit.isSuspended!);
    }

    public openPermitSuspensions(permit: CommercialFishingPermitRegisterDTO): void {
        const viewMode: boolean =
            this.canReadPermitSuspensionsRecords
            && !(this.canAddPermitSuspensionsRecords
                || this.canEditPermitSuspensionsRecords
                || this.canDeletePermitSuspensionsRecords
                || this.canRestorePermitSuspensionsRecords);

        let title: string = '';
        if (viewMode) {
            const msg: string = this.translationService.getValue('commercial-fishing.view-permit-suspension-history-dialog-title');
            title = `${msg}: ${permit.registrationNumber}`;
        }
        else {
            const msg: string = this.translationService.getValue('commercial-fishing.change-permit-suspension-history-dialog-title');
            title = `${msg}: ${permit.registrationNumber}`;
        }

        const data: SuspensionsDialogParams = new SuspensionsDialogParams({
            isPermitLicense: false,
            pageCode: permit.pageCode,
            recordId: permit.id,
            service: this.service,
            viewMode: viewMode,
            postOnAdd: false
        });

        this.openSuspensionsDialog(title, data, viewMode);
    }

    public openPermitLicenseSuspensions(permitLicense: CommercialFishingPermitLicenseRegisterDTO): void {
        const viewMode: boolean =
            this.canReadPermitLicenseSuspensionsRecords
            && !(this.canAddPermitLicenseSuspensionsRecords
                || this.canEditPermitLicenseSuspensionsRecords
                || this.canDeletePermitLicenseSuspensionsRecords
                || this.canRestorePermitLicenseSuspensionsRecords);

        let title: string = '';
        if (viewMode) {
            const msg: string = this.translationService.getValue('commercial-fishing.view-permit-license-suspension-history-dialog-title');
            title = `${msg}: ${permitLicense.registrationNumber}`;
        }
        else {
            const msg: string = this.translationService.getValue('commercial-fishing.change-permit-license-suspension-history-dialog-title');
            title = `${msg}: ${permitLicense.registrationNumber}`;
        }

        const data: SuspensionsDialogParams = new SuspensionsDialogParams({
            isPermitLicense: true,
            pageCode: permitLicense.pageCode,
            recordId: permitLicense.id,
            service: this.service,
            viewMode: viewMode,
            postOnAdd: false
        });

        this.openSuspensionsDialog(title, data, viewMode);
    }

    private openSuspensionsDialog(title: string, data: SuspensionsDialogParams, viewMode: boolean): void {
        this.suspensionsDialog.openWithTwoButtons({
            TCtor: SuspensionsComponent,
            title: title,
            translteService: this.translationService,
            componentData: data,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => { closeFn(); }
            },
            viewMode: viewMode
        }).subscribe({
            next: (result: SuspensionDataDTO[] | undefined) => {
                if (result !== null && result !== undefined) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    public openDeliveryDialog(register: CommercialFishingPermitRegisterDTO | CommercialFishingPermitLicenseRegisterDTO, viewMode: boolean = false): void {
        let auditButton: IHeaderAuditButton | undefined;

        if (register.deliveryId !== null && register.deliveryId !== undefined) {
            auditButton = {
                id: register.deliveryId,
                getAuditRecordData: this.deliveryService.getSimpleAudit.bind(this.deliveryService),
                tableName: 'ApplicationDelivery'
            };
        }

        this.deliveryDialog.openWithTwoButtons({
            TCtor: RegisterDeliveryComponent,
            title: this.translationService.getValue('commercial-fishing.delivery-data-dialog-title'),
            translteService: this.translationService,
            componentData: new RegisterDeliveryDialogParams({
                deliveryId: register.deliveryId,
                isPublicApp: false,
                service: this.deliveryService,
                pageCode: register.pageCode,
                registerId: register.id,
                viewMode: viewMode
            }),
            headerCancelButton: {
                cancelBtnClicked: this.closeDeliveryDataDialogBtnClicked.bind(this)
            },
            headerAuditButton: auditButton
        }, '1200px').subscribe({
            next: (model: ApplicationDeliveryDTO | undefined) => {
                if (model !== undefined) {
                    this.gridManager.refreshData();
                    this.cacheService.clearPermitLicenseLogBooksCache(); // clear all cache
                }
            }
        });
    }

    public deleteRegister(permit: CommercialFishingPermitRegisterDTO | CommercialFishingPermitLicenseRegisterDTO): void {
        let title: string = '';
        let message: string = '';

        if (permit.pageCode === PageCodeEnum.CommFish || permit.pageCode === PageCodeEnum.PoundnetCommFish || permit.pageCode === PageCodeEnum.RightToFishThirdCountry) {
            title = this.translationService.getValue('commercial-fishing.delete-permit');
            message = this.translationService.getValue('commercial-fishing.confirm-delete-permit-message');
        }
        else {
            title = this.translationService.getValue('commercial-fishing.delete-permit-license');
            message = this.translationService.getValue('commercial-fishing.confirm-delete-permit-license-message');
        }

        this.confirmDialog.open({
            title: title,
            message: message,
            okBtnLabel: this.translationService.getValue('commercial-fishing.delete')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok && permit?.id) {
                    this.service.deletePermit(permit.id, permit.pageCode!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                            this.cacheService.clearPermitLicenseLogBooksCache(); // clear all cache
                        },
                        error: (errorResponse: HttpErrorResponse) => {
                            if ((errorResponse.error as ErrorModel)?.code === ErrorCode.CannotDeletePermitWithLicense) {
                                const message: string = this.translationService.getValue('commercial-fishing.cannot-delete-permit-error');
                                this.snackbar.open(message, undefined, {
                                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                                });
                            }
                            else if ((errorResponse.error as ErrorModel)?.code === ErrorCode.CannotDeleteLicenseWithLogBooks) {
                                const message: string = this.translationService.getValue('commercial-fishing.cannot-delete-permit-license-error');
                                this.snackbar.open(message, undefined, {
                                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                                });
                            }
                        }
                    });
                }
            }
        });
    }

    public restoreRegister(permit: CommercialFishingPermitRegisterDTO | CommercialFishingPermitLicenseRegisterDTO): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok && permit?.id) {
                    this.service.undoDeletePermit(permit.id, permit.pageCode!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                            this.cacheService.clearPermitLicenseLogBooksCache(); // clear all cache
                        }
                    });
                }
            }
        });
    }

    public onLogBookActionEnded(actionType: OnActionEndedType): void {
        this.gridManager.refreshData(); // refresh grid
        this.cacheService.clearPermitLicenseLogBooksCache(); // clear all cache
    }

    public gotToPermitApplication(permit: CommercialFishingPermitRegisterDTO): void {
        if (this.canReadPermitApplications) {
            this.router.navigate(['commercial-fishing-applications'], { state: { applicationId: permit.applicationId } });
        }
    }

    public gotToPermitLicenseApplication(permitLicense: CommercialFishingPermitLicenseRegisterDTO): void {
        if (this.canReadPermitApplications) {
            this.router.navigate(['commercial-fishing-applications'], { state: { applicationId: permitLicense.applicationId } });
        }
    }

    private openPermitDialog(isPermit: boolean, isAdd: boolean, data: DialogParamsModel, title: string, auditButton: IHeaderAuditButton | undefined, viewMode: boolean, isSuspended: boolean): void {
        const rightButtons: IActionInfo[] = [];
        const leftButtons: IActionInfo[] = [];

        if (!viewMode && !isAdd) {
            if ((isPermit && this.canAddPermitSuspensionsRecords)
                || (!isPermit && this.canAddPermitLicenseSuspensionsRecords)) {
                leftButtons.push({
                    id: 'suspend',
                    color: 'warn',
                    translateValue: this.translationService.getValue('commercial-fishing.suspend-btn-label')
                });
            }
        }

        rightButtons.push({
            id: 'print',
            color: 'accent',
            translateValue: viewMode || isSuspended
                ? 'commercial-fishing.print'
                : 'commercial-fishing.save-print',
            isVisibleInViewMode: true
        });

        const dialog = this.editDialog.open({
            title: title,
            TCtor: EditCommercialFishingComponent,
            headerAuditButton: auditButton,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translationService,
            disableDialogClose: true,
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translationService.getValue('common.save')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translationService.getValue('common.cancel'),
            },
            rightSideActionsCollection: rightButtons,
            leftSideActionsCollection: leftButtons,
            viewMode: viewMode
        }, '1600px');

        dialog.subscribe((entry?: CommercialFishingEditDTO) => {
            if (entry !== null && entry !== undefined) {
                this.gridManager.refreshData();
                this.cacheService.clearPermitLicenseLogBooksCache(); // clear all cache
            }
        });
    }

    private buildForm(): void {
        this.formGroup = new FormGroup({
            permitTypeControl: new FormControl(),
            permitLicenseTypeControl: new FormControl(),
            permitNumberControl: new FormControl(),
            permitLicenseNumberControl: new FormControl(),
            permitIssuedDateRangeControl: new FormControl(),
            permitLicenseIssuedDateRangeControl: new FormControl(),
            shipNameControl: new FormControl(),
            shipCfrControl: new FormControl(),
            shipExternalMarkingControl: new FormControl(),
            shipRegistrationCertificateNumberControl: new FormControl(),
            poundNetNameControl: new FormControl(),
            poundNetNumberControl: new FormControl(),
            fishingGearTypeControl: new FormControl(),
            fishingGearMarkNumberControl: new FormControl(),
            fishingGearPingerNumberControl: new FormControl(),
            permitSubmittedForNameControl: new FormControl(),
            permitLicenseSubmittedForNameControl: new FormControl(),
            permitSubmittedForIdentifierControl: new FormControl(),
            permitLicenseSubmittedForIdentifierControl: new FormControl(),
            logBookNumberControl: new FormControl(),
            permitTerritoryUnitControl: new FormControl(),
            permitLicenseTerritoryUnitControl: new FormControl(),
            permitIsSuspendedControl: new FormControl(),
            permitIsExpiredControl: new FormControl(),
            permitLicenseIsSuspendedControl: new FormControl(),
            permitLicenseIsExpiredControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): CommercialFishingRegisterFilters {
        const result: CommercialFishingRegisterFilters = new CommercialFishingRegisterFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            permitTypeId: filters.getValue('permitTypeControl'),
            permitLicenseTypeId: filters.getValue('permitLicenseTypeControl'),
            permitNumber: filters.getValue('permitNumberControl'),
            permitLicenseNumber: filters.getValue('permitLicenseNumberControl'),
            permitIssuedOnStartDate: filters.getValue<DateRangeData>('permitIssuedDateRangeControl')?.start,
            permitIssuedOnEndDate: filters.getValue<DateRangeData>('permitIssuedDateRangeControl')?.end,
            permitLicenseIssuedOnStartDate: filters.getValue<DateRangeData>('permitLicenseIssuedDateRangeControl')?.start,
            permitLicenseIssuedOnEndDate: filters.getValue<DateRangeData>('permitLicenseIssuedDateRangeControl')?.end,
            shipName: filters.getValue('shipNameControl'),
            shipCfr: filters.getValue('shipCfrControl'),
            shipExternalMarking: filters.getValue('shipExternalMarkingControl'),
            shipRegistrationCertificateNumber: filters.getValue('shipRegistrationCertificateNumberControl'),
            poundNetName: filters.getValue('poundNetNameControl'),
            poundNetNumber: filters.getValue('poundNetNumberControl'),
            fishingGearTypeId: filters.getValue('fishingGearTypeControl'),
            fishingGearMarkNumber: filters.getValue('fishingGearMarkNumberControl'),
            fishingGearPingerNumber: filters.getValue('fishingGearPingerNumberControl'),
            permitSubmittedForName: filters.getValue('permitSubmittedForNameControl'),
            permitLicenseSubmittedForName: filters.getValue('permitLicenseSubmittedForNameControl'),
            permitSubmittedForIdentifier: filters.getValue('permitSubmittedForIdentifierControl'),
            permitLicenseSubmittedForIdentifier: filters.getValue('permitLicenseSubmittedForIdentifierControl'),
            logbookNumber: filters.getValue('logBookNumberControl'),
            permitTerritoryUnitId: this.hasPermitsReadAllPermission ? filters.getValue('permitTerritoryUnitControl') : undefined,
            permitLicenseTerritoryUnitId: this.hasPermitLicenseReadAllPermission ? filters.getValue('permitLicenseTerritoryUnitControl') : undefined
        });

        const permitIsSuspended = filters.getValue<ThreeState>('permitIsSuspendedControl');
        if (permitIsSuspended !== undefined && permitIsSuspended !== null) {
            switch (permitIsSuspended) {
                case 'yes':
                    result.permitIsSuspended = true;
                    break;
                case 'no':
                    result.permitIsSuspended = false;
                    break;
                case 'both':
                    result.permitIsSuspended = undefined;
                    break;
            }
        }

        const permitIsExpired = filters.getValue<ThreeState>('permitIsExpiredControl');
        if (permitIsExpired !== undefined && permitIsExpired !== null) {
            switch (permitIsExpired) {
                case 'yes':
                    result.permitIsExpired = true;
                    break;
                case 'no':
                    result.permitIsExpired = false;
                    break;
                case 'both':
                    result.permitIsExpired = undefined;
                    break;
            }
        }

        const permitLicenseIsSuspended = filters.getValue<ThreeState>('permitLicenseIsSuspendedControl');
        if (permitLicenseIsSuspended !== undefined && permitLicenseIsSuspended !== null) {
            switch (permitLicenseIsSuspended) {
                case 'yes':
                    result.permitLicenseIsSuspended = true;
                    break;
                case 'no':
                    result.permitLicenseIsSuspended = false;
                    break;
                case 'both':
                    result.permitLicenseIsSuspended = undefined;
                    break;
            }
        }

        const permitLicenseIsExpired = filters.getValue<ThreeState>('permitLicenseIsExpiredControl');
        if (permitLicenseIsExpired !== undefined && permitLicenseIsExpired !== null) {
            switch (permitLicenseIsExpired) {
                case 'yes':
                    result.permitLicenseIsExpired = true;
                    break;
                case 'no':
                    result.permitLicenseIsExpired = false;
                    break;
                case 'both':
                    result.permitLicenseIsExpired = undefined;
                    break;
            }
        }

        return result;
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeApplicationChooseDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeDeliveryDataDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}

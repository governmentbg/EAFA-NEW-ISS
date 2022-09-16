import { IRemoteTLDatatableComponent } from '@app/shared/components/data-table/interfaces/tl-remote-datatable.interface';
import { AfterViewInit, Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

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
import { IS_PUBLIC_APP } from '../../../shared/modules/application.modules';


type ThreeState = 'yes' | 'no' | 'both';

@Component({
    selector: 'commercial-fishing-register',
    templateUrl: './commercial-fishing-register.component.html'
})
export class CommercialFishingRegisterComponent implements OnInit, AfterViewInit, OnChanges {
    @Input()
    public shipId: number | undefined;

    @Input()
    public reloadData: boolean = false;

    public translationService: FuseTranslationLoaderService;
    public formGroup!: FormGroup;
    public permitTypes: NomenclatureDTO<number>[] = [];
    public permitLicenseTypes: NomenclatureDTO<number>[] = [];
    public fishingGearTypes: FishingGearNomenclatureDTO[] = [];
    public territoryUnits: NomenclatureDTO<number>[] = [];
    public permitIsSuspendedOptions: NomenclatureDTO<ThreeState>[] = [];
    public permitIsExpiredOptions: NomenclatureDTO<ThreeState>[] = [];
    public permitLicenseIsSuspendedOptions: NomenclatureDTO<ThreeState>[] = [];
    public permitLicenseIsExpiredOptions: NomenclatureDTO<ThreeState>[] = [];

    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    public readonly canReadPermitRecords: boolean;
    public readonly canAddPermitRecords: boolean;
    public readonly canEditPermitRecords: boolean;
    public readonly canRestorePermitRecords: boolean;
    public readonly canDeletePermitRecords: boolean;

    public readonly canReadPermitLicenseRecords: boolean;
    public readonly canAddPermitLicenseRecords: boolean;
    public readonly canEditPermitLicenseRecords: boolean;
    public readonly canDeletePermitLicenseRecords: boolean;
    public readonly canRestorePermitLicenseRecords: boolean;

    public readonly hasPermitsReadAllPermission: boolean;
    public readonly hasPermitLicenseReadAllPermission: boolean;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    @ViewChild(TLDataTableComponent)
    private datatable!: IRemoteTLDatatableComponent;

    private service!: ICommercialFishingService;
    private nomenclatures: CommonNomenclatures;
    private deliveryService!: IDeliveryService;
    private gridManager!: DataTableManager<CommercialFishingPermitRegisterDTO, CommercialFishingRegisterFilters>;
    private confirmDialog: TLConfirmDialog;
    private editDialog: TLMatDialog<EditCommercialFishingComponent>;
    private chooseApplicationDialog: TLMatDialog<ChooseApplicationComponent>;
    private deliveryDialog: TLMatDialog<RegisterDeliveryComponent>;

    public constructor(
        translationService: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editDialog: TLMatDialog<EditCommercialFishingComponent>,
        chooseApplicationDialog: TLMatDialog<ChooseApplicationComponent>,
        deliveryDialog: TLMatDialog<RegisterDeliveryComponent>,
        permissions: PermissionsService,
        commercialFishingService: CommercialFishingAdministrationService,
        deliveryService: DeliveryAdministrationService,
        commonNomenclatures: CommonNomenclatures
    ) {
        this.translationService = translationService;
        this.confirmDialog = confirmDialog;
        this.editDialog = editDialog;
        this.chooseApplicationDialog = chooseApplicationDialog;
        this.deliveryDialog = deliveryDialog;
        this.service = commercialFishingService;
        this.deliveryService = deliveryService;
        this.nomenclatures = commonNomenclatures;

        this.canReadPermitRecords = permissions.has(PermissionsEnum.CommercialFishingPermitRegisterRead);
        this.canAddPermitRecords = permissions.has(PermissionsEnum.CommercialFishingPermitRegisterAddRecords);
        this.canEditPermitRecords = permissions.has(PermissionsEnum.CommercialFishingPermitRegisterEditRecords);
        this.canDeletePermitRecords = permissions.hasAny(PermissionsEnum.CommercialFishingPermitRegisterDeleteRecords);
        this.canRestorePermitRecords = permissions.has(PermissionsEnum.CommercialFishingPermitRegisterRestoreRecords);

        this.canReadPermitLicenseRecords = permissions.has(PermissionsEnum.CommercialFishingPermitLicenseRegisterRead);
        this.canAddPermitLicenseRecords = permissions.has(PermissionsEnum.CommercialFishingPermitLicenseRegisterAddRecords);
        this.canEditPermitLicenseRecords = permissions.has(PermissionsEnum.CommercialFishingPermitLicenseRegisterEditRecords);
        this.canDeletePermitLicenseRecords = permissions.has(PermissionsEnum.CommercialFishingPermitLicenseRegisterDeleteRecords);
        this.canRestorePermitLicenseRecords = permissions.has(PermissionsEnum.CommercialFishingPermitLicenseRegisterRestoreRecords);

        this.hasPermitsReadAllPermission = permissions.has(PermissionsEnum.CommercialFishingPermitRegisterReadAll);
        this.hasPermitLicenseReadAllPermission = permissions.has(PermissionsEnum.CommercialFishingPermitLicenseRegisterReadAll);

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
                    case PageCodeEnum.CommFish: title = this.translationService.getValue('commercial-fishing.add-permit-dialog-title');
                        break;
                    case PageCodeEnum.PoundnetCommFish: title = this.translationService.getValue('commercial-fishing.add-poundnet-permit-dialog-title');
                        break;
                    case PageCodeEnum.RightToFishThirdCountry: title = this.translationService.getValue('commercial-fishing.add-3rd-country-permit-dialog-title');
                        break;
                    case PageCodeEnum.RightToFishResource: title = this.translationService.getValue('commercial-fishing.add-permit-license-dialog-title');
                        break;
                    case PageCodeEnum.PoundnetCommFishLic: title = this.translationService.getValue('commercial-fishing.add-poundnet-permit-license-dialog-title');
                        break;
                    case PageCodeEnum.CatchQuataSpecies: title = this.translationService.getValue('commercial-fishing.add-quata-species-permit-license-dialog-title');
                }

                this.openPermitDialog(data, title, auditButton, false, false);
            }
        });
    }

    public editRegister(permit: CommercialFishingPermitRegisterDTO | CommercialFishingPermitLicenseRegisterDTO, viewMode?: boolean): void {
        let title: string = '';
        let tableName: string = '';

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
        }
        else {
            tableName = 'PermitLicensesRegister';
        }

        const auditButton: IHeaderAuditButton = {
            id: permit.id!,
            getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
            tableName: tableName
        };

        if (viewMode) {
            switch (permit.typeCode) {
                case CommercialFishingTypesEnum.Permit: title = this.translationService.getValue('commercial-fishing.view-permit-dialog-title');
                    break;
                case CommercialFishingTypesEnum.PoundNetPermit: title = this.translationService.getValue('commercial-fishing.view-poundnet-permit-dialog-title');
                    break;
                case CommercialFishingTypesEnum.ThirdCountryPermit: title = this.translationService.getValue('commercial-fishing.view-3rd-country-permit-dialog-title');
                    break;

                case CommercialFishingTypesEnum.PermitLicense: title = this.translationService.getValue('commercial-fishing.view-permit-license-dialog-title');
                    break;
                case CommercialFishingTypesEnum.PoundNetPermitLicense: title = this.translationService.getValue('commercial-fishing.view-poundnet-permit-license-dialog-title');
                    break;
                case CommercialFishingTypesEnum.QuataSpeciesPermitLicense: title = this.translationService.getValue('commercial-fishing.view-quata-species-permit-license-dialog-title');
                    break;
            }
        }
        else {
            switch (permit.typeCode) {
                case CommercialFishingTypesEnum.Permit: title = this.translationService.getValue('commercial-fishing.edit-permit-dialog-title');
                    break;
                case CommercialFishingTypesEnum.PoundNetPermit: title = this.translationService.getValue('commercial-fishing.edit-poundnet-permit-dialog-title');
                    break;
                case CommercialFishingTypesEnum.ThirdCountryPermit: title = this.translationService.getValue('commercial-fishing.edit-3rd-country-permit-dialog-title');
                    break;

                case CommercialFishingTypesEnum.PermitLicense: title = this.translationService.getValue('commercial-fishing.edit-permit-license-dialog-title');
                    break;
                case CommercialFishingTypesEnum.PoundNetPermitLicense: title = this.translationService.getValue('commercial-fishing.edit-poundnet-permit-license-diloag-title');
                    break;
                case CommercialFishingTypesEnum.QuataSpeciesPermitLicense: title = this.translationService.getValue('commercial-fishing.edit-quata-species-permit-license-dialog-title');
                    break;
            }
        }

        this.openPermitDialog(data, title, auditButton, viewMode ?? false, permit.isSuspended!);
    }

    public openDeliveryDialog(permit: CommercialFishingPermitRegisterDTO | CommercialFishingPermitLicenseRegisterDTO): void {
        let auditButton: IHeaderAuditButton | undefined;

        if (permit.deliveryId !== null && permit.deliveryId !== undefined) {
            auditButton = {
                id: permit.deliveryId,
                getAuditRecordData: this.deliveryService.getSimpleAudit.bind(this.deliveryService),
                tableName: 'ApplicationDelivery'
            };
        }

        this.deliveryDialog.openWithTwoButtons({
            TCtor: RegisterDeliveryComponent,
            title: this.translationService.getValue('commercial-fishing.delivery-data-dialog-title'),
            translteService: this.translationService,
            componentData: new RegisterDeliveryDialogParams({
                deliveryId: permit.deliveryId,
                isPublicApp: false,
                service: this.deliveryService,
                pageCode: permit.pageCode
            }),
            headerCancelButton: {
                cancelBtnClicked: this.closeDeliveryDataDialogBtnClicked.bind(this)
            },
            headerAuditButton: auditButton
        }, '1200px').subscribe({
            next: (model: ApplicationDeliveryDTO | undefined) => {
                if (model !== undefined) {
                    this.gridManager.refreshData();
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
                        }
                    });
                }
            }
        });
    }

    private openPermitDialog(data: DialogParamsModel, title: string, auditButton: IHeaderAuditButton | undefined, viewMode: boolean, isSuspended: boolean): void {
        const rightButtons: IActionInfo[] = [];
        const leftButtons: IActionInfo[] = [];

        if (!viewMode) {
            leftButtons.push({
                id: 'suspend',
                color: 'warn',
                translateValue: this.translationService.getValue('commercial-fishing.suspend-btn-label')
            });
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
        }, '1500px');

        dialog.subscribe((entry?: CommercialFishingEditDTO) => {
            if (entry !== null && entry !== undefined) {
                this.gridManager.refreshData();
            }
        });
    }

    private buildForm(): void {
        this.formGroup = new FormGroup({
            permitTypeControl: new FormControl(),
            permitLicenseTypeControl: new FormControl(),
            numberControl: new FormControl(),
            issuedDateRangeControl: new FormControl(),
            shipNameControl: new FormControl(),
            shipCfrControl: new FormControl(),
            shipExternalMarkingControl: new FormControl(),
            shipRegistrationCertificateNumberControl: new FormControl(),
            poundNetNameControl: new FormControl(),
            poundNetNumberControl: new FormControl(),
            fishingGearTypeControl: new FormControl(),
            fishingGearMarkNumberControl: new FormControl(),
            fishingGearPingerNumberControl: new FormControl(),
            submittedForNameControl: new FormControl(),
            submittedForIdentifierControl: new FormControl(),
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
            number: filters.getValue('numberControl'),
            issuedOnRangeStartDate: filters.getValue<DateRangeData>('issuedDateRangeControl')?.start,
            issuedOnRangeEndDate: filters.getValue<DateRangeData>('issuedDateRangeControl')?.end,
            shipName: filters.getValue('shipNameControl'),
            shipCfr: filters.getValue('shipCfrControl'),
            shipExternalMarking: filters.getValue('shipExternalMarkingControl'),
            shipRegistrationCertificateNumber: filters.getValue('shipRegistrationCertificateNumberControl'),
            poundNetName: filters.getValue('poundNetNameControl'),
            poundNetNumber: filters.getValue('poundNetNumberControl'),
            fishingGearTypeId: filters.getValue('fishingGearTypeControl'),
            fishingGearMarkNumber: filters.getValue('fishingGearMarkNumberControl'),
            fishingGearPingerNumber: filters.getValue('fishingGearPingerNumberControl'),
            submittedForName: filters.getValue('submittedForNameControl'),
            submittedForIdentifier: filters.getValue('submittedForIdentifierControl'),
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

import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IAquacultureFacilitiesService } from '@app/interfaces/common-app/aquaculture-facilities.interface';
import { AquacultureFacilityDTO } from '@app/models/generated/dtos/AquacultureFacilityDTO';
import { AquacultureFacilitiesFilters } from '@app/models/generated/filters/AquacultureFacilitiesFilters';
import { AquacultureFacilitiesAdministrationService } from '@app/services/administration-app/aquaculture-facilities-administration.service';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { EditAquacultureFacilityComponent } from '@app/components/common-app/aquaculture-facilities/edit-aquaculture-facility/edit-aquaculture-facility.component';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { AquacultureSalinityEnum } from '@app/enums/aquaculture-salinity.enum';
import { AquacultureTemperatureEnum } from '@app/enums/aquaculture-temperature.enum';
import { AquacultureSystemEnum } from '@app/enums/aquaculture-system.enum';
import { RangeInputData } from '@app/shared/components/input-controls/tl-range-input/range-input.component';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { ChooseApplicationComponent } from '@app/components/common-app/applications/components/choose-application/choose-application.component';
import { ChooseApplicationDialogParams } from '@app/components/common-app/applications/components/choose-application/models/choose-application-dialog-params.model';
import { ApplicationForChoiceDTO } from '@app/models/generated/dtos/ApplicationForChoiceDTO';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { AquacultureFacilityEditDTO } from '@app/models/generated/dtos/AquacultureFacilityEditDTO';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { AquacultureStatusEnum } from '@app/enums/aquaculture-status.enum';
import { RegisterDeliveryComponent } from '@app/shared/components/register-delivery/register-delivery.component';
import { RegisterDeliveryDialogParams } from '@app/shared/components/register-delivery/models/register-delivery-dialog-params.model';
import { ApplicationDeliveryDTO } from '@app/models/generated/dtos/ApplicationDeliveryDTO';
import { IDeliveryService } from '@app/interfaces/common-app/delivery.interface';
import { DeliveryAdministrationService } from '@app/services/administration-app/delivery-administration.service';

@Component({
    selector: 'aquaculture-facilities',
    templateUrl: './aquaculture-facilities.component.html'
})
export class AquacultureFacilitiesComponent implements OnInit, AfterViewInit {
    public readonly aquacultureStatus: typeof AquacultureStatusEnum = AquacultureStatusEnum;
    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public statuses: NomenclatureDTO<number>[] = [];
    public territoryUnits: NomenclatureDTO<number>[] = [];
    public waterAreaTypes: NomenclatureDTO<number>[] = [];
    public populatedAreas: NomenclatureDTO<number>[] = [];
    public waterSalinityTypes: NomenclatureDTO<AquacultureSalinityEnum>[] = [];
    public waterTemperatureTypes: NomenclatureDTO<AquacultureTemperatureEnum>[] = [];
    public systemTypes: NomenclatureDTO<AquacultureSystemEnum>[] = [];
    public aquaticOrganisms: NomenclatureDTO<number>[] = [];
    public powerSupplyTypes: NomenclatureDTO<number>[] = [];
    public installationTypes: NomenclatureDTO<number>[] = [];

    public readonly canEditRecords: boolean;
    public readonly canDeleteRecords: boolean;
    public readonly canRestoreRecords: boolean;
    public readonly canCancelAquacultures: boolean;

    public readonly hasReadAllPermission: boolean;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private service: IAquacultureFacilitiesService;
    private deliveryService: IDeliveryService;
    private nomenclatures: CommonNomenclatures;
    private grid!: DataTableManager<AquacultureFacilityDTO, AquacultureFacilitiesFilters>;
    private editDialog: TLMatDialog<EditAquacultureFacilityComponent>;
    private chooseApplicationDialog: TLMatDialog<ChooseApplicationComponent>;
    private deliveryDialog: TLMatDialog<RegisterDeliveryComponent>;
    private confirmDialog: TLConfirmDialog;

    public constructor(
        translate: FuseTranslationLoaderService,
        service: AquacultureFacilitiesAdministrationService,
        deliveryService: DeliveryAdministrationService,
        nomenclatures: CommonNomenclatures,
        editDialog: TLMatDialog<EditAquacultureFacilityComponent>,
        chooseApplicationDialog: TLMatDialog<ChooseApplicationComponent>,
        deliveryDialog: TLMatDialog<RegisterDeliveryComponent>,
        confirmDialog: TLConfirmDialog,
        permissions: PermissionsService
    ) {
        this.translate = translate;
        this.service = service;
        this.deliveryService = deliveryService;
        this.nomenclatures = nomenclatures;
        this.editDialog = editDialog;
        this.chooseApplicationDialog = chooseApplicationDialog;
        this.deliveryDialog = deliveryDialog;
        this.confirmDialog = confirmDialog;

        this.canEditRecords = permissions.has(PermissionsEnum.AquacultureFacilitiesEditRecords);
        this.canDeleteRecords = permissions.has(PermissionsEnum.AquacultureFacilitiesDeleteRecords);
        this.canRestoreRecords = permissions.has(PermissionsEnum.AquacultureFacilitiesRestoreRecords);
        this.canCancelAquacultures = permissions.has(PermissionsEnum.AquacultureFacilitiesCancel);

        this.hasReadAllPermission = permissions.has(PermissionsEnum.AquacultureFacilitiesReadAll);

        this.buildForm();
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.AquacultureStatusTypes, this.service.getAquacultureStatusTypes.bind(this.service), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.statuses = result.filter((status: NomenclatureDTO<number>) => {
                    return status.code !== AquacultureStatusEnum[AquacultureStatusEnum.Application];
                });
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.territoryUnits = result;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.AquacultureWaterAreaTypes, this.service.getAquacultureWaterAreaTypes.bind(this.service), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.waterAreaTypes = result;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.PopulatedAreas, this.nomenclatures.getPopulatedAreas.bind(this.nomenclatures), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.populatedAreas = result;
            }
        });

        this.waterSalinityTypes = [
            new NomenclatureDTO<AquacultureSalinityEnum>({
                value: AquacultureSalinityEnum.Freshwater,
                displayName: this.translate.getValue('aquacultures.salinity-fresh'),
                isActive: true
            }),
            new NomenclatureDTO<AquacultureSalinityEnum>({
                value: AquacultureSalinityEnum.Saltwater,
                displayName: this.translate.getValue('aquacultures.salinity-salt'),
                isActive: true
            })
        ];

        this.waterTemperatureTypes = [
            new NomenclatureDTO<AquacultureTemperatureEnum>({
                value: AquacultureTemperatureEnum.Cold,
                displayName: this.translate.getValue('aquacultures.temperature-cold'),
                isActive: true
            }),
            new NomenclatureDTO<AquacultureTemperatureEnum>({
                value: AquacultureTemperatureEnum.Warm,
                displayName: this.translate.getValue('aquacultures.temperature-warm'),
                isActive: true
            }),
            new NomenclatureDTO<AquacultureTemperatureEnum>({
                value: AquacultureTemperatureEnum.Mixed,
                displayName: this.translate.getValue('aquacultures.temperature-mixed'),
                isActive: true
            })
        ];

        this.systemTypes = [
            new NomenclatureDTO<AquacultureSystemEnum>({
                value: AquacultureSystemEnum.FullSystem,
                displayName: this.translate.getValue('aquacultures.system-full'),
                isActive: true
            }),
            new NomenclatureDTO<AquacultureSystemEnum>({
                value: AquacultureSystemEnum.NonFullSystem,
                displayName: this.translate.getValue('aquacultures.system-non-full'),
                isActive: true
            })
        ];

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Fishes, this.nomenclatures.getFishTypes.bind(this.nomenclatures), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.aquaticOrganisms = result;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.AquaculturePowerSupplyTypes, this.service.getAquaculturePowerSupplyTypes.bind(this.service), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.powerSupplyTypes = result;
            }
        });


        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.AquacultureInstallationTypes, this.service.getInstallationTypes.bind(this.service), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.installationTypes = result;
            }
        });
    }

    public ngAfterViewInit(): void {
        this.grid = new DataTableManager<AquacultureFacilityDTO, AquacultureFacilitiesFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllAquacultures.bind(this.service),
            filtersMapper: this.mapFilters.bind(this),
            excelRequestServiceMethod: this.service.downloadAquacultureFacilitiesExcel.bind(this.service),
            excelFilename: this.translate.getValue('aquacultures.excel-filename')
        });

        if (window.history.state) {
            const tableId: number | undefined = window.history.state.tableId;
            const isPerson: boolean | undefined = window.history.state.isPerson;
            const id: number | undefined = window.history.state.id;

            const filters: AquacultureFacilitiesFilters = new AquacultureFacilitiesFilters();

            if (tableId !== undefined && tableId !== null) {
                filters.id = tableId;
            }

            if (id !== undefined && id !== null) {
                if (isPerson) {
                    filters.personId = id;
                }
                else {
                    filters.legalId = id;
                }
            }

            this.grid.advancedFilters = filters;
        }

        this.grid.refreshData();
    }

    public addEditAquaculture(aquaculture: AquacultureFacilityDTO | undefined, viewMode: boolean = false): void {
        let data: DialogParamsModel | undefined;
        let auditButton: IHeaderAuditButton | undefined;
        let title: string;
        const rightButtons: IActionInfo[] = [];

        if (aquaculture?.id !== undefined) {
            const readonly: boolean = !this.canEditRecords || aquaculture.status === AquacultureStatusEnum.Canceled;

            data = new DialogParamsModel({
                id: aquaculture.id,
                isApplication: false,
                isReadonly: false,
                service: this.service,
                viewMode: viewMode || readonly
            });

            auditButton = {
                id: aquaculture.id,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'AquacultureFacilityRegister'
            };

            title = viewMode
                ? this.translate.getValue('aquacultures.view-aquaculture-dialog-title')
                : this.translate.getValue('aquacultures.edit-aquaculture-dialog-title');

            if (!viewMode) {
                if (aquaculture.status === AquacultureStatusEnum.Canceled) {
                    rightButtons.push({
                        id: 'activate',
                        color: 'accent',
                        translateValue: 'aquacultures.activate',
                        isVisibleInViewMode: true
                    });
                }
                else {
                    rightButtons.push({
                        id: 'cancel',
                        color: 'warn',
                        translateValue: 'aquacultures.cancel'
                    });
                }
            }

            rightButtons.push({
                id: 'print',
                color: 'accent',
                translateValue: viewMode || readonly
                    ? 'aquacultures.print'
                    : 'aquacultures.save-and-print',
                isVisibleInViewMode: true
            });

            this.openEditDialog(data, title, auditButton, rightButtons, viewMode || readonly);
        }
        else {
            title = this.translate.getValue('aquacultures.add-aquaculture-dialog-title');

            this.chooseApplicationDialog.open({
                TCtor: ChooseApplicationComponent,
                title: this.translate.getValue('applications-register.choose-application-for-register-creation'),
                translteService: this.translate,
                componentData: new ChooseApplicationDialogParams({ pageCodes: [PageCodeEnum.AquaFarmReg] }),
                disableDialogClose: true,
                headerCancelButton: {
                    cancelBtnClicked: this.closeApplicationChooseDialogBtnClicked.bind(this)
                },
                saveBtn: {
                    id: 'save',
                    color: 'accent',
                    translateValue: this.translate.getValue('applications-register.choose')
                },
                cancelBtn: {
                    id: 'cancel',
                    color: 'primary',
                    translateValue: this.translate.getValue('common.cancel'),
                }
            }).subscribe((dialogData: { selectedApplication: ApplicationForChoiceDTO }) => {
                if (dialogData !== null && dialogData !== undefined) {
                    data = new DialogParamsModel({
                        id: undefined,
                        isApplication: false,
                        isReadonly: false,
                        viewMode: false,
                        service: this.service,
                        applicationId: dialogData.selectedApplication.id
                    });

                    this.openEditDialog(data, title, auditButton, rightButtons, viewMode);
                }
            });
        }
    }

    public deleteAquaculture(aquaculture: AquacultureFacilityDTO): void {
        this.confirmDialog.open({
            title: this.translate.getValue('aquacultures.delete-aquaculture-dialog-title'),
            message: this.translate.getValue('aquacultures.delete-aquaculture-dialog-message'),
            okBtnLabel: this.translate.getValue('aquacultures.delete-aquaculture-dialog-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.deleteAquaculture(aquaculture.id!).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
            }
        });
    }

    public undoDeleteAquaculture(aquaculture: AquacultureFacilityDTO): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.undoDeleteAquaculture(aquaculture.id!).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
            }
        });
    }

    public openDeliveryDialog(aquaculture: AquacultureFacilityDTO): void {
        let auditButton: IHeaderAuditButton | undefined;

        if (aquaculture.deliveryId !== null && aquaculture.deliveryId !== undefined) {
            auditButton = {
                id: aquaculture.deliveryId,
                getAuditRecordData: this.deliveryService.getSimpleAudit.bind(this.deliveryService),
                tableName: 'ApplicationDelivery'
            };
        }

        this.deliveryDialog.openWithTwoButtons({
            TCtor: RegisterDeliveryComponent,
            title: this.translate.getValue('aquacultures.delivery-data-dialog-title'),
            translteService: this.translate,
            componentData: new RegisterDeliveryDialogParams({
                deliveryId: aquaculture.deliveryId,
                isPublicApp: false,
                service: this.deliveryService,
                pageCode: PageCodeEnum.AquaFarmReg,
                registerId: aquaculture.id
            }),
            headerCancelButton: {
                cancelBtnClicked: this.closeDeliveryDataDialogBtnClicked.bind(this)
            },
            headerAuditButton: auditButton
        }, '1200px').subscribe({
            next: (model: ApplicationDeliveryDTO | undefined) => {
                if (model !== undefined) {
                    this.grid.refreshData();
                }
            }
        });
    }

    private openEditDialog(data: DialogParamsModel, title: string, auditButton: IHeaderAuditButton | undefined, rightButtons: IActionInfo[], viewMode: boolean): void {
        const dialog = this.editDialog.open({
            title: title,
            TCtor: EditAquacultureFacilityComponent,
            headerAuditButton: auditButton,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: true,
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translate.getValue('common.save')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translate.getValue('common.cancel'),
            },
            rightSideActionsCollection: rightButtons,
            viewMode: viewMode
        }, '1400px');

        dialog.subscribe((entry?: AquacultureFacilityEditDTO) => {
            if (entry !== undefined) {
                this.grid.refreshData();
            }
        });
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

    private buildForm(): void {
        this.form = new FormGroup({
            regNumControl: new FormControl(),
            urorControl: new FormControl(),
            registrationDateControl: new FormControl(),
            nameControl: new FormControl(),
            eikControl: new FormControl(),
            statusControl: new FormControl(),
            territoryUnitControl: new FormControl(),
            waterAreaTypeControl: new FormControl(),
            populatedAreaControl: new FormControl(),
            locationControl: new FormControl(),
            waterSalinityTypeControl: new FormControl(),
            waterTemperatureTypeControl: new FormControl(),
            systemTypeControl: new FormControl(),
            aquaticOrganismControl: new FormControl(),
            powerSupplyTypeControl: new FormControl(),
            installationTypeControl: new FormControl(),
            totalWaterAreaControl: new FormControl(),
            totalProductionCapacityControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): AquacultureFacilitiesFilters {
        const result: AquacultureFacilitiesFilters = new AquacultureFacilitiesFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            regNum: filters.getValue('regNumControl'),
            urorNum: filters.getValue('urorControl'),
            registrationDateFrom: filters.getValue<DateRangeData>('registrationDateControl')?.start,
            registrationDateTo: filters.getValue<DateRangeData>('registrationDateControl')?.end,
            name: filters.getValue('nameControl'),
            eik: filters.getValue('eikControl'),
            territoryUnitId: this.hasReadAllPermission ? filters.getValue('territoryUnitControl') : undefined,
            waterAreaTypeIds: filters.getValue('waterAreaTypeControl'),
            populatedAreaId: filters.getValue('populatedAreaControl'),
            location: filters.getValue('locationControl'),
            aquaticOrganismId: filters.getValue('aquaticOrganismControl'),
            powerSupplyTypeId: filters.getValue('powerSupplyTypeControl'),
            installationTypeIds: filters.getValue('installationTypeControl'),
            statusIds: filters.getValue('statusControl')
        });

        const waterSalinityTypes: AquacultureSalinityEnum[] | undefined = filters.getValue('waterSalinityTypeControl');
        if (waterSalinityTypes !== undefined && waterSalinityTypes !== null) {
            result.waterSalinityTypes = waterSalinityTypes.map((code: AquacultureSalinityEnum) => {
                return AquacultureSalinityEnum[code];
            });
        }

        const waterTemperatureTypes: AquacultureTemperatureEnum[] | undefined = filters.getValue('waterTemperatureTypeControl');
        if (waterTemperatureTypes !== undefined && waterTemperatureTypes !== null) {
            result.waterTemperatureTypes = waterTemperatureTypes.map((code: AquacultureTemperatureEnum) => {
                return AquacultureTemperatureEnum[code];
            });
        }

        const systemTypes: AquacultureSystemEnum[] | undefined = filters.getValue('systemTypeControl');
        if (systemTypes !== undefined && systemTypes !== null) {
            result.systemTypes = systemTypes.map((code: AquacultureSystemEnum) => {
                return AquacultureSystemEnum[code];
            });
        }

        const totalWaterArea: RangeInputData | undefined = filters.getValue<RangeInputData>('totalWaterAreaControl');
        if (totalWaterArea !== undefined && totalWaterArea !== null) {
            result.totalWaterAreaFrom = totalWaterArea.start;
            result.totalWaterAreaTo = totalWaterArea.end;
        }

        const totalProductionCapacity: RangeInputData | undefined = filters.getValue<RangeInputData>('totalProductionCapacityControl');
        if (totalProductionCapacity !== undefined && totalProductionCapacity !== null) {
            result.totalProductionCapacityFrom = totalProductionCapacity.start;
            result.totalProductionCapacityTo = totalProductionCapacity.end;
        }

        return result;
    }
}
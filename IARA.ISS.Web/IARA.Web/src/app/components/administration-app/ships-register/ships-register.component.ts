import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ApplicationForChoiceDTO } from '@app/models/generated/dtos/ApplicationForChoiceDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ShipRegisterDTO } from '@app/models/generated/dtos/ShipRegisterDTO';
import { ShipRegisterEditDTO } from '@app/models/generated/dtos/ShipRegisterEditDTO';
import { VesselTypeNomenclatureDTO } from '@app/models/generated/dtos/VesselTypeNomenclatureDTO';
import { ShipsRegisterFilters } from '@app/models/generated/filters/ShipsRegisterFilters';
import { ShipsRegisterAdministrationService } from '@app/services/administration-app/ships-register-administration.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { RangeInputData } from '@app/shared/components/input-controls/tl-range-input/range-input.component';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { ChooseApplicationComponent } from '@app/components/common-app/applications/components/choose-application/choose-application.component';
import { ChooseApplicationDialogParams } from '@app/components/common-app/applications/components/choose-application/models/choose-application-dialog-params.model';
import { EditShipComponent } from '@app/components/common-app/ships-register/edit-ship/edit-ship.component';
import { EditShipDialogParams } from '@app/components/common-app/ships-register/models/edit-ship-dialog-params.model';
import { FishingGearNomenclatureDTO } from '@app/models/generated/dtos/FishingGearNomenclatureDTO';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { FishNomenclatureDTO } from '@app/models/generated/dtos/FishNomenclatureDTO';
import { CancellationReasonDTO } from '@app/models/generated/dtos/CancellationReasonDTO';
import { CancellationReasonGroupEnum } from '@app/enums/cancellation-reason-group.enum';
import { ShipEventTypeEnum } from '@app/enums/ship-event-type.enum';

type ThreeState = 'yes' | 'no' | 'both';

@Component({
    selector: 'ships-register',
    templateUrl: './ships-register.component.html'
})
export class ShipsRegisterComponent implements OnInit, AfterViewInit {
    public readonly eventTypesEnum: typeof ShipEventTypeEnum = ShipEventTypeEnum;

    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public deletionReasons: NomenclatureDTO<number>[] = [];
    public fleetTypes: NomenclatureDTO<number>[] = [];
    public publicAidTypes: NomenclatureDTO<number>[] = [];
    public eventTypes: NomenclatureDTO<number>[] = [];
    public vesselTypes: NomenclatureDTO<number>[] = [];
    public isThirdPartyShipOptions: NomenclatureDTO<ThreeState>[] = [];
    public isCancelledOptions: NomenclatureDTO<ThreeState>[] = [];
    public isForbiddenOptions: NomenclatureDTO<ThreeState>[] = [];
    public hasVMSOptions: NomenclatureDTO<ThreeState>[] = [];
    public hasCommercialFishingLicenceOptions: NomenclatureDTO<ThreeState>[] = [];
    public fishingGear: FishingGearNomenclatureDTO[] = [];
    public shipAssociations: NomenclatureDTO<number>[] = [];
    public quotaFishes: FishNomenclatureDTO[] = [];

    public readonly canAddRecords: boolean;
    public readonly canAddThirdPartyShipRecords: boolean;
    public readonly canEditRecords: boolean;
    public readonly canReadApplications: boolean;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private grid!: DataTableManager<ShipRegisterDTO, ShipsRegisterFilters>;

    private readonly service: ShipsRegisterAdministrationService;
    private readonly nomenclatures: CommonNomenclatures;
    private readonly editDialog: TLMatDialog<EditShipComponent>;
    private readonly chooseApplicationDialog: TLMatDialog<ChooseApplicationComponent>;
    private readonly router: Router;

    public constructor(
        translate: FuseTranslationLoaderService,
        service: ShipsRegisterAdministrationService,
        nomenclatures: CommonNomenclatures,
        editDialog: TLMatDialog<EditShipComponent>,
        chooseApplicationDialog: TLMatDialog<ChooseApplicationComponent>,
        permissions: PermissionsService,
        router: Router
    ) {
        this.translate = translate;
        this.service = service;
        this.nomenclatures = nomenclatures;
        this.editDialog = editDialog;
        this.chooseApplicationDialog = chooseApplicationDialog;
        this.router = router;

        this.canAddRecords = permissions.has(PermissionsEnum.ShipsRegisterAddRecords);
        this.canAddThirdPartyShipRecords = true;
        this.canEditRecords = permissions.has(PermissionsEnum.ShipsRegisterEditRecords);
        this.canReadApplications = permissions.has(PermissionsEnum.ShipsRegisterApplicationsRead) || permissions.has(PermissionsEnum.ShipsRegisterApplicationReadAll);

        this.buildForm();
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.CancellationReasons, this.nomenclatures.getCancellationReasons.bind(this.nomenclatures), false
        ).subscribe({
            next: (reasons: CancellationReasonDTO[]) => {
                this.deletionReasons = reasons.filter(x => x.group === CancellationReasonGroupEnum.ShipCancel);
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.FleetTypes, this.service.getFleetTypes.bind(this.service), false
        ).subscribe({
            next: (types: NomenclatureDTO<number>[]) => {
                this.fleetTypes = types;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.PublicAidTypes, this.service.getPublicAidTypes.bind(this.service), false
        ).subscribe({
            next: (types: NomenclatureDTO<number>[]) => {
                this.publicAidTypes = types;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.ShipEventTypes, this.service.getEventTypes.bind(this.service), false
        ).subscribe({
            next: (types: NomenclatureDTO<number>[]) => {
                this.eventTypes = types;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.VesselTypes, this.service.getVesselTypes.bind(this.service), false
        ).subscribe({
            next: (types: VesselTypeNomenclatureDTO[]) => {
                this.vesselTypes = types;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.FishingGear, this.nomenclatures.getFishingGear.bind(this.nomenclatures), false
        ).subscribe({
            next: (gear: FishingGearNomenclatureDTO[]) => {
                this.fishingGear = gear;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Fishes, this.nomenclatures.getFishTypes.bind(this.nomenclatures), false
        ).subscribe({
            next: (fishes: FishNomenclatureDTO[]) => {
                this.quotaFishes = fishes.filter(x =>
                    x.quotas !== undefined
                    && x.quotas !== null
                    && x.quotas.filter(x => x.isActive).length !== 0);
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.ShipAssociations, this.nomenclatures.getShipAssociations.bind(this.nomenclatures), false
        ).subscribe({
            next: (assocs: NomenclatureDTO<number>[]) => {
                this.shipAssociations = assocs;
            }
        });

        this.isCancelledOptions = [
            new NomenclatureDTO<ThreeState>({
                value: undefined,
                displayName: '—',
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'yes',
                displayName: this.translate.getValue('ships-register.is-cancelled-yes'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'no',
                displayName: this.translate.getValue('ships-register.is-cancelled-no'),
                isActive: true
            })
        ];

        this.isForbiddenOptions = [
            new NomenclatureDTO<ThreeState>({
                value: undefined,
                displayName: '—',
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'yes',
                displayName: this.translate.getValue('ships-register.is-forbidden-yes'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'no',
                displayName: this.translate.getValue('ships-register.is-forbidden-no'),
                isActive: true
            })
        ];

        this.isThirdPartyShipOptions = [
            new NomenclatureDTO<ThreeState>({
                value: 'yes',
                displayName: this.translate.getValue('ships-register.third-party-ship-yes'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'no',
                displayName: this.translate.getValue('ships-register.third-party-ship-no'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'both',
                displayName: this.translate.getValue('ships-register.third-party-ship-all'),
                isActive: true
            })
        ];

        this.hasVMSOptions = [
            new NomenclatureDTO<ThreeState>({
                value: 'yes',
                displayName: this.translate.getValue('ships-register.has-vms-yes'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'no',
                displayName: this.translate.getValue('ships-register.has-vms-no'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'both',
                displayName: this.translate.getValue('ships-register.has-vms-all'),
                isActive: true
            })
        ];

        this.hasCommercialFishingLicenceOptions = [
            new NomenclatureDTO<ThreeState>({
                value: 'yes',
                displayName: this.translate.getValue('ships-register.has-commercial-fishing-licence-yes'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'no',
                displayName: this.translate.getValue('ships-register.has-commercial-fishing-licence-no'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'both',
                displayName: this.translate.getValue('ships-register.has-commercial-fishing-licence-all'),
                isActive: true
            })
        ];
    }

    public ngAfterViewInit(): void {
        this.grid = new DataTableManager<ShipRegisterDTO, ShipsRegisterFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllShips.bind(this.service),
            filtersMapper: this.mapFilters.bind(this),
            excelRequestServiceMethod: this.service.downloadShipRegisterExcel.bind(this.service),
            excelFilename: this.translate.getValue('ships-register.excel-filename')
        });

        const isPerson: boolean | undefined = window.history.state?.isPerson;
        const id: number | undefined = window.history.state?.id;

        if (!CommonUtils.isNullOrEmpty(id)) {
            if (isPerson) {
                this.grid.advancedFilters = new ShipsRegisterFilters({ personId: id });
            }
            else {
                this.grid.advancedFilters = new ShipsRegisterFilters({ legalId: id });
            }
        }

        this.grid.refreshData();
    }

    public openChooseApplicationForRegisterDialog(): void {
        this.chooseApplicationDialog.open({
            TCtor: ChooseApplicationComponent,
            title: this.translate.getValue('applications-register.choose-application-for-register-creation'),
            translteService: this.translate,
            componentData: new ChooseApplicationDialogParams({ pageCodes: [PageCodeEnum.RegVessel] }),
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
                const title: string = this.translate.getValue('ships-register.add-ship-dialog-title');

                const data = new DialogParamsModel({
                    id: undefined,
                    isApplication: false,
                    isReadonly: false,
                    viewMode: false,
                    service: this.service,
                    applicationId: dialogData.selectedApplication.id
                });

                this.openShipEditDialog(data, title, undefined);
            }
        });
    }

    public addThirdPartyShip(): void {
        const title: string = this.translate.getValue('ships-register.add-third-party-ship-dialog-title');

        const data = new EditShipDialogParams({
            id: undefined,
            isThirdPartyShip: true,
            isApplication: false,
            isReadonly: false,
            viewMode: false,
            service: this.service
        });

        this.openShipEditDialog(data, title, undefined);
    }

    public editShip(ship: ShipRegisterDTO, viewMode: boolean): void {
        this.router.navigateByUrl('/fishing-vessels/edit', {
            state: {
                id: ship.id,
                viewMode: !this.canEditRecords || viewMode,
                isThirdPartyShip: ship.isThirdPartyShip
            }
        });
    }

    public gotToApplication(ship: ShipRegisterDTO): void {
        if (this.canReadApplications) {
            this.router.navigate(['fishing-vessels-applications'], { state: { applicationId: ship.applicationId } });
        }
    }

    private buildForm(): void {
        this.form = new FormGroup({
            eventTypeControl: new FormControl(),
            eventDateControl: new FormControl(),
            cancellationReasonControl: new FormControl(),
            isCancelledControl: new FormControl(),
            isForbiddenControl: new FormControl(),
            isThirdPartyShipControl: new FormControl(),
            shipOwnerControl: new FormControl(),
            shipOwnerEgnLncControl: new FormControl(),
            fleetControl: new FormControl(),
            vesselTypeControl: new FormControl(),
            hasCommercialFishingLicenceControl: new FormControl(),
            hasVMSControl: new FormControl(),
            cfrControl: new FormControl(),
            nameControl: new FormControl(),
            externalMarkControl: new FormControl(),
            ircsCallSignControl: new FormControl(),
            publicAidTypeControl: new FormControl(),
            totalLengthControl: new FormControl(),
            grossTonnageControl: new FormControl(),
            netTonnageControl: new FormControl(),
            mainEnginePowerControl: new FormControl(),
            mainFishingGearControl: new FormControl(),
            additionalFishingGearControl: new FormControl(),
            foodLawLicenceDateControl: new FormControl(),
            allowedForQuotaFishControl: new FormControl(),
            shipAssociationControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): ShipsRegisterFilters {
        const result: ShipsRegisterFilters = new ShipsRegisterFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            eventTypeId: filters.getValue('eventTypeControl'),
            eventDateFrom: filters.getValue<DateRangeData>('eventDateControl')?.start,
            eventDateTo: filters.getValue<DateRangeData>('eventDateControl')?.end,
            isCancelled: this.threeStateToBoolean(filters.getValue<ThreeState>('isCancelledControl')),
            isForbidden: this.threeStateToBoolean(filters.getValue<ThreeState>('isForbiddenControl')),
            cancellationReasonId: filters.getValue('cancellationReasonControl'),
            isThirdPartyShip: this.threeStateToBoolean(filters.getValue<ThreeState>('isThirdPartyShipControl')),
            shipOwnerName: filters.getValue('shipOwnerControl'),
            shipOwnerEgnLnc: filters.getValue('shipOwnerEgnLncControl'),
            fleetId: filters.getValue('fleetControl'),
            vesselTypeId: filters.getValue('vesselTypeControl'),
            hasCommercialFishingLicence: this.threeStateToBoolean(filters.getValue<ThreeState>('hasCommercialFishingLicenceControl')),
            cfr: filters.getValue('cfrControl'),
            name: filters.getValue('nameControl'),
            externalMark: filters.getValue('externalMarkControl'),
            ircsCallSign: filters.getValue('ircsCallSignControl'),
            hasVMS: this.threeStateToBoolean(filters.getValue<ThreeState>('hasVMSControl')),
            publicAidTypeId: filters.getValue('publicAidTypeControl'),
            mainFishingGearId: filters.getValue('mainFishingGearControl'),
            additionalFishingGearId: filters.getValue('additionalFishingGearControl'),
            foodLawLicenceDateFrom: filters.getValue<DateRangeData>('foodLawLicenceDateControl')?.start,
            foodLawLicenceDateTo: filters.getValue<DateRangeData>('foodLawLicenceDateControl')?.end,
            allowedForQuotaFishId: filters.getValue('allowedForQuotaFishControl'),
            shipAssociationId: filters.getValue('shipAssociationControl')
        });

        const totalLength: RangeInputData | undefined = filters.getValue<RangeInputData>('totalLengthControl');
        if (totalLength !== undefined && totalLength !== null) {
            result.totalLengthFrom = totalLength.start;
            result.totalLengthTo = totalLength.end;
        }

        const grossTonnage: RangeInputData | undefined = filters.getValue('grossTonnageControl');
        if (grossTonnage !== undefined && grossTonnage !== null) {
            result.grossTonnageFrom = grossTonnage.start;
            result.grossTonnageTo = grossTonnage.end;
        }

        const netTonnage: RangeInputData | undefined = filters.getValue('netTonnageControl');
        if (netTonnage !== undefined && netTonnage !== null) {
            result.netTonnageFrom = netTonnage.start;
            result.netTonnageTo = netTonnage.end;
        }

        const mainEnginePower: RangeInputData | undefined = filters.getValue('mainEnginePowerControl');
        if (mainEnginePower !== undefined && mainEnginePower !== null) {
            result.mainEnginePowerFrom = mainEnginePower.start;
            result.mainEnginePowerTo = mainEnginePower.end;
        }

        return result;
    }

    private openShipEditDialog(data: DialogParamsModel, title: string, auditButton?: IHeaderAuditButton): void {
        const dialog = this.editDialog.open({
            title: title,
            TCtor: EditShipComponent,
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
        }, '1400px');

        dialog.subscribe({
            next: (entry: ShipRegisterEditDTO | undefined) => {
                if (entry !== undefined) {
                    this.grid.refreshData();
                }
            }
        });
    }

    private closeApplicationChooseDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private threeStateToBoolean(value: ThreeState | undefined): boolean | undefined {
        switch (value) {
            case 'yes':
                return true;
            case 'no':
                return false;
            case 'both':
            default:
                return undefined;
        }
    }
}

import { AfterViewInit, Component, ElementRef, HostListener, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn } from '@angular/forms';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { ShipEventGroupTypeEnum } from '@app/enums/ship-event-group-type.enum';
import { ShipEventTypeEnum } from '@app/enums/ship-event-type.enum';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ShipEventTypeDTO } from '@app/models/generated/dtos/ShipEventTypeDTO';
import { ShipRegisterEditDTO } from '@app/models/generated/dtos/ShipRegisterEditDTO';
import { ShipRegisterEventDTO } from '@app/models/generated/dtos/ShipRegisterEventDTO';
import { ShipsRegisterAdministrationService } from '@app/services/administration-app/ships-register-administration.service';
import { IGroupedOptions } from '@app/shared/components/input-controls/tl-autocomplete/interfaces/grouped-options.interface';
import { MessageService } from '@app/shared/services/message.service';
import { BasePageComponent } from '@app/components/common-app/base-page.component';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { ShipRegisterChangeOfCircumstancesDTO } from '@app/models/generated/dtos/ShipRegisterChangeOfCircumstancesDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ShipChangeOfCircumstancesApplicationDTO } from '@app/models/generated/dtos/ShipChangeOfCircumstancesApplicationDTO';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { ShipDeregistrationApplicationDTO } from '@app/models/generated/dtos/ShipDeregistrationApplicationDTO';
import { ShipRegisterDeregistrationDTO } from '@app/models/generated/dtos/ShipRegisterDeregistrationDTO';
import { CommercialFishingAdministrationService } from '@app/services/administration-app/commercial-fishing-administration.service';
import { FishingGearDTO } from '@app/models/generated/dtos/FishingGearDTO';
import { ShipRegisterYearlyQuotaDTO } from '@app/models/generated/dtos/ShipRegisterYearlyQuotaDTO';
import { ShipRegisterQuotaHistoryDTO } from '@app/models/generated/dtos/ShipRegisterQuotaHistoryDTO';
import { ShipRegisterCatchHistoryDTO } from '@app/models/generated/dtos/ShipRegisterCatchHistoryDTO';
import { FishingCapacityAdministrationService } from '@app/services/administration-app/fishing-capacity-administration.service';
import { IncreaseFishingCapacityDataDTO } from '@app/models/generated/dtos/IncreaseFishingCapacityDataDTO';
import { ReduceFishingCapacityDataDTO } from '@app/models/generated/dtos/ReduceFishingCapacityDataDTO';
import { ShipRegisterIncreaseCapacityDTO } from '@app/models/generated/dtos/ShipRegisterIncreaseCapacityDTO';
import { ShipRegisterReduceCapacityDTO } from '@app/models/generated/dtos/ShipRegisterReduceCapacityDTO';
import { RequestProperties } from '@app/shared/services/request-properties';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { CatchesAndSalesAdministrationService } from '@app/services/administration-app/catches-and-sales-administration.service';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { ShipRegisterLogBookPageDTO } from '@app/models/generated/dtos/ShipRegisterLogBookPageDTO';
import { ShipRegisterLogBookPagesFilters } from '@app/models/generated/filters/ShipRegisterLogBookPagesFilters';
import { EditShipLogBookPageComponent } from '@app/components/common-app/catches-and-sales/components/ship-log-book/edit-ship-log-book-page.component';
import { EditShipLogBookPageDialogParams } from '@app/components/common-app/catches-and-sales/components/ship-log-book/models/edit-ship-log-book-page-dialog-params.model';
import { MenuService } from '@app/shared/services/menu.service';
import { StatisticalFormDataDTO } from '@app/models/generated/dtos/StatisticalFormDataDTO';
import { IStatisticalFormsService } from '@app/interfaces/common-app/statistical-forms.interface';
import { StatisticalFormsAdministrationService } from '@app/services/administration-app/statistical-forms-administration.service';
import { StatisticalFormsFishVesselComponent } from '@app/components/common-app/statistical-forms/components/statistical-forms-fish-vessel/statistical-forms-fish-vessel.component';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { EditShipComponent } from '@app/components/common-app/ships-register/edit-ship/edit-ship.component';

const SHIP_DATA_TAB_INDEX: number = 0;
const FISHING_GEARS_TAB_INDEX: number = 1;
const USR_RSR_TAB_INDEX: number = 2;
const DECLARATIONS_TAB_INDEX: number = 3;
const QUOTAS_TAB_INDEX: number = 4;
const INSPECTIONS_TAB_INDEX: number = 5;
const GIVEN_POINTS_TAB_INDEX: number = 6;
const STAT_FORMS_TAB_INDEX: number = 7;

@Component({
    selector: 'edit-ship-register',
    templateUrl: './edit-ship-register.component.html',
    styleUrls: ['./edit-ship-register.component.scss']
})
export class EditShipRegisterComponent extends BasePageComponent implements OnInit, AfterViewInit, OnDestroy {
    public readonly eventTypesEnum: typeof ShipEventTypeEnum = ShipEventTypeEnum;

    public model!: ShipRegisterEditDTO;
    public selectedEventHistoryNo!: number;

    public viewMode: boolean = false;
    public isThirdPartyShip: boolean = false;
    public isChangeOfCircumstancesApplication: boolean = false;
    public isDeregistrationApplication: boolean = false;
    public isIncreaseCapacityApplication: boolean = false;
    public isReduceCapacityApplication: boolean = false;
    public service: ShipsRegisterAdministrationService;
    public capacityService: FishingCapacityAdministrationService;
    public pageCode: PageCodeEnum = PageCodeEnum.RegVessel;

    public eventTypes: IGroupedOptions<number>[] = [];

    public shipControl: FormControl = new FormControl();
    public shipId: number | undefined;

    public auditInfo: SimpleAuditDTO | undefined;

    public commercialFishingService: CommercialFishingAdministrationService;
    public fishingGearsControl: FormControl = new FormControl();
    public fishingGears: FishingGearDTO[] | undefined;

    public reloadUsrRsrData: boolean = false;
    public reloadInspectionsData: boolean = false;
    public reloadGivenPointsData: boolean = false;

    public shipQuotaControl: FormControl = new FormControl();
    public shipQuotasNomenclature: NomenclatureDTO<number>[] | undefined;
    public shipYearlyQuota: Map<number, ShipRegisterYearlyQuotaDTO> = new Map<number, ShipRegisterYearlyQuotaDTO>();
    public shipQuotaForm!: FormGroup;
    public shipQuotaHistory: ShipRegisterQuotaHistoryDTO[] = [];
    public shipQuotaCatchHistory: ShipRegisterCatchHistoryDTO[] = [];

    public statisticalForms: StatisticalFormDataDTO[] | undefined;

    public showEditsControl: FormControl = new FormControl();
    public eventTypeControl: FormControl = new FormControl(null, this.eventTypeValidator());
    public changeOfCircumstancesControl: FormControl = new FormControl();
    public deregistrationReasonControl: FormControl = new FormControl();
    public capacityChangeTonnageControl: FormControl = new FormControl();
    public capacityChangePowerControl: FormControl = new FormControl();

    public disableFieldsByEventType: ShipEventTypeEnum | undefined;

    public allEvents: ShipRegisterEventDTO[] = [];
    public events: ShipRegisterEventDTO[] = [];

    public containerHeightPx: number = 0;
    public mainPanelWidthPx: number = 0;
    public mainPanelHeightPx: number = 0;

    public isEventsPanelExpanded: boolean = true;
    public isEventsPanelOpen: boolean = true;
    public disableCompleteApplicationButton: boolean = true;

    public disableTabs: boolean = true;

    public readonly hasCatchesAndSalesReadPermission: boolean;
    public readonly hasStatisticalFormReadPermission: boolean;
    public readonly canSendFluxData: boolean;

    public getEventTypeErrorTextMethod: GetControlErrorLabelTextCallback = this.getEventTypeErrorText.bind(this);

    @ViewChild('shipRef')
    private editShipRef!: EditShipComponent;

    @ViewChild('logbookPagesTable')
    private logbookPagesTable!: TLDataTableComponent;

    private host: HTMLElement;
    private toolbarElement!: HTMLElement;
    private containerElement!: HTMLElement;
    private eventsPanelElement!: HTMLElement;
    private cocPanelElement!: HTMLElement;

    private logbookPagesGrid!: DataTableManager<ShipRegisterLogBookPageDTO, ShipRegisterLogBookPagesFilters>;
    private logbookPageEditDialog: TLMatDialog<EditShipLogBookPageComponent>;
    private initialDeclarationsLoaded: boolean = false;

    private statisticalFormEditDialog: TLMatDialog<StatisticalFormsFishVesselComponent>;

    private translate: FuseTranslationLoaderService;
    private router: Router;
    private confirmDialog: TLConfirmDialog;
    private snackbar: MatSnackBar;
    private menuService: MenuService;
    private catchesAndSalesService: ICatchesAndSalesService;
    private statisticalFormsService: IStatisticalFormsService;

    private shipsCache: Map<number, ShipRegisterEditDTO> = new Map<number, ShipRegisterEditDTO>();
    private temporaryShipsCache: Map<number, ShipRegisterEditDTO> = new Map<number, ShipRegisterEditDTO>();

    private applicationId: number | undefined;
    private shipModelChanged: boolean = false;

    private static uniqueId: number = 0;

    public constructor(
        service: ShipsRegisterAdministrationService,
        capacityService: FishingCapacityAdministrationService,
        commercialFishingService: CommercialFishingAdministrationService,
        translate: FuseTranslationLoaderService,
        router: Router,
        confirmDialog: TLConfirmDialog,
        snackbar: MatSnackBar,
        logbookPageEditDialog: TLMatDialog<EditShipLogBookPageComponent>,
        statisticalFormEditDialog: TLMatDialog<StatisticalFormsFishVesselComponent>,
        catchesAndSalesService: CatchesAndSalesAdministrationService,
        statisticalFormsService: StatisticalFormsAdministrationService,
        permissions: PermissionsService,
        messageService: MessageService,
        menuService: MenuService,
        host: ElementRef
    ) {
        super(messageService);

        this.service = service;
        this.capacityService = capacityService;
        this.commercialFishingService = commercialFishingService;
        this.translate = translate;
        this.router = router;
        this.confirmDialog = confirmDialog;
        this.snackbar = snackbar;
        this.menuService = menuService;
        this.logbookPageEditDialog = logbookPageEditDialog;
        this.statisticalFormEditDialog = statisticalFormEditDialog;
        this.catchesAndSalesService = catchesAndSalesService;
        this.statisticalFormsService = statisticalFormsService;

        this.hasCatchesAndSalesReadPermission = permissions.hasAny(PermissionsEnum.FishLogBookPageReadAll, PermissionsEnum.FishLogBookPageRead);
        this.hasStatisticalFormReadPermission = permissions.hasAny(PermissionsEnum.StatisticalFormsFishVesselReadAll, PermissionsEnum.StatisticalFormsFishVesselRead);
        this.canSendFluxData = permissions.has(PermissionsEnum.ShipsRegisterSendFluxData);

        this.host = host.nativeElement as HTMLElement;

        this.setupRouteParameters();
        this.setTitle();
    }

    public async ngOnInit(): Promise<void> {
        const types: ShipEventTypeDTO[] = await NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.ShipEventTypes, this.service.getEventTypes.bind(this.service), false
        ).toPromise();

        const map = new Map<string, NomenclatureDTO<number>[]>();

        for (const type of types) {
            const value: NomenclatureDTO<number>[] | undefined = map.get(type.groupName!);
            if (value !== undefined) {
                value.push(type);
            }
            else {
                map.set(type.groupName!, [type]);
            }
        }

        for (const [group, types] of map) {
            this.eventTypes.push({
                name: group,
                options: types
            });
        }
        this.eventTypes = this.eventTypes.slice();

        if (this.isChangeOfCircumstancesApplication && this.applicationId !== undefined && this.applicationId !== null) {
            this.service.getShipFromChangeOfCircumstancesApplication(this.applicationId).subscribe({
                next: (ship: ShipRegisterEditDTO) => {
                    this.shipId = ship.id;

                    if (this.shipId !== undefined && this.shipId !== null) {
                        this.getShipEventHistory(this.shipId);
                    }
                }
            });

            this.service.getApplication(this.applicationId, false, PageCodeEnum.ShipRegChange).subscribe({
                next: (application: ShipChangeOfCircumstancesApplicationDTO) => {
                    this.changeOfCircumstancesControl.setValue(application.changes);
                    this.changeOfCircumstancesControl.disable();
                }
            });
        }
        else if (this.isDeregistrationApplication && this.applicationId !== undefined && this.applicationId !== null) {
            this.service.getShipFromDeregistrationApplication(this.applicationId).subscribe({
                next: (ship: ShipRegisterEditDTO) => {
                    this.shipId = ship.id;

                    if (this.shipId !== undefined && this.shipId !== null) {
                        this.getShipEventHistory(this.shipId);
                    }
                }
            });

            this.service.getApplication(this.applicationId, false, PageCodeEnum.DeregShip).subscribe({
                next: (application: ShipDeregistrationApplicationDTO) => {
                    this.deregistrationReasonControl.setValue(application.deregistrationReason);
                    this.deregistrationReasonControl.disable();
                }
            });
        }
        else if (this.isIncreaseCapacityApplication && this.applicationId !== undefined && this.applicationId !== null) {
            this.service.getShipFromIncreaseCapacityApplication(this.applicationId).subscribe({
                next: (ship: ShipRegisterEditDTO) => {
                    this.shipId = ship.id;

                    if (this.shipId !== undefined && this.shipId !== null) {
                        this.getShipEventHistory(this.shipId);
                    }
                }
            });

            this.capacityService.getCapacityDataFromApplication(this.applicationId, PageCodeEnum.IncreaseFishCap).subscribe({
                next: (data: IncreaseFishingCapacityDataDTO) => {
                    this.capacityChangePowerControl.setValue(data.newPower!.toFixed(2));
                    this.capacityChangePowerControl.disable();

                    this.capacityChangeTonnageControl.setValue(data.newTonnage!.toFixed(2));
                    this.capacityChangeTonnageControl.disable();
                }
            });
        }
        else if (this.isReduceCapacityApplication && this.applicationId !== undefined && this.applicationId !== null) {
            this.service.getShipFromReduceCapacityApplication(this.applicationId).subscribe({
                next: (ship: ShipRegisterEditDTO) => {
                    this.shipId = ship.id;

                    if (this.shipId !== undefined && this.shipId !== null) {
                        this.getShipEventHistory(this.shipId);
                    }
                }
            });

            this.capacityService.getCapacityDataFromApplication(this.applicationId, PageCodeEnum.ReduceFishCap).subscribe({
                next: (data: ReduceFishingCapacityDataDTO) => {
                    this.capacityChangePowerControl.setValue(data.newPower!.toFixed(2));
                    this.capacityChangePowerControl.disable();

                    this.capacityChangeTonnageControl.setValue(data.newTonnage!.toFixed(2));
                    this.capacityChangeTonnageControl.disable();
                }
            });
        }
        else if (this.shipId !== undefined && this.shipId !== null) {
            this.getShipEventHistory(this.shipId);
        }

        this.shipQuotaForm = new FormGroup({
            quotaKgControl: new FormControl({ value: null, disabled: true }),
            totalCatchControl: new FormControl({ value: null, disabled: true }),
            leftoverQuotaKgControl: new FormControl({ value: null, disabled: true })
        });
    }

    public ngAfterViewInit(): void {
        this.calculateContainerHeightPx();
        this.calculateMainPanelWidthPx();

        this.menuService.folded.subscribe({
            next: () => {
                setTimeout(() => {
                    this.calculateMainPanelWidthPx();
                });
            }
        });

        this.logbookPagesGrid = new DataTableManager<ShipRegisterLogBookPageDTO, ShipRegisterLogBookPagesFilters>({
            tlDataTable: this.logbookPagesTable,
            searchPanel: undefined,
            requestServiceMethod: this.service.getShipLogBookPages.bind(this.service),
            filtersMapper: this.mapLogBookPagesFilters.bind(this)
        });

        this.shipControl.valueChanges.subscribe({
            next: (ship: ShipRegisterEditDTO) => {
                this.model = ship;
                this.shipModelChanged = true;

                this.disableTabs = this.model === undefined || this.model === null;
            }
        });

        this.showEditsControl.valueChanges.subscribe({
            next: (yes: boolean) => {
                this.filterEvents(yes);
            }
        });

        this.eventTypeControl.valueChanges.subscribe({
            next: (type: NomenclatureDTO<number>) => {
                if (type !== undefined && type !== null) {
                    this.disableFieldsByEventType = ShipEventTypeEnum[type.code as keyof typeof ShipEventTypeEnum];
                }
                else {
                    this.disableFieldsByEventType = undefined;
                }
                this.shipModelChanged = false;
            }
        });

        this.shipQuotaControl.valueChanges.subscribe({
            next: (value: NomenclatureDTO<number> | undefined) => {
                if (value !== undefined && value !== null) {
                    const quota: ShipRegisterYearlyQuotaDTO | undefined = this.shipYearlyQuota.get(value.value!);

                    if (quota === undefined) {
                        this.service.getShipYearlyQuota(value.value!).subscribe({
                            next: (quota: ShipRegisterYearlyQuotaDTO) => {
                                this.shipYearlyQuota.set(quota.catchQuotaId!, quota);
                                this.fillQuotaForm(quota);
                            }
                        });
                    }
                    else {
                        this.fillQuotaForm(quota);
                    }
                }
            }
        });
    }

    public ngOnDestroy(): void {
        super.ngOnDestroy();
    }

    @HostListener('window:resize')
    public onWindowResize(): void {
        this.calculateContainerHeightPx();
        this.calculateMainPanelWidthPx();
    }

    public onCocPanelResized(height: number): void {
        this.calculateMainPanelHeightPx();
    }

    public viewHistoryShip(event: ShipRegisterEventDTO): void {
        this.selectedEventHistoryNo = event.no!;

        if (this.isCurrentEventHistorySelected()) {
            this.eventTypeControl.enable();
        }
        else {
            this.eventTypeControl.setValue(undefined);
            this.eventTypeControl.disable();
        }

        if (event.isTemporary === true) {
            const ship: ShipRegisterEditDTO = this.temporaryShipsCache.get(event.shipId!)!;
            this.shipControl.setValue(ship);
            this.setTitle();
        }
        else {
            const ship: ShipRegisterEditDTO | undefined = this.shipsCache.get(event.shipId!);
            if (ship !== undefined) {
                this.shipControl.setValue(ship);
                this.setTitle();
            }
            else {
                this.shipControl.setValue(event.shipId);
            }
        }
    }

    public onShipDataLoaded(ship: ShipRegisterEditDTO): void {
        this.shipsCache.set(ship.id!, ship);
        this.setTitle();
    }

    public toggleEventsPanel(): void {
        this.isEventsPanelExpanded = !this.isEventsPanelExpanded;

        if (this.isEventsPanelOpen) {
            this.isEventsPanelOpen = false;
        }
        else {
            setTimeout(() => {
                this.isEventsPanelOpen = true;
            }, 180);
        }
    }

    public onEventsPanelToggled(event: TransitionEvent): void {
        if (event.propertyName === 'width') {
            this.calculateMainPanelWidthPx();
        }
    }

    public saveEvent(saveChanges: boolean): void {
        if (this.shipModelChanged === true && this.isCurrentEventHistorySelected()) {
            if (this.eventTypeControl.valid) {
                const event: NomenclatureDTO<number> = this.eventTypeControl.value;

                if (event !== undefined && event !== null && event.value !== undefined && event.value !== null) {
                    this.shipControl.markAllAsTouched();
                    this.editShipRef.runValidityChecker();

                    if (this.shipControl.valid) {
                        this.model.eventType = ShipEventTypeEnum[event.code as keyof typeof ShipEventTypeEnum];

                        if (saveChanges === true) {
                            if (this.model.eventType === ShipEventTypeEnum.RET || this.model.eventType === ShipEventTypeEnum.DES || this.model.eventType === ShipEventTypeEnum.EXP) {
                                this.model.isNoApplicationDeregistration = !this.isApplication();
                            }

                            if (this.model.isNoApplicationDeregistration && this.allEvents[0].usrRsr === true) {
                                this.confirmDialog.open({
                                    title: this.translate.getValue('ships-register.deregistering-ship-with-rsr'),
                                    message: this.translate.getValue('ships-register.deregistering-ship-with-rsr-message'),
                                    okBtnLabel: this.translate.getValue('ships-register.deregistering-ship-with-rsr-ok-btn-label'),
                                    hasCancelButton: false
                                }).subscribe();
                            }
                            else {
                                if (this.isThirdPartyShip) {
                                    this.service.editThirdPartyShip(this.model).subscribe({
                                        next: () => {
                                            this.shipSavedHandler();
                                            NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.Ships);
                                        }
                                    });
                                }
                                else {
                                    this.service.editShip(this.model).subscribe({
                                        next: () => {
                                            this.shipSavedHandler();
                                            NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.Ships);
                                        }
                                    });
                                }

                                this.showEditsControl.setValue(this.model.eventType === ShipEventTypeEnum.EDIT);
                            }
                        }
                        else {
                            this.allEvents.unshift(new ShipRegisterEventDTO({
                                shipId: EditShipRegisterComponent.uniqueId++,
                                date: new Date(),
                                type: ShipEventTypeEnum[this.model.eventType!],
                                no: this.allEvents.length + 1,
                                usrRsr: this.allEvents[0].usrRsr,
                                isTemporary: true
                            }));

                            if (this.model.eventType === ShipEventTypeEnum.RET || this.model.eventType === ShipEventTypeEnum.DES || this.model.eventType === ShipEventTypeEnum.EXP) {
                                this.model.isNoApplicationDeregistration = !this.isApplication();
                            }

                            this.temporaryShipsCache.set(this.allEvents[0].shipId!, this.model);

                            setTimeout(() => {
                                this.events = this.allEvents;

                                this.showEditsControl.setValue(this.model.eventType === ShipEventTypeEnum.EDIT);
                                this.filterEventTypes();

                                this.resetEdit();
                                this.viewHistoryShip(this.allEvents[0]);

                                this.disableCompleteApplicationButton = false;
                            });
                        }
                    }
                }
            }
        }
    }

    public completeChangesApplication(): void {
        if (this.temporaryShipsCache.size > 0) {
            let title: string = '';
            let message: string = '';
            let okBtnLabel: string = '';

            if (this.isChangeOfCircumstancesApplication) {
                title = this.translate.getValue('ships-register.save-changes-for-application-title');
                message = this.translate.getValue('ships-register.save-changes-for-application-message');
                okBtnLabel = this.translate.getValue('ships-register.save-changes-btn');
            }
            else if (this.isDeregistrationApplication) {
                title = this.translate.getValue('ships-register.save-changes-for-application-title-dereg');
                message = this.translate.getValue('ships-register.save-changes-for-application-message-dereg');
                okBtnLabel = this.translate.getValue('ships-register.save-changes-btn-dereg');
            }
            else if (this.isIncreaseCapacityApplication) {
                title = this.translate.getValue('ships-register.save-changes-for-application-title-increase-capacity');
                message = this.translate.getValue('ships-register.save-changes-for-application-message-increase-capacity');
                okBtnLabel = this.translate.getValue('ships-register.save-changes-btn-increase-capacity');
            }
            else if (this.isReduceCapacityApplication) {
                title = this.translate.getValue('ships-register.save-changes-for-application-title-reduce-capacity');
                message = this.translate.getValue('ships-register.save-changes-for-application-message-reduce-capacity');
                okBtnLabel = this.translate.getValue('ships-register.save-changes-btn-reduce-capacity');
            }

            this.confirmDialog.open({
                title: title,
                message: message,
                okBtnLabel: okBtnLabel
            }).subscribe({
                next: (ok: boolean) => {
                    if (ok) {
                        const events = Array.from(this.temporaryShipsCache.values()).sort((lhs: ShipRegisterEditDTO, rhs: ShipRegisterEditDTO) => {
                            if (lhs.eventDate! < rhs.eventDate!) {
                                return -1;
                            }
                            if (lhs.eventDate! > rhs.eventDate!) {
                                return 1;
                            }
                            return 0;
                        });

                        if (this.isChangeOfCircumstancesApplication) {
                            const ships: ShipRegisterChangeOfCircumstancesDTO = new ShipRegisterChangeOfCircumstancesDTO({
                                ships: events,
                                applicationId: this.applicationId
                            });

                            this.service.completeChangeOfCircumstancesApplication(ships).subscribe({
                                next: () => {
                                    this.router.navigateByUrl('/fishing-vessels');
                                }
                            });
                        }
                        else if (this.isDeregistrationApplication) {
                            if (this.allEvents[0].usrRsr === true) {
                                this.confirmDialog.open({
                                    title: this.translate.getValue('ships-register.deregistering-ship-with-rsr'),
                                    message: this.translate.getValue('ships-register.deregistering-ship-with-rsr-message'),
                                    okBtnLabel: this.translate.getValue('ships-register.deregistering-ship-with-rsr-ok-btn-label'),
                                    hasCancelButton: false
                                }).subscribe();
                            }
                            else {
                                const ships: ShipRegisterDeregistrationDTO = new ShipRegisterDeregistrationDTO({
                                    ships: events,
                                    applicationId: this.applicationId
                                });

                                this.service.completeShipDeregistrationApplication(ships).subscribe({
                                    next: () => {
                                        this.router.navigateByUrl('/fishing-vessels');
                                    }
                                });
                            }
                        }
                        else if (this.isIncreaseCapacityApplication) {
                            const ships: ShipRegisterIncreaseCapacityDTO = new ShipRegisterIncreaseCapacityDTO({
                                ships: events,
                                applicationId: this.applicationId
                            });

                            if (this.tonnageAndPowerNotMatchingWithApplication()) {
                                this.openTonnageAndPowerNotMatchingSnackbar();
                            }
                            else {
                                this.service.completeShipIncreaseCapacityApplication(ships).subscribe({
                                    next: () => {
                                        this.router.navigateByUrl('/fishing-vessels');
                                    }
                                });
                            }
                        }
                        else if (this.isReduceCapacityApplication) {
                            const ships: ShipRegisterReduceCapacityDTO = new ShipRegisterReduceCapacityDTO({
                                ships: events,
                                applicationId: this.applicationId
                            });

                            if (this.tonnageAndPowerNotMatchingWithApplication()) {
                                this.openTonnageAndPowerNotMatchingSnackbar();
                            }
                            else {
                                this.service.completeShipReduceCapacityApplication(ships).subscribe({
                                    next: () => {
                                        this.router.navigateByUrl('/fishing-vessels');
                                    }
                                });
                            }
                        }
                    }
                }
            });
        }
    }

    public cancelChangesApplication(): void {
        if (this.isChangeOfCircumstancesApplication || this.isDeregistrationApplication) {
            this.router.navigateByUrl('/fishing-vessels-applications');
        }
        else if (this.isIncreaseCapacityApplication || this.isReduceCapacityApplication) {
            this.router.navigateByUrl('/fishing-capacity-applications');
        }
    }

    public getEventTypeErrorText(controlName: string, errorValue: unknown, errorCode: string): TLError | undefined {
        if (errorCode === 'moreThanOneModPerDay') {
            if (errorValue === true) {
                return new TLError({ text: this.translate.getValue('ships-register.more-than-one-mod-error'), type: 'error' });
            }
        }
        return undefined;
    }

    public onTabChange(index: number): void {
        if (index === FISHING_GEARS_TAB_INDEX) {
            if (this.fishingGears === undefined) {
                this.service.getShipFishingGears(this.model.shipUID!).subscribe({
                    next: (result: FishingGearDTO[]) => {
                        this.fishingGears = result;
                        this.fishingGearsControl.setValue(this.fishingGears);
                    }
                });
            }
        }
        else if (index === DECLARATIONS_TAB_INDEX) {
            if (this.initialDeclarationsLoaded === false) {
                this.logbookPagesGrid.advancedFilters = new ShipRegisterLogBookPagesFilters({
                    shipUID: this.model.shipUID
                });

                this.logbookPagesGrid.refreshData();
                this.initialDeclarationsLoaded = true;
            }
        }
        else if (index === USR_RSR_TAB_INDEX) {
            if (this.reloadUsrRsrData === false) {
                this.reloadUsrRsrData = true;
            }
        }
        else if (index === INSPECTIONS_TAB_INDEX) {
            if (this.reloadInspectionsData === false) {
                this.reloadInspectionsData = true;
            }
        }
        else if (index === GIVEN_POINTS_TAB_INDEX) {
            if (this.reloadGivenPointsData === false) {
                this.reloadGivenPointsData = true;
            }
        }
        else if (index === QUOTAS_TAB_INDEX) {
            if (this.shipQuotasNomenclature === undefined) {
                this.service.getShipCatchQuotaNomenclatures(this.model.shipUID!).subscribe({
                    next: (result: NomenclatureDTO<number>[]) => {
                        this.shipQuotasNomenclature = result;
                        this.shipQuotaControl.setValue(this.shipQuotasNomenclature[0]);
                    }
                });
            }
        }
        else if (index === STAT_FORMS_TAB_INDEX) {
            if (this.statisticalForms === undefined) {
                this.service.getShipStatisticalForms(this.model.shipUID!).subscribe({
                    next: (result: StatisticalFormDataDTO[]) => {
                        this.statisticalForms = result;
                    }
                });
            }
        }
    }

    public viewShipLogBookPage(page: ShipRegisterLogBookPageDTO): void {
        this.logbookPageEditDialog.openWithTwoButtons({
            title: this.translate.getValue('ships-register.ship-declarations-view-declaration-dialog-title'),
            TCtor: EditShipLogBookPageComponent,
            translteService: this.translate,
            headerCancelButton: {
                cancelBtnClicked: this.closeViewShipLogBookPageDialog.bind(this)
            },
            headerAuditButton: {
                id: page.id!,
                tableName: 'CatchSales.ShipLogBookPages',
                getAuditRecordData: this.service.getShipLogBookPageSimpleAudit.bind(this.service)
            },
            componentData: new EditShipLogBookPageDialogParams({
                id: page.id,
                service: this.catchesAndSalesService,
                viewMode: true
            }),
            viewMode: true,
            disableDialogClose: false
        }, '1450px').subscribe();
    }

    public viewStatisticalFormFishVessel(form: StatisticalFormDataDTO): void {
        this.statisticalFormEditDialog.openWithTwoButtons({
            title: this.translate.getValue('ships-register.ship-stat-form-view-form-dialog-title'),
            TCtor: StatisticalFormsFishVesselComponent,
            translteService: this.translate,
            headerCancelButton: {
                cancelBtnClicked: this.closeViewStatisticalFormDialog.bind(this)
            },
            componentData: new DialogParamsModel({
                id: form.id,
                isApplication: false,
                isReadonly: true,
                viewMode: true,
                service: this.statisticalFormsService
            }),
            viewMode: true,
            disableDialogClose: false
        }, '1400px').subscribe();
    }

    public auditBtnClicked(): void {
        if (this.model?.id !== undefined && this.model?.id !== null) {
            this.service.getSimpleAudit(this.model.id).subscribe({
                next: (audit: SimpleAuditDTO) => {
                    this.auditInfo = audit;
                }
            });
        }
    }

    public detailedAuditBtnClicked(): void {
        if (this.model?.id !== undefined && this.model?.id !== null) {
            this.router.navigateByUrl('/system-log', {
                state: {
                    tableId: this.model.id.toString(),
                    tableName: 'RShips.ShipRegister'
                }
            });
        }
    }

    public sendEventVcdToFlux(): void {
        if (this.canSendFluxData) {
            this.confirmDialog.open({
                title: `${this.translate.getValue('ships-register.send-event-vcd-to-flux-confirmation-title')}`,
                message: this.translate.getValue('ships-register.send-event-vcd-to-flux-confirmation-message'),
                okBtnLabel: this.translate.getValue('ships-register.send-event-vcd-to-flux-confirmation-ok-btn')
            }).subscribe({
                next: (yes: boolean) => {
                    if (yes) {
                        this.service.reportShipVCDToFlux(this.model.id!).subscribe({
                            next: () => {
                                // nothing to do
                            }
                        });
                    }
                }
            });
        }
    }

    public sendEventVedToFlux(): void {
        if (this.canSendFluxData) {
            this.confirmDialog.open({
                title: `${this.translate.getValue('ships-register.send-event-ved-to-flux-confirmation-title')}`,
                message: this.translate.getValue('ships-register.send-event-ved-to-flux-confirmation-message'),
                okBtnLabel: this.translate.getValue('ships-register.send-event-ved-to-flux-confirmation-ok-btn')
            }).subscribe({
                next: (yes: boolean) => {
                    if (yes) {
                        this.service.reportShipVEDToFlux(this.model.id!).subscribe({
                            next: () => {
                                // nothing to do
                            }
                        });
                    }
                }
            });
        }
    }

    public sendHistoryToFlux(): void {
        if (this.canSendFluxData) {
            this.confirmDialog.open({
                title: `${this.translate.getValue('ships-register.send-history-to-flux-confirmation-title')} ${this.model.name}`,
                message: this.translate.getValue('ships-register.send-history-to-flux-confirmation-message'),
                okBtnLabel: this.translate.getValue('ships-register.send-history-to-flux-confirmation-ok-btn')
            }).subscribe({
                next: (yes: boolean) => {
                    if (yes) {
                        this.service.reportShipHistoryToFlux(this.model.id!).subscribe({
                            next: () => {
                                // nothing to do
                            }
                        });
                    }
                }
            });
        }
    }

    private calculateContainerHeightPx(): void {
        if (!this.toolbarElement) {
            this.toolbarElement = document.getElementsByTagName('toolbar').item(0) as HTMLElement;
        }

        this.containerHeightPx = window.innerHeight - this.toolbarElement.offsetHeight;

        this.calculateMainPanelHeightPx();
    }

    private calculateMainPanelWidthPx(): void {
        if (!this.containerElement) {
            this.containerElement = this.host.getElementsByClassName('container').item(0) as HTMLElement;
        }

        if (!this.eventsPanelElement) {
            this.eventsPanelElement = this.host.getElementsByClassName('events-panel').item(0) as HTMLElement;
        }

        this.mainPanelWidthPx = this.containerElement.offsetWidth - this.eventsPanelElement.offsetWidth;
    }

    private calculateMainPanelHeightPx(): void {
        if (!this.cocPanelElement) {
            this.cocPanelElement = this.host.getElementsByClassName('change-of-circumstances-panel').item(0) as HTMLElement;
        }

        if (this.cocPanelElement) {
            this.mainPanelHeightPx = this.containerHeightPx - this.cocPanelElement.offsetHeight;
        }
        else {
            this.mainPanelHeightPx = this.containerHeightPx;
        }
    }

    private closeViewShipLogBookPageDialog(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeViewStatisticalFormDialog(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private fillQuotaForm(quota: ShipRegisterYearlyQuotaDTO): void {
        this.shipQuotaForm.get('quotaKgControl')!.setValue(quota.quotaKg);
        this.shipQuotaForm.get('totalCatchControl')!.setValue(quota.totalCatch);
        this.shipQuotaForm.get('leftoverQuotaKgControl')!.setValue(quota.leftoverQuotaKg);

        setTimeout(() => {
            this.shipQuotaHistory = quota.quotaHistory ?? [];
            this.shipQuotaCatchHistory = quota.catchHistory ?? [];
        });
    }

    private shipSavedHandler(): void {
        this.resetEdit();
        this.getShipEventHistory(this.model.id!);
    }

    private resetEdit(): void {
        this.shipModelChanged = false;
        this.eventTypeControl.setValue(undefined);
        this.shipControl.markAsUntouched();
        this.shipControl.updateValueAndValidity({ emitEvent: false, onlySelf: false });
    }

    private getShipEventHistory(shipId: number): void {
        this.service.getShipEventHistory(shipId).subscribe({
            next: (events: ShipRegisterEventDTO[]) => {
                setTimeout(() => {
                    this.allEvents = this.events = events;

                    if (this.allEvents[0].type === ShipEventTypeEnum[ShipEventTypeEnum.EDIT]) {
                        this.showEditsControl.setValue(true);
                        this.filterEvents(true);
                    }
                    else {
                        this.showEditsControl.setValue(false);
                        this.filterEvents(false);
                    }
                    this.filterEventTypes();

                    this.viewHistoryShip(this.allEvents[0]);
                });
            }
        });
    }

    private filterEvents(showEdits: boolean): void {
        if (showEdits) {
            this.events = this.allEvents.slice();
        }
        else {
            this.events = this.allEvents.filter(x => x.type !== ShipEventGroupTypeEnum[ShipEventGroupTypeEnum.EDIT]);
        }
    }

    private filterEventTypes(): void {
        const unapplicableTypes: string[] = [
            ShipEventTypeEnum[ShipEventTypeEnum.CST],
            ShipEventTypeEnum[ShipEventTypeEnum.CEN],
            ShipEventTypeEnum[ShipEventTypeEnum.CHA],
            ShipEventTypeEnum[ShipEventTypeEnum.IMP]
        ];

        // при унищожен кораб, не може да се прави нищо
        if (this.allEvents[0].type === ShipEventTypeEnum[ShipEventTypeEnum.DES]) {
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.EDIT]);
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.MOD]);
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.EXP]);
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.RET]);
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.DES]);
        }
        // при отписан кораб, не може да бъде отписан отново и не могат да се правят модификации
        else if (this.allEvents[0].type === ShipEventTypeEnum[ShipEventTypeEnum.RET] || this.allEvents[0].type === ShipEventTypeEnum[ShipEventTypeEnum.EXP]) {
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.MOD]);
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.DES]);
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.RET]);
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.EXP]);
        }
        // при заявление за промяна в обстоятелствата не може да се отпише кораб
        else if (this.isChangeOfCircumstancesApplication) {
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.EDIT]);
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.DES]);
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.RET]);
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.EXP]);
        }
        // при заявление за отписване не може да се правят модификации, а само събития за отписване
        else if (this.isDeregistrationApplication) {
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.EDIT]);
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.MOD]);
        }
        // при заявления за увеличаване/намаляване на капацитет не може да се отпише кораб
        else if (this.isIncreaseCapacityApplication || this.isReduceCapacityApplication) {
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.EDIT]);
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.EXP]);
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.RET]);
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.DES]);
        }
        // при кораб, не от трети страни, не може да се прави EXP събитие (служебно отписване) и модификация извън заявление
        else if (!this.isThirdPartyShip) {
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.EXP]);
            unapplicableTypes.push(ShipEventTypeEnum[ShipEventTypeEnum.MOD]);
        }

        for (const group of this.eventTypes) {
            group.options = (group.options as NomenclatureDTO<number>[]).filter(x => !unapplicableTypes.includes(x.code!));
        }

        this.eventTypes = this.eventTypes.filter((group: IGroupedOptions<number>) => {
            const options: NomenclatureDTO<number>[] = group.options as NomenclatureDTO<number>[];
            return options.length > 0 && !options.some(y => y.isActive === false);
        });
    }

    private setupRouteParameters(): void {
        const state: {
            id: number,
            applicationId: number,
            viewMode: boolean,
            isThirdPartyShip: boolean,
            isChangeOfCircumstancesApplication: boolean,
            isDeregistrationApplication: boolean,
            isIncreaseCapacityApplication: false,
            isReduceCapacityApplication: false
        } = window.history.state;

        this.shipId = state.id;
        this.applicationId = state.applicationId;
        this.viewMode = state.viewMode;
        this.isThirdPartyShip = state.isThirdPartyShip;
        this.isChangeOfCircumstancesApplication = state.isChangeOfCircumstancesApplication;
        this.isDeregistrationApplication = state.isDeregistrationApplication;
        this.isIncreaseCapacityApplication = state.isIncreaseCapacityApplication;
        this.isReduceCapacityApplication = state.isReduceCapacityApplication;

        if (this.isChangeOfCircumstancesApplication) {
            this.pageCode = PageCodeEnum.ShipRegChange;
        }
        else if (this.isDeregistrationApplication) {
            this.pageCode = PageCodeEnum.DeregShip;
        }

        if (this.shipId === undefined || this.shipId === null) {
            if (!this.showBottomPanel()) {
                this.router.navigate(['/fishing-vessels']);
            }
            else if (this.applicationId === undefined || this.applicationId === null) {
                this.router.navigate(['/fishing-vessels-applications']);
            }
        }
    }

    private isApplication(): boolean {
        return this.isChangeOfCircumstancesApplication
            || this.isDeregistrationApplication
            || this.isIncreaseCapacityApplication
            || this.isReduceCapacityApplication
            || (this.applicationId !== undefined && this.applicationId !== null);
    }

    private setTitle(): void {
        setTimeout(() => {
            let title: string = this.viewMode
                ? this.translate.getValue('ships-register.view-ship-dialog-title')
                : this.translate.getValue('ships-register.edit-ship-dialog-title');

            if (this.model !== undefined && this.model !== null) {
                title = `${title} – ${this.translate.getValue('ships-register.title-name')}: ${this.model.name}`;

                if (this.model.cfr !== undefined && this.model.cfr !== null) {
                    title = `${title} │ ${this.translate.getValue('ships-register.title-cfr')}: ${this.model.cfr}`;
                }
                else {
                    title = `${title} │ ${this.translate.getValue('ships-register.title-external-mark')}: ${this.model.externalMark}`;
                }
            }

            this.messageService.sendMessage(title);
        });
    }

    private isCurrentEventHistorySelected(): boolean {
        return this.selectedEventHistoryNo === this.allEvents.length;
    }

    private eventTypeValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.eventTypeControl) {
                const event: NomenclatureDTO<number> = this.eventTypeControl.value;

                if (event !== undefined && event !== null && event.value !== undefined && event.value !== null) {
                    if (event.code !== ShipEventTypeEnum[ShipEventTypeEnum.EDIT]) {
                        const lastEvent: ShipRegisterEventDTO | undefined = this.allEvents.find(x => x.type !== ShipEventTypeEnum[ShipEventTypeEnum.EDIT]);

                        if (lastEvent) {
                            const now: Date = new Date();

                            if (this.datesEqual(lastEvent.date!, now)) {
                                if (event.code !== lastEvent.type) {
                                    return { moreThanOneModPerDay: true };
                                }
                            }
                        }
                    }
                }
            }
            return null;
        };
    }

    private showBottomPanel(): boolean {
        return this.isChangeOfCircumstancesApplication || this.isDeregistrationApplication || this.isIncreaseCapacityApplication || this.isReduceCapacityApplication;
    }

    private tonnageAndPowerNotMatchingWithApplication(): boolean {
        const tonnage: number = Number(this.capacityChangeTonnageControl.value);
        const power: number = Number(this.capacityChangePowerControl.value);

        return tonnage !== Number(this.model.grossTonnage) || power !== Number(this.model.mainEnginePower)
    }

    private openTonnageAndPowerNotMatchingSnackbar(): void {
        this.snackbar.open(
            this.translate.getValue('ships-register.tonnage-and-power-not-matching-for-application-error-msg'),
            undefined,
            {
                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
            }
        );
    }

    private mapLogBookPagesFilters(filters: FilterEventArgs): ShipRegisterLogBookPagesFilters {
        const result = new ShipRegisterLogBookPagesFilters({
            freeTextSearch: undefined,
            showInactiveRecords: false,
            shipUID: this.model.shipUID
        });

        return result;
    }

    private datesEqual(lhs: Date, rhs: Date): boolean {
        return lhs.getFullYear() === rhs.getFullYear()
            && lhs.getMonth() === rhs.getMonth()
            && lhs.getDate() === rhs.getDate();
    }
}
import { AfterViewInit, Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { EditDialogInfo } from '@app/components/common-app/applications/models/edit-dialog-info.model';
import { InspectionStatesEnum } from '@app/enums/inspection-states.enum';
import { InspectionTypesEnum } from '@app/enums/inspection-types.enum';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { InspectionDTO } from '@app/models/generated/dtos/InspectionDTO';
import { InspectionsFilters } from '@app/models/generated/filters/InspectionsFilters';
import { InspectionsService } from '@app/services/administration-app/inspections.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { SecurityService } from '@app/services/common-app/security.service';
import { Router } from '@angular/router';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { InspectionSelectionComponent } from '@app/components/administration-app/control-activity/inspections/dialogs/inspection-selection/inspection-selection.component';
import { SignInspectionComponent } from '@app/components/administration-app/control-activity/inspections/dialogs/sign-inspection/sign-inspection.component';
import { InspectionDialogParamsModel } from '@app/components/administration-app/control-activity/inspections/models/inspection-dialog-params.model';
import { EditObservationAtSeaComponent } from '@app/components/administration-app/control-activity/inspections/dialogs/edit-observation-at-sea/edit-observation-at-sea.component';
import { EditInspectionAtSeaComponent } from '@app/components/administration-app/control-activity/inspections/dialogs/edit-inspection-at-sea/edit-inspection-at-sea.component';
import { EditInspectionAtPortComponent } from '@app/components/administration-app/control-activity/inspections/dialogs/edit-inspection-at-port/edit-inspection-at-port.component';
import { EditInspectionTransshipmentComponent } from '@app/components/administration-app/control-activity/inspections/dialogs/edit-inspection-transshipment/edit-inspection-transshipment.component';
import { EditInspectionVehicleComponent } from '@app/components/administration-app/control-activity/inspections/dialogs/edit-inspection-vehicle/edit-inspection-vehicle.component';
import { EditInspectionAtMarketComponent } from '@app/components/administration-app/control-activity/inspections/dialogs/edit-inspection-at-market/edit-inspection-at-market.component';
import { EditInspectionAquacultureComponent } from '@app/components/administration-app/control-activity/inspections/dialogs/edit-inspection-aquaculture/edit-inspection-aquaculture.component';
import { EditInspectionPersonComponent } from '@app/components/administration-app/control-activity/inspections/dialogs/edit-inspection-person/edit-inspection-person.component';
import { EditCheckWaterObjectComponent } from '@app/components/administration-app/control-activity/inspections/dialogs/edit-check-water-object/edit-check-water-object.component';
import { EditInspectionFishingGearComponent } from '@app/components/administration-app/control-activity/inspections/dialogs/edit-inspection-fishing-gear/edit-inspection-fishing-gear.component';
import { ReviewOldInspectionComponent } from '@app/components/administration-app/control-activity/inspections/dialogs/review-old-inspection/review-old-inspection.component';

@Component({
    selector: 'inspections-register',
    templateUrl: './inspections-register.component.html',
})
export class InspectionsComponent implements OnInit, AfterViewInit, OnChanges {
    @Input()
    public shipId: number | undefined;

    @Input()
    public reloadData: boolean = false;

    @Input()
    public logBookPageId: number | undefined;

    @Input()
    public logBookType: LogBookTypesEnum | undefined;

    public isInspector: boolean = false;
    public userId: number | undefined;

    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public readonly inspectionTypesEnum: typeof InspectionTypesEnum = InspectionTypesEnum;
    public readonly inspectionStatesEnum: typeof InspectionStatesEnum = InspectionStatesEnum;

    public territoryNodes: NomenclatureDTO<number>[] = [];
    public inspectionTypes: NomenclatureDTO<number>[] = [];
    public states: NomenclatureDTO<number>[] = [];
    public ships: NomenclatureDTO<number>[] = [];
    public aquacultureFacilities: NomenclatureDTO<number>[] = [];
    public poundNets: NomenclatureDTO<number>[] = [];

    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;
    public readonly canReadRecords: boolean;
    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;
    public readonly canDeleteRecords: boolean;
    public readonly canRestoreRecords: boolean;
    public readonly canEditInspectionNumber: boolean;
    public readonly canExportRecords: boolean;
    public readonly canDownloadRecords: boolean;
    public readonly canReadAuanRecords: boolean;
    public readonly canEditLockedInspections: boolean;

    public canResolveCrossChecks: boolean = false;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private gridManager!: DataTableManager<InspectionDTO, InspectionsFilters>;

    private readonly service: InspectionsService;
    private readonly nomenclatures: CommonNomenclatures;
    private readonly router: Router;

    private readonly confirmDialog: TLConfirmDialog;
    private readonly matDialog: MatDialog;
    private readonly addDialog: TLMatDialog<InspectionSelectionComponent>;
    private readonly signDialog: TLMatDialog<SignInspectionComponent>;

    public constructor(
        service: InspectionsService,
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        matDialog: MatDialog,
        confirmDialog: TLConfirmDialog,
        addDialog: TLMatDialog<InspectionSelectionComponent>,
        signDialog: TLMatDialog<SignInspectionComponent>,
        permissions: PermissionsService,
        authService: SecurityService,
        router: Router
    ) {
        this.service = service;
        this.nomenclatures = nomenclatures;
        this.translate = translate;
        this.matDialog = matDialog;
        this.confirmDialog = confirmDialog;
        this.router = router;

        this.addDialog = addDialog;
        this.signDialog = signDialog;

        this.canReadRecords = permissions.hasAny(PermissionsEnum.InspectionsReadAll, PermissionsEnum.InspectionsRead);
        this.canAddRecords = permissions.has(PermissionsEnum.InspectionsAddRecords);
        this.canEditRecords = permissions.has(PermissionsEnum.InspectionsEditRecords);
        this.canDeleteRecords = permissions.has(PermissionsEnum.InspectionsDeleteRecords);
        this.canRestoreRecords = permissions.has(PermissionsEnum.InspectionsRestoreRecords);
        this.canEditInspectionNumber = permissions.has(PermissionsEnum.InspectionEditNumber);
        this.canDownloadRecords = permissions.has(PermissionsEnum.InspectionDownload);
        this.canExportRecords = permissions.has(PermissionsEnum.InspectionExport);
        this.canReadAuanRecords = permissions.hasAny(PermissionsEnum.AuanRegisterReadAll, PermissionsEnum.AuanRegisterRead);
        this.canEditLockedInspections = permissions.has(PermissionsEnum.InspectionLockedEdit);

        this.userId = authService.User!.userId!;

        this.buildForm();
    }

    public ngOnInit(): void {
        if ((this.shipId === null || this.shipId === undefined) && (this.logBookPageId === null || this.logBookPageId === undefined)) {
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false
            ).subscribe({
                next: (result: NomenclatureDTO<number>[]) => {
                    this.territoryNodes = result;
                }
            });

            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.InspectionTypes, this.nomenclatures.getInspectionTypes.bind(this.nomenclatures), false
            ).subscribe({
                next: (result: NomenclatureDTO<number>[]) => {
                    this.inspectionTypes = result;
                }
            });

            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.InspectionStates, this.service.getInspectionStates.bind(this.service), false
            ).subscribe({
                next: (result: NomenclatureDTO<number>[]) => {
                    this.states = result;
                }
            });

            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false
            ).subscribe({
                next: (result: NomenclatureDTO<number>[]) => {
                    this.ships = result;
                }
            });

            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.PoundNets, this.nomenclatures.getPoundNets.bind(this.nomenclatures), false
            ).subscribe({
                next: (result: NomenclatureDTO<number>[]) => {
                    this.poundNets = result;
                }
            });

            this.service.getAquacultures().subscribe({
                next: (result: NomenclatureDTO<number>[]) => {
                    this.aquacultureFacilities = result;
                }
            });

            this.service.getIsInspector().subscribe({
                next: (value) => {
                    this.isInspector = value ?? false;
                }
            });

            //добавяне на инспекция след периода за разрешаване на кръстосани проверки 
            this.service.canResolveCrossChecks().subscribe({
                next: (value: boolean) => {
                    this.canResolveCrossChecks = value;
                }
            });
        }
        else {
            this.isInspector = true;
        }
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<InspectionDTO, InspectionsFilters>({
            tlDataTable: this.datatable,
            searchPanel: (this.shipId === null || this.shipId === undefined) && (this.logBookPageId === null || this.logBookPageId === undefined) ? this.searchpanel : undefined,
            requestServiceMethod: this.service.getAll.bind(this.service),
            filtersMapper: this.mapFilters.bind(this),
        });

        const subjectId: number | undefined = window.history.state?.id;
        const isPerson: boolean | undefined = window.history.state?.isPerson;

        if (!CommonUtils.isNullOrEmpty(subjectId)) {
            if (isPerson === true) {
                this.gridManager.advancedFilters = new InspectionsFilters({
                    inspectedPersonId: subjectId,
                    subjectIsLegal: false
                });
            }
            else if (isPerson === false) {
                this.gridManager.advancedFilters = new InspectionsFilters({
                    inspectedLegalId: subjectId,
                    subjectIsLegal: true
                });
            }

            this.gridManager.refreshData();
        }

        if (this.logBookPageId !== undefined && this.logBookPageId !== null && this.logBookType !== undefined && this.logBookType !== null) {
            switch (this.logBookType) {
                case LogBookTypesEnum.Admission:
                    this.gridManager.advancedFilters = new InspectionsFilters({
                        addmissionLogBookPageId: this.logBookPageId
                    });
                    break;
                case LogBookTypesEnum.Aquaculture:
                    this.gridManager.advancedFilters = new InspectionsFilters({
                        aquacultureLogBookPageId: this.logBookPageId
                    });
                    break;
                case LogBookTypesEnum.FirstSale:
                    this.gridManager.advancedFilters = new InspectionsFilters({
                        firstSaleLogBookPageId: this.logBookPageId
                    });
                    break;
                case LogBookTypesEnum.Ship:
                    this.gridManager.advancedFilters = new InspectionsFilters({
                        shipLogBookPageId: this.logBookPageId
                    });
                    break;
                case LogBookTypesEnum.Transportation:
                    this.gridManager.advancedFilters = new InspectionsFilters({
                        transportationLogBookPageId: this.logBookPageId
                    });
                    break;
            }

            this.gridManager.refreshData();
        }

        if (this.shipId !== undefined && this.shipId !== null) {
            this.gridManager.advancedFilters = new InspectionsFilters({
                shipId: this.shipId ?? undefined,
            });
        }

        if (this.shipId === null || this.shipId === undefined) {
            this.gridManager.refreshData();
        }
    }

    public addEditEntry(entry: InspectionDTO | undefined, viewMode: boolean = false): void {
        if (!this.isInspector) {
            this.confirmDialog.open({
                title: this.translate.getValue('inspections.user-not-inspector-title'),
                message: this.translate.getValue('inspections.user-not-inspector-msg'),
                okBtnLabel: this.translate.getValue('inspections.okay'),
                hasCancelButton: false
            }).subscribe();

            return;
        }

        if (!this.canResolveCrossChecks && (this.shipId === null || this.shipId === undefined) && (this.logBookPageId === null || this.logBookPageId === undefined)) {
            this.confirmDialog.open({
                title: this.translate.getValue('inspections.user-has-unresolved-cross-checks-title'),
                message: this.translate.getValue('inspections.user-has-unresolved-cross-checks-message'),
                okBtnLabel: this.translate.getValue('inspections.user-has-unresolved-cross-checks-ok-btn'),
                hasCancelButton: true
            }).subscribe({
                next: (ok: boolean) => {
                    if (ok) {
                        this.router.navigateByUrl('/cross-checks-results');
                    }
                }
            });

            return;
        }

        if (entry === null || entry === undefined) {
            this.openAddSelectionDialog();
        }
        else {
            const dialogInfo: EditDialogInfo = this.getEditDialogInfo(entry.inspectionType!, false, viewMode);
            this.openEditDialog(dialogInfo, entry, viewMode);
        }
    }

    public signEntry(entry: InspectionDTO): void {
        const dialog = this.signDialog.open(
            {
                title: this.translate.getValue('inspections.sign-inspection-title'),
                TCtor: SignInspectionComponent,
                headerCancelButton: {
                    cancelBtnClicked: this.closeDialogBtnClicked.bind(this),
                },
                componentData: entry,
                translteService: this.translate,
                disableDialogClose: true,
                saveBtn: {
                    id: 'save',
                    color: 'accent',
                    translateValue: this.translate.getValue('inspections.sign-inspection'),
                },
                cancelBtn: {
                    id: 'cancel',
                    color: 'primary',
                    translateValue: this.translate.getValue('common.cancel'),
                },
                viewMode: false,
            }, '120em');

        dialog.subscribe({
            next: (result: InspectionDTO | undefined) => {
                if (result !== null && result !== undefined) {
                    this.gridManager.refreshData();
                }
            },
        });
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const reloadData: boolean | undefined = changes['reloadData']?.currentValue;

        if (reloadData === true) {
            this.gridManager?.refreshData();
        }
    }

    public deleteEntry(entry: InspectionDTO): void {
        this.confirmDialog
            .open({
                title: this.translate.getValue('inspections.delete-inspection-dialog-title'),
                message: this.translate.getValue('inspections.delete-inspection-dialog-message'),
                okBtnLabel: this.translate.getValue('inspections.delete-inspection-dialog-ok-btn-label'),
            })
            .subscribe({
                next: (ok: boolean) => {
                    if (ok) {
                        this.service.delete(entry.id!).subscribe({
                            next: () => {
                                this.gridManager.refreshData();
                            },
                        });
                    }
                },
            });
    }

    public restoreEntry(entry: InspectionDTO): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.restore(entry.id!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        },
                    });
                }
            },
        });
    }

    public openAddSelectionDialog(): void {
        const title: string = this.translate.getValue('inspections.add-dialog');

        const dialog = this.addDialog.open(
            {
                title: title,
                TCtor: InspectionSelectionComponent,
                headerAuditButton: undefined,
                headerCancelButton: {
                    cancelBtnClicked: this.closeDialogBtnClicked.bind(this),
                },
                componentData: undefined,
                translteService: this.translate,
            },
            '50em'
        );

        dialog.subscribe({
            next: (result: InspectionTypesEnum | undefined) => {
                if (result !== undefined && result !== null) {
                    const editDialogInfo: EditDialogInfo = this.getEditDialogInfo(result, true);
                    this.openEditDialog(editDialogInfo, undefined, false);
                }
            },
        });
    }

    public openEditDialog(
        dialogInfo: EditDialogInfo,
        entry: InspectionDTO | undefined,
        readOnly: boolean = false
    ): void {
        let auditBtn: IHeaderAuditButton | undefined;
        let data: InspectionDialogParamsModel | undefined;

        const rightSideButtons: IActionInfo[] = [
            {
                id: 'draft',
                color: 'primary',
                translateValue: this.translate.getValue('common.save-draft'),
            },
        ];

        if (entry !== null && entry !== undefined) {
            auditBtn = {
                id: entry.id!,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'InspectionRegister'
            };

            if (entry.inspectionState === InspectionStatesEnum.Submitted ||
                entry.inspectionState === InspectionStatesEnum.Signed
            ) {
                readOnly = true;

                if (entry.inspectionType !== InspectionTypesEnum.OTH) {
                    if (this.canDownloadRecords) {
                        rightSideButtons.push(
                            {
                                id: 'print',
                                color: 'primary',
                                translateValue: 'inspections.print-inspection',
                                isVisibleInViewMode: true,
                            }
                        ); //TODO Add export btn
                    }
                }
            }

            //ако няма правото за връщане за редакция, може да коригира само своите инспекции, ако не са минали 48 часа от създаването им
            if ((this.shipId === null || this.shipId === undefined)
                && entry.inspectionState !== InspectionStatesEnum.Draft
                && entry.inspectionType !== InspectionTypesEnum.OTH
                && (this.canEditLockedInspections || (!entry.isReportLocked && entry.createdByUserId === this.userId)) 
            ) {
                rightSideButtons.push({
                    id: 'more-corrections-needed',
                    color: 'primary',
                    translateValue: 'inspections.confirm-need-for-corrections',
                    icon: { id: 'ic-fluent-doc-person-20-regular', size: this.icIconSize },
                    isVisibleInViewMode: true,
                });
            }
            
            data = new InspectionDialogParamsModel({
                id: entry.id,
                viewMode: readOnly,
                canEditNumber: this.canEditInspectionNumber,
                canEditLockedInspections: this.canEditLockedInspections,
                isReportLocked: entry.isReportLocked,
                userIsSameAsInspector: entry.createdByUserId === this.userId,
                pageCode: entry.inspectionState === InspectionStatesEnum.Signed ? PageCodeEnum.SignInspections : PageCodeEnum.Inspections,
                service: this.service
            });
        }
        else {
            data = new InspectionDialogParamsModel({
                canEditNumber: this.canEditInspectionNumber
            });
        }

        const dialog = dialogInfo.editDialog.open(
            {
                title: dialogInfo.viewTitle,
                TCtor: dialogInfo.editDialogTCtor,
                headerAuditButton: auditBtn,
                headerCancelButton: {
                    cancelBtnClicked: this.closeDialogBtnClicked.bind(this),
                },
                componentData: data,
                translteService: this.translate,
                disableDialogClose: true,
                saveBtn: {
                    id: 'save',
                    color: 'accent',
                    translateValue: this.translate.getValue('inspections.submit'),
                },
                cancelBtn: {
                    id: 'cancel',
                    color: 'primary',
                    translateValue: this.translate.getValue('common.cancel'),
                },
                rightSideActionsCollection: rightSideButtons,
                viewMode: readOnly,
                defaultFullscreen: true
            },
            '120em'
        );

        dialog.subscribe({
            next: (result: InspectionDTO | undefined) => {
                if (result !== null && result !== undefined) {
                    this.gridManager.refreshData();
                }
            },
        });
    }

    public closeDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private mapFilters(filters: FilterEventArgs): InspectionsFilters {
        const result = new InspectionsFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            territoryNode: filters.getValue('territoryNodeControl'),
            inspector: filters.getValue('inspectorNameControl'),
            inspectionTypeId: this.form.get('inspectionTypeControl')!.value?.value,
            reportNumber: filters.getValue('reportNumberControl'),
            subjectIsLegal: filters.getValue('isLegalControl'),
            dateFrom: filters.getValue<DateRangeData>('dateRangeControl')?.start,
            dateTo: filters.getValue<DateRangeData>('dateRangeControl')?.end,
            stateIds: filters.getValue('stateControl'),
            shipId: filters.getValue('shipControl'),
            aquacultureId: filters.getValue('aquacultureControl'),
            unregisteredShipName: filters.getValue('unregisteredShipControl'),
            poundNetId: filters.getValue('poundNetControl'),
            fishermanName: filters.getValue('fishermanNameControl'),
            waterObjectName: filters.getValue('waterObjectControl'),
            firstSaleCenterName: filters.getValue('firstSaleCenterNameControl'),
            tractorLicensePlateNumber: filters.getValue('tractorLicensePlateNumberControl'),
            trailerLicensePlateNumber: filters.getValue('trailerLicensePlateNumberControl')
        });

        if (result.subjectIsLegal === true) {
            result.subjectEIK = filters.getValue('eikControl');
            result.subjectName = filters.getValue('legalNameControl');
        } else {
            result.subjectEGN = filters.getValue('egnControl');
            result.subjectName = filters.getValue('personNameControl');
        }

        return result;
    }

    private getEditDialogInfo(
        inspectionCode: InspectionTypesEnum,
        adding: boolean = false,
        readonly: boolean = false
    ): EditDialogInfo {
        let dialog: TLMatDialog<IDialogComponent> | undefined;
        let dialogTCtor: unknown | undefined;
        let title: string | undefined;

        const registerTitle: string = this.translate.getValue('inspections.register-title');

        switch (inspectionCode) {
            case InspectionTypesEnum.OFS:
                {
                    dialog = new TLMatDialog<EditObservationAtSeaComponent>(this.matDialog);
                    dialogTCtor = EditObservationAtSeaComponent;
                    if (adding) {
                        title = this.translate.getValue('inspections.add-ofs-dialog-title');
                    } else if (readonly) {
                        title = this.translate.getValue('inspections.view-ofs-dialog-title');
                    } else {
                        title = this.translate.getValue('inspections.edit-ofs-dialog-title');
                    }
                }
                break;
            case InspectionTypesEnum.IBS:
                {
                    dialog = new TLMatDialog<EditInspectionAtSeaComponent>(this.matDialog);
                    dialogTCtor = EditInspectionAtSeaComponent;
                    if (adding) {
                        title = this.translate.getValue('inspections.add-ibs-dialog-title');
                    } else if (readonly) {
                        title = this.translate.getValue('inspections.view-ibs-dialog-title');
                    } else {
                        title = this.translate.getValue('inspections.edit-ibs-dialog-title');
                    }
                }
                break;
            case InspectionTypesEnum.IBP:
                {
                    dialog = new TLMatDialog<EditInspectionAtPortComponent>(this.matDialog);
                    dialogTCtor = EditInspectionAtPortComponent;
                    if (adding) {
                        title = this.translate.getValue('inspections.add-ibp-dialog-title');
                    } else if (readonly) {
                        title = this.translate.getValue('inspections.view-ibp-dialog-title');
                    } else {
                        title = this.translate.getValue('inspections.edit-ibp-dialog-title');
                    }
                }
                break;
            case InspectionTypesEnum.ITB:
                {
                    dialog = new TLMatDialog<EditInspectionTransshipmentComponent>(this.matDialog);
                    dialogTCtor = EditInspectionTransshipmentComponent;
                    if (adding) {
                        title = this.translate.getValue('inspections.add-itb-dialog-title');
                    } else if (readonly) {
                        title = this.translate.getValue('inspections.view-itb-dialog-title');
                    } else {
                        title = this.translate.getValue('inspections.edit-itb-dialog-title');
                    }
                }
                break;
            case InspectionTypesEnum.IVH:
                {
                    dialog = new TLMatDialog<EditInspectionVehicleComponent>(this.matDialog);
                    dialogTCtor = EditInspectionVehicleComponent;
                    if (adding) {
                        title = this.translate.getValue('inspections.add-ivh-dialog-title');
                    } else if (readonly) {
                        title = this.translate.getValue('inspections.view-ivh-dialog-title');
                    } else {
                        title = this.translate.getValue('inspections.edit-ivh-dialog-title');
                    }
                }
                break;
            case InspectionTypesEnum.IFS:
                {
                    dialog = new TLMatDialog<EditInspectionAtMarketComponent>(this.matDialog);
                    dialogTCtor = EditInspectionAtMarketComponent;
                    if (adding) {
                        title = this.translate.getValue('inspections.add-ifs-dialog-title');
                    } else if (readonly) {
                        title = this.translate.getValue('inspections.view-ifs-dialog-title');
                    } else {
                        title = this.translate.getValue('inspections.edit-ifs-dialog-title');
                    }
                }
                break;
            case InspectionTypesEnum.IAQ:
                {
                    dialog = new TLMatDialog<EditInspectionAquacultureComponent>(this.matDialog);
                    dialogTCtor = EditInspectionAquacultureComponent;
                    if (adding) {
                        title = this.translate.getValue('inspections.add-iaq-dialog-title');
                    } else if (readonly) {
                        title = this.translate.getValue('inspections.view-iaq-dialog-title');
                    } else {
                        title = this.translate.getValue('inspections.edit-iaq-dialog-title');
                    }
                }
                break;
            case InspectionTypesEnum.IFP:
                {
                    dialog = new TLMatDialog<EditInspectionPersonComponent>(this.matDialog);
                    dialogTCtor = EditInspectionPersonComponent;
                    if (adding) {
                        title = this.translate.getValue('inspections.add-ifp-dialog-title');
                    } else if (readonly) {
                        title = this.translate.getValue('inspections.view-ifp-dialog-title');
                    } else {
                        title = this.translate.getValue('inspections.edit-ifp-dialog-title');
                    }
                }
                break;
            case InspectionTypesEnum.CWO:
                {
                    dialog = new TLMatDialog<EditCheckWaterObjectComponent>(this.matDialog);
                    dialogTCtor = EditCheckWaterObjectComponent;
                    if (adding) {
                        title = this.translate.getValue('inspections.add-cwo-dialog-title');
                    } else if (readonly) {
                        title = this.translate.getValue('inspections.view-cwo-dialog-title');
                    } else {
                        title = this.translate.getValue('inspections.edit-cwo-dialog-title');
                    }
                }
                break;
            case InspectionTypesEnum.IGM:
                {
                    dialog = new TLMatDialog<EditInspectionFishingGearComponent>(this.matDialog);
                    dialogTCtor = EditInspectionFishingGearComponent;
                    if (adding) {
                        title = this.translate.getValue('inspections.add-igm-dialog-title');
                    } else if (readonly) {
                        title = this.translate.getValue('inspections.view-igm-dialog-title');
                    } else {
                        title = this.translate.getValue('inspections.edit-igm-dialog-title');
                    }
                }
                break;
            case InspectionTypesEnum.OTH:
                {
                    dialog = new TLMatDialog<ReviewOldInspectionComponent>(this.matDialog);
                    dialogTCtor = ReviewOldInspectionComponent;
                    if (adding) {
                        title = this.translate.getValue('inspections.add-oth-dialog-title');
                    } else if (readonly) {
                        title = this.translate.getValue('inspections.view-oth-dialog-title');
                    } else {
                        title = this.translate.getValue('inspections.edit-oth-dialog-title');
                    }
                }
                break;
        }

        if (!dialog || !dialogTCtor || title === undefined || title === null) {
            throw new Error('Invalid inspection type. Cannot find dialog.');
        }

        return new EditDialogInfo({
            editDialog: dialog,
            editDialogTCtor: dialogTCtor,
            viewTitle: title,
            viewRegisterTitle: registerTitle,
        });
    }

    private buildForm(): void {
        this.form = new FormGroup({
            territoryNodeControl: new FormControl(),
            inspectorNameControl: new FormControl(),
            stateControl: new FormControl(),
            inspectionTypeControl: new FormControl(),
            reportNumberControl: new FormControl(),
            isLegalControl: new FormControl(),
            legalNameControl: new FormControl(),
            eikControl: new FormControl(),
            personNameControl: new FormControl(),
            egnControl: new FormControl(),
            dateRangeControl: new FormControl(),
            aquacultureControl: new FormControl(),
            shipControl: new FormControl(),
            poundNetControl: new FormControl(),
            unregisteredShipControl: new FormControl(),
            firstSaleCenterControl: new FormControl(),
            fishermanNameControl: new FormControl(),
            waterObjectControl: new FormControl(),
            firstSaleCenterNameControl: new FormControl(),
            tractorLicensePlateNumberControl: new FormControl(),
            trailerLicensePlateNumberControl: new FormControl()
        });
    }
}

import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ScientificPermitStatusEnum } from '@app/enums/scientific-permit-status.enum';
import { IDeliveryService } from '@app/interfaces/common-app/delivery.interface';
import { IScientificFishingService } from '@app/interfaces/common-app/scientific-fishing.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ApplicationDeliveryDTO } from '@app/models/generated/dtos/ApplicationDeliveryDTO';
import { ApplicationForChoiceDTO } from '@app/models/generated/dtos/ApplicationForChoiceDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ScientificFishingOutingDTO } from '@app/models/generated/dtos/ScientificFishingOutingDTO';
import { ScientificFishingPermitDTO } from '@app/models/generated/dtos/ScientificFishingPermitDTO';
import { ScientificFishingFilters } from '@app/models/generated/filters/ScientificFishingFilters';
import { ScientificFishingPublicFilters } from '@app/models/generated/filters/ScientificFishingPublicFilters';
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
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { ChooseApplicationComponent } from '../applications/components/choose-application/choose-application.component';
import { ChooseApplicationDialogParams } from '../applications/components/choose-application/models/choose-application-dialog-params.model';
import { EditScientificFishingOutingComponent } from './components/edit-scientific-fishing-outing/edit-scientific-fishing-outing.component';
import { EditScientificPermitComponent } from './components/edit-scientific-permit/edit-scientific-permit.component';
import { EditScientificFishingOutingDialogParams } from './models/edit-scientific-fishing-outing-dialog-params.model';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { ScientificFishingReasonNomenclatureDTO } from '@app/models/generated/dtos/ScientificFishingReasonNomenclatureDTO';

@Component({
    selector: 'scientific-fishing-content',
    templateUrl: './scientific-fishing-content.component.html',
    styleUrls: ['./scientific-fishing-content.component.scss']
})
export class ScientificFishingContent implements OnInit, AfterViewInit {
    @Input()
    public isPublicApp!: boolean;

    @Input()
    public service!: IScientificFishingService;

    @Input()
    public deliveryService!: IDeliveryService;

    public translationService: FuseTranslationLoaderService;
    public formGroup!: FormGroup;

    public permitReasons: NomenclatureDTO<number>[] = [];
    public permitLegalReasons: NomenclatureDTO<number>[] = [];
    public permitStatuses: NomenclatureDTO<number>[] = [];

    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;
    public readonly permitStatus: typeof ScientificPermitStatusEnum = ScientificPermitStatusEnum;

    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;
    public readonly canDeleteRecords: boolean;
    public readonly canRestoreRecords: boolean;
    public readonly canAddOutings: boolean;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private gridManager!: DataTableManager<ScientificFishingPermitDTO, ScientificFishingFilters | ScientificFishingPublicFilters>;

    private readonly confirmDialog: TLConfirmDialog;
    private readonly editDialog: TLMatDialog<EditScientificPermitComponent>;
    private readonly outingDialog: TLMatDialog<EditScientificFishingOutingComponent>;
    private readonly chooseApplicationDialog: TLMatDialog<ChooseApplicationComponent>;
    private readonly deliveryDialog: TLMatDialog<RegisterDeliveryComponent>;

    public constructor(
        translationService: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editDialog: TLMatDialog<EditScientificPermitComponent>,
        outingDialog: TLMatDialog<EditScientificFishingOutingComponent>,
        chooseApplicationDialog: TLMatDialog<ChooseApplicationComponent>,
        deliveryDialog: TLMatDialog<RegisterDeliveryComponent>,
        permissions: PermissionsService
    ) {
        this.translationService = translationService;
        this.confirmDialog = confirmDialog;
        this.editDialog = editDialog;
        this.outingDialog = outingDialog;
        this.chooseApplicationDialog = chooseApplicationDialog;
        this.deliveryDialog = deliveryDialog;

        this.canAddRecords = !this.isPublicApp && permissions.has(PermissionsEnum.ScientificFishingAddRecords);
        this.canEditRecords = !this.isPublicApp && permissions.has(PermissionsEnum.ScientificFishingEditRecords);
        this.canDeleteRecords = !this.isPublicApp && permissions.has(PermissionsEnum.ScientificFishingDeleteRecords);
        this.canRestoreRecords = !this.isPublicApp && permissions.has(PermissionsEnum.ScientificFishingRestoreRecords);
        this.canAddOutings = permissions.has(PermissionsEnum.ScientificFishingAddOutings);

        this.buildForm();
    }

    public ngOnInit(): void {
        if (!this.isPublicApp) {
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.SciPermitReasons, this.service.getPermitReasons.bind(this.service), false
            ).subscribe({
                next: (result: ScientificFishingReasonNomenclatureDTO[]) => {
                    this.permitReasons = result.filter(x => !x.isLegalReason);
                    this.permitLegalReasons = result.filter(x => x.isLegalReason);
                }
            });

            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.SciPermitStatuses, this.service.getPermitStatuses.bind(this.service), false
            ).subscribe({
                next: (result: NomenclatureDTO<number>[]) => {
                    this.permitStatuses = result.filter(x => x.code !== ScientificPermitStatusEnum[ScientificPermitStatusEnum.Application]);
                }
            });
        }
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<ScientificFishingPermitDTO, ScientificFishingFilters | ScientificFishingPublicFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllPermits.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        const isPerson: boolean | undefined = window.history.state?.isPerson;
        const id: number | undefined = window.history.state?.id;

        if (!CommonUtils.isNullOrEmpty(id)) {
            if (isPerson) {
                this.gridManager.advancedFilters = new ScientificFishingFilters({ personId: id });
            }
            else {
                this.gridManager.advancedFilters = new ScientificFishingFilters({ legalId: id });
            }
        }

        this.gridManager.refreshData();
    }

    public createEditPermit(permit?: ScientificFishingPermitDTO, viewMode?: boolean): void {
        let data: DialogParamsModel | undefined;
        let auditButton: IHeaderAuditButton | undefined;
        let title: string;
        const rightButtons: IActionInfo[] = [];
        const leftButtons: IActionInfo[] = [];

        if (permit?.id !== undefined) {
            const isReadOnly: boolean = !this.canEditRecords;

            data = new DialogParamsModel({ id: permit.id, isApplication: false, isReadonly: isReadOnly || viewMode, service: this.service });

            if (!this.isPublicApp) {
                auditButton = {
                    id: permit.id,
                    getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                    tableName: 'ScientificPermitRegister'
                };
            }

            if (viewMode) {
                title = this.translationService.getValue('scientific-fishing.view-permit-dialog-title');
            }
            else {
                title = this.translationService.getValue('scientific-fishing.edit-permit-dialog-title');
            }

            if (!this.isPublicApp && !viewMode) {
                if (permit.permitStatus === ScientificPermitStatusEnum.Canceled) {
                    leftButtons.push({
                        id: 'activate',
                        color: 'accent',
                        translateValue: this.translationService.getValue('scientific-fishing.activate')
                    });
                }
                else {
                    leftButtons.push({
                        id: 'annul',
                        color: 'warn',
                        translateValue: this.translationService.getValue('scientific-fishing.annul')
                    });
                }
            }

            if (!this.isPublicApp) {
                if (viewMode || permit.permitStatus === ScientificPermitStatusEnum.Canceled) {
                    rightButtons.push({
                        id: 'print',
                        color: 'accent',
                        translateValue: this.translationService.getValue('scientific-fishing.print'),
                        isVisibleInViewMode: true
                    }, {
                        id: 'print-gov',
                        color: 'accent',
                        translateValue: this.translationService.getValue('scientific-fishing.print-gov'),
                        isVisibleInViewMode: true
                    }, {
                        id: 'print-gov-project',
                        color: 'accent',
                        translateValue: this.translationService.getValue('scientific-fishing.print-gov-project'),
                        isVisibleInViewMode: true
                    });
                }
                else {
                    rightButtons.push({
                        id: 'save-print',
                        color: 'accent',
                        translateValue: this.translationService.getValue('scientific-fishing.save-print')
                    }, {
                        id: 'save-print-gov',
                        color: 'accent',
                        translateValue: this.translationService.getValue('scientific-fishing.save-print-gov')
                    }, {
                        id: 'save-print-gov-project',
                        color: 'accent',
                        translateValue: this.translationService.getValue('scientific-fishing.save-print-gov-project')
                    });
                }
            }

            this.openPermitDialog(data, title, auditButton, rightButtons, leftButtons, viewMode ?? false);
        }
        else {
            title = this.translationService.getValue('scientific-fishing.add-permit-dialog-title');

            rightButtons.push({
                id: 'save-print',
                color: 'accent',
                translateValue: this.translationService.getValue('scientific-fishing.save-print')
            }, {
                id: 'save-print-gov',
                color: 'accent',
                translateValue: this.translationService.getValue('scientific-fishing.save-print-gov')
            }, {
                id: 'save-print-gov-project',
                color: 'accent',
                translateValue: this.translationService.getValue('scientific-fishing.save-print-gov-project')
            });

            this.chooseApplicationDialog.open({
                TCtor: ChooseApplicationComponent,
                title: this.translationService.getValue('applications-register.choose-application-for-register-creation'),
                translteService: this.translationService,
                componentData: new ChooseApplicationDialogParams({ pageCodes: [PageCodeEnum.SciFi] }),
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
                        isApplication: false,
                        isReadonly: false,
                        viewMode: false,
                        service: this.service,
                        applicationId: dialogData.selectedApplication.id
                    });

                    this.openPermitDialog(data, title, auditButton, rightButtons, leftButtons, viewMode ?? false);
                }
            });
        }
    }

    public openDeliveryDialog(permit: ScientificFishingPermitDTO): void {
        let auditButton: IHeaderAuditButton | undefined;

        if (permit.deliveryId !== null && permit.deliveryId !== undefined && !this.isPublicApp) {
            auditButton = {
                id: permit.deliveryId,
                getAuditRecordData: this.deliveryService.getSimpleAudit.bind(this.deliveryService),
                tableName: 'ApplicationDelivery'
            };
        }

        this.deliveryDialog.openWithTwoButtons({
            TCtor: RegisterDeliveryComponent,
            title: this.translationService.getValue('scientific-fishing.delivery-data-dialog-title'),
            translteService: this.translationService,
            componentData: new RegisterDeliveryDialogParams({
                deliveryId: permit.deliveryId,
                isPublicApp: this.isPublicApp,
                service: this.deliveryService,
                pageCode: PageCodeEnum.SciFi,
                registerId: permit.id
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

    public addOuting(permitId?: number): void {
        if (permitId !== undefined) {
            const dialog = this.outingDialog.openWithTwoButtons({
                title: this.translationService.getValue('scientific-fishing.add-outing-dialog-title'),
                TCtor: EditScientificFishingOutingComponent,
                headerCancelButton: {
                    cancelBtnClicked: this.closeAddOutingDialogBtnClicked.bind(this)
                },
                componentData: new EditScientificFishingOutingDialogParams(permitId, this.service, undefined, true),
                translteService: this.translationService,
                disableDialogClose: true
            }, '1400px');

            dialog.subscribe((entry?: ScientificFishingOutingDTO) => {
                if (entry !== null && entry !== undefined) {
                    this.gridManager.refreshData();
                }
            });
        }
    }

    public deletePermit(permit: ScientificFishingPermitDTO): void {
        this.confirmDialog.open({
            title: this.translationService.getValue('scientific-fishing.delete-permit'),
            message: this.translationService.getValue('scientific-fishing.confirm-delete-message'),
            okBtnLabel: this.translationService.getValue('scientific-fishing.delete')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok && permit?.id) {
                    this.service.deletePermit(permit.id).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        }
                    });
                }
            }
        });
    }

    public restorePermit(permit: ScientificFishingPermitDTO): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok && permit?.id) {
                    this.service.undoDeletePermit(permit.id).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        }
                    });
                }
            }
        });
    }

    private openPermitDialog(
        data: DialogParamsModel,
        title: string,
        auditButton: IHeaderAuditButton | undefined,
        rightButtons: IActionInfo[],
        leftButtons: IActionInfo[],
        viewMode: boolean): void {
        const dialog = this.editDialog.open({
            title: title,
            TCtor: EditScientificPermitComponent,
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
        }, '1400px');

        dialog.subscribe((entry?: ScientificFishingPermitDTO) => {
            if (entry !== undefined) {
                this.gridManager.refreshData();
            }
        });
    }

    private buildForm(): void {
        if (this.isPublicApp) {
            this.formGroup = new FormGroup({
                requestNumberControl: new FormControl(),
                permitNumberControl: new FormControl(),
                creationDateRangeControl: new FormControl(),
                validityDateRangeControl: new FormControl(),
            });
        }
        else {
            this.formGroup = new FormGroup({
                requestNumberControl: new FormControl(),
                permitNumberControl: new FormControl(),
                creationDateRangeControl: new FormControl(),
                validityDateRangeControl: new FormControl(),

                permitReasonsControl: new FormControl(),
                permitLegalReasonsControl: new FormControl(),

                permitRequesterControl: new FormControl(),
                permitOwnerNameControl: new FormControl(),
                permitOwnerEgnControl: new FormControl(),

                scientificOrganizationControl: new FormControl(),
                researchWaterAreaControl: new FormControl(),
                aquaticOrganismTypeControl: new FormControl(),

                gearTypeControl: new FormControl(),
                permitStatusesControl: new FormControl()
            });
        }
    }

    private mapFilters(filters: FilterEventArgs): ScientificFishingFilters | ScientificFishingPublicFilters {
        let result: ScientificFishingFilters | ScientificFishingPublicFilters;

        if (this.isPublicApp) {
            result = new ScientificFishingPublicFilters({
                freeTextSearch: filters.searchText,
                showInactiveRecords: filters.showInactiveRecords,

                eventisNum: filters.getValue('requestNumberControl'),
                permitNumber: filters.getValue('permitNumberControl'),
                creationDateFrom: filters.getValue<DateRangeData>('creationDateRangeControl')?.start,
                creationDateTo: filters.getValue<DateRangeData>('creationDateRangeControl')?.end,
                validFrom: filters.getValue<DateRangeData>('validityDateRangeControl')?.start,
                validTo: filters.getValue<DateRangeData>('validityDateRangeControl')?.end
            });
        }
        else {
            result = new ScientificFishingFilters({
                freeTextSearch: filters.searchText,
                showInactiveRecords: filters.showInactiveRecords,

                eventisNum: filters.getValue('requestNumberControl'),
                permitNumber: filters.getValue('permitNumberControl'),
                creationDateFrom: filters.getValue<DateRangeData>('creationDateRangeControl')?.start,
                creationDateTo: filters.getValue<DateRangeData>('creationDateRangeControl')?.end,
                validFrom: filters.getValue<DateRangeData>('validityDateRangeControl')?.start,
                validTo: filters.getValue<DateRangeData>('validityDateRangeControl')?.end,

                permitReasonIds: filters.getValue('permitReasonsControl'),
                permitLegalReasonIds: filters.getValue('permitLegalReasonsControl'),
                permitRequesterName: filters.getValue('permitRequesterControl'),
                permitOwnerName: filters.getValue('permitOwnerNameControl'),
                permitOwnerEgn: filters.getValue('permitOwnerEgnControl'),

                scientificOrganizationName: filters.getValue('scientificOrganizationControl'),
                researchWaterArea: filters.getValue('researchWaterAreaControl'),
                aquaticOrganismType: filters.getValue('aquaticOrganismTypeControl'),

                gearType: filters.getValue('gearTypeControl'),
                statuses: filters.getValue('permitStatusesControl')
            });
        }
        return result;
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeApplicationChooseDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeAddOutingDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeDeliveryDataDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}

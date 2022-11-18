import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { ApplicationHierarchyTypesEnum } from '@app/enums/application-hierarchy-types.enum';
import { ApplicationStatusesEnum } from '@app/enums/application-statuses.enum';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { IRecreationalFishingService } from '@app/interfaces/common-app/recreational-fishing.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { ReasonData } from '@app/models/common/reason-data.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PaymentDataDTO } from '@app/models/generated/dtos/PaymentDataDTO';
import { RecreationalFishingTicketApplicationDTO } from '@app/models/generated/dtos/RecreationalFishingTicketApplicationDTO';
import { RecreationalFishingTicketApplicationFilters } from '@app/models/generated/filters/RecreationalFishingTicketApplicationFilters';
import { RecreationalFishingAdministrationService } from '@app/services/administration-app/recreational-fishing-administration.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { RecreationalFishingPublicService } from '@app/services/public-app/recreational-fishing-public.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { DialogCloseCallback } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { PaymentDataComponent } from '@app/shared/components/payment-data/payment-data.component';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { EnterReasonComponent } from '../../applications/components/enter-reason/enter-reason.component';
import { ReasonDialogParams } from '../../applications/components/enter-reason/models/reason-dialog-params.model';
import { RecreationalFishingTicketComponent } from '../tickets/components/ticket/recreational-fishing-ticket.component';
import { EditTicketDialogParams } from './models/edit-ticket-dialog-params.model';

type ThreeState = 'yes' | 'no' | 'both';

@Component({
    selector: 'recreational-fishing-applications-content',
    templateUrl: './recreational-fishing-applications-content.component.html'
})
export class RecreationalFishingApplicationsContentComponent implements OnInit, AfterViewInit {
    @Input()
    public isAssociation: boolean = false;

    @Input()
    public service!: IRecreationalFishingService;

    @Input()
    public applicationsService!: IApplicationsService;

    @Input()
    public isDashboardMode: boolean = false;

    @Input()
    public recordsPerPage: number = 20;

    @Input()
    public getAllServiceMethod!: (request: GridRequestModel<RecreationalFishingTicketApplicationFilters>) => Observable<GridResultModel<RecreationalFishingTicketApplicationDTO>>;

    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public types!: NomenclatureDTO<number>[];
    public periods!: NomenclatureDTO<number>[];
    public statuses!: NomenclatureDTO<number>[];
    public isDuplicateOptions!: NomenclatureDTO<ThreeState>[];

    public canAddRecords!: boolean;
    public canEditRecords!: boolean;
    public canDeleteRecords!: boolean;
    public canRestoreRecords!: boolean;
    public canProcessPaymentData!: boolean;
    public canCancelRecords!: boolean;
    public canInspectAndCorrectRecords!: boolean;

    public readonly applicationStatusEnum: typeof ApplicationStatusesEnum = ApplicationStatusesEnum;
    public readonly applicationSourceTypeEnum: typeof ApplicationHierarchyTypesEnum = ApplicationHierarchyTypesEnum;
    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private paymentTypes!: NomenclatureDTO<number>[];

    private grid!: DataTableManager<RecreationalFishingTicketApplicationDTO, RecreationalFishingTicketApplicationFilters>;
    private router: Router;
    private permissions: PermissionsService;
    private nomenclatures: CommonNomenclatures;
    private confirmDialog: TLConfirmDialog;
    private paymentDataDialog: TLMatDialog<PaymentDataComponent>;
    private statusReasonDialog: TLMatDialog<EnterReasonComponent>;
    private editDialog: TLMatDialog<RecreationalFishingTicketComponent>;

    public constructor(
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        router: Router,
        nomenclatures: CommonNomenclatures,
        permissions: PermissionsService,
        paymentDataDialog: TLMatDialog<PaymentDataComponent>,
        statusReasonDialog: TLMatDialog<EnterReasonComponent>,
        editDialog: TLMatDialog<RecreationalFishingTicketComponent>
    ) {
        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.router = router;
        this.permissions = permissions;
        this.nomenclatures = nomenclatures;
        this.paymentDataDialog = paymentDataDialog;
        this.statusReasonDialog = statusReasonDialog;
        this.editDialog = editDialog;

        this.buildForm();
    }

    public ngOnInit(): void {
        this.setupPermissions();

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.TicketTypes, this.service.getTicketTypes.bind(this.service), false
        ).subscribe({
            next: (types: NomenclatureDTO<number>[]) => {
                this.types = types;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.TicketPeriods, this.service.getTicketPeriods.bind(this.service), false
        ).subscribe({
            next: (periods: NomenclatureDTO<number>[]) => {
                this.periods = periods;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.OfflinePaymentTypes, this.nomenclatures.getOfflinePaymentTypes.bind(this.nomenclatures), false
        ).subscribe({
            next: (paymentTypes: NomenclatureDTO<number>[]) => {
                this.paymentTypes = paymentTypes;
            }
        });

        this.isDuplicateOptions = [
            new NomenclatureDTO<ThreeState>({
                value: 'yes',
                displayName: this.translate.getValue('recreational-fishing.application-is-duplicate-yes'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'no',
                displayName: this.translate.getValue('recreational-fishing.application-is-duplicate-no'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'both',
                displayName: this.translate.getValue('recreational-fishing.application-is-duplicate-both'),
                isActive: true
            })
        ];

        if (!this.isAssociation) {
            const service: RecreationalFishingAdministrationService = this.service as RecreationalFishingAdministrationService;

            service.getAllTicketStatuses().subscribe({
                next: (statuses: NomenclatureDTO<number>[]) => {
                    this.statuses = statuses;
                }
            });
        }
    }

    public ngAfterViewInit(): void {
        if (this.isAssociation === true) {
            const service = this.service as RecreationalFishingPublicService;

            this.grid = new DataTableManager<RecreationalFishingTicketApplicationDTO, RecreationalFishingTicketApplicationFilters>({
                tlDataTable: this.datatable,
                searchPanel: this.searchpanel,
                requestServiceMethod: service.getAllAssociationTicketApplications.bind(this.service, service.currentUserChosenAssociation!.value!),
                filtersMapper: this.mapFilters.bind(this)
            });
        }
        else {
            this.grid = new DataTableManager<RecreationalFishingTicketApplicationDTO, RecreationalFishingTicketApplicationFilters>({
                tlDataTable: this.datatable,
                searchPanel: this.searchpanel,
                requestServiceMethod: this.getAllServiceMethod ?? this.service.getAllTicketApplications.bind(this.service),
                filtersMapper: this.mapFilters.bind(this)
            });

            const ticketNum: string | undefined = window.history.state?.ticketNum;

            if (!CommonUtils.isNullOrEmpty(ticketNum)) {
                this.grid.advancedFilters = new RecreationalFishingTicketApplicationFilters({ ticketNum: ticketNum });
            }

            const personId: number | undefined = window.history.state?.id;

            if (!CommonUtils.isNullOrEmpty(personId)) {
                this.grid.advancedFilters = new RecreationalFishingTicketApplicationFilters({ personId: personId });
            }
        }

        this.grid.refreshData();
    }

    public onAddClicked(): void {
        if (this.isAssociation) {
            this.router.navigate(['/association_ticket_issuing']);
        }
        else {
            this.router.navigate(['/ticket_issuing']);
        }
    }

    public deleteApplication(ticket: RecreationalFishingTicketApplicationDTO): void {
        this.confirmDialog.open({
            title: this.translate.getValue('recreational-fishing.delete-application-dialog-title'),
            message: this.translate.getValue('recreational-fishing.delete-application-dialog-msg'),
            okBtnLabel: this.translate.getValue('recreational-fishing.delete')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok === true) {
                    this.service.deleteApplication(ticket.id!).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
            }
        });
    }

    public restoreApplication(ticket: RecreationalFishingTicketApplicationDTO): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok === true) {
                    this.service.undoDeleteApplication(ticket.id!).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
            }
        });
    }

    public enterPaymentDataActionClicked(ticket: RecreationalFishingTicketApplicationDTO): void {
        const headerTitle: string = this.translate.getValue('applications-register.enter-payment-data-dialog-title');
        const data = {
            paymentTypes: this.paymentTypes,
            viewMode: false,
            service: this.applicationsService,
            applicationId: ticket.applicationId,
            paymentDateMax: new Date()
        };
        const dialog = this.paymentDataDialog.openWithTwoButtons({
            title: headerTitle,
            TCtor: PaymentDataComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeEnterPaymentDataDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate
        }, '850px');

        dialog.subscribe((paymentData: PaymentDataDTO) => {
            if (paymentData !== null && paymentData !== undefined) {
                this.grid.refreshData();
            }
        });
    }

    public paymentRefusalActoinClicked(ticket: RecreationalFishingTicketApplicationDTO): void {
        this.statusReasonDialog.openWithTwoButtons({
            title: this.translate.getValue('applications-register.payment-refusal-reason-dialog-title'),
            TCtor: EnterReasonComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closePaymentRefusalDialogBtnClicked.bind(this)
            },
            translteService: this.translate,
            componentData: new ReasonDialogParams({
                isApplication: true,
                id: ticket.applicationId,
                saveReasonServiceMethod: this.applicationsService.applicationPaymentRefusal.bind(this.applicationsService)
            }),
            disableDialogClose: true
        }, '800px')
            .subscribe((reasonData: ReasonData) => {
                if (reasonData !== null && reasonData !== undefined) {
                    this.grid.refreshData();
                }
            });
    }

    public cancellationActionClicked(ticket: RecreationalFishingTicketApplicationDTO): void {
        this.statusReasonDialog.openWithTwoButtons({
            title: this.translate.getValue('applications-register.cancellation-reason-dialog-title'),
            TCtor: EnterReasonComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeCancellationActionDialogBtnClicked.bind(this)
            },
            translteService: this.translate,
            componentData: new ReasonDialogParams({
                isApplication: true,
                id: ticket.applicationId,
                saveReasonServiceMethod: this.applicationsService.applicationAnnulment.bind(this.applicationsService)
            }),
            disableDialogClose: true
        }, '800px')
            .subscribe((reasonData: ReasonData) => {
                if (reasonData !== null && reasonData !== undefined) {
                    this.grid.refreshData();
                }
            });
    }

    public editActionClicked(ticket: RecreationalFishingTicketApplicationDTO, viewMode: boolean = false): void {
        let status: ApplicationStatusesEnum = ticket.applicationStatus!;

        if (status === ApplicationStatusesEnum.CORR_BY_USR_NEEDED && !viewMode) {
            this.applicationsService.initiateApplicationCorrections(ticket.applicationId!).subscribe({
                next: (newStatus: ApplicationStatusesEnum) => {
                    status = newStatus;
                    this.openEditDialog(ticket, status, ticket.prevStatusCode, viewMode);
                }
            });
        }
        else {
            this.openEditDialog(ticket, status, ticket.prevStatusCode, viewMode);
        }
    }

    public showTicket(ticket: RecreationalFishingTicketApplicationDTO): void {
        this.navigateByUrl('/ticket_applications', ticket.ticketNum!);
    }

    private setupPermissions(): void {
        if (this.isAssociation) {
            this.canAddRecords = this.permissions.has(PermissionsEnum.AssociationsTicketsAddRecords);
            this.canEditRecords = this.permissions.has(PermissionsEnum.AssociationsTicketApplicationsEditRecords);
            this.canDeleteRecords = this.permissions.has(PermissionsEnum.AssociationsTicketApplicationsDeleteRecords);
            this.canRestoreRecords = this.permissions.has(PermissionsEnum.AssociationsTicketApplicationsRestoreRecords);
            this.canProcessPaymentData = this.permissions.has(PermissionsEnum.AssociationsTicketApplicationsProcessPaymentData);
            this.canCancelRecords = this.permissions.has(PermissionsEnum.AssociationsTicketApplicationsCancel);
            this.canInspectAndCorrectRecords = this.permissions.has(PermissionsEnum.AssociationsTicketApplicationsInspectAndCorrectRegiXData);
        }
        else {
            this.canAddRecords = this.permissions.has(PermissionsEnum.TicketsAddRecords);
            this.canEditRecords = this.permissions.has(PermissionsEnum.TicketApplicationsEditRecords);
            this.canDeleteRecords = this.permissions.has(PermissionsEnum.TicketApplicationsDeleteRecords);
            this.canRestoreRecords = this.permissions.has(PermissionsEnum.TicketApplicationsRestoreRecords);
            this.canProcessPaymentData = this.permissions.has(PermissionsEnum.TicketApplicationsProcessPaymentData);
            this.canCancelRecords = this.permissions.has(PermissionsEnum.TicketApplicationsCancelRecord);
            this.canInspectAndCorrectRecords = this.permissions.has(PermissionsEnum.TicketApplicationsInspectAndCorrectRegiXData);
        }
    }

    private openEditDialog(
        ticket: RecreationalFishingTicketApplicationDTO,
        status: ApplicationStatusesEnum,
        prevApplicationStatus: ApplicationStatusesEnum | undefined,
        viewMode: boolean = false
    ): void {
        const params = new EditTicketDialogParams({
            id: ticket.id,
            applicationId: ticket.applicationId,
            isApplication: true,
            showOnlyRegiXData: false,
            showRegiXData: false,
            isReadonly: viewMode,
            viewMode: viewMode,
            isAssociation: this.isAssociation,
            isPersonal: false,
            type: this.types.find(x => x.value === ticket.ticketTypeId)!,
            period: this.periods.find(x => x.value === ticket.ticketPeriodId)!,
            service: this.service
        });
        debugger;
        if (viewMode) {
            this.openEditDialogForAll(params, this.translate.getValue('recreational-fishing.application-view-ticket-dialog-title'));
        }
        else {
            switch (status) {
                case ApplicationStatusesEnum.INSP_CORR_FROM_EMP: {
                    params.showOnlyRegiXData = true;
                    this.openEditDialogForRegix(params, this.translate.getValue('recreational-fishing.application-edit-ticket-regix-dialog-title'));
                } break;
                case ApplicationStatusesEnum.FILL_BY_APPL: {
                    // Ако не сме в RegiX режим И предишният статус на заявлението е бил Стартирани проверки
                    debugger;
                    if (prevApplicationStatus === ApplicationStatusesEnum.EXT_CHK_STARTED) {
                        params.showRegiXData = true;
                    }

                    this.openEditDialogForAll(params, this.translate.getValue('recreational-fishing.application-edit-ticket-dialog-title'));
                } break;
            }
        }
    }

    private openEditDialogForRegix(params: EditTicketDialogParams, title: string): void {
        this.editDialog.open({
            title: title,
            TCtor: RecreationalFishingTicketComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            headerAuditButton: {
                id: params.id,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'FishingTicket'
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: 'common.cancel'
            },
            rightSideActionsCollection: [
                {
                    id: 'save-and-start-regix-check',
                    color: 'primary',
                    translateValue: 'applications-register.save-and-start-regix-check',
                    buttonData: { callbackFn: this.saveApplicationAndStartRegixChecksActionClicked.bind(this) },
                    icon: { id: 'ic-fluent-doc-sync-24-regular', size: this.icIconSize },
                    disabled: true
                }, {
                    id: 'more-corrections-needed',
                    color: 'warn',
                    translateValue: 'applications-register.send-for-further-corrections',
                    buttonData: { callbackFn: this.sendApplicationForUserCorrectionsActionClicked.bind(this) },
                    icon: { id: 'ic-fluent-doc-person-20-regular', size: this.icIconSize }
                }, {
                    id: 'no-corrections-needed',
                    color: 'accent',
                    translateValue: 'applications-register.confirm-data-correctness',
                    buttonData: { callbackFn: this.confirmNoErrorsForApplicationActionClicked.bind(this) },
                    icon: { id: 'ic-fluent-doc-checkmark-24-regular', size: this.icIconSize }
                }
            ],
            translteService: this.translate,
            componentData: params,
            disableDialogClose: true,
            viewMode: params.viewMode
        }, '1400px')
            .subscribe({
                next: (result: unknown | undefined) => {
                    if (result !== undefined) {
                        this.grid.refreshData();
                    }
                }
            });
    }

    private openEditDialogForAll(params: EditTicketDialogParams, title: string): void {
        this.editDialog.openWithTwoButtons({
            title: title,
            TCtor: RecreationalFishingTicketComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            headerAuditButton: {
                id: params.id,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'FishingTicket'
            },
            rightSideActionsCollection: [{
                id: 'issue-duplicate',
                color: 'accent',
                translateValue: this.translate.getValue('recreational-fishing.issue-duplicate'),
                isVisibleInViewMode: true
            }, {
                id: 'print',
                color: 'accent',
                translateValue: this.translate.getValue('recreational-fishing.print'),
                isVisibleInViewMode: true
            }],
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: 'common.cancel'
            },
            saveBtn: {
                id: 'save',
                color: 'primary',
                translateValue: 'common.save'
            },
            translteService: this.translate,
            componentData: params,
            disableDialogClose: true,
            viewMode: params.viewMode
        }, '1400px')
            .subscribe({
                next: (result: unknown | undefined) => {
                    if (result !== undefined) {
                        this.grid.refreshData();
                    }
                }
            });
    }

    private saveApplicationAndStartRegixChecksActionClicked(model: IApplicationRegister, dialogClose: DialogCloseCallback): void {
        (this.service as RecreationalFishingAdministrationService).editApplicationDataAndStartRegixChecks(model).subscribe(() => {
            dialogClose(model);
        });
    }

    private confirmNoErrorsForApplicationActionClicked(applicationId: number, dialogClose: DialogCloseCallback): void {
        this.applicationsService.confirmNoErrorsForApplication(applicationId).subscribe(() => {
            dialogClose(applicationId);
        });
    }

    private sendApplicationForUserCorrectionsActionClicked(model: IApplicationRegister, dialogClose: DialogCloseCallback): void {
        this.statusReasonDialog.openWithTwoButtons({
            title: this.translate.getValue('applications-register.send-for-user-corrections-reason-dialog-title'),
            TCtor: EnterReasonComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeCancellationActionDialogBtnClicked.bind(this)
            },
            translteService: this.translate,
            componentData: new ReasonDialogParams({
                isApplication: true,
                id: model.applicationId,
                reasonFieldValue: model.statusReason
            }),
            disableDialogClose: true
        }, '800px')
            .subscribe((reasonData: ReasonData) => {
                if (reasonData !== undefined) {
                    model.statusReason = reasonData.reason;
                    this.applicationsService.sendApplicationForUserCorrections(model.applicationId!, model.statusReason!).subscribe(() => {
                        this.grid.refreshData();
                        dialogClose();
                    });
                }
            });
    }

    private closeEnterPaymentDataDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closePaymentRefusalDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeCancellationActionDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            ticketNumControl: new FormControl(),
            typesControl: new FormControl(),
            periodsControl: new FormControl(),
            holderControl: new FormControl(),
            holderEgnControl: new FormControl(),
            validFromControl: new FormControl(),
            validToControl: new FormControl(),
            issuerControl: new FormControl(),
            isDuplicateControl: new FormControl(),
            statusesControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): RecreationalFishingTicketApplicationFilters {
        const result = new RecreationalFishingTicketApplicationFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            ticketNum: filters.getValue('ticketNumControl'),
            typeIds: filters.getValue('typesControl'),
            periodIds: filters.getValue('periodsControl'),
            ticketHolderName: filters.getValue('holderControl'),
            ticketHolderEGN: filters.getValue('holderEgnControl'),
            ticketIssuerName: filters.getValue('issuerControl'),
            validFrom: filters.getValue('validFromControl'),
            validTo: filters.getValue('validToControl'),
            statusIds: filters.getValue('statusesControl')
        });

        const isDuplicate: ThreeState | undefined = filters.getValue<ThreeState>('isDuplicateControl');
        switch (isDuplicate) {
            case 'yes':
                result.isDuplicate = true;
                break;
            case 'no':
                result.isDuplicate = false;
                break;
            default:
            case 'both':
                result.isDuplicate = undefined;
                break;
        }

        return result;
    }

    private navigateByUrl(url: string, ticketNum: string): void {
        this.router.navigateByUrl(url, { state: { ticketNum: ticketNum } });
    }
}
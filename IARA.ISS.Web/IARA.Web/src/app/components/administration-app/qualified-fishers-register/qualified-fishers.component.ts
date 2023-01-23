import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { QualifiedFishersFilters } from '@app/models/generated/filters/QualifiedFishersFilters';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { QualifiedFisherDTO } from '@app/models/generated/dtos/QualifiedFisherDTO';
import { QualifiedFisherEditDTO } from '@app/models/generated/dtos/QualifiedFisherEditDTO';
import { QualifiedFishersService } from '@app/services/administration-app/qualified-fishers.service';
import { IRemoteTLDatatableComponent } from '@app/shared/components/data-table/interfaces/tl-remote-datatable.interface';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { MessageService } from '@app/shared/services/message.service';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { BasePageComponent } from '@app/components/common-app/base-page.component';
import { EditFisherComponent } from '@app/components/common-app/qualified-fishers/edit-fisher.component';
import { DeliveryAdministrationService } from '@app/services/administration-app/delivery-administration.service';
import { IDeliveryService } from '@app/interfaces/common-app/delivery.interface';
import { RegisterDeliveryComponent } from '@app/shared/components/register-delivery/register-delivery.component';
import { RegisterDeliveryDialogParams } from '@app/shared/components/register-delivery/models/register-delivery-dialog-params.model';
import { ApplicationDeliveryDTO } from '@app/models/generated/dtos/ApplicationDeliveryDTO';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { MaritimeEducationFishermanDialogParamsModel } from '@app/components/common-app/qualified-fishers/models/maritime-education-fisherman-dialog-params.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { ChooseApplicationComponent } from '@app/components/common-app/applications/components/choose-application/choose-application.component';
import { ChooseApplicationDialogParams } from '@app/components/common-app/applications/components/choose-application/models/choose-application-dialog-params.model';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ApplicationForChoiceDTO } from '@app/models/generated/dtos/ApplicationForChoiceDTO';
import { Router } from '@angular/router';

@Component({
    selector: 'qualified-fishers-register',
    templateUrl: './qualified-fishers.component.html'
})
export class QualifiedFishersComponent extends BasePageComponent implements AfterViewInit {
    public translationService: FuseTranslationLoaderService;
    public filterFormGroup: FormGroup;

    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;
    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;
    public readonly canDeleteRecords: boolean;
    public readonly canRestoreRecords: boolean;
    public readonly canAddDiplomaFishermanRecords: boolean;

    public readonly canReadApplications: boolean;

    @ViewChild(TLDataTableComponent)
    private datatable!: IRemoteTLDatatableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private readonly service: QualifiedFishersService;
    private gridManager!: DataTableManager<QualifiedFisherDTO, QualifiedFishersFilters>;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly editDialog: TLMatDialog<EditFisherComponent>;
    private readonly deliveryService!: IDeliveryService;
    private readonly deliveryDialog: TLMatDialog<RegisterDeliveryComponent>;
    private readonly chooseApplicationDialog: TLMatDialog<ChooseApplicationComponent>;
    private readonly router: Router;

    public constructor(
        translationService: FuseTranslationLoaderService,
        service: QualifiedFishersService,
        editDialog: TLMatDialog<EditFisherComponent>,
        confirmDialog: TLConfirmDialog,
        permissions: PermissionsService,
        messageService: MessageService,
        deliveryService: DeliveryAdministrationService,
        deliveryDialog: TLMatDialog<RegisterDeliveryComponent>,
        chooseApplicationDialog: TLMatDialog<ChooseApplicationComponent>,
        router: Router
    ) {
        super(messageService);

        this.canAddRecords = permissions.has(PermissionsEnum.QualifiedFishersAddRecords);
        this.canEditRecords = permissions.has(PermissionsEnum.QualifiedFishersEditRecords);
        this.canDeleteRecords = permissions.has(PermissionsEnum.QualifiedFishersDeleteRecords);
        this.canRestoreRecords = permissions.has(PermissionsEnum.QualifiedFishersRestoreRecords);
        this.canAddDiplomaFishermanRecords = permissions.has(PermissionsEnum.QualifiedFishersAddDiplomaFishermanRecords);

        this.canReadApplications = permissions.has(PermissionsEnum.QualifiedFishersApplicationsRead) || permissions.has(PermissionsEnum.QualifiedFishersApplicationsReadAll);

        this.translationService = translationService;
        this.service = service;
        this.editDialog = editDialog;
        this.confirmDialog = confirmDialog;
        this.deliveryService = deliveryService;
        this.deliveryDialog = deliveryDialog;
        this.chooseApplicationDialog = chooseApplicationDialog;
        this.router = router;

        this.filterFormGroup = new FormGroup({
            nameControl: new FormControl(),
            egnControl: new FormControl(),
            dateRangeControl: new FormControl(),
            registrationNumControl: new FormControl(),
            diplomaNumberControl: new FormControl()
        });
    }

    public ngAfterViewInit(): void {

        this.gridManager = new DataTableManager<QualifiedFisherDTO, QualifiedFishersFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAll.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        this.datatable.activeRecordChanged.subscribe({
            next: (element: QualifiedFisherDTO) => {
                if (element.isActive) {
                    this.editEntry(element, !this.canEditRecords);
                }
            }
        });

        const personId: number | undefined = window.history.state?.id;

        if (!CommonUtils.isNullOrEmpty(personId)) {
            this.gridManager.advancedFilters = new QualifiedFishersFilters({ personId: personId });
        }

        this.gridManager.refreshData();
    }

    private mapFilters(filters: FilterEventArgs): QualifiedFishersFilters {
        const filtersObj = new QualifiedFishersFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords
        });

        filtersObj.name = filters.getValue('nameControl');
        filtersObj.egn = filters.getValue('egnControl');
        filtersObj.registeredDateFrom = filters.getValue<DateRangeData>('dateRangeControl')?.start;
        filtersObj.registeredDateTo = filters.getValue<DateRangeData>('dateRangeControl')?.end;
        filtersObj.registrationNum = filters.getValue('registrationNumControl');
        filtersObj.diplomaNr = filters.getValue('diplomaNumberControl');

        return filtersObj;
    }

    public addEntry(): void {
        let data: DialogParamsModel | undefined;
        let headerTitle: string = '';

        this.chooseApplicationDialog.open({
            TCtor: ChooseApplicationComponent,
            title: this.translationService.getValue('applications-register.choose-application-for-register-creation'),
            translteService: this.translationService,
            componentData: new ChooseApplicationDialogParams({
                pageCodes: [
                    PageCodeEnum.CommFishLicense
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
                    showOnlyRegiXData: false,
                    isThirdCountry: false,
                    isReadonly: false,
                    isApplication: false,
                    viewMode: false,
                    service: this.service,
                    applicationId: dialogData.selectedApplication.id,
                    pageCode: dialogData.selectedApplication.pageCode
                });

                headerTitle = this.translationService.getValue('qualified-fishers-page.add-qualified-fisher-dialog-title');

                this.openQualifiedFisherDialog(data, undefined, headerTitle, false);
            }
        });
    }

    public editEntry(entry?: QualifiedFisherDTO, readOnly?: boolean): void {
        let data: DialogParamsModel | MaritimeEducationFishermanDialogParamsModel | undefined;
        let headerAuditBtn: IHeaderAuditButton | undefined;

        let headerTitle: string = '';

        if (entry !== undefined && entry.id !== undefined) {
            if (entry.isWithMaritimeEducation === true) {
                data = new MaritimeEducationFishermanDialogParamsModel({
                    id: entry.id,
                    viewMode: readOnly ?? false,
                    service: this.service,
                    pageCode: entry.pageCode
                });
            }
            else {
                data = new DialogParamsModel({
                    id: entry.id,
                    isApplication: false,
                    viewMode: readOnly,
                    service: this.service,
                    pageCode: entry.pageCode
                });
            }

            headerAuditBtn = {
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                id: entry.id,
                tableName: 'FishermenRegister'
            } as IHeaderAuditButton;

            if (readOnly) {
                headerTitle = this.translationService.getValue('qualified-fishers-page.view-dialog');
            }
            else {
                headerTitle = this.translationService.getValue('qualified-fishers-page.edit-dialog');
            }
        }
        else {
            headerTitle = this.translationService.getValue('qualified-fishers-page.add-dialog');
        }

        this.openQualifiedFisherDialog(data, headerAuditBtn, headerTitle, readOnly);
    }

    public addDiplomaFishermanEntry(): void {
        const headerTitle: string = this.translationService.getValue('qualified-fishers-page.add-fisherman-widh-diploma-dialog-title');
        const data: MaritimeEducationFishermanDialogParamsModel = new MaritimeEducationFishermanDialogParamsModel({
            viewMode: false,
            service: this.service
        });

        this.openQualifiedFisherDialog(data, undefined, headerTitle);
    }

    public addThirdCountryDiplomaFishermanEntry(): void {
        const headerTitle: string = this.translationService.getValue('qualified-fishers-page.add-third-country-fisherman-widh-diploma-dialog-title');
        const data: MaritimeEducationFishermanDialogParamsModel = new MaritimeEducationFishermanDialogParamsModel({
            viewMode: false,
            service: this.service,
            isThirdCountryFisherman: true
        });

        this.openQualifiedFisherDialog(data, undefined, headerTitle);
    }

    private openQualifiedFisherDialog(
        data: DialogParamsModel | MaritimeEducationFishermanDialogParamsModel | undefined,
        headerAuditBtn: IHeaderAuditButton | undefined,
        headerTitle: string,
        viewMode: boolean = false
    ): void {
        let rightButtons: IActionInfo[] | undefined = undefined;

        if (data instanceof DialogParamsModel) {
            rightButtons = [
                {
                    id: 'print',
                    color: 'accent',
                    translateValue: viewMode ? 'qualified-fishers-page.print' : 'qualified-fishers-page.save-print',
                    isVisibleInViewMode: true
                }
            ];
        }

        const dialogResult = this.editDialog.open({
            title: headerTitle,
            TCtor: EditFisherComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            componentData: data,
            viewMode: viewMode,
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
            rightSideActionsCollection: rightButtons
        }, '100em');

        dialogResult.subscribe((result: QualifiedFisherEditDTO | undefined) => {
            if (result !== undefined && result !== null) {
                this.gridManager.refreshData();
            }
        });
    }

    public openDeliveryDialog(entry: QualifiedFisherDTO): void {
        let auditButton: IHeaderAuditButton | undefined;

        if (entry.deliveryId !== null && entry.deliveryId !== undefined) {
            auditButton = {
                id: entry.deliveryId,
                getAuditRecordData: this.deliveryService.getSimpleAudit.bind(this.deliveryService),
                tableName: 'ApplicationDelivery'
            };
        }

        this.deliveryDialog.openWithTwoButtons({
            TCtor: RegisterDeliveryComponent,
            title: this.translationService.getValue('commercial-fishing.delivery-data-dialog-title'),
            translteService: this.translationService,
            componentData: new RegisterDeliveryDialogParams({
                deliveryId: entry.deliveryId,
                isPublicApp: false,
                service: this.deliveryService,
                pageCode: entry.pageCode,
                registerId: entry.id,
                viewMode: !this.canEditRecords
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

    public deleteFisher(entry: QualifiedFisherDTO): void {
        if (entry !== undefined) {
            this.confirmDialog.open({
                title: this.translationService.getValue('qualified-fishers-page.delete-fisher'),
                message: `${this.translationService.getValue('qualified-fishers-page.delete-fisher-sure')} ${entry.name}?`,
                okBtnLabel: this.translationService.getValue('qualified-fishers-page.do-delete')
            }).subscribe((result: boolean) => {
                if (result) {
                    if (entry.id != undefined) {
                        this.service.delete(entry.id).subscribe({
                            next: () => {
                                this.gridManager.deleteRecord(entry);
                            }
                        });
                    }
                }
            });
        }
    }

    public restoreFisher(entry: QualifiedFisherDTO): void {
        if (entry.id != undefined) {
            this.service.undoDelete(entry.id).subscribe({
                next: () => {
                    this.gridManager.undoDeleteRecord(entry);
                }
            });
        }
    }

    public gotToApplication(entry: QualifiedFisherDTO): void {
        if (this.canReadApplications) {
            this.router.navigate(['qualified-fishers-applications'], { state: { applicationId: entry.applicationId } });
        }
    }

    private closeDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeDeliveryDataDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeApplicationChooseDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}
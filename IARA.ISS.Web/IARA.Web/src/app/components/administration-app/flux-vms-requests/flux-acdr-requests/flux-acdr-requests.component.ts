﻿import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IRemoteTLDatatableComponent } from '@app/shared/components/data-table/interfaces/tl-remote-datatable.interface';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { ViewFluxVmsRequestsComponent } from '../view-flux-vms-requests.component';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { FluxVmsRequestsService } from '@app/services/administration-app/flux-vms-requests.service';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { FluxResponseStatuses } from '@app/enums/flux-response-statuses.enum';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { FluxAcdrRequestDTO } from '@app/models/generated/dtos/FluxAcdrRequestDTO';
import { FluxFvmsDomainsEnum } from '@app/enums/flux-fvms-domains.enum';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { EditFluxAcdrRequestsComponent } from './edit-flux-acdr-request/edit-flux-acdr-request.component';
import { ViewFluxVmsRequestsDialogParams } from '../models/view-flux-vms-requests-dialog-params.model';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { UploadFluxAcdrRequestsComponent } from './upload-flux-acdr-request/upload-flux-acdr-request.component';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { DatePipe } from '@angular/common';
import { FluxAcdrRequestFilters } from '@app/models/generated/filters/FluxAcdrRequestFilters';
import { FluxAcdrReportStatusEnum } from '@app/enums/flux-acdr-report-status.enum';
import { FluxAcdrRequestEditDTO } from '@app/models/generated/dtos/FluxAcdrRequestEditDTO';
import { FluxAcdrReportDTO } from '@app/models/generated/dtos/FluxAcdrReportDTO';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';

type ThreeState = 'yes' | 'no' | 'both';

@Component({
    selector: 'flux-acdr-requests',
    templateUrl: './flux-acdr-requests.component.html'
})
export class FluxAcdrRequestsComponent implements OnInit, AfterViewInit {
    public translate: FuseTranslationLoaderService;
    public form: FormGroup;

    public responseStatuses: NomenclatureDTO<string>[] = [];
    public reportStatuses: NomenclatureDTO<string>[] = [];
    public isModifiedOptions: NomenclatureDTO<ThreeState>[] = [];

    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;

    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;
    public readonly faIconSize: number = CommonUtils.FA_ICON_SIZE;

    public readonly fluxAcdrReportStatusEnum: typeof FluxAcdrReportStatusEnum = FluxAcdrReportStatusEnum;

    @ViewChild(TLDataTableComponent)
    private datatable!: IRemoteTLDatatableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private grid!: DataTableManager<FluxAcdrRequestDTO, FluxAcdrRequestFilters>;
    private service: FluxVmsRequestsService;
    private viewDialog: TLMatDialog<ViewFluxVmsRequestsComponent>;
    private queryDialog: TLMatDialog<EditFluxAcdrRequestsComponent>;
    private uploadDialog: TLMatDialog<UploadFluxAcdrRequestsComponent>;
    private confirmDialog: TLConfirmDialog;
    private datePipe: DatePipe;

    public constructor(
        translate: FuseTranslationLoaderService,
        service: FluxVmsRequestsService,
        viewDialog: TLMatDialog<ViewFluxVmsRequestsComponent>,
        queryDialog: TLMatDialog<EditFluxAcdrRequestsComponent>,
        uploadDialog: TLMatDialog<UploadFluxAcdrRequestsComponent>,
        confirmDialog: TLConfirmDialog,
        permissions: PermissionsService,
        datePipe: DatePipe
    ) {
        this.translate = translate;
        this.service = service;

        this.viewDialog = viewDialog;
        this.queryDialog = queryDialog;
        this.uploadDialog = uploadDialog;
        this.confirmDialog = confirmDialog;
        this.datePipe = datePipe;

        this.canAddRecords = permissions.has(PermissionsEnum.FLUXVMSRequestsAddRecords);
        this.canEditRecords = permissions.has(PermissionsEnum.FLUXVMSRequestsEditRecords);

        this.form = this.buildForm();
    }

    public ngOnInit(): void {
        for (const level in FluxResponseStatuses) {
            if (isNaN(Number(level))) {
                this.responseStatuses.push(new NomenclatureDTO<string>({
                    value: level,
                    displayName: level !== FluxResponseStatuses[FluxResponseStatuses.NoResponse]
                        ? level
                        : this.translate.getValue('flux-vms-requests.no-response-status'),
                    isActive: true
                }));
            }
        }

        this.reportStatuses = [
            new NomenclatureDTO<string>({
                value: FluxAcdrReportStatusEnum[FluxAcdrReportStatusEnum.GENERATED],
                displayName: this.translate.getValue('flux-vms-requests.acdr-report-status-generated'),
            }),
            new NomenclatureDTO<string>({
                value: FluxAcdrReportStatusEnum[FluxAcdrReportStatusEnum.MANUAL],
                displayName: this.translate.getValue('flux-vms-requests.acdr-report-status-manual'),
            }),
            new NomenclatureDTO<string>({
                value: FluxAcdrReportStatusEnum[FluxAcdrReportStatusEnum.DOWNLOADED],
                displayName: this.translate.getValue('flux-vms-requests.acdr-report-status-downloaded'),
            }),
            new NomenclatureDTO<string>({
                value: FluxAcdrReportStatusEnum[FluxAcdrReportStatusEnum.UPLOADED],
                displayName: this.translate.getValue('flux-vms-requests.acdr-report-status-uploaded'),
            }),
            new NomenclatureDTO<string>({
                value: FluxAcdrReportStatusEnum[FluxAcdrReportStatusEnum.SENT],
                displayName: this.translate.getValue('flux-vms-requests.acdr-report-status-sent'),
            })
        ];

        this.isModifiedOptions = [
            new NomenclatureDTO<ThreeState>({
                value: 'yes',
                displayName: this.translate.getValue('flux-vms-requests.acdr-is-modified-yes'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'no',
                displayName: this.translate.getValue('flux-vms-requests.acdr-is-modified-no'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'both',
                displayName: this.translate.getValue('flux-vms-requests.acdr-is-modified-both'),
                isActive: true
            })
        ];
    }

    public ngAfterViewInit(): void {
        this.grid = new DataTableManager<FluxAcdrRequestDTO, FluxAcdrRequestFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllAcdrRequests.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        this.grid.refreshData();
    }

    public openViewDialog(report: FluxAcdrReportDTO): void {
        const data: ViewFluxVmsRequestsDialogParams = new ViewFluxVmsRequestsDialogParams({
            id: report.requestId,
            acdrId: report.id,
            reportStatus: report.reportStatus
        });

        const headerAuditBtn: IHeaderAuditButton = {
            id: report.requestId!,
            getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
            tableName: 'iss.FLUXFVMSRequests'
        };

        this.viewDialog.open({
            title: this.translate.getValue('flux-vms-requests.request-dialog-title'),
            TCtor: ViewFluxVmsRequestsComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction): void => {
                    closeFn();
                }
            },
            headerAuditButton: headerAuditBtn,
            componentData: data,
            translteService: this.translate,
            viewMode: true
        });
    }

    public downloadRequest(acdrId: number): void {
        this.service.downloadAcdrRequestContent(acdrId).subscribe({
            next: (result: boolean | undefined) => {
                if (result) {
                    setTimeout(() => {
                        this.grid.refreshData();
                    }, 2000);
                }
            }
        });
    }

    public replayRequest(report: FluxAcdrReportDTO): void {
        this.confirmDialog.open().toPromise().then(result => {
            if (result) {
                const webServiceNameParts: string[] = report.webServiceName?.split('/') ?? [];

                this.service.replayRequest(report.requestId as number, FluxFvmsDomainsEnum[webServiceNameParts[0] as keyof typeof FluxFvmsDomainsEnum], webServiceNameParts[1])
                    .subscribe();

                setTimeout(() => {
                    this.grid.refreshData();
                }, 2000);
            }
        });
    }

    public uploadRequest(acdr: FluxAcdrRequestDTO): void {
        const title: string = `${this.translate.getValue('flux-vms-requests.ecr-query-request-title')} ${this.datePipe.transform(acdr.periodEnd, 'MM.yyyy')}`;

        this.uploadDialog.open({
            title: title,
            TCtor: UploadFluxAcdrRequestsComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction): void => {
                    closeFn();
                }
            },
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translate.getValue('flux-vms-requests.generate-btn-label')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translate.getValue('common.cancel'),
            },
            componentData: new DialogParamsModel({ id: acdr.fluxRequestId }),
            translteService: this.translate,
            viewMode: false
        }, '600px').subscribe({
            next: (result: boolean | undefined) => {
                if (result) {
                    this.grid.refreshData();
                }
            }
        });
    }

    public generateAcdrQueryForMonth(acdr: FluxAcdrRequestDTO): void {
        this.confirmDialog.open().toPromise().then(result => {
            if (result) {
                const request: FluxAcdrRequestEditDTO = new FluxAcdrRequestEditDTO({
                    month: acdr.periodMonth,
                    year: acdr.periodYear
                });

                this.service.addAcdrQueryRequest(request).subscribe({
                    next: () => {
                        setTimeout(() => {
                            this.grid.refreshData();
                        }, 2000);
                    }
                });
            }
        });
    }

    public openAcdrQueryDialog(): void {
        this.queryDialog.open({
            title: this.translate.getValue('flux-vms-requests.ecr-query-request-title'),
            TCtor: EditFluxAcdrRequestsComponent,
            headerAuditButton: undefined,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction): void => {
                    closeFn();
                }
            },
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translate.getValue('flux-vms-requests.generate-btn-label')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translate.getValue('common.cancel'),
            },
            componentData: undefined,
            translteService: this.translate
        }, '600px').subscribe({
            next: (result: boolean | undefined) => {
                if (result) {
                    setTimeout(() => {
                        this.grid.refreshData();
                    }, 2000);
                }
            }
        });
    }

    private buildForm(): FormGroup {
        const result: FormGroup = new FormGroup({
            monthControl: new FormControl(),
            requestDateTimeFromControl: new FormControl(),
            requestDateTimeToControl: new FormControl(),
            responseDateTimeFromControl: new FormControl(),
            responseDateTimeToControl: new FormControl(),
            requestUuidControl: new FormControl(),
            responseUuidControl: new FormControl(),
            responseStatusControl: new FormControl(),
            requestContentControl: new FormControl(),
            responseContentControl: new FormControl(),
            reportStatusControl: new FormControl(),
            isModifiedControl: new FormControl()
        });

        return result;
    }

    private mapFilters(filters: FilterEventArgs): FluxAcdrRequestFilters {
        const result: FluxAcdrRequestFilters = new FluxAcdrRequestFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            requestDateFrom: filters.getValue('requestDateTimeFromControl'),
            requestDateTo: filters.getValue('requestDateTimeToControl'),
            responseDateFrom: filters.getValue('responseDateTimeFromControl'),
            responseDateTo: filters.getValue('responseDateTimeToControl'),
            requestUUID: filters.getValue('requestUuidControl'),
            responseUUID: filters.getValue('responseUuidControl'),
            responseStatuses: filters.getValue('responseStatusControl'),
            requestContent: filters.getValue('requestContentControl'),
            responseContent: filters.getValue('responseContentControl'),
            reportStatuses: filters.getValue('reportStatusControl'),
            requestMonthDateFrom: filters.getValue('monthControl')
        });

        const isModified: ThreeState | undefined = filters.getValue<ThreeState>('isModifiedControl');
        switch (isModified) {
            case 'yes':
                result.isModified = true;
                break;
            case 'no':
                result.isModified = false;
                break;
            default:
            case 'both':
                result.isModified = undefined;
                break;
        }

        return result;
    }
}
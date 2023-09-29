import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IRemoteTLDatatableComponent } from '@app/shared/components/data-table/interfaces/tl-remote-datatable.interface';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { FLUXVMSRequestDTO } from '@app/models/generated/dtos/FLUXVMSRequestDTO';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { FLUXVMSRequestFilters } from '@app/models/generated/filters/FLUXVMSRequestFilters';
import { FluxVmsRequestsService } from '@app/services/administration-app/flux-vms-requests.service';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { ViewFluxVmsRequestsComponent } from './view-flux-vms-requests.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { FluxResponseStatuses } from '@app/enums/flux-response-statuses.enum';
import { FluxFvmsDomainsEnum } from '@app/enums/flux-fvms-domains.enum';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FluxFAQueryComponent } from './flux-fa-query/flux-fa-query.component';
import { FluxSalesQueryComponent } from './flux-sales-query/flux-sales-query.component';
import { FluxVesselQueryComponent } from './flux-vessel-query/flux-vessel-query.component';

@Component({
    selector: 'flux-vms-requests',
    templateUrl: './flux-vms-requests.component.html'
})
export class FluxVmsRequestsComponent implements OnInit, AfterViewInit {
    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public responseStatuses: NomenclatureDTO<string>[] = [];
    public domainNames: NomenclatureDTO<string>[] = [];

    public fluxFlapRequestsLoaded: boolean = false;

    @ViewChild(TLDataTableComponent)
    private datatable!: IRemoteTLDatatableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private gridManager!: DataTableManager<FLUXVMSRequestDTO, FLUXVMSRequestFilters>
    private service: FluxVmsRequestsService;
    private viewDialog: TLMatDialog<ViewFluxVmsRequestsComponent>;
    private confirmDialog: TLConfirmDialog;
    private faQueryDialog: TLMatDialog<FluxFAQueryComponent>;
    private salesQueryDialog: TLMatDialog<FluxSalesQueryComponent>;
    private vesselQueryDialog: TLMatDialog<FluxVesselQueryComponent>;

    public constructor(
        translate: FuseTranslationLoaderService,
        service: FluxVmsRequestsService,
        viewDialog: TLMatDialog<ViewFluxVmsRequestsComponent>,
        confirmDialog: TLConfirmDialog,
        faQueryDialog: TLMatDialog<FluxFAQueryComponent>,
        salesQueryDialog: TLMatDialog<FluxSalesQueryComponent>,
        vesselQueryDialog: TLMatDialog<FluxVesselQueryComponent>
    ) {
        this.translate = translate;
        this.service = service;
        this.confirmDialog = confirmDialog;
        this.viewDialog = viewDialog;
        this.faQueryDialog = faQueryDialog;
        this.salesQueryDialog = salesQueryDialog;
        this.vesselQueryDialog = vesselQueryDialog;

        this.buildForm();
    }

    public ngOnInit(): void {
        for (const level in FluxResponseStatuses) {
            if (isNaN(Number(level))) {
                this.responseStatuses.push(new NomenclatureDTO<string>({
                    value: level,
                    displayName: level,
                    isActive: true
                }));
            }
        }

        for (const domain in FluxFvmsDomainsEnum) {
            if (isNaN(Number(domain))) {
                this.domainNames.push(new NomenclatureDTO<string>({
                    value: domain,
                    displayName: domain,
                    isActive: true
                }));
            }
        }
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<FLUXVMSRequestDTO, FLUXVMSRequestFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAll.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        this.gridManager.refreshData();
    }

    public openDialog(request: FLUXVMSRequestDTO): void {
        this.viewDialog.open({
            title: this.translate.getValue('flux-vms-requests.request-dialog-title'),
            TCtor: ViewFluxVmsRequestsComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction): void => {
                    closeFn();
                }
            },
            componentData: new DialogParamsModel({ id: request.id }),
            translteService: this.translate,
            viewMode: true
        });
    }

    public replayRequest(request: FLUXVMSRequestDTO): void {

        this.confirmDialog.open().toPromise().then(result => {
            if (result) {
                const webServiceNameParts: string[] = request.webServiceName?.split('/') ?? [];

                this.service.replayRequest(request.id as number, FluxFvmsDomainsEnum[webServiceNameParts[0] as keyof typeof FluxFvmsDomainsEnum], webServiceNameParts[1])
                    .subscribe();
            }
        });
    }

    public tabChanged(event: MatTabChangeEvent): void {
        if (event.index === 1) {
            this.fluxFlapRequestsLoaded = true;
        }
    }

    public openFluxFAQueryDialog(): void {
        this.faQueryDialog.openWithTwoButtons({
            title: this.translate.getValue('flux-vms-requests.fa-query-request-title'),
            TCtor: FluxFAQueryComponent,
            headerAuditButton: undefined,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction): void => {
                    closeFn();
                }
            },
            componentData: undefined,
            translteService: this.translate
        }, '1000px').subscribe({
            next: (result: boolean | undefined) => {
                if (result) {
                    setTimeout(() => {
                        this.gridManager.refreshData();
                    }, 2000);
                }
            }
        });
    }

    public openFluxSalesQueryDialog(): void {
        this.salesQueryDialog.openWithTwoButtons({
            title: this.translate.getValue('flux-vms-requests.sales-query-request-title'),
            TCtor: FluxSalesQueryComponent,
            headerAuditButton: undefined,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction): void => {
                    closeFn();
                }
            },
            componentData: undefined,
            translteService: this.translate
        }, '1000px').subscribe({
            next: (result: boolean | undefined) => {
                if (result) {
                    setTimeout(() => {
                        this.gridManager.refreshData();
                    }, 2000);
                }
            }
        });
    }

    public openFluxVesselQueryDialog(): void {
        this.vesselQueryDialog.openWithTwoButtons({
            title: this.translate.getValue('flux-vms-requests.vessel-query-request-title'),
            TCtor: FluxVesselQueryComponent,
            headerAuditButton: undefined,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction): void => {
                    closeFn();
                }
            },
            componentData: undefined,
            translteService: this.translate
        }, '1000px').subscribe({
            next: (result: boolean | undefined) => {
                if (result) {
                    setTimeout(() => {
                        this.gridManager.refreshData();
                    }, 2000);
                }
            }
        });
    }

    private buildForm(): void {
        this.form = new FormGroup({
            webServiceNameControl: new FormControl(),
            requestDateTimeFromControl: new FormControl(),
            requestDateTimeToControl: new FormControl(),
            responseDateTimeFromControl: new FormControl(),
            responseDateTimeToControl: new FormControl(),
            requestUuidControl: new FormControl(),
            responseUuidControl: new FormControl(),
            responseStatusControl: new FormControl(),
            requestContentControl: new FormControl(),
            responseContentControl: new FormControl(),
            domainNameControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): FLUXVMSRequestFilters {
        const result: FLUXVMSRequestFilters = new FLUXVMSRequestFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            webServiceName: filters.getValue('webServiceNameControl'),
            requestDateFrom: filters.getValue('requestDateTimeFromControl'),
            requestDateTo: filters.getValue('requestDateTimeToControl'),
            responseDateFrom: filters.getValue('responseDateTimeFromControl'),
            responseDateTo: filters.getValue('responseDateTimeToControl'),
            requestUUID: filters.getValue('requestUuidControl'),
            responseUUID: filters.getValue('responseUuidControl'),
            responseStatuses: filters.getValue('responseStatusControl'),
            requestContent: filters.getValue('requestContentControl'),
            responseContent: filters.getValue('responseContentControl'),
            domainNames: filters.getValue('domainNameControl')
        });

        return result;
    }
}
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
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { FluxResponseStatuses } from '@app/enums/flux-response-statuses.enum';
import { FluxFvmsDomainsEnum } from '../../../enums/flux-fvms-domains.enum';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';

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


    public constructor(
        translate: FuseTranslationLoaderService,
        service: FluxVmsRequestsService,
        viewDialog: TLMatDialog<ViewFluxVmsRequestsComponent>,
        confirmDialog: TLConfirmDialog
    ) {
        this.translate = translate;
        this.service = service;
        this.confirmDialog = confirmDialog;
        this.viewDialog = viewDialog;

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

    private buildForm(): void {
        this.form = new FormGroup({
            webServiceNameControl: new FormControl(),
            requestDateTimeControl: new FormControl(),
            responseDateTimeControl: new FormControl(),
            requestUuidControl: new FormControl(),
            responseUuidControl: new FormControl(),
            responseStatusControl: new FormControl(),
            domainNameControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): FLUXVMSRequestFilters {
        const result: FLUXVMSRequestFilters = new FLUXVMSRequestFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            webServiceName: filters.getValue('webServiceNameControl'),
            requestDateFrom: filters.getValue<DateRangeData>('requestDateTimeControl')?.start,
            requestDateTo: filters.getValue<DateRangeData>('requestDateTimeControl')?.end,
            responseDateFrom: filters.getValue<DateRangeData>('responseDateTimeControl')?.start,
            responseDateTo: filters.getValue<DateRangeData>('responseDateTimeControl')?.end,
            requestUUID: filters.getValue('requestUuidControl'),
            responseUUID: filters.getValue('responseUuidControl'),
            responseStatuses: filters.getValue('responseStatusControl'),
            domainNames: filters.getValue('domainNameControl')
        });

        return result;
    }
}
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { IFluxVmsRequestsService } from '@app/interfaces/administration-app/flux-vms-requests.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { FluxFlapRequestDTO } from '@app/models/generated/dtos/FluxFlapRequestDTO';
import { FluxFlapRequestEditDTO } from '@app/models/generated/dtos/FluxFlapRequestEditDTO';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { FluxFlapRequestFilters } from '@app/models/generated/filters/FluxFlapRequestFilters';
import { FluxVmsRequestsService } from '@app/services/administration-app/flux-vms-requests.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { IRemoteTLDatatableComponent } from '@app/shared/components/data-table/interfaces/tl-remote-datatable.interface';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { EditFluxFlapRequestComponent } from './edit-flux-flap-request/edit-flux-flap-request.component';

@Component({
    selector: 'flux-flap-requests',
    templateUrl: './flux-flap-requests.component.html'
})
export class FluxFlapRequestsComponent implements OnInit, AfterViewInit {
    public translate: FuseTranslationLoaderService;
    public form: FormGroup;

    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;

    public ships: ShipNomenclatureDTO[] = [];

    @ViewChild(TLDataTableComponent)
    private datatable!: IRemoteTLDatatableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private grid!: DataTableManager<FluxFlapRequestDTO, FluxFlapRequestFilters>;
    private service: IFluxVmsRequestsService;
    private nomenclatures: CommonNomenclatures;
    private editDialog: TLMatDialog<EditFluxFlapRequestComponent>;

    public constructor(
        translate: FuseTranslationLoaderService,
        service: FluxVmsRequestsService,
        nomenclatures: CommonNomenclatures,
        editDialog: TLMatDialog<EditFluxFlapRequestComponent>,
        permissions: PermissionsService
    ) {
        this.translate = translate;
        this.service = service;
        this.nomenclatures = nomenclatures;
        this.editDialog = editDialog;

        this.canAddRecords = permissions.has(PermissionsEnum.FLUXVMSRequestsAddRecords);
        this.canEditRecords = permissions.has(PermissionsEnum.FLUXVMSRequestsEditRecords);

        this.form = this.buildForm();
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false
        ).subscribe({
            next: (ships: ShipNomenclatureDTO[]) => {
                this.ships = ships;
            }
        });
    }

    public ngAfterViewInit(): void {
        this.grid = new DataTableManager<FluxFlapRequestDTO, FluxFlapRequestFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllFlapRequests.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        this.grid.refreshData();
    }

    public openDialog(flap: FluxFlapRequestDTO | undefined): void {
        let title: string;
        let audit: IHeaderAuditButton | undefined;
        let data: DialogParamsModel | undefined;

        if (flap === undefined) {
            title = this.translate.getValue('flux-vms-requests.flap-request-add-dialog-title');
        }
        else {
            title = this.translate.getValue('flux-vms-requests.flap-request-view-dialog-title');

            audit = {
                id: flap.id!,
                getAuditRecordData: this.service.getFlapRequestAudit.bind(this.service),
                tableName: 'iss.FluxFlapRequest'
            };

            data = new DialogParamsModel({
                id: flap.id,
                viewMode: true
            });
        }

        this.editDialog.openWithTwoButtons({
            title: title,
            TCtor: EditFluxFlapRequestComponent,
            headerAuditButton: audit,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction): void => {
                    closeFn()
                }
            },
            componentData: data,
            translteService: this.translate,
            viewMode: flap !== undefined
        }, '1200px').subscribe({
            next: (result: FluxFlapRequestEditDTO | undefined) => {
                if (result !== undefined) {
                    this.grid.refreshData();
                }
            }
        });
    }

    private buildForm(): FormGroup {
        return new FormGroup({
            shipControl: new FormControl(),
            shipIdentifierControl: new FormControl(),
            shipNameControl: new FormControl(),
            requestUuidControl: new FormControl(),
            requestDateControl: new FormControl(),
            responseUuidControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): FluxFlapRequestFilters {
        const result: FluxFlapRequestFilters = new FluxFlapRequestFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            shipId: filters.getValue('shipControl'),
            shipIdentifier: filters.getValue('shipIdentifierControl'),
            shipName: filters.getValue('shipNameControl'),
            requestUuid: filters.getValue('requestUuidControl'),
            requestDateFrom: filters.getValue<DateRangeData>('requestDateControl')?.start,
            requestDateTo: filters.getValue<DateRangeData>('requestDateControl')?.end,
            responseUuid: filters.getValue('responseUuidControl')
        });

        return result;
    }
}
import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IFishingCapacityService } from '@app/interfaces/common-app/fishing-capacity.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { MaximumFishingCapacityDTO } from '@app/models/generated/dtos/MaximumFishingCapacityDTO';
import { MaximumFishingCapacityEditDTO } from '@app/models/generated/dtos/MaximumFishingCapacityEditDTO';
import { MaximumFishingCapacityFilters } from '@app/models/generated/filters/MaximumFishingCapacityFilters';
import { FishingCapacityAdministrationService } from '@app/services/administration-app/fishing-capacity-administration.service';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { EditMaximumFishingCapacityComponent } from './edit-maximum-fishing-capacity/edit-maximum-fishing-capacity.component';

@Component({
    selector: 'maximum-fishing-capacity',
    templateUrl: './maximum-fishing-capacity.component.html'
})
export class MaximumFishingCapacityComponent implements AfterViewInit {
    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private service: IFishingCapacityService;
    private editDialog: TLMatDialog<EditMaximumFishingCapacityComponent>;
    private grid!: DataTableManager<MaximumFishingCapacityDTO, MaximumFishingCapacityFilters>;

    public constructor(
        translate: FuseTranslationLoaderService,
        service: FishingCapacityAdministrationService,
        editDialog: TLMatDialog<EditMaximumFishingCapacityComponent>,
        permissions: PermissionsService
    ) {
        this.translate = translate;
        this.service = service;
        this.editDialog = editDialog;

        this.canAddRecords = permissions.has(PermissionsEnum.MaximumCapacityAddRecords);
        this.canEditRecords = permissions.has(PermissionsEnum.MaximumCapacityEditRecords);

        this.buildForm();
    }

    public ngAfterViewInit(): void {
        this.grid = new DataTableManager<MaximumFishingCapacityDTO, MaximumFishingCapacityFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllMaximumCapacities.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        this.grid.refreshData();
    }

    public createEditMaximumCapacity(capacity: MaximumFishingCapacityDTO | undefined, viewMode: boolean): void {
        let data: DialogParamsModel | undefined;
        let auditButton: IHeaderAuditButton | undefined;
        let title: string;

        if (capacity?.id !== undefined) {
            data = new DialogParamsModel({
                id: capacity.id,
                isReadonly: viewMode
            });
            auditButton = {
                id: capacity.id,
                getAuditRecordData: this.service.getMaximumCapacitySimpleAudit.bind(this.service),
                tableName: 'CountryCapacityRegister'
            };

            title = viewMode
                ? this.translate.getValue('maximum-capacity.view-capacity-dialog-title')
                : this.translate.getValue('maximum-capacity.edit-capacity-dialog-title');
        }
        else {
            title = this.translate.getValue('maximum-capacity.add-capacity-dialog-title');
        }

        const dialog = this.editDialog.openWithTwoButtons({
            title: title,
            TCtor: EditMaximumFishingCapacityComponent,
            headerAuditButton: auditButton,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: !viewMode,
            viewMode: viewMode
        }, '1000px');

        dialog.subscribe({
            next: (entry?: MaximumFishingCapacityEditDTO) => {
                if (entry !== undefined) {
                    this.grid.refreshData();
                }
            }
        });
    }

    private buildForm(): void {
        this.form = new FormGroup({
            regulationControl: new FormControl(),
            dateFromControl: new FormControl(),
            dateToControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): MaximumFishingCapacityFilters {
        const result: MaximumFishingCapacityFilters = new MaximumFishingCapacityFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            regulation: filters.getValue('regulationControl'),
            dateFrom: filters.getValue('dateFromControl'),
            dateTo: filters.getValue('dateToControl')
        });

        return result;
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PermissionRegisterDTO } from '@app/models/generated/dtos/PermissionRegisterDTO';
import { PermissionRegisterEditDTO } from '@app/models/generated/dtos/PermissionRegisterEditDTO';
import { PermissionsRegisterFilters } from '@app/models/generated/filters/PermissionsRegisterFilters';
import { PermissionsRegisterService } from '@app/services/administration-app/permissions-register.service';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { EditPermissionComponent } from './components/edit-permission/edit-permission.component';
import { IPermissionsRegisterService } from '@app/interfaces/administration-app/permissions-register.interface';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';

@Component({
    selector: 'permissions-register',
    templateUrl: './permissions-register.component.html'
})
export class PermissionsRegisterComponent implements OnInit, AfterViewInit {
    public translateService: FuseTranslationLoaderService;
    public formGroup!: FormGroup;

    public types: NomenclatureDTO<number>[] = [];
    public groups: NomenclatureDTO<number>[] = [];
    public roles: NomenclatureDTO<number>[] = [];

    public readonly canEditRecords: boolean;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private grid!: DataTableManager<PermissionRegisterDTO, PermissionsRegisterFilters>;
    private service: IPermissionsRegisterService;
    private editDialog: TLMatDialog<EditPermissionComponent>;

    public constructor(
        service: PermissionsRegisterService,
        translateService: FuseTranslationLoaderService,
        editDialog: TLMatDialog<EditPermissionComponent>,
        permissions: PermissionsService
    ) {
        this.service = service;
        this.translateService = translateService;
        this.editDialog = editDialog;

        this.canEditRecords = permissions.has(PermissionsEnum.PermissionsRegisterEditRecords);

        this.buildForm();
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.PermissionTypes, this.service.getPermissionTypes.bind(this.service), false
        ).subscribe({
            next: (types: NomenclatureDTO<number>[]) => {
                this.types = types;
            }
        });

        NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.PermissionGroups, this.service.getPermissionGroups.bind(this.service), false
        ).subscribe({
            next: (groups: NomenclatureDTO<number>[]) => {
                this.groups = groups;
            }
        });

        NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.Roles, this.service.getAllRoles.bind(this.service), false
        ).subscribe({
            next: (roles: NomenclatureDTO<number>[]) => {
                this.roles = roles;
            }
        });
    }

    public ngAfterViewInit(): void {
        this.grid = new DataTableManager<PermissionRegisterDTO, PermissionsRegisterFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllPermissions.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        this.grid.refreshData();
    }

    public editPermission(permission: PermissionRegisterDTO, viewMode: boolean): void {
        const data: DialogParamsModel = new DialogParamsModel({
            id: permission.id!,
            isReadonly: viewMode
        });

        const auditButton: IHeaderAuditButton = {
            id: permission.id!,
            getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
            tableName: 'Npermission'
        };

        const title: string = viewMode
            ? this.translateService.getValue('permissions-register.view-permission-dialog-title')
            : this.translateService.getValue('permissions-register.edit-permission-dialog-title');

        const dialog = this.editDialog.openWithTwoButtons({
            title: title,
            TCtor: EditPermissionComponent,
            headerAuditButton: auditButton,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translateService,
            disableDialogClose: !viewMode,
            viewMode: viewMode
        }, '1000px');

        dialog.subscribe({
            next: (entry: PermissionRegisterEditDTO | undefined) => {
                if (entry !== undefined && entry !== null) {
                    this.grid.refreshData();
                }
            }
        });
    }

    private mapFilters(filters: FilterEventArgs): PermissionsRegisterFilters {
        const result: PermissionsRegisterFilters = new PermissionsRegisterFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            name: filters.getValue('nameControl'),
            groupId: filters.getValue('groupControl'),
            roleId: filters.getValue('roleControl'),
            typeIds: filters.getValue('typeControl')
        });
        return result;
    }

    private buildForm(): void {
        this.formGroup = new FormGroup({
            nameControl: new FormControl(),
            groupControl: new FormControl(),
            typeControl: new FormControl(),
            roleControl: new FormControl()
        });
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}

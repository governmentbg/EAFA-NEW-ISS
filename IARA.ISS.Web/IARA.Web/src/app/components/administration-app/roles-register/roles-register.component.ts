import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RoleRegisterDTO } from '@app/models/generated/dtos/RoleRegisterDTO';
import { RoleRegisterEditDTO } from '@app/models/generated/dtos/RoleRegisterEditDTO';
import { RolesRegisterFilters } from '@app/models/generated/filters/RolesRegisterFilters';
import { RolesRegisterService } from '@app/services/administration-app/roles-register.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { EditRoleComponent } from './components/edit-role/edit-role.component';
import { ReplaceRoleComponent } from './components/replace-role/replace-role.component';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';

@Component({
    selector: 'roles-register',
    templateUrl: './roles-register.component.html'
})
export class RolesRegisterComponent implements OnInit, AfterViewInit {
    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public permissions: NomenclatureDTO<number>[] = [];

    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;
    public readonly canDeleteRecords: boolean;
    public readonly canRestoreRecords: boolean;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private grid!: DataTableManager<RoleRegisterDTO, RolesRegisterFilters>;

    private service: RolesRegisterService;
    private nomenclatures: CommonNomenclatures;
    private confirmDialog: TLConfirmDialog;
    private editDialog: TLMatDialog<EditRoleComponent>;
    private replaceDialog: TLMatDialog<ReplaceRoleComponent>;

    public constructor(
        rolesService: RolesRegisterService,
        nomenclaturesService: CommonNomenclatures,
        translationService: FuseTranslationLoaderService,
        permissions: PermissionsService,
        confirmDialog: TLConfirmDialog,
        editDialog: TLMatDialog<EditRoleComponent>,
        replaceDialog: TLMatDialog<ReplaceRoleComponent>
    ) {
        this.service = rolesService;
        this.nomenclatures = nomenclaturesService;
        this.translate = translationService;
        this.confirmDialog = confirmDialog;
        this.editDialog = editDialog;
        this.replaceDialog = replaceDialog;

        this.canAddRecords = permissions.has(PermissionsEnum.RolesRegisterAddRecords);
        this.canEditRecords = permissions.has(PermissionsEnum.RolesRegisterEditRecords);
        this.canDeleteRecords = permissions.has(PermissionsEnum.RolesRegisterDeleteRecords);
        this.canRestoreRecords = permissions.has(PermissionsEnum.RolesRegisterRestoreRecords);

        this.buildForm();
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Permissions, this.nomenclatures.getPermissions.bind(this.nomenclatures), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.permissions = result;
            }
        });
    }

    public ngAfterViewInit(): void {
        this.grid = new DataTableManager<RoleRegisterDTO, RolesRegisterFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllRoles.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        this.grid.refreshData();
    }

    public createEditRole(role: RoleRegisterDTO | undefined, viewMode: boolean): void {
        let data: DialogParamsModel | undefined;
        let auditButton: IHeaderAuditButton | undefined;
        let title: string;

        if (role?.id !== undefined) {
            data = new DialogParamsModel({ id: role.id, isReadonly: viewMode });

            auditButton = {
                id: role.id,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'Role'
            };

            title = viewMode
                ? this.translate.getValue('roles-register.view-role-dialog-title')
                : this.translate.getValue('roles-register.edit-role-dialog-title');
        }
        else {
            title = this.translate.getValue('roles-register.add-role-dialog-title');
        }

        const dialog = this.editDialog.openWithTwoButtons({
            title: title,
            TCtor: EditRoleComponent,
            headerAuditButton: auditButton,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: true,
            viewMode: viewMode
        }, '1400px');

        dialog.subscribe({
            next: (entry: RoleRegisterEditDTO | undefined) => {
                if (entry !== undefined && entry !== null) {
                    this.grid.refreshData();
                }
            }
        });
    }

    public deleteRole(role: RoleRegisterDTO): void {
        if (role.usersCount === 0) {
            this.confirmDialog.open({
                title: this.translate.getValue('roles-register.delete-role-dialog-title'),
                message: this.translate.getValue('roles-register.confirm-delete-message'),
                okBtnLabel: this.translate.getValue('roles-register.delete')
            }).subscribe({
                next: (ok: boolean) => {
                    if (ok === true) {
                        this.service.deleteRole(role.id!).subscribe({
                            next: () => {
                                this.grid.refreshData();
                            }
                        });
                    }
                }
            });
        }
        else {
            const dialog = this.replaceDialog.openWithTwoButtons({
                title: this.translate.getValue('roles-register.delete-role-dialog-title'),
                TCtor: ReplaceRoleComponent,
                headerCancelButton: {
                    cancelBtnClicked: this.closeReplaceDialogBtnClicked.bind(this)
                },
                componentData: new DialogParamsModel({ id: role.id! }),
                translteService: this.translate,
                disableDialogClose: true
            }, '800px');

            dialog.subscribe({
                next: (ok: boolean) => {
                    if (ok === true) {
                        this.grid.refreshData();
                    }
                }
            });
        }
    }

    public restoreRole(role: RoleRegisterDTO): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok === true) {
                    this.service.undoDeleteRole(role.id!).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
            }
        });
    }

    private buildForm(): void {
        this.form = new FormGroup({
            codeControl: new FormControl(),
            nameControl: new FormControl(),
            permissionControl: new FormControl(),
            validityDateRangeControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): RolesRegisterFilters {
        const result = new RolesRegisterFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            code: filters.getValue('codeControl'),
            name: filters.getValue('nameControl'),
            permissionId: filters.getValue('permissionControl'),
            validFrom: filters.getValue<DateRangeData>('validityDateRangeControl')?.start,
            validTo: filters.getValue<DateRangeData>('validityDateRangeControl')?.end
        });
        return result;
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeReplaceDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}

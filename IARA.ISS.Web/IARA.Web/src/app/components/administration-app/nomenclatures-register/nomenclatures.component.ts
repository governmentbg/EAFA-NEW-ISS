import { FlatTreeControl } from '@angular/cdk/tree';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatTreeFlatDataSource, MatTreeFlattener } from '@angular/material/tree';
import { Observable } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PermissionTypeEnum } from '@app/enums/permission-type.enum';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ColumnDTO } from '@app/models/generated/dtos/ColumnDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureTableDTO } from '@app/models/generated/dtos/NomenclatureTableDTO';
import { NomenclaturesFilters } from '@app/models/generated/filters/NomenclaturesFilters';
import { NomenclaturesRegisterService } from '@app/services/administration-app/nomenclatures-register.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { MessageService } from '@app/shared/services/message.service';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { BasePageComponent } from '@app/components/common-app/base-page.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { INomenclaturesService } from '@app/interfaces/administration-app/nomenclatures-register.interface';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { EditNomenclatureComponent } from './edit-nomenclatures.component';

interface ITreeNode {
    id?: number;
    name: string;
    children?: ITreeNode[]
}

class FlatNode {
    public id: number | undefined;
    public level: number = 0;
    public name: string = '';
    public expandable: boolean = false;

    public constructor(init?: Partial<FlatNode>) {
        Object.assign(this, init);
    }
}

class ColumnDef {
    public columnName: string | undefined;
    public propertyNamePascal: string | undefined;
    public propertyNameCamel: string | undefined;
    public dataType: string | undefined;
    public width: number | undefined;
    public isUnique: boolean | undefined;

    public constructor(init?: Partial<ColumnDef>) {
        Object.assign(this, init);
    }
}

interface IsActiveNomenclature {
    isActive: boolean | undefined;
}

interface ValidityNomenclature {
    validFrom: Date | undefined;
    validTo: Date | undefined;
}

@Component({
    selector: 'nomenclatures',
    templateUrl: './nomenclatures.component.html',
    styleUrls: ['./nomenclatures.component.scss']
})
export class NomenclaturesComponent extends BasePageComponent implements OnInit {
    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public canAddRecords: boolean = false;
    public canEditRecords: boolean = false;
    public canDeleteRecords: boolean = false;
    public canRestoreRecords: boolean = false;

    public dataColumns: ColumnDef[] = [];

    public selectedNode: FlatNode | undefined;
    public treeControl: FlatTreeControl<FlatNode>;
    public treeDataSource: MatTreeFlatDataSource<ITreeNode, FlatNode>;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    private treeFlattener: MatTreeFlattener<ITreeNode, FlatNode>;

    private nomenclatureGroups: NomenclatureDTO<number>[] = [];
    private nomenclatureTables: NomenclatureTableDTO[] = [];
    private selectedTable!: NomenclatureTableDTO;

    private service: INomenclaturesService;
    private confirmDialog: TLConfirmDialog;
    private editDialog: TLMatDialog<EditNomenclatureComponent>;

    private grid!: DataTableManager<Record<string, unknown>, NomenclaturesFilters>;

    private readonly hasAddPermission: boolean = false;
    private readonly hasEditPermission: boolean = false;
    private readonly hasDeletePermission: boolean = false;
    private readonly hasRestorePermission: boolean = false;

    private readonly tablePermissions: Map<number, PermissionTypeEnum[]> = new Map<number, PermissionTypeEnum[]>();
    private readonly tableColumns: Map<number, ColumnDTO[]> = new Map<number, ColumnDTO[]>();

    public constructor(
        translate: FuseTranslationLoaderService,
        service: NomenclaturesRegisterService,
        confirmDialog: TLConfirmDialog,
        editDialog: TLMatDialog<EditNomenclatureComponent>,
        permissions: PermissionsService,
        messageService: MessageService
    ) {
        super(messageService);

        this.translate = translate;
        this.service = service;
        this.editDialog = editDialog;
        this.confirmDialog = confirmDialog;

        this.hasAddPermission = permissions.has(PermissionsEnum.NomenclaturesAddRecords);
        this.hasEditPermission = permissions.has(PermissionsEnum.NomenclaturesEditRecords);
        this.hasDeletePermission = permissions.has(PermissionsEnum.NomenclaturesDeleteRecords);
        this.hasRestorePermission = permissions.has(PermissionsEnum.NomenclaturesRestoreRecords);

        this.treeFlattener = new MatTreeFlattener(this.treeFlattenerTransformer, node => node.level, node => node.expandable, node => node.children);
        this.treeControl = new FlatTreeControl<FlatNode>(node => node.level, node => node.expandable);
        this.treeDataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

        this.buildForm();
    }

    public ngOnInit(): void {
        this.getNomenclatureGroups().subscribe({
            next: (nomenclatureGroups: NomenclatureDTO<number>[]) => {
                this.nomenclatureGroups = nomenclatureGroups;

                this.getNomenclatureTables().subscribe({
                    next: (nomenclatureTables: NomenclatureTableDTO[]) => {
                        this.nomenclatureTables = nomenclatureTables;

                        if (this.nomenclatureTables.some(x => x.groupId === undefined || x.groupId === null)) {
                            this.nomenclatureGroups.push(new NomenclatureDTO<number>({
                                value: undefined,
                                displayName: this.translate.getValue('nomenclatures-page.other')
                            }));
                        }

                        const groups: NomenclatureDTO<number>[] = nomenclatureGroups.filter(x => this.nomenclatureTables.some(y => y.groupId === x.value));

                        this.treeDataSource.data = groups.map((x: NomenclatureDTO<number>) => {
                            const tables: NomenclatureTableDTO[] = nomenclatureTables.filter(y => y.groupId === x.value);

                            const children: ITreeNode[] = tables.map((y: NomenclatureTableDTO) => {
                                return {
                                    id: y.value,
                                    name: y.displayName!,
                                    children: []
                                };
                            });

                            const node: ITreeNode = {
                                name: this.translate.getValue('nomenclatures-page.' + x.displayName).replace('nomenclatures-page.', ''),
                                children: children
                            };

                            return node;
                        });
                    }
                });
            }
        });
    }

    public addEditNomenclature(entity: Record<string, unknown>, readonly: boolean): void {
        let data: DialogParamsModel | undefined;
        let auditBtn: IHeaderAuditButton | undefined;
        let title: string = '';

        if (entity !== undefined && entity !== null) {
            data = new DialogParamsModel({
                id: entity.id as number,
                isReadonly: readonly
            });

            auditBtn = {
                id: entity.id as number,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service)
            };

            title = this.translate.getValue('nomenclatures-page.edit-dialog');
        }
        else {
            title = this.translate.getValue('nomenclatures-page.add-dialog');
        }

        const dialogResult = this.editDialog.openWithTwoButtons({
            title: title,
            TCtor: EditNomenclatureComponent,
            headerAuditButton: auditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            viewMode: readonly
        }, '900px');

        dialogResult.subscribe({
            next: (result: Record<string, unknown> | undefined) => {
                if (result) {
                    this.grid.refreshData();
                }
            }
        });
    }

    public deleteNomenclature(entity: Record<string, unknown>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('nomenclatures-page.delete-nom'),
            message: this.translate.getValue('nomenclatures-page.delete-nom-sure'),
            okBtnLabel: this.translate.getValue('nomenclatures-page.do-delete')
        }).subscribe({
            next: (result: boolean) => {
                if (result === true) {
                    this.service.delete(entity.id as number).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
            }
        });
    }

    public restoreNomenclature(entity: Record<string, unknown>): void {
        this.service.undoDelete(entity.id as number).subscribe({
            next: () => {
                this.grid.refreshData();
            }
        });
    }

    public isActive(element: Record<string, unknown>): boolean {
        if (this.hasIsActive(element)) {
            return element.isActive!;
        }

        if (this.hasValidity(element)) {
            const now: Date = new Date();
            return new Date(element.validFrom!) <= now && new Date(element.validTo!) > now;
        }

        return false;
    }

    public nodeSelected(node: FlatNode): void {
        this.selectedNode = node;
        this.selectedTable = this.nomenclatureTables.find(x => x.value === node.id)!;

        if (this.selectedTable.value !== null && this.selectedTable.value !== undefined) {
            this.service.tableId = this.selectedTable.value;
        }

        this.messageService.sendMessage(this.translate.getValue('navigation.nomenclatures') + ' – ' + node.name);

        this.reloadTable();
    }

    public hasChild(_: number, node: FlatNode): boolean {
        return node.expandable;
    }

    private reloadTable(): void {
        this.reloadTablePermissions();
        this.reloadTableColumns();
    }

    private reloadTablePermissions(): void {
        const permissions: PermissionTypeEnum[] | undefined = this.tablePermissions.get(this.service.tableId!);
        if (permissions !== undefined) {
            this.setPermissions(permissions);
        }
        else {
            this.service.getTablePermissions().subscribe({
                next: (result: PermissionTypeEnum[]) => {
                    this.tablePermissions.set(this.service.tableId!, result);
                    this.setPermissions(result);
                }
            });
        }
    }

    private setPermissions(permissions: PermissionTypeEnum[]): void {
        this.canAddRecords = this.hasAddPermission && permissions.some(x => x === PermissionTypeEnum.ADD);
        this.canEditRecords = this.hasEditPermission && permissions.some(x => x === PermissionTypeEnum.EDIT);
        this.canDeleteRecords = this.hasDeletePermission && permissions.some(x => x === PermissionTypeEnum.DELETE);
        this.canRestoreRecords = this.hasRestorePermission && permissions.some(x => x === PermissionTypeEnum.RESTORE);
    }

    private reloadTableColumns(): void {
        const columns: ColumnDTO[] | undefined = this.tableColumns.get(this.service.tableId!);
        if (columns !== undefined) {
            this.setDataColumns(columns);
            this.refreshGrid();
        }
        else {
            this.service.getColumns().subscribe({
                next: (columns: ColumnDTO[]) => {
                    this.tableColumns.set(this.service.tableId!, columns);

                    this.setDataColumns(columns);
                    this.refreshGrid();
                }
            });
        }
    }

    private setDataColumns(columns: ColumnDTO[]): void {
        this.dataColumns = columns
            .filter(x => !x.isForeignKey)
            .map(x => {
                return new ColumnDef({
                    columnName: this.getColumnName(x),
                    propertyNamePascal: x.propertyName!,
                    propertyNameCamel: x.propertyName!.charAt(0).toLowerCase() + x.propertyName!.slice(1),
                    dataType: CommonUtils.toColumnDataType(x.dataType),
                    width: NomenclaturesComponent.getFlexRate(x.propertyName)
                })
            });
    }

    private refreshGrid(): void {
        if (this.grid === undefined || this.grid === null) {
            this.grid = new DataTableManager<Record<string, unknown>, NomenclaturesFilters>({
                tlDataTable: this.datatable,
                searchPanel: this.searchpanel,
                requestServiceMethod: this.service.getAll.bind(this.service),
                filtersMapper: this.mapFilters.bind(this)
            });
        }

        this.grid.refreshData();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            // TODO
        });
    }

    private mapFilters(filters: FilterEventArgs): NomenclaturesFilters {
        const filtersObj = new NomenclaturesFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            tableId: this.selectedTable.value
        });

        return filtersObj;
    }

    private closeDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private getColumnName(column: ColumnDTO): string {
        const key: string = 'nomenclatures-page.' + column.propertyName;
        const name: string = this.translate.getValue(key);
        return name !== key ? name : column.displayName!;
    }

    private static getFlexRate(property: string | undefined): number {
        switch (property?.toLowerCase()) {
            case 'code':
                return 0.33;
            case 'validfrom':
                return 0.4;
            case 'validto':
                return 0.4;
            default:
                return 1;
        }
    }

    private treeFlattenerTransformer(node: ITreeNode, level: number): FlatNode {
        return new FlatNode({
            expandable: !!node.children && node.children.length > 0,
            name: node.name,
            level: level,
            id: node.id,
        });
    }

    private getNomenclatureGroups(): Observable<NomenclatureDTO<number>[]> {
        return NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.NomenclatureGroups, this.service.getGroups.bind(this.service), false
        );
    }

    private getNomenclatureTables(): Observable<NomenclatureTableDTO[]> {
        return NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.NomenclatureTables, this.service.getTables.bind(this.service), false
        );
    }

    private hasIsActive(element: unknown): element is IsActiveNomenclature {
        const record: Record<string, unknown> = this.asRecord(element);
        return 'isActive' in record || 'IsActive' in record;
    }

    private hasValidity(element: unknown): element is ValidityNomenclature {
        const record: Record<string, unknown> = this.asRecord(element);
        return ('validFrom' in record || 'ValidFrom' in record) && ('validTo' in record || 'ValidTo' in record);
    }

    private asRecord(element: unknown): Record<string, unknown> {
        return element as Record<string, unknown>;
    }
}
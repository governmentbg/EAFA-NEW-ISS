import { FlatTreeControl } from '@angular/cdk/tree';
import { AfterViewInit, Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatTreeFlatDataSource, MatTreeFlattener } from '@angular/material/tree';
import { forkJoin } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PermissionTypeEnum } from '@app/enums/permission-type.enum';
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
import { MenuService } from '@app/shared/services/menu.service';
import { ColumnDef } from './models/column-def.model';
import { FlatNode } from './models/flat-node.model';
import { TreeNode } from './interfaces/tree-node.interface';
import { IsActiveNomenclature } from './interfaces/is-active-nomenclaure.interface';
import { ValidityNomenclature } from './interfaces/validity-nomenclature.interface';
import { EditNomenclatureParams } from './models/edit-nomenclature-params.model';

@Component({
    selector: 'nomenclatures',
    templateUrl: './nomenclatures.component.html',
    styleUrls: ['./nomenclatures.component.scss']
})
export class NomenclaturesComponent extends BasePageComponent implements OnInit, AfterViewInit {
    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public canAddRecords: boolean = false;
    public canEditRecords: boolean = false;
    public canDeleteRecords: boolean = false;
    public canRestoreRecords: boolean = false;

    public dataColumns: ColumnDef[] = [];

    public selectedNode: FlatNode | undefined;
    public treeControl: FlatTreeControl<FlatNode>;
    public treeDataSource: MatTreeFlatDataSource<TreeNode, FlatNode>;

    public searchInputControl: FormControl = new FormControl();
    public isTreeExpanded: boolean = true;

    public containerHeightPx: number = 0;
    public mainPanelWidthPx: number = 0;
    public mainPanelHeightPx: number = 0;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    private treeFlattener: MatTreeFlattener<TreeNode, FlatNode>;

    private host: HTMLElement;
    private toolbarElement!: HTMLElement;
    private containerElement!: HTMLElement;
    private treePanelElement!: HTMLElement;

    private nomenclatureGroups: NomenclatureDTO<number>[] = [];
    private nomenclatureTables: NomenclatureTableDTO[] = [];
    private selectedTable!: NomenclatureTableDTO;

    private service: INomenclaturesService;
    private confirmDialog: TLConfirmDialog;
    private menuService: MenuService;
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
        menuService: MenuService,
        host: ElementRef,
        messageService: MessageService
    ) {
        super(messageService);

        this.translate = translate;
        this.service = service;
        this.editDialog = editDialog;
        this.confirmDialog = confirmDialog;
        this.menuService = menuService;

        this.host = host.nativeElement as HTMLElement;

        this.hasAddPermission = permissions.has(PermissionsEnum.NomenclaturesAddRecords);
        this.hasEditPermission = permissions.has(PermissionsEnum.NomenclaturesEditRecords);
        this.hasDeletePermission = permissions.has(PermissionsEnum.NomenclaturesDeleteRecords);
        this.hasRestorePermission = permissions.has(PermissionsEnum.NomenclaturesRestoreRecords);

        this.treeFlattener = new MatTreeFlattener(
            this.treeFlattenerTransformer,
            (node: FlatNode) => node.level,
            (node: FlatNode) => node.expandable,
            (node: TreeNode) => node.children
        );

        this.treeControl = new FlatTreeControl<FlatNode>(
            (node: FlatNode) => node.level,
            (node: FlatNode) => node.expandable
        );

        this.treeDataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

        this.searchInputControl.valueChanges.subscribe({
            next: (value: string) => {
                this.setTreeDataSource(value);
            }
        });

        this.buildForm();
    }

    public ngOnInit(): void {
        forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.NomenclatureGroups, this.service.getGroups.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.NomenclatureTables, this.service.getTables.bind(this.service), false)
        ).subscribe({
            next: ([nomenclatureGroups, nomenclatureTables]: [NomenclatureDTO<number>[], NomenclatureTableDTO[]]) => {
                this.nomenclatureGroups = nomenclatureGroups;
                this.nomenclatureTables = nomenclatureTables;

                if (this.nomenclatureTables.some(x => x.groupId === undefined || x.groupId === null)) {
                    this.nomenclatureGroups.push(new NomenclatureDTO<number>({
                        value: undefined,
                        displayName: this.translate.getValue('nomenclatures-page.other'),
                        isActive: true
                    }));
                }

                this.setTreeDataSource();
            }
        });
    }

    public ngAfterViewInit(): void {
        this.calculateContainerHeightPx();
        this.calculateMainPanelWidthPx();

        this.menuService.folded.subscribe({
            next: () => {
                setTimeout(() => {
                    this.calculateMainPanelWidthPx();
                });
            }
        });
    }

    @HostListener('window:resize')
    public onWindowResize(): void {
        this.calculateContainerHeightPx();
        this.calculateMainPanelWidthPx();
    }

    public addEditNomenclature(entity: Record<string, unknown> | undefined, readonly: boolean): void {
        let data: EditNomenclatureParams;
        let auditBtn: IHeaderAuditButton | undefined;
        let title: string;

        if (entity !== undefined && entity !== null) {
            data = new EditNomenclatureParams({
                id: entity.id as number,
                viewMode: readonly,
                columns: this.tableColumns.get(this.service.tableId!)!
            });

            auditBtn = {
                id: entity.id as number,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service)
            };

            title = this.translate.getValue('nomenclatures-page.edit-dialog');
        }
        else {
            data = new EditNomenclatureParams({
                id: undefined,
                viewMode: false,
                columns: this.tableColumns.get(this.service.tableId!)!
            });

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

    public isValidityNomenclature(element: Record<string, unknown>): boolean {
        return this.hasValidity(element);
    }

    public nodeSelected(node: FlatNode): void {
        this.selectedNode = node;
        this.selectedTable = this.nomenclatureTables.find(x => x.value === node.id)!;

        if (this.selectedTable.value !== null && this.selectedTable.value !== undefined) {
            this.service.tableId = this.selectedTable.value;
        }

        this.messageService.sendMessage(this.translate.getValue('navigation.nomenclatures') + ' – ' + node.name);

        this.reloadTable();

        this.calculateMainPanelWidthPx();
    }

    public hasChild(_: number, node: FlatNode): boolean {
        return node.expandable;
    }

    public toggleTree(): void {
        if (this.isTreeExpanded) {
            this.isTreeExpanded = false;
        }
        else {
            setTimeout(() => {
                this.isTreeExpanded = true;
            }, 180);
        }
    }

    public onTreePanelToggled(event: TransitionEvent): void {
        if (event.propertyName === 'width') {
            this.calculateMainPanelWidthPx();
        }
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
            .filter((column: ColumnDTO) => {
                return !column.isForeignKey;
            })
            .map((column: ColumnDTO) => {
                return new ColumnDef({
                    columnName: this.getColumnName(column),
                    propertyNamePascal: column.propertyName!,
                    propertyNameCamel: column.propertyName!.charAt(0).toLowerCase() + column.propertyName!.slice(1),
                    dataType: CommonUtils.toColumnDataType(column.dataType),
                    width: NomenclaturesComponent.getFlexRate(column.propertyName)
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
            codeControl: new FormControl(),
            nameControl: new FormControl(),
            validFromControl: new FormControl(),
            validToControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): NomenclaturesFilters {
        const filtersObj = new NomenclaturesFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            tableId: this.selectedTable.value,
            code: filters.getValue('codeControl'),
            name: filters.getValue('nameControl'),
            validityDateFrom: filters.getValue('validFromControl'),
            validityDateTo: filters.getValue('validToControl')
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

    private treeFlattenerTransformer(node: TreeNode, level: number): FlatNode {
        return new FlatNode({
            expandable: !!node.children && node.children.length > 0,
            name: node.name,
            level: level,
            id: node.id,
        });
    }

    private setTreeDataSource(filter?: string): void {
        const groups: NomenclatureDTO<number>[] = this.nomenclatureGroups.filter((group: NomenclatureDTO<number>) => {
            if (this.nomenclatureTables.some(y => y.groupId === group.value)) {
                if (filter && filter.length > 0) {
                    const tables: NomenclatureTableDTO[] = this.nomenclatureTables.filter(x => x.groupId === group.value);
                    if (tables.some(x => x.displayName?.toLowerCase().includes(filter.toLowerCase()))) {
                        return true;
                    }
                    return false;
                }

                return true;
            }
            return false;
        });

        this.treeDataSource.data = groups.map((x: NomenclatureDTO<number>) => {
            const tables: NomenclatureTableDTO[] = this.nomenclatureTables.filter((table: NomenclatureTableDTO) => {
                if (table.groupId === x.value) {
                    if (filter && filter.length > 0) {
                        if (table.displayName?.toLowerCase().includes(filter.toLowerCase())) {
                            return true;
                        }
                        return false;
                    }
                    return true;
                }
                return false;
            });

            const children: TreeNode[] = tables.map((y: NomenclatureTableDTO) => {
                return {
                    id: y.value,
                    name: y.displayName!,
                    children: []
                };
            });

            const node: TreeNode = {
                name: this.translate.getValue('nomenclatures-page.' + x.displayName).replace('nomenclatures-page.', ''),
                children: children
            };

            return node;
        });

        if (filter && filter.length >= 3) {
            this.treeControl.expandAll();
        }
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

    private calculateContainerHeightPx(): void {
        if (!this.toolbarElement) {
            this.toolbarElement = document.getElementsByTagName('toolbar').item(0) as HTMLElement;
        }

        this.containerHeightPx = window.innerHeight - this.toolbarElement.offsetHeight;
        this.mainPanelHeightPx = this.containerHeightPx;
    }

    private calculateMainPanelWidthPx(): void {
        if (!this.containerElement) {
            this.containerElement = this.host.getElementsByClassName('container').item(0) as HTMLElement;
        }

        if (!this.treePanelElement) {
            this.treePanelElement = this.host.getElementsByClassName('tree-panel').item(0) as HTMLElement;
        }

        this.mainPanelWidthPx = this.containerElement.offsetWidth - this.treePanelElement.offsetWidth;
    }
}
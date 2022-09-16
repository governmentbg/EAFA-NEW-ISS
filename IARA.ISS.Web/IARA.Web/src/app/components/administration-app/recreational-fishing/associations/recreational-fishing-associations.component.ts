import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';

import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RecreationalFishingAssociationDTO } from '@app/models/generated/dtos/RecreationalFishingAssociationDTO';
import { RecreationalFishingAssociationEditDTO } from '@app/models/generated/dtos/RecreationalFishingAssociationEditDTO';
import { RecreationalFishingPossibleAssociationLegalDTO } from '@app/models/generated/dtos/RecreationalFishingPossibleAssociationLegalDTO';
import { RecreationalFishingAssociationsFilters } from '@app/models/generated/filters/RecreationalFishingAssociationsFilters';
import { RecreationalFishingAssociationService } from '@app/services/administration-app/recreational-fishing-association.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { RecreationalFishingAddAssociationComponent } from './add-association/recreational-fishing-add-association.component';
import { RecreationalFishingEditAssociationComponent } from './edit-association/recreational-fishing-edit-association.component';
import { AssociationEditDialogParams } from './models/association-edit-dialog-params.model';

type AssociationStatus = 'active' | 'inactive';

@Component({
    selector: 'recreational-fishing-associations',
    templateUrl: './recreational-fishing-associations.component.html'
})
export class RecreationalFishingAssociationsComponent implements OnInit, AfterViewInit {
    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public territoryUnits!: NomenclatureDTO<number>[];
    public associationStatuses!: NomenclatureDTO<AssociationStatus>[];

    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;
    public readonly canRestoreRecords: boolean;

    public readonly hasReadAllPermission: boolean;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private grid!: DataTableManager<RecreationalFishingAssociationDTO, RecreationalFishingAssociationsFilters>;

    private service: RecreationalFishingAssociationService;
    private nomenclatures: CommonNomenclatures;
    private confirmDialog: TLConfirmDialog;
    private addDialog: TLMatDialog<RecreationalFishingAddAssociationComponent>;
    private editDialog: TLMatDialog<RecreationalFishingEditAssociationComponent>;

    public constructor(
        service: RecreationalFishingAssociationService,
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        permissions: PermissionsService,
        confirmDialog: TLConfirmDialog,
        addDialog: TLMatDialog<RecreationalFishingAddAssociationComponent>,
        editDialog: TLMatDialog<RecreationalFishingEditAssociationComponent>
    ) {
        this.service = service;
        this.nomenclatures = nomenclatures;
        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.addDialog = addDialog;
        this.editDialog = editDialog;

        this.canAddRecords = permissions.has(PermissionsEnum.AssociationsAddRecords);
        this.canEditRecords = permissions.has(PermissionsEnum.AssociationsEditRecords);
        this.canRestoreRecords = permissions.has(PermissionsEnum.AssociationsRestoreRecords);

        this.hasReadAllPermission = permissions.has(PermissionsEnum.AssociationsReadAll);

        this.buildForm();
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.territoryUnits = result;
            }
        });

        this.associationStatuses = [
            new NomenclatureDTO<AssociationStatus>({
                value: 'active',
                displayName: this.translate.getValue('recreational-fishing.association-active'),
                isActive: true
            }),
            new NomenclatureDTO<AssociationStatus>({
                value: 'inactive',
                displayName: this.translate.getValue('recreational-fishing.association-canceled'),
                isActive: true
            })
        ];
    }

    public ngAfterViewInit(): void {
        this.grid = new DataTableManager<RecreationalFishingAssociationDTO, RecreationalFishingAssociationsFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllAssociations.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        this.grid.refreshData();
    }

    public addAssociation(): void {
        const dialog = this.addDialog.open({
            title: this.translate.getValue('recreational-fishing.choose-legal-for-association'),
            TCtor: RecreationalFishingAddAssociationComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeAddDialogBtnClicked.bind(this)
            },
            translteService: this.translate,
            disableDialogClose: true,
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translate.getValue('common.save')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translate.getValue('common.cancel')
            }
        }, '1200px');

        dialog.subscribe((selection?: RecreationalFishingPossibleAssociationLegalDTO) => {
            if (selection !== undefined) {
                const data = new AssociationEditDialogParams({
                    id: selection.id,
                    adding: true
                });

                const title: string = this.translate.getValue('recreational-fishing.add-association-dialog-title');

                this.openAddOrEditDialog(title, data);
            }
        });
    }

    public editAssociation(association: RecreationalFishingAssociationDTO, viewMode: boolean): void {
        if (association.id !== undefined) {
            const data = new AssociationEditDialogParams({
                id: association.id,
                readonly: viewMode || association.isCanceled
            });

            const auditButton: IHeaderAuditButton = {
                id: association.id,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'FishingAssociation'
            };

            const title: string = viewMode
                ? this.translate.getValue('recreational-fishing.view-association-dialog-title')
                : this.translate.getValue('recreational-fishing.edit-association-dialog-title');

            const rightButtons: IActionInfo[] = [];

            if (!viewMode) {
                if (association.isCanceled) {
                    rightButtons.push({
                        id: 'activate',
                        color: 'accent',
                        translateValue: this.translate.getValue('recreational-fishing.activate-association')
                    });
                }
                else {
                    rightButtons.push({
                        id: 'annul',
                        color: 'warn',
                        translateValue: this.translate.getValue('recreational-fishing.annul-association')
                    });
                }
            }

            this.openAddOrEditDialog(title, data, auditButton, rightButtons, viewMode);
        }
    }

    public deleteAssociation(association: RecreationalFishingAssociationDTO): void {
        this.confirmDialog.open({
            title: this.translate.getValue('recreational-fishing.delete-association-dialog-title'),
            message: this.translate.getValue('recreational-fishing.delete-association-dialog-msg'),
            okBtnLabel: this.translate.getValue('recreational-fishing.delete')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok && association?.id) {
                    this.service.deleteAssociation(association.id).subscribe({
                        next: () => {
                            this.grid.refreshData();
                            NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.FishingAssociations);
                        }
                    });
                }
            }
        });
    }

    public restoreAssociation(association: RecreationalFishingAssociationDTO): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok && association?.id) {
                    this.service.undoDeleteAssociation(association.id).subscribe({
                        next: () => {
                            this.grid.refreshData();
                            NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.FishingAssociations);
                        }
                    });
                }
            }
        });
    }

    private buildForm(): void {
        this.form = new FormGroup({
            nameControl: new FormControl(),
            eikControl: new FormControl(),
            territoryUnitControl: new FormControl(),
            representativeControl: new FormControl(),
            statusesControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): RecreationalFishingAssociationsFilters {
        const result = new RecreationalFishingAssociationsFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            name: filters.getValue('nameControl'),
            eik: filters.getValue('eikControl'),
            territoryUnitId: this.hasReadAllPermission ? filters.getValue('territoryUnitControl') : undefined,
            representativePersonId: filters.getValue('representativeControl')
        });

        const statuses: AssociationStatus[] | undefined = filters.getValue('statusesControl');
        if (statuses !== undefined && statuses !== null) {
            if (statuses.length === 1) {
                result.showCanceled = statuses[0] === 'inactive';
            }
        }
        return result;
    }

    private openAddOrEditDialog(
        title: string,
        data: AssociationEditDialogParams,
        auditButton?: IHeaderAuditButton,
        rightButtons?: IActionInfo[],
        viewMode: boolean = false
    ): void {
        const dialog = this.editDialog.open({
            title: title,
            TCtor: RecreationalFishingEditAssociationComponent,
            headerAuditButton: auditButton,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: true,
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translate.getValue('common.save')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translate.getValue('common.cancel')
            },
            rightSideActionsCollection: rightButtons,
            viewMode
        }, '1200px');

        dialog.subscribe((entry?: RecreationalFishingAssociationEditDTO) => {
            if (entry !== undefined) {
                this.grid.refreshData();
                NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.FishingAssociations);
            }
        });
    }

    private closeAddDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}
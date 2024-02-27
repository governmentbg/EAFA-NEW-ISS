import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { CrossCheckDTO } from '@app/models/generated/dtos/CrossCheckDTO';
import { CrossChecksFilters } from '@app/models/generated/filters/CrossChecksFilters';
import { CrossChecksService } from '@app/services/administration-app/cross-checks.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { EditCrossCheckComponent } from './edit-cross-check/edit-cross-check.component';
import { CrossCheckEditDTO } from '@app/models/generated/dtos/CrossCheckEditDTO';
import { CrossChecksAutoExecFrequencyEnum } from '@app/enums/cross-checks-auto-exec-frequency.enum';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';

@Component({
    selector: 'cross-checks',
    templateUrl: './cross-checks.component.html'
})
export class CrossChecksComponent implements OnInit, AfterViewInit {
    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;
    public readonly canDeleteRecords: boolean;
    public readonly canRestoreRecords: boolean;
    public readonly canExecuteRecords: boolean;

    public levels: NomenclatureDTO<number>[] = [];
    public groups: NomenclatureDTO<number>[] = [];

    public execFrequencies: typeof CrossChecksAutoExecFrequencyEnum = CrossChecksAutoExecFrequencyEnum;
    public autoExecFrequencyCodes: NomenclatureDTO<CrossChecksAutoExecFrequencyEnum>[] = [];

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private grid!: DataTableManager<CrossCheckDTO, CrossChecksFilters>;

    private service: CrossChecksService;
    private confirmDialog: TLConfirmDialog;
    private editDialog: TLMatDialog<EditCrossCheckComponent>;

    public constructor(
        translate: FuseTranslationLoaderService,
        service: CrossChecksService,
        confirmDialog: TLConfirmDialog,
        editDialog: TLMatDialog<EditCrossCheckComponent>,
        permissions: PermissionsService
    ) {
        this.translate = translate;
        this.service = service;
        this.confirmDialog = confirmDialog;
        this.editDialog = editDialog;

        this.canAddRecords = permissions.has(PermissionsEnum.CrossChecksAddRecords);
        this.canEditRecords = permissions.has(PermissionsEnum.CrossChecksEditRecords);
        this.canDeleteRecords = permissions.has(PermissionsEnum.CrossChecksDeleteRecords);
        this.canRestoreRecords = permissions.has(PermissionsEnum.CrossChecksRestoreRecords);
        this.canExecuteRecords = permissions.has(PermissionsEnum.CrossChecksExecuteRecords);

        for (let i = 1; i < 6; ++i) {
            this.levels.push(new NomenclatureDTO<number>({
                value: i,
                displayName: `${i}`,
                isActive: true
            }));
        }

        this.autoExecFrequencyCodes = [
            new NomenclatureDTO<CrossChecksAutoExecFrequencyEnum>({
                value: CrossChecksAutoExecFrequencyEnum.Manual,
                displayName: translate.getValue('cross-check.manual'),
                isActive: true
            }),
            new NomenclatureDTO<CrossChecksAutoExecFrequencyEnum>({
                value: CrossChecksAutoExecFrequencyEnum.Daily,
                displayName: translate.getValue('cross-check.daily'),
                isActive: true
            }),
            new NomenclatureDTO<CrossChecksAutoExecFrequencyEnum>({
                value: CrossChecksAutoExecFrequencyEnum.Weekly,
                displayName: translate.getValue('cross-check.weekly'),
                isActive: true
            }),
            new NomenclatureDTO<CrossChecksAutoExecFrequencyEnum>({
                value: CrossChecksAutoExecFrequencyEnum.Monthly,
                displayName: translate.getValue('cross-check.monthly'),
                isActive: true
            }),
            new NomenclatureDTO<CrossChecksAutoExecFrequencyEnum>({
                value: CrossChecksAutoExecFrequencyEnum.Repeating,
                displayName: translate.getValue('cross-check.repeating'),
                isActive: true
            })
        ];

        this.buildForm();
    }

    public ngOnInit(): void {
        this.service.getAllReportGroups().subscribe({
            next: (groups: NomenclatureDTO<number>[]) => {
                this.groups = groups;
            }
        });
    }

    public ngAfterViewInit(): void {
        this.grid = new DataTableManager<CrossCheckDTO, CrossChecksFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllCrossChecks.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        this.grid.refreshData();
    }

    public createEditCrossCheck(crossCheck: CrossCheckDTO | undefined, viewMode: boolean): void {
        let data: DialogParamsModel | undefined = undefined;
        let auditButton: IHeaderAuditButton | undefined = undefined;
        let title: string;
        const rightButtons: IActionInfo[] = [];

        if (crossCheck?.id !== undefined) {
            data = new DialogParamsModel({ id: crossCheck.id, isReadonly: viewMode });
            auditButton = {
                id: crossCheck.id,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'CrossCheck'
            };

            title = viewMode
                ? this.translate.getValue('cross-check.view-cross-check-dialog-title')
                : this.translate.getValue('cross-check.edit-cross-check-dialog-title');

            if (this.canExecuteRecords) {
                rightButtons.push({
                    id: 'execute-cross-check',
                    color: 'accent',
                    translateValue: this.translate.getValue('cross-check.execute-cross-check-btn')
                });
            }
        }
        else {
            title = this.translate.getValue('cross-check.add-cross-check-dialog-title');
        }

        const dialog = this.editDialog.openWithTwoButtons({
            title: title,
            TCtor: EditCrossCheckComponent,
            headerAuditButton: auditButton,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: true,
            viewMode: viewMode,
            rightSideActionsCollection: rightButtons
        }, '1400px');

        dialog.subscribe((entry?: CrossCheckEditDTO) => {
            if (entry !== undefined) {
                this.grid.refreshData();
            }
        });
    }

    public deleteCrossCheck(crossCheck: CrossCheckDTO): void {
        this.confirmDialog.open({
            title: this.translate.getValue('cross-check.delete-cross-check-dialog-title'),
            message: this.translate.getValue('cross-check.confirm-delete-message'),
            okBtnLabel: this.translate.getValue('cross-check.delete')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok && crossCheck?.id) {
                    this.service.deleteCrossCheck(crossCheck.id).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
            }
        });
    }

    public executeCrossChecks(execFrequency: string): void {
        this.service.executeCrossChecks(execFrequency).subscribe();
    }

    public restoreCrossCheck(crossCheck: CrossCheckDTO): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok && crossCheck?.id) {
                    this.service.undoDeleteCrossCheck(crossCheck.id).subscribe({
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
            nameControl: new FormControl(),
            checkedTableControl: new FormControl(),
            executionDataSourceControl: new FormControl(),
            reportGroupControl: new FormControl(),
            levelControl: new FormControl(),
            autoExecFrequencyCodesControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): CrossChecksFilters {
        const result = new CrossChecksFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            name: filters.getValue('nameControl'),
            checkedTable: filters.getValue('checkedTableControl'),
            dataSource: filters.getValue('executionDataSourceControl'),
            groupIds: filters.getValue('reportGroupControl'),
            errorLevels: filters.getValue('levelControl')
        });

        const execCodes: CrossChecksAutoExecFrequencyEnum[] | undefined = filters.getValue('autoExecFrequencyCodesControl');
        if (execCodes !== undefined && execCodes !== null) {
            result.autoExecFrequencyCodes = execCodes.map((code: CrossChecksAutoExecFrequencyEnum) => {
                return CrossChecksAutoExecFrequencyEnum[code];
            });
        }
        return result;
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}

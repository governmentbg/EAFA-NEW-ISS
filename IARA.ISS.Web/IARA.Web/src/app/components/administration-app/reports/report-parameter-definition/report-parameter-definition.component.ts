import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { BaseDialogParamsModel } from '@app/models/common/base-dialog-params.model';
import { NReportParameterDTO } from '@app/models/generated/dtos/NReportParameterDTO';
import { NReportParameterEditDTO } from '@app/models/generated/dtos/NReportParameterEditDTO';
import { ReportParameterDefinitionFilters } from '@app/models/generated/filters/ReportParameterDefinitionFilters';
import { ReportAdministrationService } from '@app/services/administration-app/report-administration.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { IRemoteTLDatatableComponent } from '@app/shared/components/data-table/interfaces/tl-remote-datatable.interface';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { DialogCloseCallback } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { EditReportParameterDefinitionComponent } from './edit-report-parameter-definition.component';
import { IReportService } from '@app/interfaces/administration-app/report.interface';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';

@Component({
    selector: 'report-parameter-definition',
    templateUrl: './report-parameter-definition.component.html'
})
export class ReportParameterDefinitionComponent implements AfterViewInit {
    public filtersFormGroup: FormGroup;

    public canAddNParameters: boolean;
    public canEditNParameters: boolean;
    public canDeleteNParameters: boolean;
    public canRestoreNParameters: boolean;

    private readonly reportService: IReportService;
    private readonly translateService: FuseTranslationLoaderService;
    private readonly editDialog: TLMatDialog<EditReportParameterDefinitionComponent>;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly permissionsService: PermissionsService;

    @ViewChild(TLDataTableComponent)
    private dataTable!: IRemoteTLDatatableComponent;
    @ViewChild(SearchPanelComponent)
    private searchPanel!: SearchPanelComponent;

    private gridManager!: DataTableManager<NReportParameterDTO, ReportParameterDefinitionFilters>;

    public constructor(reportService: ReportAdministrationService,
        translateService: FuseTranslationLoaderService,
        editDialog: TLMatDialog<EditReportParameterDefinitionComponent>,
        confirmDialog: TLConfirmDialog,
        permissionsService: PermissionsService) {
        this.permissionsService = permissionsService;
        this.reportService = reportService;
        this.translateService = translateService;
        this.editDialog = editDialog;
        this.confirmDialog = confirmDialog;

        this.canAddNParameters = this.permissionsService.has(PermissionsEnum.ReportParameterAddRecords);
        this.canEditNParameters = this.permissionsService.has(PermissionsEnum.ReportParameterEditRecords);
        this.canDeleteNParameters = this.permissionsService.has(PermissionsEnum.ReportParameterDeleteRecords);
        this.canRestoreNParameters = this.permissionsService.has(PermissionsEnum.ReportParameterRestoreRecords);

        this.filtersFormGroup = new FormGroup({
            dateRangeControl: new FormControl()
        });
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<NReportParameterDTO, ReportParameterDefinitionFilters>({
            tlDataTable: this.dataTable,
            searchPanel: this.searchPanel,
            requestServiceMethod: this.reportService.getAllNParameters.bind(this.reportService),
            filtersMapper: this.mapFilters
        });

        this.gridManager.refreshData();
    }

    public closeDialogBtnClicked(closeFn: DialogCloseCallback): void {
        closeFn();
    }

    public addOrEditNParameter(nParameter?: NReportParameterEditDTO, viewMode: boolean = false): void {
        let title: string = this.translateService.getValue('report-parameter-definition.dialog-parameter-edit-title');
        let auditButtons: IHeaderAuditButton | undefined;
        let data: BaseDialogParamsModel | undefined;
        const nParameterId: number | undefined = nParameter?.id;

        if (nParameter !== undefined) {
            if (viewMode) title = this.translateService.getValue('report-parameter-definition.dialog-parameter-view-title');

            if (nParameterId !== undefined) {
                data = new BaseDialogParamsModel({
                    viewMode: viewMode,
                    id: nParameterId
                });

                auditButtons = {
                    getAuditRecordData: this.reportService.getParametersAudit.bind(this.reportService),
                    id: nParameterId,
                    tableName: 'NreportParameter',
                } as IHeaderAuditButton;
            }
        }
        else {
            title = this.translateService.getValue('report-parameter-definition.dialog-parameter-add-title');
        }

        const dialog = this.editDialog.openWithTwoButtons({
            TCtor: EditReportParameterDefinitionComponent,
            title: title,
            componentData: data,
            headerAuditButton: auditButtons,
            translteService: this.translateService,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            viewMode: viewMode
        }, '1400px');

        dialog.subscribe({
            next: (inputNParameter: NReportParameterEditDTO | undefined) => {
                if (inputNParameter !== undefined) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    public deleteNParameter(nParameter: NReportParameterDTO): void {
        this.confirmDialog.open({
            title: this.translateService.getValue('report-parameter-definition.dialog-parameter-delete-title'),
            message: this.translateService.getValue('report-parameter-definition.dialog-parameter-delete-message'),
            okBtnLabel: this.translateService.getValue('report-parameter-definition.dialog-parameter-delete-button')
        }).subscribe({
            next: (result: boolean) => {
                if (result) {
                    this.reportService.deleteNParameter(nParameter.id!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        }
                    });
                }
            }
        });
    }

    public undoDeletedNParameter(nParameter: NReportParameterDTO): void {
        this.confirmDialog.open({
            title: this.translateService.getValue('report-parameter-definition.dialog-parameter-restore-title'),
            message: this.translateService.getValue('report-parameter-definition.dialog-parameter-restore-message'),
            okBtnLabel: this.translateService.getValue('report-parameter-definition.dialog-parameter-restore-button')
        }).subscribe({
            next: (result: boolean) => {
                if (result) {
                    this.reportService.undoDeletedNParameter(nParameter.id!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        }
                    });
                }
            }
        });
    }

    private mapFilters(filters: FilterEventArgs): ReportParameterDefinitionFilters {
        return new ReportParameterDefinitionFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,
            validFrom: filters.getValue<DateRangeData>('dateRangeControl')?.start,
            validTo: filters.getValue<DateRangeData>('dateRangeControl')?.end
        });
    }
}
import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { CrossCheckResultDTO } from '@app/models/generated/dtos/CrossCheckResultDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CrossCheckResultsFilters } from '@app/models/generated/filters/CrossCheckResultsFilters';
import { CrossChecksService } from '@app/services/administration-app/cross-checks.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { DialogAssignedUserParamsModel } from '@app/models/common/dialog-assigned-user-params.model';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CrossCheckResultsAssignedUserComponent } from '../cross-check-results-assigned-user/cross-check-results-assigned-user.component';
import { CrossCheckResultsResolutionComponent } from '../cross-check-results-resolution/cross-check-results-resolution.component';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';

@Component({
    selector: 'cross-checks-results',
    templateUrl: './cross-checks-results.component.html'
})
export class CrossChecksResultsComponent implements AfterViewInit {
    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public readonly canReadRecords: boolean;
    public readonly canDeleteRecords: boolean;
    public readonly canRestoreRecords: boolean;
    public readonly canAssignRecords: boolean;
    public readonly canResolveRecords: boolean;

    public resolutions: NomenclatureDTO<number>[];
    public assignedUsers: NomenclatureDTO<number>[];

    private readonly commonNomenclaturesService: CommonNomenclatures;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private grid!: DataTableManager<CrossCheckResultDTO, CrossCheckResultsFilters>;

    private service: CrossChecksService;
    private confirmDialog: TLConfirmDialog;
    private assingedUserDialog: TLMatDialog<CrossCheckResultsAssignedUserComponent>;
    private resolutionDialog: TLMatDialog<CrossCheckResultsResolutionComponent>;

    public constructor(
        commonNomenclaturesService: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        service: CrossChecksService,
        confirmDialog: TLConfirmDialog,
        assingedUserDialog: TLMatDialog<CrossCheckResultsAssignedUserComponent>,
        resolutionDialog: TLMatDialog<CrossCheckResultsResolutionComponent>,
        permissions: PermissionsService
    ) {
        this.commonNomenclaturesService = commonNomenclaturesService;
        this.translate = translate;
        this.service = service;
        this.confirmDialog = confirmDialog;
        this.assingedUserDialog = assingedUserDialog;
        this.resolutionDialog = resolutionDialog;

        this.canReadRecords = permissions.has(PermissionsEnum.CrossCheckResultsRead);
        this.canDeleteRecords = permissions.has(PermissionsEnum.CrossCheckResultsDeleteRecords);
        this.canRestoreRecords = permissions.has(PermissionsEnum.CrossCheckResultsRestoreRecords);
        this.canAssignRecords = permissions.has(PermissionsEnum.CrossCheckResultsAssign);
        this.canResolveRecords = permissions.has(PermissionsEnum.CrossCheckResultsResolve);

        this.resolutions = [];
        this.assignedUsers = [];

        this.buildForm();
    }

    public async ngAfterViewInit(): Promise<void> {
        const nomenclatures: NomenclatureDTO<number>[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.CheckResolutionTypes, this.service.getCheckResolutionTypes.bind(this.service)
            ),
            this.commonNomenclaturesService.getUserNames()
        ).toPromise();

        this.resolutions = nomenclatures[0];
        this.assignedUsers = nomenclatures[1];

        this.grid = new DataTableManager<CrossCheckResultDTO, CrossCheckResultsFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllCrossCheckResults.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        this.grid.refreshData();
    }

    public editCrossCheckResolution(crossCheckResult: CrossCheckResultDTO | undefined): void {
        let data: DialogParamsModel | undefined = undefined;
        let auditButton: IHeaderAuditButton | undefined = undefined;
        if (crossCheckResult?.id !== undefined) {
            data = new DialogParamsModel({ id: crossCheckResult.id, isReadonly: false });
            auditButton = {
                id: crossCheckResult.id,
                getAuditRecordData: this.service.getCrossCheckResultSimpleAudit.bind(this.service),
                tableName: 'CrossCheckResult'
            };
        }

        const dialog = this.resolutionDialog.openWithTwoButtons({
            title: this.translate.getValue('cross-check-results.edit-cross-check-resolution'),
            TCtor: CrossCheckResultsResolutionComponent,
            headerAuditButton: auditButton,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: true,
            viewMode: false
        }, '900px');

        dialog.subscribe((entry?: CrossCheckResultDTO) => {
            if (entry !== undefined) {
                this.grid.refreshData();
            }
        });
    }

    public assignCrossCheckResult(crossCheckResult: CrossCheckResultDTO | undefined): void {
        let data: DialogAssignedUserParamsModel | undefined = undefined;

        if (crossCheckResult?.id !== undefined) {
            data = new DialogAssignedUserParamsModel(
                {
                    id: crossCheckResult.id,
                    userId: crossCheckResult.assignedUserId,
                    isReadonly: false
                });
        }

        const dialog = this.assingedUserDialog.openWithTwoButtons({
            title: this.translate.getValue('cross-check-results.assigned-user-cross-check-result'),
            TCtor: CrossCheckResultsAssignedUserComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: true,
            viewMode: false
        }, '900px');

        dialog.subscribe((entry?: CrossCheckResultDTO) => {
            if (entry !== undefined) {
                this.grid.refreshData();
            }
        });
    }

    public deleteCrossCheckResult(crossCheckResult: CrossCheckResultDTO): void {
        this.confirmDialog.open({
            title: this.translate.getValue('cross-check-results.delete-cross-check-result-dialog-title'),
            message: this.translate.getValue('cross-check-results.confirm-delete-message'),
            okBtnLabel: this.translate.getValue('cross-check-results.delete')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok && crossCheckResult?.id) {
                    this.service.deleteCrossCheckResult(crossCheckResult.id).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
            }
        });
    }

    public restoreCrossCheckResult(crossCheckresult: CrossCheckResultDTO): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok && crossCheckresult?.id) {
                    this.service.undoDeleteCrossCheckResult(crossCheckresult.id).subscribe({
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
            checkCodeControl: new FormControl(),
            checkNameControl: new FormControl(),
            checkTableNameControl: new FormControl(),
            resolutionControl: new FormControl(),
            validityControl: new FormControl(),
            assignedUserControl: new FormControl(),
            tableIdControl: new FormControl(),
            errorDescriptionControl: new FormControl(),
        });
    }

    private mapFilters(filters: FilterEventArgs): CrossCheckResultsFilters {
        const result = new CrossCheckResultsFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,
            checkCode: filters.getValue('checkCodeControl'),
            checkName: filters.getValue('checkNameControl'),
            checkTableName: filters.getValue('checkTableNameControl'),
            resolutionIds: filters.getValue('resolutionControl'),
            validFrom: filters.getValue<DateRangeData>('validityControl')?.start,
            validTo: filters.getValue<DateRangeData>('validityControl')?.end,
            assignedUserId: filters.getValue('assignedUserControl'),
            tableId: filters.getValue('tableIdControl'),
            errorDescription: filters.getValue('errorDescriptionControl')
        });

        return result;
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}
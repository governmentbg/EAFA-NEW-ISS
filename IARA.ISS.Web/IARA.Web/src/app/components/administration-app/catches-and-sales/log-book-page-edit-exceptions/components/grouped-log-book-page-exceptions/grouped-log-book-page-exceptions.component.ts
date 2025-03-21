import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { LogBookPageEditExceptionFilters } from '@app/models/generated/filters/LogBookPageEditExceptionFilters';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { ILogBookPageEditExceptionsService } from '@app/interfaces/administration-app/log-book-page-edit-exceptions.interface';
import { LogBookPageEditExceptionsService } from '@app/services/administration-app/log-book-page-edit-exceptions.service';
import { IGroupedOptions } from '@app/shared/components/input-controls/tl-autocomplete/interfaces/grouped-options.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { LogBookPageEditNomenclatureDTO } from '@app/models/generated/dtos/LogBookPageEditNomenclatureDTO';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IRemoteTLDatatableComponent } from '@app/shared/components/data-table/interfaces/tl-remote-datatable.interface';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { LogBookPageExceptionGroupedDTO } from '@app/models/generated/dtos/LogBookPageExceptionGroupedDTO';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { EditLogBookPageExceptionsGroupedComponent } from '../edit-log-book-page-exceptions-grouped/edit-log-book-page-exceptions-grouped.component';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { EditLogBookPageExceptionsGroupedParameters } from '../../models/edit-log-book-page-exceptions-grouped.model';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { SystemUserNomenclatureDTO } from '@app/models/generated/dtos/SystemUserNomenclatureDTO';

@Component({
    selector: 'grouped-log-book-page-exceptions',
    templateUrl: './grouped-log-book-page-exceptions.component.html'
})
export class GroupedLogBookPageExceptionsComponent implements OnInit, AfterViewInit {
    public translate: FuseTranslationLoaderService;
    public form: FormGroup;

    public users: IGroupedOptions<number>[] = [];
    public logBookTypes: NomenclatureDTO<number>[] = [];
    public logBooks: LogBookPageEditNomenclatureDTO[] = [];

    public readonly canAddExceptionRecords: boolean;
    public readonly canEditExceptionRecords: boolean;
    public readonly canDeleteExceptionRecords: boolean;
    public readonly canRestoreExceptionRecords: boolean;

    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    @ViewChild(TLDataTableComponent)
    private datatable!: IRemoteTLDatatableComponent;

    private gridManager!: DataTableManager<LogBookPageExceptionGroupedDTO, LogBookPageEditExceptionFilters>;

    private readonly service: ILogBookPageEditExceptionsService;
    private readonly nomenclatures: CommonNomenclatures;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly editDialog: TLMatDialog<EditLogBookPageExceptionsGroupedComponent>;

    public constructor(
        translate: FuseTranslationLoaderService,
        service: LogBookPageEditExceptionsService,
        nomenclatures: CommonNomenclatures,
        permissions: PermissionsService,
        confirmDialog: TLConfirmDialog,
        editDialog: TLMatDialog<EditLogBookPageExceptionsGroupedComponent>
    ) {
        this.translate = translate;
        this.service = service;
        this.nomenclatures = nomenclatures;
        this.confirmDialog = confirmDialog;
        this.editDialog = editDialog;

        this.canAddExceptionRecords = permissions.has(PermissionsEnum.LogBookPageEditExceptionsAddRecords);
        this.canEditExceptionRecords = permissions.has(PermissionsEnum.LogBookPageEditExceptionsEditRecords);
        this.canDeleteExceptionRecords = permissions.has(PermissionsEnum.LogBookPageEditExceptionsDeleteRecords);
        this.canRestoreExceptionRecords = permissions.has(PermissionsEnum.LogBookPageEditExceptionsRestoreRecords);

        this.form = this.buildForm();
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.LogBookTypes,
            this.nomenclatures.getLogBookTypes.bind(this.nomenclatures),
            false
        ).subscribe({
            next: (items: NomenclatureDTO<number>[]) => {
                this.logBookTypes = items;
            }
        });

        this.service.getAllUsersNomenclature().subscribe({
            next: (items: SystemUserNomenclatureDTO[]) => {
                this.users = [
                    {
                        name: this.translate.getValue('log-book-page-edit-exceptions.internal-users-group'),
                        options: items.filter(x => x.isInternalUser === true)
                    },
                    {
                        name: this.translate.getValue('log-book-page-edit-exceptions.external-users-group'),
                        options: items.filter(x => x.isInternalUser === false)
                    }
                ];
            }
        });

        this.service.getActiveLogBooksNomenclature().subscribe({
            next: (items: LogBookPageEditNomenclatureDTO[]) => {
                this.logBooks = items;
            }
        });
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<LogBookPageExceptionGroupedDTO, LogBookPageEditExceptionFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllGroupedLogBookPageExceptions.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        this.gridManager.refreshData();
    }

    public addExceptionRecords(): void {
        const title: string = this.translate.getValue('log-book-page-edit-exceptions.add-grouped-exceptions-dialog-title');
        const dialogData: EditLogBookPageExceptionsGroupedParameters = new EditLogBookPageExceptionsGroupedParameters({
            viewMode: false
        });

        this.openEditDialog(title, dialogData, false);
    }

    public editExceptionRecords(model: LogBookPageExceptionGroupedDTO, viewMode: boolean = false): void {
        let title: string = '';
        const dialogData: EditLogBookPageExceptionsGroupedParameters = new EditLogBookPageExceptionsGroupedParameters({
            model: model,
            viewMode: viewMode
        });

        if (viewMode) {
            title = this.translate.getValue('log-book-page-edit-exceptions.view-grouped-exceptions-dialog-title');
        }
        else {
            title = this.translate.getValue('log-book-page-edit-exceptions.edit-grouped-exceptions-dialog-title');
        }

        this.openEditDialog(title, dialogData, viewMode);
    }

    public copyExceptionRecords(model: LogBookPageExceptionGroupedDTO): void {
        const title: string = this.translate.getValue('log-book-page-edit-exceptions.add-grouped-exceptions-dialog-title');
        const dialogData: EditLogBookPageExceptionsGroupedParameters = new EditLogBookPageExceptionsGroupedParameters({
            model: model,
            viewMode: false,
            isCopy: true
        });

        this.openEditDialog(title, dialogData, false);
    }

    public deleteExceptionRecords(model: LogBookPageExceptionGroupedDTO): void {
        this.confirmDialog.open({
            title: this.translate.getValue('log-book-page-edit-exceptions.delete-exceptions-grouped-dialog-title'),
            message: this.translate.getValue('log-book-page-edit-exceptions.delete-exceptions-grouped-message'),
            okBtnLabel: this.translate.getValue('log-book-page-edit-exceptions.delete-exceptions-grouped-ok-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    const ids: number[] = model.logBookPageExceptionIds ?? [];

                    if (ids !== undefined && ids !== null && ids.length > 0) {
                        this.service.removeLogBookPageExceptionsGrouped(ids).subscribe({
                            next: () => {
                                this.gridManager.refreshData();
                            }
                        });
                    }
                }
            }
        });
    }

    public restoreExceptionRecords(model: LogBookPageExceptionGroupedDTO): void {
        this.confirmDialog.open({
            title: this.translate.getValue('log-book-page-edit-exceptions.restore-exceptions-grouped-dialog-title'),
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    const ids: number[] = model.logBookPageExceptionIds ?? [];

                    if (ids !== undefined && ids !== null && ids.length > 0) {
                        this.service.restoreLogBookPageExceptionsGrouped(ids).subscribe({
                            next: () => {
                                this.gridManager.refreshData();
                            }
                        });
                    }
                }
            }
        });
    }

    private buildForm(): FormGroup {
        return new FormGroup({
            userControl: new FormControl(),
            logBookTypesControl: new FormControl(),
            logBookControl: new FormControl(),
            exceptionActiveDateFromControl: new FormControl(),
            exceptionActiveDateToControl: new FormControl(),
            editPageDateFromControl: new FormControl(),
            editPageDateToControl: new FormControl(),
        });
    }

    private openEditDialog(title: string, dialogData: EditLogBookPageExceptionsGroupedParameters, viewMode: boolean): void {
        this.editDialog.openWithTwoButtons({
            TCtor: EditLogBookPageExceptionsGroupedComponent,
            translteService: this.translate,
            title: title,
            componentData: dialogData,
            headerAuditButton: undefined,
            viewMode: viewMode,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => { closeFn(); }
            },
        }, '1200px').subscribe({
            next: (result: LogBookPageExceptionGroupedDTO | undefined) => {
                if (result !== null && result !== undefined) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    private mapFilters(filters: FilterEventArgs): LogBookPageEditExceptionFilters {
        const result: LogBookPageEditExceptionFilters = new LogBookPageEditExceptionFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            userId: filters.getValue('userControl'),
            logBookTypeIds: filters.getValue('logBookTypesControl'),
            logBookId: filters.getValue('logBookControl'),
            exceptionActiveDateFrom: filters.getValue<Date>('exceptionActiveDateFromControl'),
            exceptionActiveDateTo: filters.getValue<Date>('exceptionActiveDateToControl'),
            editPageDateFrom: filters.getValue<Date>('editPageDateFromControl'),
            editPageDateTo: filters.getValue<Date>('editPageDateToControl')
        });

        return result;
    }
}
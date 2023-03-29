import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { IGroupedOptions } from '@app/shared/components/input-controls/tl-autocomplete/interfaces/grouped-options.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { LogBookPageEditExceptionRegisterDTO } from '@app/models/generated/dtos/LogBookPageEditExceptionRegisterDTO';
import { LogBookPageEditExceptionFilters } from '@app/models/generated/filters/LogBookPageEditExceptionFilters';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IRemoteTLDatatableComponent } from '@app/shared/components/data-table/interfaces/tl-remote-datatable.interface';
import { LogBookPageEditExceptionsService } from '@app/services/administration-app/log-book-page-edit-exceptions.service';
import { ILogBookPageEditExceptionsService } from '@app/interfaces/administration-app/log-book-page-edit-exceptions.interface';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { SystemUserNomenclatureDTO } from '@app/models/generated/dtos/SystemUserNomenclatureDTO';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { EditLogBookPageEditExceptionComponent } from './components/edit-log-book-page-edit-exception.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { LogBookPageEditNomenclatureDTO } from '@app/models/generated/dtos/LogBookPageEditNomenclatureDTO';
import { LogBookPageEditExceptionEditDTO } from '@app/models/generated/dtos/LogBookPageEditExceptionEditDTO';

@Component({
    selector: 'log-book-page-edit-exceptions',
    templateUrl: './log-book-page-edit-exceptions.component.html'
})
export class LogBookPageEditExceptionsComponent implements OnInit, AfterViewInit {
    public translationService: FuseTranslationLoaderService;

    public formGroup!: FormGroup;
    public users: IGroupedOptions<number>[] = [];
    public logBookTypes: NomenclatureDTO<number>[] = [];
    public logBooks: LogBookPageEditNomenclatureDTO[] = [];

    public readonly canAddExceptionRecords: boolean;
    public readonly canEditExceptionRecords: boolean;
    public readonly canDeleteExceptionRecords: boolean;
    public readonly canRestoreExceptionRecords: boolean;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    @ViewChild(TLDataTableComponent)
    private datatable!: IRemoteTLDatatableComponent;

    private gridManager!: DataTableManager<LogBookPageEditExceptionRegisterDTO, LogBookPageEditExceptionFilters>;

    private readonly service: ILogBookPageEditExceptionsService;
    private readonly nomenclaturesService: CommonNomenclatures;
    private readonly editDialog: TLMatDialog<EditLogBookPageEditExceptionComponent>;
    private readonly confirmDialog: TLConfirmDialog;

    public constructor(
        translationService: FuseTranslationLoaderService,
        permissions: PermissionsService,
        service: LogBookPageEditExceptionsService,
        nomenclaturesService: CommonNomenclatures,
        editDialog: TLMatDialog<EditLogBookPageEditExceptionComponent>,
        confirmDialog: TLConfirmDialog
    ) {
        this.translationService = translationService;
        this.service = service;
        this.nomenclaturesService = nomenclaturesService;
        this.editDialog = editDialog;
        this.confirmDialog = confirmDialog;

        this.canAddExceptionRecords = permissions.has(PermissionsEnum.LogBookPageEditExceptionsAddRecords);
        this.canEditExceptionRecords = permissions.has(PermissionsEnum.LogBookPageEditExceptionsEditRecords);
        this.canDeleteExceptionRecords = permissions.has(PermissionsEnum.LogBookPageEditExceptionsDeleteRecords);
        this.canRestoreExceptionRecords = permissions.has(PermissionsEnum.LogBookPageEditExceptionsRestoreRecords);

        this.buildForm();
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.LogBookTypes,
            this.nomenclaturesService.getLogBookTypes.bind(this.nomenclaturesService),
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
                        name: this.translationService.getValue('log-book-page-edit-exceptions.internal-users-group'),
                        options: items.filter(x => x.isInternalUser === true)
                    },
                    {
                        name: this.translationService.getValue('log-book-page-edit-exceptions.external-users-group'),
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
        this.gridManager = new DataTableManager<LogBookPageEditExceptionRegisterDTO, LogBookPageEditExceptionFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllLogBookPageEditExceptions.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        this.gridManager.refreshData();
    }

    public addRecord(): void {
        const title: string = this.translationService.getValue('log-book-page-edit-exceptions.add-exception-dialog-title');
        const dialogData: DialogParamsModel = new DialogParamsModel({
            viewMode: false
        });

        this.openEditDialog(title, dialogData, false, undefined);
    }

    public editRecord(model: LogBookPageEditExceptionRegisterDTO, viewMode: boolean): void {
        let title: string = '';
        const dialogData: DialogParamsModel = new DialogParamsModel({
            id: model.id,
            viewMode: viewMode
        });
        const headerButton: IHeaderAuditButton = {
            id: model.id!,
            getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
            tableName: 'LogBookPageEditException'
        };

        if (viewMode) {
            title = this.translationService.getValue('log-book-page-edit-exceptions.view-exception-dialog-title');
        }
        else {
            title = this.translationService.getValue('log-book-page-edit-exceptions.edit-exception-dialog-title');
        }

        this.openEditDialog(title, dialogData, viewMode, headerButton);
    }

    public deleteRecord(model: LogBookPageEditExceptionRegisterDTO): void {
        this.confirmDialog.open({
            title: this.translationService.getValue('log-book-page-edit-exceptions.delete-exception-dialog-title'),
            message: this.translationService.getValue('log-book-page-edit-exceptions.delete-exception-message'),
            okBtnLabel: this.translationService.getValue('log-book-page-edit-exceptions.delete-exception-ok-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.deleteLogBookPageEditException(model.id!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        }
                    });
                }
            }
        });
    }

    public restoreRecord(model: LogBookPageEditExceptionRegisterDTO): void {
        this.confirmDialog.open({
            title: this.translationService.getValue('log-book-page-edit-exceptions.restore-exception-dialog-title'),
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.restoreLogBookPageEditException(model.id!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        }
                    });
                }
            }
        });
    }

    private buildForm(): void {
        this.formGroup = new FormGroup({
            userControl: new FormControl(),
            logBookTypesControl: new FormControl(),
            logBookControl: new FormControl(),
            exceptionActiveDateFromControl: new FormControl(),
            exceptionActiveDateToControl: new FormControl(),
            editPageDateFromControl: new FormControl(),
            editPageDateToControl: new FormControl()
        });
    }

    private openEditDialog(title: string, dialogData: DialogParamsModel, viewMode: boolean, auditButton: IHeaderAuditButton | undefined): void {
        this.editDialog.openWithTwoButtons({
            TCtor: EditLogBookPageEditExceptionComponent,
            translteService: this.translationService,
            title: title,
            componentData: dialogData,
            headerAuditButton: auditButton,
            viewMode: viewMode,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => { closeFn(); }
            },
        }, '800px').subscribe({
            next: (result: LogBookPageEditExceptionEditDTO | undefined) => {
                if (result !== null && result !== undefined) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    private mapFilters(filters: FilterEventArgs): LogBookPageEditExceptionFilters {
        return new LogBookPageEditExceptionFilters({
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
    }
}
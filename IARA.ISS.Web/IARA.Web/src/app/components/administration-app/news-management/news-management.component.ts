import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NewsManagementDTO } from '@app/models/generated/dtos/NewsManagementDTO';
import { NewsManagementFilters } from '@app/models/generated/filters/NewsManagmentFilters';
import { NewsManagementService } from '@app/services/administration-app/news-management.service';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { EditNewsManagementComponent } from './edit-news-management.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { DialogCloseCallback } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { NewsManagementDialogParams } from './models/news-management-dialog-params.model';
import { NewsManagementEditDTO } from '@app/models/generated/dtos/NewsManagementEditDTO';
import { INewsManagementService } from '@app/interfaces/administration-app/news-management.interface';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';

type ThreeState = 'yes' | 'no' | 'both';

@Component({
    selector: 'news-management',
    templateUrl: './news-management.component.html'
})
export class NewsManagementComponent implements OnInit, AfterViewInit {
    public translateService: FuseTranslationLoaderService;
    public filtersFormGroup: FormGroup;

    public statusCategories: NomenclatureDTO<ThreeState>[] = [];

    public readonly canAddNews: boolean;
    public readonly canEditNews: boolean;
    public readonly canDeleteNews: boolean;
    public readonly canRestoreNews: boolean;

    @ViewChild(TLDataTableComponent)
    private readonly datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private readonly searchpanel!: SearchPanelComponent;

    private gridManager!: DataTableManager<NewsManagementDTO, NewsManagementFilters>;
    private editDialog: TLMatDialog<EditNewsManagementComponent>;
    private confirmDialog!: TLConfirmDialog;
    private newsManagmentService: INewsManagementService;

    constructor(
        translateService: FuseTranslationLoaderService,
        newsManagmentService: NewsManagementService,
        confirmDialog: TLConfirmDialog,
        editDialog: TLMatDialog<EditNewsManagementComponent>,
        permissionsService: PermissionsService
    ) {
        this.translateService = translateService;
        this.newsManagmentService = newsManagmentService;
        this.confirmDialog = confirmDialog;
        this.editDialog = editDialog;

        this.canAddNews = permissionsService.has(PermissionsEnum.NewsManagementAddRecords);
        this.canEditNews = permissionsService.has(PermissionsEnum.NewsManagementEditRecords);
        this.canDeleteNews = permissionsService.has(PermissionsEnum.NewsManagementDeleteRecords);;
        this.canRestoreNews = permissionsService.has(PermissionsEnum.NewsManagementRestoreRecords);

        this.filtersFormGroup = new FormGroup({
            titleControl: new FormControl(),
            contentControl: new FormControl(),
            isPublishedControl: new FormControl(),
            dateRangeControl: new FormControl()
        });
    }

    public ngOnInit(): void {
        this.statusCategories = [
            new NomenclatureDTO<ThreeState>({
                displayName: '—',
                value: undefined,
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                displayName: this.translateService.getValue('news-management.three-state-yes'),
                value: 'yes',
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                displayName: this.translateService.getValue('news-management.three-state-no'),
                value: 'no',
                isActive: true
            })
        ];
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<NewsManagementDTO, NewsManagementFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.newsManagmentService.getAll.bind(this.newsManagmentService),
            filtersMapper: this.mapFilters
        });

        this.gridManager.refreshData();
    }

    public openEditDialog(id?: number, isView: boolean = false): void {
        let title: string;
        let auditButton: IHeaderAuditButton | undefined;
        let data: NewsManagementDialogParams | undefined;

        //edit, view
        if (id !== undefined) {
            data = new NewsManagementDialogParams({
                id: id,
                viewMode: isView
            });

            if (isView) {
                title = this.translateService.getValue('news-management.dialog-title-view');
            }
            else {
                title = this.translateService.getValue('news-management.dialog-title-edit');
            }

            auditButton = {
                getAuditRecordData: this.newsManagmentService.getSimpleAudit.bind(this.newsManagmentService),
                id: id,
                tableName: 'News.News',
            };
        }
        //add
        else {
            title = this.translateService.getValue('news-management.dialog-title-add');
        }

        this.editDialog.openWithTwoButtons({
            TCtor: EditNewsManagementComponent,
            title: title,
            componentData: data,
            headerAuditButton: auditButton,
            translteService: this.translateService,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            viewMode: isView
        }).subscribe({
            next: (result: NewsManagementEditDTO | undefined) => {
                if (result !== undefined) {
                    this.gridManager.refreshData();
                }
            }
        })
    }

    public deleteNews(news: NewsManagementDTO): void {
        this.confirmDialog.open({
            title: this.translateService.getValue('news-management.dialog-delete-title'),
            message: this.translateService.getValue('news-management.dialog-delete-message'),
            okBtnLabel: this.translateService.getValue('news-management.dialog-delete-btn')
        }).subscribe({
            next: (result: boolean) => {
                if (result === true) {
                    this.newsManagmentService.deleteNews(news.id!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        }
                    });
                }
            }
        });
    }

    public undoDeletedNews(news: NewsManagementDTO): void {
        this.confirmDialog.open({
            title: this.translateService.getValue('news-management.dialog-restore-title'),
            message: this.translateService.getValue('news-management.dialog-restore-message'),
            okBtnLabel: this.translateService.getValue('news-management.dialog-restore-btn')
        }).subscribe({
            next: (result: boolean) => {
                if (result === true) {
                    this.newsManagmentService.undoDeletedNews(news.id!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        }
                    })
                }
            }
        });
    }

    private closeDialogBtnClicked(closeFn: DialogCloseCallback): void {
        closeFn();
    }

    private mapFilters(filters: FilterEventArgs): NewsManagementFilters {
        const newsManagmentFilters: NewsManagementFilters = new NewsManagementFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,
            dateFrom: filters.getValue<DateRangeData>('dateRangeControl')?.start,
            dateTo: filters.getValue<DateRangeData>('dateRangeControl')?.end,
            content: filters.getValue('contentControl'),
            title: filters.getValue('titleControl')
        });

        const result = filters.getValue<ThreeState>('isPublishedControl');
        if (result !== undefined && result !== null) {
            switch (result) {
                case 'yes':
                    newsManagmentFilters.isPublished = true;
                    break;
                case 'no':
                    newsManagmentFilters.isPublished = false;
                    break;
                case undefined:
                    newsManagmentFilters.isPublished = undefined;
                    break;
            }
        }

        return newsManagmentFilters;
    }
}
import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TranslationManagementDTO } from '@app/models/generated/dtos/TranslationManagementDTO';
import { TranslationManagementFilters } from '@app/models/generated/filters/TranslationManagementFilters';
import { TranslationManagementService } from '@app/services/administration-app/translation-management.service';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { MessageService } from '@app/shared/services/message.service';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { EditTranslationComponent } from './edit-translation.component';
import { BasePageComponent } from '@app/components/common-app/base-page.component';
import { RequestProperties } from '@app/shared/services/request-properties';
import { TLHelpComponent } from '@app/shared/components/tl-help/tl-help.component';
import { TranslationTypeEnum } from '@app/enums/translation-type.enum';
import { TranslationResourceTypeEnum } from '@app/enums/translation-resource-type.enum';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TranslationManagementEditDTO } from '@app/models/generated/dtos/TranslationManagementEditDTO';
import { ITranslationManagementService } from '@app/interfaces/administration-app/translation-management.interface';
import { EditTranslationDialogParams } from './models/edit-translation-dialog-params.model';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';

@Component({
    selector: 'translation-management',
    templateUrl: './translation-management.component.html'
})
export class TranslationManagementComponent extends BasePageComponent implements OnInit, AfterViewInit {
    @Input()
    public resourceType!: TranslationResourceTypeEnum;

    public readonly resourceTypes: typeof TranslationResourceTypeEnum = TranslationResourceTypeEnum;

    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public groups: string[] = [];
    public translationTypes: NomenclatureDTO<TranslationTypeEnum>[] = [];

    public readonly canEditRecords: boolean;

    public editModeLabel!: string;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private grid!: DataTableManager<TranslationManagementDTO, TranslationManagementFilters>;
    private editDialog: TLMatDialog<EditTranslationComponent>;
    private snackBar: MatSnackBar;

    private service: ITranslationManagementService;

    public constructor(
        translate: FuseTranslationLoaderService,
        service: TranslationManagementService,
        editDialog: TLMatDialog<EditTranslationComponent>,
        permissions: PermissionsService,
        messageService: MessageService,
        snackBar: MatSnackBar
    ) {
        super(messageService);

        this.translate = translate;
        this.service = service;
        this.editDialog = editDialog;
        this.snackBar = snackBar;

        this.canEditRecords = permissions.has(PermissionsEnum.TranslationEditRecords);

        this.buildForm();

        this.translationTypes = [
            new NomenclatureDTO<TranslationTypeEnum>({
                value: TranslationTypeEnum.WEB,
                displayName: this.translate.getValue('translation-management.translation-types-web'),
                isActive: true
            }),
            new NomenclatureDTO<TranslationTypeEnum>({
                value: TranslationTypeEnum.MOBILE_PUBLIC,
                displayName: this.translate.getValue('translation-management.translation-types-mobile-public'),
                isActive: true
            }),
            new NomenclatureDTO<TranslationTypeEnum>({
                value: TranslationTypeEnum.MOBILE_INSP,
                displayName: this.translate.getValue('translation-management.translation-types-mobile-insp'),
                isActive: true
            })
        ];

        this.updateEditModeLabel();
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.TranslationGroups, this.service.getGroups.bind(this.service), false
        ).subscribe({
            next: (groups: NomenclatureDTO<number>[]) => {
                this.groups = groups.map(x => x.displayName!);
            } 
        });
    }

    public ngAfterViewInit(): void {
        this.grid = new DataTableManager<TranslationManagementDTO, TranslationManagementFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.resourceType === TranslationResourceTypeEnum.Label
                ? this.service.getAllLabelTranslations.bind(this.service)
                : this.service.getAllHelpTranslations.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        this.grid.refreshData();
    }

    public addEditTranslation(entry: TranslationManagementDTO, viewMode: boolean): void {
        const dialog = this.editDialog.openWithTwoButtons({
            title: viewMode
                ? this.translate.getValue('translation-management.read-dialog')
                : this.translate.getValue('translation-management.edit-dialog'),
            TCtor: EditTranslationComponent,
            headerAuditButton: {
                id: entry.id!,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'Admin.NTranslationResources'
            },
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction): void => {
                    closeFn();
                }
            },
            componentData: new EditTranslationDialogParams({
                id: entry.id,
                viewMode: viewMode
            }),
            translteService: this.translate,
            viewMode: viewMode
        }, '50em');

        dialog.subscribe({
            next: (result: TranslationManagementEditDTO | undefined) => {
                if (result) {
                    this.grid.refreshData();
                }
            }
        });
    }

    public toggleEditMode(): void {
        TLHelpComponent.toggleShowAllHelpers();
        this.updateEditModeLabel();

        if (TLHelpComponent.alwaysShowHelpers) {
            this.snackBar.open(this.translate.getValue('translation-management.show-all-helpers-turned-on'), undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationSucc,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassSucc
            });
        }
        else {
            this.snackBar.open(this.translate.getValue('translation-management.show-all-helpers-turned-off'), undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationSucc,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassSucc
            });
        }
    }

    private buildForm(): void {
        this.form = new FormGroup({
            codeControl: new FormControl(),
            groupControl: new FormControl(),
            languageCodeControl: new FormControl(),
            groupCodeControl: new FormControl(),
            translationTypeControl: new FormControl(),
            translationValueBGControl: new FormControl(),
            translationValueENControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): TranslationManagementFilters {
        const result = new TranslationManagementFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            code: filters.getValue('codeControl'),
            groupCode: filters.getValue('groupControl'),
            translationValueBG: filters.getValue('translationValueBGControl'),
            translationValueEN: filters.getValue('translationValueENControl')
        });

        const translationType: TranslationTypeEnum | undefined = filters.getValue<TranslationTypeEnum>('translationTypeControl');
        if (translationType !== undefined && translationType !== null) {
            result.translationType = TranslationTypeEnum[translationType];
        }

        return result;
    }

    private updateEditModeLabel(): void {
        if (TLHelpComponent.alwaysShowHelpers) {
            this.editModeLabel = this.translate.getValue('translation-management.edit-mode-off');
        }
        else {
            this.editModeLabel = this.translate.getValue('translation-management.edit-mode-on');
        }
    }
}
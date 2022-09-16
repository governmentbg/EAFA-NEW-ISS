import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';

import { UserLegalStatusEnum } from '@app/enums/user-legal-status.enum';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ApplicationForChoiceDTO } from '@app/models/generated/dtos/ApplicationForChoiceDTO';
import { LegalEntityDTO } from '@app/models/generated/dtos/LegalEntityDTO';
import { LegalEntityEditDTO } from '@app/models/generated/dtos/LegalEntityEditDTO';
import { LegalEntitiesFilters } from '@app/models/generated/filters/LegalEntitiesFilters';
import { LegalEntitiesAdministrationService } from '@app/services/administration-app/legal-entities-administration.service';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { ChooseApplicationComponent } from '../../common-app/applications/components/choose-application/choose-application.component';
import { ChooseApplicationDialogParams } from '../../common-app/applications/components/choose-application/models/choose-application-dialog-params.model';
import { EditLegalEntityComponent } from '../../common-app/legals/edit-legal-entity/edit-legal-entity.component';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';

@Component({
    selector: 'legal-entities',
    templateUrl: './legal-entities.component.html'
})
export class LegalEntitiesComponent implements AfterViewInit {
    public translate: FuseTranslationLoaderService;
    public formGroup!: FormGroup;

    public canAddRecords!: boolean;
    public canEditRecords!: boolean;
    public canDeleteRecords!: boolean;
    public canRestoreRecords!: boolean;

    public legalStatusEnum: typeof UserLegalStatusEnum = UserLegalStatusEnum;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private permissions: PermissionsService;
    private gridManager!: DataTableManager<LegalEntityDTO, LegalEntitiesFilters>;
    private editDialog: TLMatDialog<EditLegalEntityComponent>;
    private chooseApplicationDialog: TLMatDialog<ChooseApplicationComponent>;
    private service!: LegalEntitiesAdministrationService;

    public constructor(
        translate: FuseTranslationLoaderService,
        permissions: PermissionsService,
        service: LegalEntitiesAdministrationService,
        editDialog: TLMatDialog<EditLegalEntityComponent>,
        chooseApplicationDialog: TLMatDialog<ChooseApplicationComponent>
    ) {
        this.translate = translate;
        this.permissions = permissions;
        this.editDialog = editDialog;
        this.chooseApplicationDialog = chooseApplicationDialog;
        this.service = service;

        this.canAddRecords = this.permissions.has(PermissionsEnum.LegalEntitiesAddRecords);
        this.canEditRecords = this.permissions.has(PermissionsEnum.LegalEntitiesEditRecords);
        this.canDeleteRecords = this.permissions.has(PermissionsEnum.LegalEntitiesDeleteRecords);
        this.canRestoreRecords = this.permissions.has(PermissionsEnum.LegalEntitiesRestoreRecords);

        this.buildForm();
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<LegalEntityDTO, LegalEntitiesFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllLegalEntities.bind(this.service),
            filtersMapper: this.mapFilters
        });

        this.gridManager.refreshData();
    }

    public openChooseApplicationForRegisterDialog(): void {
        this.chooseApplicationDialog.open({
            TCtor: ChooseApplicationComponent,
            title: this.translate.getValue('applications-register.choose-application-for-register-creation'),
            translteService: this.translate,
            componentData: new ChooseApplicationDialogParams({ pageCodes: [PageCodeEnum.LE] }),
            disableDialogClose: true,
            headerCancelButton: {
                cancelBtnClicked: this.closeApplicationChooseDialogBtnClicked.bind(this)
            },
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translate.getValue('applications-register.choose')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translate.getValue('common.cancel'),
            }
        }).subscribe((dialogData: { selectedApplication: ApplicationForChoiceDTO }) => {
            if (dialogData !== null && dialogData !== undefined) {
                const title: string = this.translate.getValue('legal-entities-page.add-legal-entity-dialog-title');

                const data = new DialogParamsModel({
                    id: undefined,
                    isApplication: false,
                    isReadonly: false,
                    viewMode: false,
                    service: this.service,
                    applicationId: dialogData.selectedApplication.id
                });

                this.openEditDialog(data, title, undefined);
            }
        });
    }

    public editLegalEntity(legal: LegalEntityDTO, viewMode: boolean): void {
        const data: DialogParamsModel = new DialogParamsModel({
            id: legal.id!,
            isApplication: false,
            isReadonly: viewMode,
            viewMode: viewMode,
            service: this.service
        });

        const auditBtn: IHeaderAuditButton = {
            id: legal.id!,
            getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
            tableName: 'Legal'
        };

        const title: string = viewMode
            ? this.translate.getValue('legal-entities-page.view-legal-entity-dialog-title')
            : this.translate.getValue('legal-entities-page.edit-legal-entity-dialog-title');

        this.openEditDialog(data, title, auditBtn);
    }

    private openEditDialog(data: DialogParamsModel, title: string, auditButton?: IHeaderAuditButton): void {
        const dialog = this.editDialog.open({
            title: title,
            TCtor: EditLegalEntityComponent,
            headerAuditButton: auditButton,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
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
                translateValue: this.translate.getValue('common.cancel'),
            },
            viewMode: data.viewMode
        });

        dialog.subscribe({
            next: (entry: LegalEntityEditDTO | undefined) => {
                if (entry !== undefined) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    private buildForm(): void {
        this.formGroup = new FormGroup({
            legalNameControl: new FormControl(),
            eikControl: new FormControl(),
            registeredDateRangeControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): LegalEntitiesFilters {
        const result = new LegalEntitiesFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            eik: filters.getValue('eikControl'),
            legalName: filters.getValue('legalNameControl'),
            registeredDateFrom: filters.getValue<DateRangeData>('registeredDateRangeControl')?.start,
            registeredDateTo: filters.getValue<DateRangeData>('registeredDateRangeControl')?.end
        });

        return result;
    }

    private closeDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeApplicationChooseDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}
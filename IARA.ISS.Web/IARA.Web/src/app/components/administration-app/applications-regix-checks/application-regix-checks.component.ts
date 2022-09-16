import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IRemoteTLDatatableComponent } from '@app/shared/components/data-table/interfaces/tl-remote-datatable.interface';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { ApplicationRegixCheckRequestDTO } from '@app/models/generated/dtos/ApplicationRegixCheckRequestDTO';
import { ApplicationRegiXChecksFilters } from '@app/models/generated/filters/ApplicationRegiXChecksFilters';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { ViewApplicationRegixChecksComponent } from './view-application-regix-checks.component';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { ApplicationRegixChecksService } from '@app/services/administration-app/application-regix-checks.service';
import { IApplicationsRegisterService } from '@app/interfaces/administration-app/applications-register.interface';
import { ApplicationsProcessingService } from '@app/services/administration-app/applications-processing.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { RegixCheckStatusesEnum } from '@app/enums/regix-check-statuses.enum';

@Component({
    selector: 'application-regix-checks',
    templateUrl: './application-regix-checks.component.html'
})
export class ApplicationRegixChecksComponent implements OnInit, AfterViewInit {
    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public applicationTypes: NomenclatureDTO<number>[] = [];
    public errorLevels: NomenclatureDTO<string>[] = [];

    @ViewChild(TLDataTableComponent)
    private datatable!: IRemoteTLDatatableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private gridManager!: DataTableManager<ApplicationRegixCheckRequestDTO, ApplicationRegiXChecksFilters>;
    private service: ApplicationRegixChecksService;
    private viewDialog: TLMatDialog<ViewApplicationRegixChecksComponent>;
    private applicationRegisterService!: IApplicationsRegisterService;

    public constructor(
        translate: FuseTranslationLoaderService,
        service: ApplicationRegixChecksService,
        applicationRegisterService: ApplicationsProcessingService,
        viewDialog: TLMatDialog<ViewApplicationRegixChecksComponent>
    ) {
        this.translate = translate;
        this.service = service;
        this.applicationRegisterService = applicationRegisterService;
        this.viewDialog = viewDialog;

        this.buildForm();
    }

    public ngOnInit(): void {
        this.applicationRegisterService.getApplicationTypes().subscribe({
            next: (types: NomenclatureDTO<number>[]) => {
                this.applicationTypes = types;
            }
        });

        for (const level in RegixCheckStatusesEnum) {
            if (isNaN(Number(level))) {
                this.errorLevels.push(new NomenclatureDTO<string>({
                    value: level,
                    displayName: level,
                    isActive: true
                }));
            }
        }
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<ApplicationRegixCheckRequestDTO, ApplicationRegiXChecksFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAll.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        this.gridManager.refreshData();
    }

    public openDialog(regixCheck: ApplicationRegixCheckRequestDTO): void {
        this.viewDialog.open({
            title: this.translate.getValue('application-regix-checks.request-dialog-title'),
            TCtor: ViewApplicationRegixChecksComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction): void => {
                    closeFn();
                }
            },
            componentData: new DialogParamsModel({ id: regixCheck.id }),
            translteService: this.translate,
            viewMode: true
        });
    }

    private buildForm(): void {
        this.form = new FormGroup({
            applicationIdControl: new FormControl(undefined, TLValidators.number(undefined, undefined, 0)),
            applicationTypeIdControl: new FormControl(),
            errorLevelControl: new FormControl(),
            webServiceNameControl: new FormControl(),
            requestDateTimeControl: new FormControl(),
            responseDateTimeControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): ApplicationRegiXChecksFilters {
        const result: ApplicationRegiXChecksFilters = new ApplicationRegiXChecksFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            applicationId: filters.getValue('applicationIdControl'),
            applicationTypeId: filters.getValue('applicationTypeIdControl'),
            errorLevels: filters.getValue('errorLevelControl'),
            webServiceName: filters.getValue('webServiceNameControl'),
            requestDateFrom: filters.getValue<DateRangeData>('requestDateTimeControl')?.start,
            requestDateTo: filters.getValue<DateRangeData>('requestDateTimeControl')?.end,
            responseDateFrom: filters.getValue<DateRangeData>('responseDateTimeControl')?.start,
            responseDateTo: filters.getValue<DateRangeData>('responseDateTimeControl')?.end
        });

        return result;
    }
}
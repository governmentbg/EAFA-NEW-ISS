import { IApplicationsRegisterService } from '@app/interfaces/administration-app/applications-register.interface';
import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { ApplicationProcessingPermissions } from '@app/models/common/application-processing-permissions.model';
import { ApplicationsRegisterData } from '@app/models/common/applications-register-data.model';
import { ApplicationRegisterDTO } from '@app/models/generated/dtos/ApplicationRegisterDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ApplicationsRegisterFilters } from '@app/models/generated/filters/ApplicationsRegisterFilters';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { ApplicationsTableComponent, ApplicationTablePageType } from '../applications-table/applications-table.component';
import { ApplicationProcessingHasPermissions } from '../models/application-processing-has-permissions.model';


type ThreeState = 'yes' | 'no' | 'both';

@Component({
    selector: 'applications-register',
    templateUrl: './applications-register.component.html'
})
export class ApplicationsRegisterComponent<T extends IDialogComponent> implements OnInit, AfterViewInit {
    @Input() public applicationsRegisterData!: Map<PageCodeEnum, ApplicationsRegisterData<T>>;
    @Input() public service!: IApplicationsRegisterService;
    @Input() public applicationsService!: IApplicationsService;
    @Input() public processingPermissions!: Map<PageCodeEnum, ApplicationProcessingPermissions>;
    @Input() public pageType: ApplicationTablePageType = 'ApplicationPage';
    @Input() public isPublicApp: boolean = false;

    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public statuses!: NomenclatureDTO<number>[];
    public types!: NomenclatureDTO<number>[];
    public sources!: NomenclatureDTO<number>[];
    public showOnlyAssignedOptions!: NomenclatureDTO<ThreeState>[];

    public applicationProcessingHasPermissions: Map<PageCodeEnum, ApplicationProcessingHasPermissions> = new Map<PageCodeEnum, ApplicationProcessingHasPermissions>();

    @ViewChild(ApplicationsTableComponent)
    private applicationsTable!: ApplicationsTableComponent<T>;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private permissions: PermissionsService;
    private grid!: DataTableManager<ApplicationRegisterDTO, ApplicationsRegisterFilters>;

    public constructor(translate: FuseTranslationLoaderService, permissions: PermissionsService) {
        this.translate = translate;
        this.permissions = permissions;

        this.buildForm();
    }

    public ngOnInit(): void {
        for (const map of this.processingPermissions) {
            const applicationProcessingHasPermission: ApplicationProcessingHasPermissions = new ApplicationProcessingHasPermissions({
                canAddRecords: this.permissions.has(map[1].addPermission),
                canEditRecords: this.permissions.has(map[1].editPermission),
                canDeleteRecords: this.permissions.has(map[1].deletePermission),
                canRestoreRecords: this.permissions.has(map[1].restorePermission),
                canCancelRecords: this.permissions.has(map[1].cancelPermssion),

                canEnterEventisNumber: this.permissions.has(map[1].enterEventisNumberPermission ?? ''),
                canInspectAndCorrectRecords: this.permissions.has(map[1].inspectAndCorrectPermssion ?? ''),
                canProcessPaymentData: this.permissions.has(map[1].processPaymentDataPermission ?? ''),
                canConfirmDataRegularity: this.permissions.has(map[1].checkDataRegularityPermission ?? ''),
                canAddAdministrativeActRecords: this.permissions.has(map[1].addAdministrativeActPermission ?? ''),
                canViewAdministrativeActRecords: this.permissions.has(map[1].readAdministrativeActPermission ?? ''),
                canDownloadOnlineApplications: this.permissions.has(map[1].downloadOnlineApplicationsPermission ?? ''),
                canUploadOnlineApplications: this.permissions.has(map[1].uploadOnlineApplicationsPermission ?? ''),
                canReAssignApplicationRecords: this.permissions.has(map[1].canReAssignApplicationsPermission ?? '')
            });

            this.applicationProcessingHasPermissions.set(map[0], applicationProcessingHasPermission);
        }

        this.service.getApplicationStatuses().subscribe({
            next: (statuses: NomenclatureDTO<number>[]) => {
                this.statuses = statuses;
            }
        });

        this.service.getApplicationTypes().subscribe({
            next: (types: NomenclatureDTO<number>[]) => {
                this.types = types;
            }
        });

        this.service.getApplicationSources().subscribe({
            next: (sources: NomenclatureDTO<number>[]) => {
                this.sources = sources;
            }
        });

        if (this.pageType === 'FileInPage') {
            this.showOnlyAssignedOptions = [
                new NomenclatureDTO<ThreeState>({
                    value: 'yes',
                    displayName: this.translate.getValue('applications-register.assigned-only'),
                    isActive: true
                }),
                new NomenclatureDTO<ThreeState>({
                    value: 'no',
                    displayName: this.translate.getValue('applications-register.not-assigned-only'),
                    isActive: true
                }),
                new NomenclatureDTO<ThreeState>({
                    value: 'both',
                    displayName: this.translate.getValue('applications-register.assigned-show-all'),
                    isActive: true
                })
            ];
        }
    }

    public ngAfterViewInit(): void {
        this.grid = new DataTableManager<ApplicationRegisterDTO, ApplicationsRegisterFilters>({
            tlDataTable: this.applicationsTable.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllApplications.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        if (!this.isPublicApp) {
            const accessCode: string | undefined = window.history.state?.accessCode;

            if (!CommonUtils.isNullOrEmpty(accessCode)) {
                this.grid.advancedFilters = new ApplicationsRegisterFilters({ accessCode: accessCode });
            }
        }

        this.grid.refreshData();
    }

    public deleteApplication(applicationId: number): void {
        this.service.deleteApplication(applicationId).subscribe({
            next: () => {
                this.grid.refreshData();
            }
        });
    }

    public restoreApplication(applicationId: number): void {
        this.service.undoDeleteApplication(applicationId).subscribe({
            next: () => {
                this.grid.refreshData();
            }
        });
    }

    public addedOrEdittedApplication(applicationId: number): void {
        this.grid.refreshData();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            accessCodeControl: new FormControl(),
            eventisNumControl: new FormControl(),
            applicationTypeControl: new FormControl(),
            applicationStatusControl: new FormControl(),
            applicationDateRangeControl: new FormControl(),
            submittedForControl: new FormControl(),
            submittedForEgnLncControl: new FormControl(),
            applicationSourceControl: new FormControl(),
            showOnlyAssignedControl: new FormControl(),
            assignedToControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): ApplicationsRegisterFilters {
        const result = new ApplicationsRegisterFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            accessCode: filters.getValue('accessCodeControl'),
            eventisNum: filters.getValue('eventisNumControl'),
            applicationTypeId: filters.getValue('applicationTypeControl'),
            applicationStatusId: filters.getValue('applicationStatusControl'),
            dateFrom: filters.getValue<DateRangeData>('applicationDateRangeControl')?.start,
            dateTo: filters.getValue<DateRangeData>('applicationDateRangeControl')?.end,
            submittedFor: filters.getValue('submittedForControl'),
            submittedForEgnLnc: filters.getValue('submittedForEgnLncControl'),
            applicationSourceId: filters.getValue('applicationSourceControl')
        });

        if (this.pageType === 'FileInPage') {
            const showAssigned = filters.getValue<ThreeState>('showOnlyAssignedControl');
            if (showAssigned !== undefined && showAssigned !== null) {
                switch (showAssigned) {
                    case 'yes':
                        result.showAssignedApplications = true;
                        break;
                    case 'no':
                        result.showAssignedApplications = false;
                        break;
                    case 'both':
                        result.showAssignedApplications = undefined;
                        break;
                }
            }

            result.assignedTo = filters.getValue('assignedToControl');
        }

        return result;
    }
}
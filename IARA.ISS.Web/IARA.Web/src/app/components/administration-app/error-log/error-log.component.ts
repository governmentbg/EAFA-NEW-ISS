import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { ErrorLogDTO } from '@app/models/generated/dtos/ErrorLogDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ErrorLogFilters } from '@app/models/generated/filters/ErrorLogFilters';
import { ErrorLogService } from '@app/services/administration-app/error-log.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { IRemoteTLDatatableComponent } from '@app/shared/components/data-table/interfaces/tl-remote-datatable.interface';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ErrorLogSeverityEnum } from '@app/enums/error-log-severity.enum';

@Component({
    selector: 'error-log',
    templateUrl: './error-log.component.html'
})
export class ErrorLogComponent implements AfterViewInit, OnInit {
    public translationService: FuseTranslationLoaderService;
    public errorLogFormGroup: FormGroup;
    public usernames: NomenclatureDTO<number>[] = [];
    public severityTypes: NomenclatureDTO<ErrorLogSeverityEnum>[] = [];

    @ViewChild(TLDataTableComponent)
    public datatable!: IRemoteTLDatatableComponent;

    @ViewChild(SearchPanelComponent)
    public searchpanel!: SearchPanelComponent;

    private gridManager!: DataTableManager<ErrorLogDTO, ErrorLogFilters>;
    private service: ErrorLogService;
    private commonNomenclaturesService!: CommonNomenclatures;
    private snackBar: MatSnackBar;

    public constructor(service: ErrorLogService,
        translationService: FuseTranslationLoaderService,
        commonNomenclaturesService: CommonNomenclatures,
        snackBar: MatSnackBar
    ) {
        this.translationService = translationService;
        this.service = service;
        this.commonNomenclaturesService = commonNomenclaturesService;
        this.snackBar = snackBar;

        this.errorLogFormGroup = new FormGroup({
            errorLogDateRangeControl: new FormControl(),
            usernameControl: new FormControl(),
            severityControl: new FormControl(),
            classControl: new FormControl(),
            idControl: new FormControl()
        });

        this.severityTypes = [
            new NomenclatureDTO<ErrorLogSeverityEnum>({
                value: ErrorLogSeverityEnum.Debug,
                displayName: translationService.getValue('error-log.severity-debug'),
                isActive: true
            }),
            new NomenclatureDTO<ErrorLogSeverityEnum>({
                value: ErrorLogSeverityEnum.Error,
                displayName: translationService.getValue('error-log.severity-error'),
                isActive: true
            }),
            new NomenclatureDTO<ErrorLogSeverityEnum>({
                value: ErrorLogSeverityEnum.Information,
                displayName: translationService.getValue('error-log.severity-info'),
                isActive: true
            }),
            new NomenclatureDTO<ErrorLogSeverityEnum>({
                value: ErrorLogSeverityEnum.Warn,
                displayName: translationService.getValue('error-log.severity-warn'),
                isActive: true
            })
        ];
    }

    public ngOnInit(): void {
        this.commonNomenclaturesService.getUserNames().subscribe((result: NomenclatureDTO<number>[]) => {
            this.usernames = result;
        });
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<ErrorLogDTO, ErrorLogFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAll.bind(this.service),
            filtersMapper: this.mapFilters
        });

        this.gridManager.refreshData();
    }

    public copyMessageToClipboard(row: GridRow<ErrorLogDTO>): string {
        return row.data.message!;
    }

    public copyStackTraceToClipboard(row: GridRow<ErrorLogDTO>): string {
        return row.data.stackTrace!;
    }

    public messageCopied(copied: boolean): void {
        const messageSuccess: string = this.translationService.getValue('error-log.message-copied-successfully');
        const messageFail: string = this.translationService.getValue('error-log.message-copy-failed');
        this.valueCopied(copied, messageSuccess, messageFail);
    }

    public stackTraceCopied(copied: boolean): void {
        const messageSuccess: string = this.translationService.getValue('error-log.stack-trace-copied-successfully');
        const messageFail: string = this.translationService.getValue('error-log.stack-trace-copy-failed');
        this.valueCopied(copied, messageSuccess, messageFail);
    }

    private valueCopied(copied: boolean, messageSuccess: string, messageFail: string): void {
        if (copied === true) {
            this.snackBar.open(messageSuccess, undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationSucc,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassSucc
            });
        }
        else {
            this.snackBar.open(messageFail, undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
            });
        }
    }

    private mapFilters(filters: FilterEventArgs): ErrorLogFilters {
        const result = new ErrorLogFilters({
            freeTextSearch: filters.searchText,

            errorLogDateFrom: filters.getValue<DateRangeData>('errorLogDateRangeControl')?.start,
            errorLogDateTo: filters.getValue<DateRangeData>('errorLogDateRangeControl')?.end,
            userId: filters.getValue('usernameControl'),
            class: filters.getValue('classControl'),
            errorLogId: filters.getValue('idControl')
        });

        const severityTypes: ErrorLogSeverityEnum[] | undefined = filters.getValue('severityControl');
        if (severityTypes !== undefined && severityTypes !== null) {
            result.severity = severityTypes.map((type: ErrorLogSeverityEnum) => {
                return ErrorLogSeverityEnum[type];
            })
        }

        return result;
    }
}
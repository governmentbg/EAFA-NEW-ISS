import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';
import { MatTabChangeEvent } from '@angular/material/tabs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IInspectorsService } from '@app/interfaces/administration-app/inspectors.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { InspectorsRegisterDTO } from '@app/models/generated/dtos/InspectorsRegisterDTO';
import { InspectorsFilters } from '@app/models/generated/filters/InspectorsFilters';
import { InspectorsService } from '@app/services/administration-app/inspectors.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { CommandTypes } from '@app/shared/components/data-table/enums/command-type.enum';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { InspectorsRegisterEditDTO } from '@app/models/generated/dtos/InspectorsRegisterEditDTO';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { TLValidators } from '@app/shared/utils/tl-validators';

@Component({
    selector: 'inspectors',
    templateUrl: './inspectors-register.component.html'
})
export class InspectorsRegisterComponent implements AfterViewInit, OnInit {
    public translate: FuseTranslationLoaderService;
    public inspectorsFormGroup!: FormGroup;
    public addInspectorFormGroup!: FormGroup;
    public readOnly: boolean = false;
    public usernames: NomenclatureDTO<number>[] = [];
    public inspectorsRegisters: InspectorsRegisterDTO[] = [];

    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;
    public readonly canDeleteRecords: boolean;
    public readonly canRestoreRecords: boolean;

    public unregisteredInspectorsLoaded: boolean = false;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private service: IInspectorsService;
    private commonNomenclaturesService!: CommonNomenclatures;
    private snackbar: MatSnackBar;

    private gridManager!: DataTableManager<InspectorsRegisterDTO, InspectorsFilters>;

    public constructor(
        inspectorsService: InspectorsService,
        translate: FuseTranslationLoaderService,
        commonNomenclaturesService: CommonNomenclatures,
        snackbar: MatSnackBar,
        permissions: PermissionsService
    ) {
        this.service = inspectorsService;
        this.translate = translate;
        this.commonNomenclaturesService = commonNomenclaturesService;
        this.snackbar = snackbar;

        this.canAddRecords = permissions.has(PermissionsEnum.InspectionsAddRecords);
        this.canEditRecords = permissions.has(PermissionsEnum.InspectionsEditRecords);
        this.canDeleteRecords = permissions.has(PermissionsEnum.InspectionsDeleteRecords);
        this.canRestoreRecords = permissions.has(PermissionsEnum.InspectionsRestoreRecords);

        this.buildForm();
    }

    public ngOnInit(): void {
        this.commonNomenclaturesService.getUserNames().subscribe((result: NomenclatureDTO<number>[]) => {
            this.usernames = result;
        });
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<InspectorsRegisterDTO, InspectorsFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllRegistered.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });
        this.gridManager.refreshData();

    }

    public inspectorRecordChanged(event: RecordChangedEventArgs<InspectorsRegisterDTO>): void {
        event.Record.inspectionSequenceNum = Number(event.Record.inspectionSequenceNum)!;

        switch (event.Command) {
            case CommandTypes.Add: {
                this.service.addInspector(event.Record).subscribe({
                    next: () => {
                        this.gridManager.refreshData();
                    },
                    error: (response: HttpErrorResponse) => {
                        this.handleInspectorAlreadyExistsError(response);
                        this.gridManager.refreshData();
                    }
                });
            } break;
            case CommandTypes.Delete: {
                this.deleteInspector(event.Record.id!);
            } break;
            case CommandTypes.Edit: {
                this.service.editInspector(event.Record).subscribe({
                    next: () => {
                        this.gridManager.refreshData();
                    },
                    error: (response: HttpErrorResponse) => {
                        this.handleInspectorAlreadyExistsError(response);
                        this.gridManager.refreshData();
                    }
                });
            } break;
            case CommandTypes.UndoDelete: {
                this.restoreInspector(event.Record.id!);
            } break;
        }
    }

    public onAddRecord(): void {
        this.addInspectorFormGroup.get('userIdControlHidden')!.enable();
    }

    public onEditRecord(record: InspectorsRegisterEditDTO): void {
        this.addInspectorFormGroup.get('userIdControlHidden')!.disable();
    }

    public deleteInspector(inspectorId: number): void {
        this.service.deleteInspector(inspectorId).subscribe({
            next: () => {
                this.gridManager.refreshData();
            }
        });
    }

    public restoreInspector(inspectorId: number): void {
        this.service.undoDeleteInspector(inspectorId).subscribe({
            next: () => {
                this.gridManager.refreshData();
            }
        });
    }

    public tabChanged(event: MatTabChangeEvent): void {
        if (event.index === 1) {
            this.unregisteredInspectorsLoaded = true;
        }
    }

    private buildForm(): void {
        this.inspectorsFormGroup = new FormGroup({
            inspectorFirstNameControl: new FormControl(),
            inspectorLastNameControl: new FormControl(),
            userIdControl: new FormControl(),
            inspectorCardNumControl: new FormControl()
        });

        this.addInspectorFormGroup = new FormGroup({
            firstNameControl: new FormControl({ value: null, disabled: true }),
            lastNameControl: new FormControl({ value: null, disabled: true }),
            userIdControl: new FormControl(null, Validators.required),
            inspectorCardNumControl: new FormControl(null, [Validators.required, Validators.maxLength(5)]),
            inspectionSequenceNumControl: new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 0)])
        });
    }

    private mapFilters(filters: FilterEventArgs) {
        return new InspectorsFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            firstName: filters.getValue('inspectorFirstNameControl'),
            lastName: filters.getValue('inspectorLastNameControl'),
            userId: filters.getValue('userIdControl'),
            inspectionSequenceNum: filters.getValue('inspectionSequenceNumControl'),
            inspectorCardNum: filters.getValue('inspectorCardNumControl')
        });
    }

    private handleInspectorAlreadyExistsError(response: HttpErrorResponse): void {
        const error: ErrorModel | undefined = response.error;

        if (error?.code === ErrorCode.InspectorAlreadyExists) {
            this.snackbar.open(this.translate.getValue('inspectors-register.inspector-already-exists-error'), undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
            });
        }
    }
}
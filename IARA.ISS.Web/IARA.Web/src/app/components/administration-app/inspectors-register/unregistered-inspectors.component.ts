import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { IInspectorsService } from '@app/interfaces/administration-app/inspectors.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { InspectorsRegisterDTO } from '@app/models/generated/dtos/InspectorsRegisterDTO';
import { UnregisteredPersonEditDTO } from '@app/models/generated/dtos/UnregisteredPersonEditDTO';
import { InspectorsFilters } from '@app/models/generated/filters/InspectorsFilters';
import { InspectorsService } from '@app/services/administration-app/inspectors.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { CommandTypes } from '@app/shared/components/data-table/enums/command-type.enum';
import { IRemoteTLDatatableComponent } from '@app/shared/components/data-table/interfaces/tl-remote-datatable.interface';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { IdentifierTypeEnum } from '@app/enums/identifier-type.enum';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { TLValidators } from '@app/shared/utils/tl-validators';

@Component({
    selector: 'unregistered-inspectors',
    templateUrl: './unregistered-inspectors.component.html'
})
export class UnregisteredInspectorsComponent implements AfterViewInit {
    public translate: FuseTranslationLoaderService;
    public unregInspectorsFormGroup!: FormGroup;
    public addInspectorFormGroup!: FormGroup;
    public readOnly: boolean = false;
    public institutions: NomenclatureDTO<number>[] = [];
    public model!: UnregisteredPersonEditDTO;

    public identifierTypes: NomenclatureDTO<string>[] = [];

    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;
    public readonly canDeleteRecords: boolean;
    public readonly canRestoreRecords: boolean;

    @ViewChild(TLDataTableComponent)
    private datatable!: IRemoteTLDatatableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private service: IInspectorsService;
    private readonly commonNomenclaturesService: CommonNomenclatures;
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

        this.identifierTypes = [
            new NomenclatureDTO<string>({
                value: IdentifierTypeEnum[IdentifierTypeEnum.EGN],
                displayName: this.translate.getValue('inspectors-register.egn'),
                isActive: true
            }),
            new NomenclatureDTO<string>({
                value: IdentifierTypeEnum[IdentifierTypeEnum.LNC],
                displayName: this.translate.getValue('inspectors-register.lnc'),
                isActive: true
            }),
            new NomenclatureDTO<string>({
                value: IdentifierTypeEnum[IdentifierTypeEnum.FORID],
                displayName: this.translate.getValue('inspectors-register.forid'),
                isActive: true
            })
        ];

        this.buildForm();
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Institutions, this.commonNomenclaturesService.getInstitutions.bind(this.commonNomenclaturesService), false
        ).subscribe((result: NomenclatureDTO<number>[]) => {
            this.institutions = result;
        });
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<InspectorsRegisterDTO, InspectorsFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllUnregistered.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        this.gridManager.refreshData();
    }

    public inspectorRecordChanged(event: RecordChangedEventArgs<InspectorsRegisterDTO>): void {
        const inspector: UnregisteredPersonEditDTO = this.buildUnregisteredInspector(event.Record);

        switch (event.Command) {
            case CommandTypes.Add: {
                this.service.addUnregisteredInspector(inspector).subscribe({
                    next: () => {
                        this.gridManager.refreshData();
                    },
                    error: (response: HttpErrorResponse) => {
                        this.handleInspectorAlreadyExistsError(response);
                    }
                });
            } break;
            case CommandTypes.Edit: {
                this.service.editUnregisteredInspector(inspector).subscribe({
                    next: () => {
                        this.gridManager.refreshData();
                    },
                    error: (response: HttpErrorResponse) => {
                        this.handleInspectorAlreadyExistsError(response);
                    }
                });
            } break;
            case CommandTypes.Delete: {
                this.deleteInspector(inspector.id!);
            } break;
            case CommandTypes.UndoDelete: {
                this.restoreInspector(inspector.id!);
            } break;
        }
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

    private buildForm(): void {
        this.unregInspectorsFormGroup = new FormGroup({
            unregPersonNameControl: new FormControl(),
            egnLncControl: new FormControl(),
            institutionIdControl: new FormControl(),
            inspectorCardNumControl: new FormControl()
        });

        this.addInspectorFormGroup = new FormGroup({
            egnLncControl: new FormControl(null, [Validators.required, Validators.maxLength(20), TLValidators.number(0)]),
            identifierTypeControl: new FormControl(null, Validators.required),
            firstNameControl: new FormControl(null, [Validators.required, Validators.maxLength(200)]),
            lastNameControl: new FormControl(null, [Validators.required, Validators.maxLength(200)]),
            inspectorCardNumControl: new FormControl(null, Validators.maxLength(5)),
            institutionIdControl: new FormControl(null, Validators.required),
            commentsControl: new FormControl(null, Validators.maxLength(4000))
        });
    }

    private mapFilters(filters: FilterEventArgs) {
        return new InspectorsFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            unregPersonName: filters.getValue('unregPersonNameControl'),
            egnLnc: filters.getValue('egnLncControl'),
            institutionId: filters.getValue('institutionIdControl'),
            inspectorCardNum: filters.getValue('inspectorCardNumControl')
        });
    }

    private buildUnregisteredInspector(inspector: InspectorsRegisterDTO): UnregisteredPersonEditDTO {
        const result = new UnregisteredPersonEditDTO({
            id: inspector.id,
            identifierType: IdentifierTypeEnum[inspector.identifierType as keyof typeof IdentifierTypeEnum],
            egnLnc: inspector.egnLnc,
            firstName: inspector.firstName,
            lastName: inspector.lastName,
            inspectorCardNum: inspector.inspectorCardNum,
            institutionId: inspector.institutionId,
            comments: inspector.comments
        });

        return result;
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
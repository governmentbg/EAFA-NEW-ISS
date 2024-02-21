import { AfterViewInit, Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { IPenalPointsService } from '@app/interfaces/administration-app/penal-points.interface';
import { PenalPointsDTO } from '@app/models/generated/dtos/PenalPointsDTO';
import { PenalPointsFilters } from '@app/models/generated/filters/PenalPointsFilters';
import { PenalPointsService } from '@app/services/administration-app/penal-points.service';
import { PenalPointsEditDTO } from '@app/models/generated/dtos/PenalPointsEditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PointsTypeEnum } from '@app/enums/points-type.enum';
import { EditPenalPointsComponent } from '@app/components/administration-app/control-activity/awarded-points/edit-penal-points/edit-penal-points.component';
import { EditPenalPointsDecreePickerComponent } from '@app/components/administration-app/control-activity/awarded-points/edit-penal-points-decree-picker/edit-penal-points-decree-picker.component';
import { EditPenalPointsDialogParams } from '@app/components/administration-app/control-activity/awarded-points/models/edit-penal-points-dialog-params.model';

@Component({
    selector: 'penal-points',
    templateUrl: './penal-points.component.html'
})
export class PenalPointsComponent implements OnInit, AfterViewInit, OnChanges {
    @Input()
    public shipId: number | undefined;

    @Input()
    public penalDecreeId: number | undefined;

    @Input()
    public reloadData: boolean = false;

    @Input()
    public recordsPerPage: number = 20;

    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public ships: NomenclatureDTO<number>[] = [];
    public orderTypes: NomenclatureDTO<boolean>[] = [];
    public pointsTypes: NomenclatureDTO<PointsTypeEnum>[] = [];

    public readonly canReadRecords: boolean;
    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;
    public readonly canDeleteRecords: boolean;
    public readonly canRestoreRecords: boolean;

    public openedFromMenu: boolean = false;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private grid!: DataTableManager<PenalPointsDTO, PenalPointsFilters>;

    private readonly service: IPenalPointsService;
    private readonly nomenclatures: CommonNomenclatures;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly editDialog: TLMatDialog<EditPenalPointsComponent>;
    private readonly decreePickerDialog: TLMatDialog<EditPenalPointsDecreePickerComponent>;

    public constructor(
        service: PenalPointsService,
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editDialog: TLMatDialog<EditPenalPointsComponent>,
        decreePickerDialog: TLMatDialog<EditPenalPointsDecreePickerComponent>,
        permissions: PermissionsService
    ) {
        this.service = service;
        this.nomenclatures = nomenclatures;
        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.editDialog = editDialog;
        this.decreePickerDialog = decreePickerDialog;

        this.canReadRecords = permissions.hasAny(PermissionsEnum.AwardedPointsRead, PermissionsEnum.AwardedPointsReadAll);
        this.canAddRecords = permissions.has(PermissionsEnum.AwardedPointsAddRecords);
        this.canEditRecords = permissions.has(PermissionsEnum.AwardedPointsEditRecords);
        this.canDeleteRecords = permissions.has(PermissionsEnum.AwardedPointsDeleteRecords);
        this.canRestoreRecords = permissions.has(PermissionsEnum.AwardedPointsRestoreRecords);

        this.buildForm();

        this.orderTypes = [
            new NomenclatureDTO<boolean>({
                value: true,
                displayName: this.translate.getValue('penal-points.increase-points'),
                isActive: true
            }),
            new NomenclatureDTO<boolean>({
                value: false,
                displayName: this.translate.getValue('penal-points.decrease-points'),
                isActive: true
            })
        ];

        this.pointsTypes = [
            new NomenclatureDTO<PointsTypeEnum>({
                value: PointsTypeEnum.PermitOwner,
                displayName: this.translate.getValue('penal-points.points-type-permit-owner'),
                isActive: true
            }),
            new NomenclatureDTO<PointsTypeEnum>({
                value: PointsTypeEnum.QualifiedFisher,
                displayName: this.translate.getValue('penal-points.points-type-qualified-fisher'),
                isActive: true
            })
        ];
    }

    public ngOnInit(): void {
        this.openedFromMenu = (this.shipId === null || this.shipId === undefined) && (this.penalDecreeId === null || this.penalDecreeId === undefined);

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.ships = result;
            }
        });
    }

    public ngAfterViewInit(): void {
        this.grid = new DataTableManager<PenalPointsDTO, PenalPointsFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.openedFromMenu ? this.searchpanel : undefined,
            requestServiceMethod: this.service.getAllPenalPoints.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        const isPerson: boolean | undefined = window.history.state?.isPerson;
        let legalId: number | undefined;
        let personId: number | undefined;
        if (isPerson === true) {
            personId = window.history.state?.id;
        }

        if (isPerson === false) {
            legalId = window.history.state?.id;
        }

        this.grid.advancedFilters = new PenalPointsFilters({
            shipId: this.shipId ?? undefined,
            penalDecreeId: this.penalDecreeId ?? undefined,
            personId: personId ?? undefined,
            legalId: legalId ?? undefined
        });

        if ((this.shipId === null || this.shipId === undefined)) {
            this.grid.refreshData();
        }
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const reloadData: boolean | undefined = changes['reloadData']?.currentValue;

        if (reloadData === true) {
            this.grid?.refreshData();
        }
    }

    public addEditPenalPoints(points: PenalPointsDTO | undefined, viewMode: boolean): void {
        if (points !== undefined && points !== null) {
            const data: EditPenalPointsDialogParams = new EditPenalPointsDialogParams({
                id: points.id,
                penalDecreeId: points.penalDecreeId,
                type: points.pointsType,
                isReadonly: viewMode
            });

            const auditBtn: IHeaderAuditButton = {
                id: points.id!,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'PenalPointsRegister'
            };

            const title: string = viewMode
                ? this.translate.getValue('penal-points.view-penal-points-dialog-title')
                : this.translate.getValue('penal-points.edit-penal-points-dialog-title');

            const dialog = this.editDialog.openWithTwoButtons({
                title: title,
                TCtor: EditPenalPointsComponent,
                headerAuditButton: auditBtn,
                headerCancelButton: {
                    cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
                },
                componentData: data,
                translteService: this.translate,
                disableDialogClose: true,
                viewMode: viewMode
            }, '1400px');

            dialog.subscribe({
                next: (entry: PenalPointsEditDTO | undefined) => {
                    if (entry !== undefined && entry !== null) {
                        this.grid.refreshData();
                    }
                }
            });
        }
        else {
            const title: string = this.translate.getValue('penal-points.choose-penal-decree-dialog-title');

            const dialog = this.decreePickerDialog.openWithTwoButtons({
                title: title,
                TCtor: EditPenalPointsDecreePickerComponent,
                headerAuditButton: undefined,
                headerCancelButton: {
                    cancelBtnClicked: this.closeChooseDecreeDialogBtnClicked.bind(this)
                },
                componentData: undefined,
                translteService: this.translate,
                disableDialogClose: true,
                viewMode: viewMode
            }, '600px');

            dialog.subscribe({
                next: (entry: PenalPointsEditDTO | undefined) => {
                    if (entry !== undefined && entry !== null) {
                        this.grid.refreshData();
                    }
                }
            });
        }
    }

    public deletePenalPoints(points: PenalPointsDTO): void {
        this.confirmDialog.open({
            title: this.translate.getValue('penal-points.delete-penal-points-dialog-title'),
            message: this.translate.getValue('penal-points.delete-penal-points-dialog-message'),
            okBtnLabel: this.translate.getValue('penal-points.delete-penal-points-dialog-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok === true) {
                    this.service.deletePenalPoints(points.id!).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
            }
        });
    }

    public restorePenalPoints(points: PenalPointsDTO): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok === true) {
                    this.service.undoDeletePenalPoints(points.id!).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
            }
        });
    }

    private buildForm(): void {
        this.form = new FormGroup({
            decreeNumControl: new FormControl(),
            decreeDateRangeControl: new FormControl(),
            orderNumControl: new FormControl(),
            orderDateRangeControl: new FormControl(),
            pointsOrderTypeControl: new FormControl(),
            pointsTypeControl: new FormControl(),
            permitNumControl: new FormControl(),
            permitLicenseNumControl: new FormControl(),
            shipNameControl: new FormControl(),
            shipCfrControl: new FormControl(),
            shipExternalMarkingControl: new FormControl(),
            shipRegistrationCertificateNumControl: new FormControl(),
            permitOwnerNameControl: new FormControl(),
            permitOwnerIdentifierControl: new FormControl(),
            captainNameControl: new FormControl(),
            captainIdentifierControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): PenalPointsFilters {
        const result = new PenalPointsFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            penalDecreeNum: filters.getValue('decreeNumControl'),
            penalDecreeDateFrom: filters.getValue<DateRangeData>('decreeDateRangeControl')?.start,
            penalDecreeDateTo: filters.getValue<DateRangeData>('decreeDateRangeControl')?.end,
            decreeNum: filters.getValue('orderNumControl'),
            decreeDateFrom: filters.getValue<DateRangeData>('orderDateRangeControl')?.start,
            decreeDateTo: filters.getValue<DateRangeData>('orderDateRangeControl')?.end,
            pointsType: filters.getValue<PointsTypeEnum>('pointsTypeControl'),
            isIncreasePoints: filters.getValue<boolean>('pointsOrderTypeControl'),
            permitNum: filters.getValue('permitNumControl'),
            permitLicenseNum: filters.getValue('permitLicenseNumControl'),
            shipName: filters.getValue('shipNameControl'),
            shipCfr: filters.getValue('shipCfrControl'),
            shipExternalMarking: filters.getValue('shipExternalMarkingControl'),
            shipRegistrationCertificateNumber: filters.getValue('shipRegistrationCertificateNumControl'),
            permitOwnerName: filters.getValue('permitOwnerNameControl'),
            permitOwnerIdentifier: filters.getValue('permitOwnerIdentifierControl'),
            captainName: filters.getValue('captainNameControl'),
            captainIdentifier: filters.getValue('captainIdentifierControl')
        });

        return result;
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeChooseDecreeDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}

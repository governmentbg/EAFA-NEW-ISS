import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { forkJoin } from 'rxjs';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { PenalPointsAppealDTO } from '@app/models/generated/dtos/PenalPointsAppealDTO';
import { PenalPointsEditDTO } from '@app/models/generated/dtos/PenalPointsEditDTO';
import { PenalPointsAuanDecreeDataDTO } from '@app/models/generated/dtos/PenalPointsAuanDecreeDataDTO';
import { IPenalPointsService } from '@app/interfaces/administration-app/penal-points.interface';
import { PenalPointsService } from '@app/services/administration-app/penal-points.service';
import { EditPenalPointsDialogParams } from '../models/edit-penal-points-dialog-params.model';
import { EditPenalPointsComplaintStatusComponent } from '../edit-penal-points-complaint-status/edit-penal-points-complaint-status.component';
import { EditPenalPointsComplaintDialogParams } from '../models/edit-penal-points-complaint-dialog-params.model';
import { PointsTypeEnum } from '@app/enums/points-type.enum';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { PenalPointsOrderDTO } from '@app/models/generated/dtos/PenalPointsOrderDTO';
import { PermitNomenclatureDTO } from '@app/models/generated/dtos/PermitNomenclatureDTO';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';
import { ShipsUtils } from '@app/shared/utils/ships.utils';

@Component({
    selector: 'edit-penal-points',
    templateUrl: './edit-penal-points.component.html'
})
export class EditPenalPointsComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form!: FormGroup;

    public readonly service!: IPenalPointsService;
    public readonly pointsTypes: typeof PointsTypeEnum = PointsTypeEnum;

    public territoryUnits: NomenclatureDTO<number>[] = [];
    public ships: ShipNomenclatureDTO[] = [];
    public captains: NomenclatureDTO<number>[] = [];
    public orderTypes: NomenclatureDTO<boolean>[] = [];
    public permits: PermitNomenclatureDTO[] = [];
    public permitLicenses: NomenclatureDTO<number>[] = [];

    public isAdding: boolean = false;
    public viewMode: boolean = false;
    public type!: PointsTypeEnum;
    public isPermitOwnerPerson: boolean = false;
    public isQualifiedFisher: boolean = false;
    public noShipSelected: boolean = true;
    public shipDataTitle: string | undefined;
    public complaintTitle: string | undefined;
    public isPermitOwnerLabel: string | undefined;

    public complaintStatuses: PenalPointsAppealDTO[] = [];
    public pointsOrders: PenalPointsOrderDTO[] = [];

    @ViewChild('complaintStatusesTable')
    private complaintStatusesTable!: TLDataTableComponent;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private decreeId!: number;
    private pointsId: number | undefined;
    private model!: PenalPointsEditDTO;

    private fisherId: number | undefined;
    private permitOwnerPersonId: number | undefined;
    private permitOwnerLegalId: number | undefined;
    private pointsTotalCount: number | undefined;

    private readonly nomenclatures: CommonNomenclatures;
    private readonly translate: FuseTranslationLoaderService;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly editComplaintDialog: TLMatDialog<EditPenalPointsComplaintStatusComponent>;

    public constructor(
        service: PenalPointsService,
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editComplaintDialog: TLMatDialog<EditPenalPointsComplaintStatusComponent>
    ) {
        this.service = service;
        this.nomenclatures = nomenclatures;
        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.editComplaintDialog = editComplaintDialog;

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
    }

    public async ngOnInit(): Promise<void> {
        this.isAdding = this.pointsId === undefined || this.pointsId === null;

        const nomenclatures: (NomenclatureDTO<number>)[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false)
        ).toPromise();

        this.territoryUnits = nomenclatures[0];
        this.ships = nomenclatures[1];

        if (this.type === PointsTypeEnum.PermitOwner) {
            this.shipDataTitle = this.translate.getValue('penal-points.edit-ship-data');
            this.complaintTitle = this.translate.getValue('penal-points.edit-ship-complaint-data');
            this.isPermitOwnerLabel = this.translate.getValue('penal-points.edit-is-permit-owner');
        }
        else {
            this.shipDataTitle = this.translate.getValue('penal-points.edit-captain-data');
            this.complaintTitle = this.translate.getValue('penal-points.edit-captain-complaint-data');
            this.isPermitOwnerLabel = this.translate.getValue('penal-points.edit-is-captain');
        }

        this.service.getPenalPointsAuanDecreeData(this.decreeId).subscribe({
            next: (data: PenalPointsAuanDecreeDataDTO) => {

                this.fillAuanDecreeData(data);
                if (this.pointsId === undefined || this.pointsId === null) {
                    this.model = new PenalPointsEditDTO();
                }
                else {
                    this.service.getPenalPoints(this.pointsId).subscribe({
                        next: (points: PenalPointsEditDTO) => {
                            this.model = points;
                            this.fillForm();
                        }
                    });
                }
            }
        });

        this.form.get('shipControl')!.valueChanges.subscribe({
            next: (ship: ShipNomenclatureDTO | undefined | string) => {
                if (ship !== null && ship !== undefined) {
                    const shipId: number | undefined = (ship as ShipNomenclatureDTO).value;

                    if (shipId !== undefined && shipId !== null) {
                        this.noShipSelected = false;

                        if (this.type === PointsTypeEnum.PermitOwner) {
                            this.getShipPermitsNomenclature(shipId);
                        }
                        else {
                            this.getShipPermitLicensesNomenclature(shipId);
                        }
                    }
                }
                else {
                    if (this.type === PointsTypeEnum.PermitOwner) {
                        this.permits = [];
                        this.form.get('permitControl')!.reset();
                        this.form.get('permitControl')!.updateValueAndValidity();
                    }
                    else {
                        this.permitLicenses = [];
                        this.form.get('permitLicenseControl')!.reset();
                        this.form.get('permitLicenseControl')!.updateValueAndValidity();
                    }
                    this.noShipSelected = true;
                }
            }
        });
    }

    public ngAfterViewInit(): void {
        if (this.type === PointsTypeEnum.PermitOwner) {
            this.form.get('permitControl')!.setValidators(Validators.required);
            this.form.get('permitControl')!.updateValueAndValidity();
            this.form.get('permitControl')!.markAsPending();
        }
        else {
            this.form.get('permitLicenseControl')!.setValidators(Validators.required);
            this.form.get('permitLicenseControl')!.updateValueAndValidity();
            this.form.get('permitLicenseControl')!.markAsPending();
        }

        this.form.controls.permitControl.valueChanges.subscribe({
            next: (permit: PermitNomenclatureDTO | undefined) => {
                this.pointsOrders = [];
                this.form.get('pointsTotalCountControl')!.reset();
                this.form.get('pointsTotalCountControl')!.updateValueAndValidity();

                if (permit !== undefined && permit !== null && typeof permit !== 'string') {
                    this.permitOwnerPersonId = permit.shipOwnerPersonId;
                    this.permitOwnerLegalId = permit.shipOwnerLegalId;
                    this.isQualifiedFisher = false;
                    this.isPermitOwnerPerson = this.permitOwnerPersonId !== undefined && this.permitOwnerPersonId !== null;

                    if (this.isPermitOwnerPerson) {
                        this.getPermitOrders(permit.shipOwnerPersonId!, this.isQualifiedFisher, this.isPermitOwnerPerson);
                    }
                    else {
                        this.getPermitOrders(permit.shipOwnerLegalId!, this.isQualifiedFisher, this.isPermitOwnerPerson);
                    }
                }
            }
        });

        this.form.controls.permitLicenseControl.valueChanges.subscribe({
            next: (permit: PermitNomenclatureDTO | undefined) => {
                this.pointsOrders = [];
                this.form.get('pointsTotalCountControl')!.reset();
                this.form.get('pointsTotalCountControl')!.updateValueAndValidity();

                if (permit !== undefined && permit !== null && typeof permit !== 'string') {
                    this.fisherId = permit.captainId;
                    this.isQualifiedFisher = true;

                    this.getPermitOrders(permit.captainId!, this.isQualifiedFisher, this.isPermitOwnerPerson);
                }
            }
        });
    }

    public setData(data: EditPenalPointsDialogParams | undefined, wrapperData: DialogWrapperData): void {
        if (data !== undefined && data !== null) {
            this.decreeId = data.penalDecreeId;
            this.pointsId = data.id;
            this.type = data.type;
            this.viewMode = data.isReadonly ?? false;
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.viewMode) {
            dialogClose();
        }

        this.form.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            if (this.pointsId !== undefined && this.pointsId !== null) {
                this.service.editPenalPoints(this.model).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    }
                });
            }
            else {
                this.service.addPenalPoints(this.model).subscribe({
                    next: (id: number) => {
                        this.model.id = id;
                        dialogClose(this.model);
                    }
                });
            }
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public addEditComplaint(complaint: PenalPointsAppealDTO | undefined, viewMode: boolean = false): void {
        let data: EditPenalPointsComplaintDialogParams | undefined;
        let auditBtn: IHeaderAuditButton | undefined;
        let title: string;

        if (complaint !== undefined) {
            data = new EditPenalPointsComplaintDialogParams({
                model: complaint,
                viewMode: this.viewMode || viewMode,
                service: this.service
            });

            if (complaint.id !== undefined) {
                auditBtn = {
                    id: complaint.id,
                    getAuditRecordData: this.service.getPenalPointsStatusAudit.bind(this.service),
                    tableName: 'PenalPointComplaintStatus'
                };
            }

            if (this.viewMode || viewMode) {
                title = this.translate.getValue('penal-points.view-penal-points-complaint-dialog-title');
            }
            else {
                title = this.translate.getValue('penal-points.edit-penal-points-complaint-dialog-title');
            }
        }
        else {
            data = new EditPenalPointsComplaintDialogParams({
                model: undefined,
                viewMode: false,
                service: this.service
            });

            title = this.translate.getValue('penal-points.add-penal-points-complaint-dialog-title');
        }
        const dialog = this.editComplaintDialog.openWithTwoButtons({
            title: title,
            TCtor: EditPenalPointsComplaintStatusComponent,
            headerAuditButton: auditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditComplaintDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            viewMode: viewMode
        }, '1200px');

        dialog.subscribe({
            next: (result: PenalPointsAppealDTO | undefined) => {
                if (result !== undefined) {
                    if (complaint !== undefined) {
                        complaint = result;
                    }
                    else {
                        this.complaintStatuses.push(result);
                    }

                    this.complaintStatuses = this.complaintStatuses.slice();
                }
            }
        })
    }

    public deleteComplaint(complaint: GridRow<PenalPointsAppealDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('penal-points.delete-penal-points-complaint-dialog-title'),
            message: this.translate.getValue('penal-points.delete-penal-points-complaint-dialog-message'),
            okBtnLabel: this.translate.getValue('penal-points.delete-penal-points-complaint-dialog-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.complaintStatusesTable.softDelete(complaint);
                }
            }
        })
    }

    public undoDeleteComplaint(complaint: GridRow<PenalPointsAppealDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.complaintStatusesTable.softUndoDelete(complaint);
                }
            }
        })
    }

    public downloadedPersonData(person: PersonFullDataDTO): void {
        this.form.get('personControl')!.setValue(person.person);
    }

    private closeEditComplaintDialogBtnClicked(closeeFn: HeaderCloseFunction): void {
        closeeFn();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            issuerControl: new FormControl(null, Validators.maxLength(500)),
            territoryUnitControl: new FormControl(null),
            reportNoteNumControl: new FormControl(null, Validators.maxLength(50)),
            reportNoteDateControl: new FormControl(null),

            auanNumControl: new FormControl(null),
            auanDateControl: new FormControl(null),
            decreeNumControl: new FormControl(null),
            decreeIssueDateControl: new FormControl(null),
            decreeEffectiveDateControl: new FormControl(null),
            auanInspectedEntityControl: new FormControl(null),

            permitControl: new FormControl(null),
            permitLicenseControl: new FormControl(null),
            isPermitOwnerControl: new FormControl(true),
            personControl: new FormControl(null),

            shipControl: new FormControl(null),
            orderTypeControl: new FormControl(null, Validators.required),
            orderNumControl: new FormControl(null, [Validators.required, Validators.maxLength(20)]),
            issueDateControl: new FormControl(null, Validators.required),
            effectiveDateControl: new FormControl(null),
            deliveryDateControl: new FormControl(null),
            pointsAmountControl: new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 0)]),
            pointsTotalCountControl: new FormControl(null),

            captainControl: new FormControl(null),

            commentsControl: new FormControl(null, Validators.maxLength(4000)),

            filesControl: new FormControl(null)
        });
    }

    private fillForm(): void {
        this.form.get('reportNoteNumControl')!.setValue(this.model.reportNoteNum);
        this.form.get('reportNoteDateControl')!.setValue(this.model.reportNoteDate);
        this.form.get('permitControl')!.setValue(this.permits.find(x => x.value === this.model.permitId));
        this.form.get('permitLicenseControl')!.setValue(this.permitLicenses.find(x => x.value === this.model.permitLicenseId));
        this.form.get('isPermitOwnerControl')!.setValue(this.model.isPermitOwner);
        this.form.get('orderNumControl')!.setValue(this.model.decreeNum);
        this.form.get('issuerControl')!.setValue(this.model.issuer);
        this.form.get('issueDateControl')!.setValue(this.model.issueDate);
        this.form.get('deliveryDateControl')!.setValue(this.model.deliveryDate);
        this.form.get('effectiveDateControl')!.setValue(this.model.effectiveDate);
        this.form.get('pointsAmountControl')!.setValue(this.model.pointsAmount);
        this.form.get('commentsControl')!.setValue(this.model.comments);
        this.form.get('filesControl')!.setValue(this.model.files);

        if (this.model.shipId !== undefined && this.model.shipId !== null) {
            const selectedShip: ShipNomenclatureDTO = ShipsUtils.get(this.ships, this.model.shipId);
            this.form.get('shipControl')!.setValue(selectedShip);
        }

        if (this.model.isPermitOwner === false) {
            this.form.get('personControl')!.setValue(this.model.personOwner);
        }

        const isIncreasePoints: boolean = this.model.isIncreasePoints ?? true;
        const orderType: NomenclatureDTO<boolean> = this.orderTypes.find(x => x.value === isIncreasePoints)!;
        this.form.get('orderTypeControl')!.setValue(orderType);

        setTimeout(() => {
            this.complaintStatuses = this.model.appealStatuses ?? [];
        });

        if (this.viewMode) {
            this.form.disable();
        }
    }

    private fillModel(): void {
        this.model.decreeId = this.decreeId;
        this.model.pointsType = this.type;

        this.model.reportNoteNum = this.form.get('reportNoteNumControl')!.value;
        this.model.reportNoteDate = this.form.get('reportNoteDateControl')!.value;

        this.model.isIncreasePoints = (this.form.get('orderTypeControl')!.value as NomenclatureDTO<boolean>)?.value;

        this.model.shipId = this.form.get('shipControl')!.value?.value;
        this.model.permitId = this.form.get('permitControl')!.value?.value;
        this.model.permitLicenseId = this.form.get('permitLicenseControl')!.value?.value;
        this.model.decreeNum = this.form.get('orderNumControl')!.value;
        this.model.issuer = this.form.get('issuerControl')!.value;
        this.model.issueDate = this.form.get('issueDateControl')!.value;
        this.model.effectiveDate = this.form.get('effectiveDateControl')!.value;
        this.model.deliveryDate = this.form.get('deliveryDateControl')!.value;
        this.model.pointsAmount = this.form.get('pointsAmountControl')!.value;
        this.model.comments = this.form.get('commentsControl')!.value;

        this.model.permitOwnerPersonId = this.permitOwnerPersonId;
        this.model.permitOwnerLegalId = this.permitOwnerLegalId;

        this.model.qualifiedFisherId = this.fisherId;
        this.model.isPermitOwner = this.form.get('isPermitOwnerControl')!.value ?? true;

        if (this.model.isPermitOwner === false) {
            this.model.personOwner = this.form.get('personControl')!.value;
        }

        this.model.files = this.form.get('filesControl')!.value;

        this.model.appealStatuses = this.getComplaintStatusesFromTable();
    }

    private fillAuanDecreeData(data: PenalPointsAuanDecreeDataDTO): void {
        this.form.get('territoryUnitControl')!.setValue(this.territoryUnits.find(x => x.value === data.territoryUnitId));
        this.form.get('auanNumControl')!.setValue(data.auanNum);
        this.form.get('auanDateControl')!.setValue(data.auanDate);
        this.form.get('decreeNumControl')!.setValue(data.decreeNum);
        this.form.get('decreeIssueDateControl')!.setValue(data.decreeIssueDate);
        this.form.get('decreeEffectiveDateControl')!.setValue(data.decreeEffectiveDate);
        this.form.get('auanInspectedEntityControl')!.setValue(data.inspectedEntity);

        if (data.shipId !== undefined && data.shipId !== null) {
            const selectedShip: ShipNomenclatureDTO = ShipsUtils.get(this.ships, data.shipId);
            this.form.get('shipControl')!.setValue(selectedShip);
        }
    }

    private getComplaintStatusesFromTable(): PenalPointsAppealDTO[] {
        if (!this.isAdding) {
            const rows = this.complaintStatusesTable.rows as PenalPointsAppealDTO[];

            return rows.map(x => new PenalPointsAppealDTO({
                id: x.id,
                courtId: x.courtId,
                statusId: x.statusId,
                appealNum: x.appealNum,
                appealDate: x.appealDate,
                decreeNum: x.decreeNum,
                decreeDate: x.decreeDate,
                dateOfChange: x.dateOfChange,
                details: x.details,
                isActive: x.isActive
            }));
        }
        return [];
    }

    private getShipPermitsNomenclature(shipId: number): void {
        this.service.getPermitNomenclatures(shipId, false).subscribe({
            next: (values: PermitNomenclatureDTO[]) => {
                this.permits = values;
                const permitId: number | undefined = this.model?.permitId;
                if (permitId !== null && permitId !== undefined) {
                    this.form.get('permitControl')!.setValue(this.permits.find(x => x.value === permitId));
                }
            }
        });
    }

    private getShipPermitLicensesNomenclature(shipId: number): void {
        this.service.getPermitLicensesNomenclatures(shipId).subscribe({
            next: (values: PermitNomenclatureDTO[]) => {
                this.permitLicenses = values;
                const permitLicenseId: number | undefined = this.model?.permitLicenseId;
                if (permitLicenseId !== null && permitLicenseId !== undefined) {
                    this.form.get('permitLicenseControl')!.setValue(this.permitLicenses.find(x => x.value === permitLicenseId));
                }
            }
        });
    }

    private getPermitOrders(orderId: number, isFisher: boolean, isPermitOwnerPerson: boolean): void {
        this.service.getPermitOrders(orderId, isFisher, isPermitOwnerPerson).subscribe({
            next: (values: PenalPointsOrderDTO[]) => {
                this.pointsOrders = values;

                const pointsToAdd: number[] = values.filter(x => x.isIncreasePoints).map(x => x.pointsAmount!);
                const pointsToDelete: number[] = values.filter(x => !x.isIncreasePoints).map(x => x.pointsAmount!)

                this.pointsTotalCount = this.sum(pointsToAdd) - this.sum(pointsToDelete);
                this.form.get('pointsTotalCountControl')?.setValue(this.pointsTotalCount);
            }
        });
    }

    private sum(nums: number[]): number {
        return nums.reduce((sum: number, current: number) => { return sum + current; }, 0);
    }
}
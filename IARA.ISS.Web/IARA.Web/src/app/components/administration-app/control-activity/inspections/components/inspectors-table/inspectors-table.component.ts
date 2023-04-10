import { Component, EventEmitter, Input, OnInit, Output, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ValidationErrors, ValidatorFn } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { InspectorTableParams } from './models/inspector-table-params';
import { EditInspectorComponent } from '../../dialogs/edit-inspector/edit-inspector.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { InspectorTableModel } from '../../models/inspector-table-model';
import { InspectorDuringInspectionDTO } from '@app/models/generated/dtos/InspectorDuringInspectionDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { AuthService } from '@app/shared/services/auth.service';
import { InspectionsService } from '@app/services/administration-app/inspections.service';

@Component({
    selector: 'inspectors-table',
    templateUrl: './inspectors-table.component.html'
})
export class InspectorsTableComponent extends CustomFormControl<InspectorDuringInspectionDTO[]> implements OnInit {

    @Input()
    public institutions: NomenclatureDTO<number>[] = [];

    @Output()
    public headInspectorChanged = new EventEmitter<string[]>();

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    public inspectors: InspectorTableModel[] = [];

    private readonly translate: FuseTranslationLoaderService;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly editEntryDialog: TLMatDialog<EditInspectorComponent>;
    private readonly authService: AuthService;
    private readonly service: InspectionsService;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editEntryDialog: TLMatDialog<EditInspectorComponent>,
        authService: AuthService,
        service: InspectionsService
    ) {
        super(ngControl);

        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.editEntryDialog = editEntryDialog;
        this.authService = authService;
        this.service = service;

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.control.updateValueAndValidity();
            }
        });
    }

    public async ngOnInit(): Promise<void> {
        this.initCustomFormControl();
    }

    public writeValue(value: InspectorDuringInspectionDTO[]): void {
        if (value !== undefined && value !== null && value.length !== 0) {
            const currentUserId: number | undefined = this.authService.userRegistrationInfo?.id;

            const inspectors = value.map(f => new InspectorTableModel({
                address: f.address,
                cardNum: f.cardNum,
                territoryCode: f.territoryCode,
                citizenshipId: f.citizenshipId,
                comment: f.comment,
                egnLnc: f.egnLnc,
                eik: f.eik,
                firstName: f.firstName,
                hasBulgarianAddressRegistration: f.hasBulgarianAddressRegistration,
                hasIdentifiedHimself: f.hasIdentifiedHimself,
                inspectorId: f.inspectorId,
                isLegal: f.isLegal,
                isNotRegistered: f.isNotRegistered,
                lastName: f.lastName,
                middleName: f.middleName,
                unregisteredPersonId: f.unregisteredPersonId,
                userId: f.userId,
                institutionId: f.institutionId,
                id: f.id,
                institution: this.institutions.find(s => s.value === f.institutionId)?.displayName,
                isInCharge: f.isInCharge,
                isCurrentUser: f.userId === currentUserId,
                isActive: f.isActive,
            }));

            setTimeout(() => {
                this.inspectors = inspectors;

                this.changeReportNumber(this.inspectors.find(f => f.isInCharge)!);
            });
        }
        else {
            setTimeout(() => {
                this.inspectors = this.inspectors.filter(f => f.isCurrentUser);

                if (this.inspectors.length === 0) {
                    return;
                }

                this.inspectors[0].isInCharge = true;
                this.changeReportNumber(this.inspectors[0]);
            });
        }
    }

    public addEditEntry(inspector?: InspectorTableModel, viewMode?: boolean): void {
        const readOnly: boolean = this.isDisabled || viewMode === true;

        let data: InspectorTableParams | undefined;
        let title: string;

        if (inspector !== undefined && inspector !== null) {
            data = new InspectorTableParams({
                model: inspector,
                readOnly: readOnly,
                isEdit: true,
                excludeIds: this.inspectors.map(f => f.inspectorId!),
            });

            if (readOnly) {
                title = this.translate.getValue('inspections.view-inspector-dialog-title');
            }
            else {
                title = this.translate.getValue('inspections.edit-inspector-dialog-title');
            }
        }
        else {
            data = new InspectorTableParams({
                excludeIds: this.inspectors.map(f => f.inspectorId!),
            });

            title = this.translate.getValue('inspections.add-inspector-dialog-title');
        }

        const dialog = this.editEntryDialog.openWithTwoButtons({
            title: title,
            TCtor: EditInspectorComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => closeFn()
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: !readOnly,
            viewMode: readOnly
        }, '800px');

        dialog.subscribe((result: InspectorTableModel) => {
            if (result !== undefined && result !== null) {
                if (result.isInCharge) {
                    for (const insp of this.inspectors) {
                        insp.isInCharge = false;
                    }

                    this.changeReportNumber(result);
                }

                if (inspector !== undefined) {
                    const index = this.inspectors.findIndex(f => f === inspector);
                    this.inspectors[index] = result;

                    result.isCurrentUser = inspector.isCurrentUser;
                }
                else {
                    this.inspectors.push(result);
                }

                if (!this.inspectors.find(f => f.isInCharge)) {
                    this.inspectors[0].isInCharge = true;
                    this.changeReportNumber(this.inspectors[0]);
                }

                this.inspectors = this.inspectors.slice();
                this.onChanged(this.getValue());
            }
        });
    }

    public deleteEntry(inspector: GridRow<InspectorTableModel>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('inspections.inspector-table-delete-dialog-title'),
            message: this.translate.getValue('inspections.inspector-table-delete-message'),
            okBtnLabel: this.translate.getValue('inspections.inspector-table-delete-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.datatable.softDelete(inspector);
                    this.inspectors.splice(this.inspectors.indexOf(inspector.data), 1);

                    if (this.inspectors.length > 0 && !this.inspectors.find(f => f.isInCharge)) {
                        this.inspectors[0].isInCharge = true;
                        this.changeReportNumber(this.inspectors[0]);
                    }

                    this.onChanged(this.getValue());
                }
            }
        });
    }

    protected buildForm(): AbstractControl {
        return new FormControl(undefined, this.inspectorsValidator());
    }

    protected getValue(): InspectorDuringInspectionDTO[] {
        return this.inspectors.map(f => new InspectorDuringInspectionDTO({
            address: f.address,
            cardNum: f.cardNum,
            citizenshipId: f.citizenshipId,
            comment: f.comment,
            egnLnc: f.egnLnc,
            eik: f.eik,
            firstName: f.firstName,
            hasBulgarianAddressRegistration: f.hasBulgarianAddressRegistration,
            hasIdentifiedHimself: f.hasIdentifiedHimself,
            id: f.id,
            inspectorId: f.inspectorId,
            institutionId: f.institutionId,
            isActive: f.isActive,
            isInCharge: f.isInCharge,
            isLegal: f.isLegal,
            isNotRegistered: f.isNotRegistered,
            lastName: f.lastName,
            middleName: f.middleName,
            unregisteredPersonId: f.unregisteredPersonId,
            userId: f.userId,
            territoryCode: f.territoryCode,
            institution: f.institution,
        }));
    }

    private changeReportNumber(inspector: InspectorTableModel): void {
        this.service.getNextReportNumber(inspector.userId!).subscribe({
            next: (value) => {
                this.headInspectorChanged.emit([inspector.territoryCode!, inspector.cardNum!, value!.num]);
            }
        })
    }

    private inspectorsValidator(): ValidatorFn {
        return (): ValidationErrors | null => {
            if (this.inspectors !== undefined && this.inspectors !== null) {
                const registeredInspectors = this.inspectors.filter(f => f.inspectorId != null);

                if (registeredInspectors.find((f, i) => registeredInspectors.find((s, i2) => i !== i2 && s.inspectorId === f.inspectorId))) {
                    return { 'inspectorsMatch': true };
                }
            }
            return null;
        };
    }
}
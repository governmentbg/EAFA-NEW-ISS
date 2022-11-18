import { Component, Input, OnChanges, OnInit, Self, SimpleChange, SimpleChanges, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, NgControl, ValidationErrors, ValidatorFn } from '@angular/forms';

import { FishingCapacityHolderDTO } from '@app/models/generated/dtos/FishingCapacityHolderDTO';
import { FishingCapacityHolderRegixDataDTO } from '@app/models/generated/dtos/FishingCapacityHolderRegixDataDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IFishingCapacityService } from '@app/interfaces/common-app/fishing-capacity.interface';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { TransferFishingCapacityTableEntryParams } from './models/transfer-fishing-capacity-table-entry-params.model';
import { TransferFishingCapacityTableEntryComponent } from './transfer-fishing-capacity-table-entry/transfer-fishing-capacity-table-entry.component';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';

type CapacityHolderType = FishingCapacityHolderDTO | FishingCapacityHolderRegixDataDTO;

@Component({
    selector: 'transfer-fishing-capacity-table',
    templateUrl: './transfer-fishing-capacity-table.component.html'
})
export class TransferFishingCapacityTableComponent extends CustomFormControl<CapacityHolderType[]> implements OnInit, OnChanges {
    @Input()
    public showOnlyRegiXData: boolean = false;

    @Input()
    public isDraft: boolean = false;

    @Input()
    public isEditing: boolean = false;

    @Input()
    public service!: IFishingCapacityService;

    @Input()
    public maxGrossTonnage: number | undefined;

    @Input()
    public maxPower: number | undefined;

    @Input()
    public submittedBy: ApplicationSubmittedByDTO | undefined;

    @Input()
    public expectedResults: FishingCapacityHolderRegixDataDTO[] = [];

    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    public entries: CapacityHolderType[] = [];
    public isDisabled: boolean = false;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    private remainingTonnage: number | undefined;
    private remainingPower: number | undefined;

    private translate: FuseTranslationLoaderService;
    private confirmDialog: TLConfirmDialog;
    private editEntryDialog: TLMatDialog<TransferFishingCapacityTableEntryComponent>;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editEntryDialog: TLMatDialog<TransferFishingCapacityTableEntryComponent>
    ) {
        super(ngControl, false);

        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.editEntryDialog = editEntryDialog;
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const maxGrossTonnage: SimpleChange | undefined = changes['maxGrossTonnage'];
        const maxPower: SimpleChange | undefined = changes['maxPower'];

        if (maxGrossTonnage || maxPower) {
            this.recalculateRemainingCapacity();
            this.control.updateValueAndValidity({ emitEvent: false });
        }
    }

    public writeValue(entries: CapacityHolderType[]): void {
        if (entries !== undefined && entries !== null) {
            this.entries = entries;
            this.updateEntriesNameAndEgnEik();
        }
        else {
            this.entries = [];
        }
    }

    public addEditEntry(holder?: CapacityHolderType, viewMode?: boolean): void {
        const readOnly: boolean = this.isDisabled || viewMode === true;

        let data: TransferFishingCapacityTableEntryParams | undefined;
        let headerAuditBtn: IHeaderAuditButton | undefined;
        let title: string;

        if (holder !== undefined) {
            data = new TransferFishingCapacityTableEntryParams({
                model: holder,
                isEgnLncReadOnly: this.isEditing,
                readOnly: readOnly,
                showOnlyRegixData: this.showOnlyRegiXData,
                remainingPower: this.remainingPower,
                remainingTonnage: this.remainingTonnage,
                submittedBy: this.submittedBy,
                expectedResults: this.expectedResults?.find(x => x.id === holder?.id) ?? new FishingCapacityHolderRegixDataDTO()
            });

            if (holder.id !== undefined) {
                headerAuditBtn = {
                    id: holder.id,
                    getAuditRecordData: this.service.getFishingCapacityHolderAudit.bind(this.service)
                };
            }

            if (readOnly) {
                title = this.translate.getValue('fishing-capacity.view-fishing-capacity-holder-dialog-title');
            }
            else {
                title = this.translate.getValue('fishing-capacity.edit-fishing-capacity-holder-dialog-title');
            }
        }
        else {
            data = new TransferFishingCapacityTableEntryParams({
                showOnlyRegixData: this.showOnlyRegiXData,
                remainingPower: this.remainingPower,
                remainingTonnage: this.remainingTonnage,
                submittedBy: this.submittedBy
            });

            title = this.translate.getValue('fishing-capacity.add-fishing-capacity-holder-dialog-title');
        }

        const dialog = this.editEntryDialog.openWithTwoButtons({
            title: title,
            TCtor: TransferFishingCapacityTableEntryComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditHolderDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: !readOnly,
            viewMode: readOnly
        }, '1200px');

        dialog.subscribe((result: CapacityHolderType) => {
            if (result) {
                if (holder !== undefined) {
                    holder = result;
                }
                else {
                    this.entries.push(result);
                }

                this.entries = this.entries.slice();
                this.onTouched(this.entries);
                this.onChanged(this.entries);

                this.recalculateRemainingCapacity();
                this.control.updateValueAndValidity();
            }
        });
    }

    public deleteEntry(holder: GridRow<FishingCapacityHolderDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('fishing-capacity.holder-table-delete-dialog-title'),
            message: this.translate.getValue('fishing-capacity.holder-table-delete-message'),
            okBtnLabel: this.translate.getValue('fishing-capacity.holder-table-delete-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.datatable.softDelete(holder);
                    this.onTouched(this.entries);
                    this.onChanged(this.entries);

                    this.recalculateRemainingCapacity();
                    this.control.updateValueAndValidity();
                }
            }
        });
    }

    public restoreEntry(holder: GridRow<FishingCapacityHolderDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.datatable.softUndoDelete(holder);
                    this.onTouched(this.entries);
                    this.onChanged(this.entries);

                    this.recalculateRemainingCapacity();
                    this.control.updateValueAndValidity();
                }
            }
        });
    }

    protected buildForm(): AbstractControl {
        return new FormControl(null, this.capacityValidator());
    }

    protected getValue(): CapacityHolderType[] {
        return this.entries;
    }

    private closeEditHolderDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private recalculateRemainingCapacity(): void {
        if (!this.showOnlyRegiXData) {
            if (this.maxGrossTonnage !== undefined && this.maxGrossTonnage !== null && this.maxPower !== undefined && this.maxPower !== null) {
                const entries: FishingCapacityHolderDTO[] = (this.entries as FishingCapacityHolderDTO[]).filter(x => x.isActive);
                const tonnageSum: number = entries.map(x => x.transferredTonnage!).reduce((a, b) => a + b, 0);
                const powerSum: number = entries.map(x => x.transferredPower!).reduce((a, b) => a + b, 0);

                this.remainingTonnage = Number((this.maxGrossTonnage - tonnageSum).toFixed(2));
                this.remainingPower = Number((this.maxPower - powerSum).toFixed(2));
            }
            else {
                this.remainingTonnage = undefined;
                this.remainingPower = undefined;
            }
        }
    }

    private capacityValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.showOnlyRegiXData !== true) {
                const errors: ValidationErrors = {};

                const entries = ((this.entries as FishingCapacityHolderDTO[]) ?? []).filter(x => x.isActive);

                if (entries.length === 0) {
                    errors['noHolders'] = true;
                    return errors;
                }

                if (this.maxGrossTonnage !== undefined && this.maxGrossTonnage !== null && this.maxPower !== undefined && this.maxPower !== null) {
                    const tonnageSum: number = entries.map(x => x.transferredTonnage!).reduce((a, b) => a + b, 0);
                    const powerSum: number = entries.map(x => x.transferredPower!).reduce((a, b) => a + b, 0);

                    if (CommonUtils.round(tonnageSum, 2) !== CommonUtils.round(this.maxGrossTonnage, 2)) {
                        errors['tonnageNotIdentical'] = true;
                    }

                    if (CommonUtils.round(powerSum, 2) !== CommonUtils.round(this.maxPower, 2)) {
                        errors['powerNotIdentical'] = true;
                    }
                }

                return Object.keys(errors).length > 0 ? errors : null;
            }

            return null;
        };
    }

    private updateEntriesNameAndEgnEik(): void {
        for (const entry of this.entries) {
            if (entry.isHolderPerson) {
                entry.name = '';
                if (entry.person!.firstName !== undefined && entry.person!.firstName !== null) {
                    entry.name = `${entry.person!.firstName}`;
                }
                if (entry.person!.middleName !== undefined && entry.person!.middleName !== null) {
                    entry.name = `${entry.name} ${entry.person!.middleName}`;
                }
                if (entry.person!.lastName !== undefined && entry.person!.lastName !== null) {
                    entry.name = `${entry.name} ${entry.person!.lastName}`;
                }

                entry.egnEik = entry.person!.egnLnc?.egnLnc;
            }
            else {
                entry.name = entry.legal!.name;
                entry.egnEik = entry.legal!.eik;
            }
        }
    }
}
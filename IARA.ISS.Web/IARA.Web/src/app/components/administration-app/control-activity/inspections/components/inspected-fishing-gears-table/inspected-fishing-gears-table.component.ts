import { Component, Input, OnChanges, OnInit, Self, SimpleChanges, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, NgControl, ValidationErrors, ValidatorFn } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { InspectedFishingGearDTO } from '@app/models/generated/dtos/InspectedFishingGearDTO';
import { InspectedFishingGearTableParams } from './models/inspected-fishing-gear-table-params';
import { EditInspectedFishingGearComponent } from '../../dialogs/edit-inspected-fishing-gear/edit-inspected-fishing-gear.component';
import { InspectedFishingGearTableModel } from './models/inspected-fishing-gear-table.model';
import { FishingGearDTO } from '@app/models/generated/dtos/FishingGearDTO';
import { InspectedFishingGearEnum } from '@app/enums/inspected-fishing-gear.enum';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { InspectionsService } from '@app/services/administration-app/inspections.service';

@Component({
    selector: 'inspected-fishing-gears-table',
    templateUrl: './inspected-fishing-gears-table.component.html'
})
export class InspectedFishingGearsTableComponent extends CustomFormControl<InspectedFishingGearDTO[]> implements OnInit, OnChanges {
    @Input()
    public permitIds: number[] = [];

    @Input()
    public hasAttachedAppliances: boolean = false;

    public readonly inspectedFishingGearEnum: typeof InspectedFishingGearEnum = InspectedFishingGearEnum;

    public fishingGears: InspectedFishingGearTableModel[] = [];

    private allFishingGears: InspectedFishingGearTableModel[] = [];

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    private readonly translate: FuseTranslationLoaderService;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly editEntryDialog: TLMatDialog<EditInspectedFishingGearComponent>;
    private readonly service: InspectionsService;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editEntryDialog: TLMatDialog<EditInspectedFishingGearComponent>,
        service: InspectionsService
    ) {
        super(ngControl);

        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.editEntryDialog = editEntryDialog;
        this.service = service;

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.control.updateValueAndValidity();
            }
        });
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const permitIds = changes['permitIds'];

        if (permitIds !== null && permitIds !== undefined) {
            if (this.allFishingGears.length > 0) {
                this.fishingGears = this.allFishingGears.filter(x =>
                    x.gear.permittedFishingGear === null
                    || x.gear.permittedFishingGear === undefined
                    || this.permitIds.includes(x.gear.permittedFishingGear.permitId!)
                    || this.permitIds.length === 0
                );
            }
        }
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public markAllAsNotAvailable(): void {
        for (const fishingGear of this.fishingGears) {
            if (!fishingGear.gear.checkInspectedMatchingRegisteredGear) {
                fishingGear.gear.checkInspectedMatchingRegisteredGear = InspectedFishingGearEnum.R;
            }
        }
    }

    public writeValue(value: InspectedFishingGearDTO[]): void {
        if (value !== undefined && value !== null && value.length > 0) {
            const fishingGears = value.map(f => {
                const fishingGear: FishingGearDTO = (f.inspectedFishingGear ?? f.permittedFishingGear)!;

                let checkName: string | undefined = undefined;

                if (f.checkInspectedMatchingRegisteredGear === null || f.checkInspectedMatchingRegisteredGear === undefined) {
                    f.checkInspectedMatchingRegisteredGear = InspectedFishingGearEnum.R;
                }

                switch (f.checkInspectedMatchingRegisteredGear) {
                    case InspectedFishingGearEnum.I:
                        checkName = this.translate.getValue('inspections.toggle-unregistered');
                        break;
                    case InspectedFishingGearEnum.N:
                        checkName = this.translate.getValue('inspections.toggle-does-not-match');
                        break;
                    case InspectedFishingGearEnum.Y:
                        checkName = this.translate.getValue('inspections.toggle-matches');
                        break;
                    case InspectedFishingGearEnum.R:
                        checkName = this.translate.getValue('inspections.toggle-not-available');
                        break;
                }

                return new InspectedFishingGearTableModel({
                    gear: f,
                    type: fishingGear.type,
                    count: fishingGear.count,
                    netEyeSize: fishingGear.netEyeSize,
                    marksNumbers: fishingGear.marksNumbers,
                    checkName: checkName
                })
            });

            setTimeout(() => {
                this.allFishingGears = fishingGears;
                this.fishingGears = fishingGears.filter(x =>
                    x.gear.permittedFishingGear === null
                    || x.gear.permittedFishingGear === undefined
                    || this.permitIds.includes(x.gear.permittedFishingGear.permitId!)
                    || this.permitIds.length === 0
                );
            });
        }
        else {
            this.fishingGears = [];
            this.allFishingGears = [];
        }
    }

    public addEditEntry(fishingGear?: InspectedFishingGearTableModel, viewMode?: boolean): void {
        const readOnly: boolean = this.isDisabled || viewMode === true;

        let data: InspectedFishingGearTableParams | undefined;
        let title: string;
        let auditBtn: IHeaderAuditButton | undefined;

        if (fishingGear !== undefined && fishingGear !== null) {
            data = new InspectedFishingGearTableParams({
                model: fishingGear,
                readOnly: readOnly,
                isRegistered: fishingGear.gear.permittedFishingGear !== null && fishingGear.gear.permittedFishingGear !== undefined,
                isEdit: true,
                hasAttachedAppliances: this.hasAttachedAppliances
            });

            if (fishingGear?.gear?.id !== undefined && fishingGear?.gear?.id !== null) {
                auditBtn = {
                    id: fishingGear.gear.id!,
                    getAuditRecordData: this.service.getInspectedFishingGearSimpleAudit.bind(this.service),
                    tableName: 'InspectedFishingGears'
                };
            }

            title = readOnly
                ? this.translate.getValue('inspections.view-inspected-gear-dialog-title')
                : this.translate.getValue('inspections.edit-inspected-gear-dialog-title');
        }
        else {
            data = new InspectedFishingGearTableParams({
                model: new InspectedFishingGearTableModel({
                    gear: new InspectedFishingGearDTO({
                        checkInspectedMatchingRegisteredGear: InspectedFishingGearEnum.I
                    })
                }),
                isEdit: false,
                isRegistered: false,
                hasAttachedAppliances: this.hasAttachedAppliances
            });

            title = this.translate.getValue('inspections.add-inspected-gear-dialog-title');
        }

        const dialog = this.editEntryDialog.openWithTwoButtons({
            title: title,
            TCtor: EditInspectedFishingGearComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => closeFn()
            },
            headerAuditButton: auditBtn,
            componentData: data,
            translteService: this.translate,
            disableDialogClose: !readOnly,
            viewMode: readOnly
        }, fishingGear?.gear?.permittedFishingGear == undefined ? '800px' : '1600px');

        dialog.subscribe((result: InspectedFishingGearTableModel) => {
            if (result !== undefined && result !== null) {
                if (fishingGear !== undefined) {
                    fishingGear = result;
                }
                else {
                    this.fishingGears.push(result);
                    this.allFishingGears.push(result);
                }

                this.fishingGears = this.fishingGears.slice();
                this.onChanged(this.getValue());
                this.control.updateValueAndValidity();
            }
        });
    }

    public deleteEntry(fishingGear: GridRow<InspectedFishingGearTableModel>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('inspections.inspected-gear-delete-dialog-title'),
            message: this.translate.getValue('inspections.inspected-gear-delete-message'),
            okBtnLabel: this.translate.getValue('inspections.inspected-gear-delete-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.datatable.softDelete(fishingGear);
                    this.fishingGears.splice(this.fishingGears.indexOf(fishingGear.data), 1);
                    this.onChanged(this.getValue());
                    this.control.updateValueAndValidity();
                }
            }
        });
    }

    protected buildForm(): AbstractControl {
        return new FormControl(undefined, this.fishingGearsValidator());
    }

    protected getValue(): InspectedFishingGearDTO[] {
        const result: InspectedFishingGearDTO[] = this.allFishingGears.filter(x => x.gear.isActive).map(x => new InspectedFishingGearDTO({
            id: x.gear.id,
            inspectedFishingGear: x.gear.inspectedFishingGear,
            permittedFishingGear: x.gear.permittedFishingGear,
            checkInspectedMatchingRegisteredGear: x.gear.checkInspectedMatchingRegisteredGear,
            hasAttachedAppliances: x.gear.hasAttachedAppliances,
            isActive: x.gear.isActive
        }));

        return result;
    }

    private fishingGearsValidator(): ValidatorFn {
        return (): ValidationErrors | null => {
            if (this.fishingGears !== undefined && this.fishingGears !== null) {
                for (const fishingGear of this.fishingGears) {
                    if (fishingGear.gear.checkInspectedMatchingRegisteredGear === null || fishingGear.gear.checkInspectedMatchingRegisteredGear === undefined) {
                        return { 'fishingGearMustBeChecked': true };
                    }
                }
            }
            return null;
        };
    }
}
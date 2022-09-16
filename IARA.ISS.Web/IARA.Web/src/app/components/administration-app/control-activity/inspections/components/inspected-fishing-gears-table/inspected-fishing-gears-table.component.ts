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

@Component({
    selector: 'inspected-fishing-gears-table',
    templateUrl: './inspected-fishing-gears-table.component.html'
})
export class InspectedFishingGearsTableComponent extends CustomFormControl<InspectedFishingGearDTO[]> implements OnInit, OnChanges {

    public fishingGears: InspectedFishingGearTableModel[] = [];

    @Input()
    public permitIds: number[] = [];

    @Input()
    public hasAttachedAppliances: boolean = false;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    private allFishingGears: InspectedFishingGearTableModel[] = [];

    public readonly inspectedFishingGearEnum: typeof InspectedFishingGearEnum = InspectedFishingGearEnum;

    private readonly translate: FuseTranslationLoaderService;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly editEntryDialog: TLMatDialog<EditInspectedFishingGearComponent>;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editEntryDialog: TLMatDialog<EditInspectedFishingGearComponent>
    ) {
        super(ngControl);

        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.editEntryDialog = editEntryDialog;

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
                this.fishingGears = this.allFishingGears
                    .filter(f => f.DTO.permittedFishingGear === null
                        || f.DTO.permittedFishingGear === undefined
                        || this.permitIds.includes(f.DTO.permittedFishingGear.permitId!));
            }
        }
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(value: InspectedFishingGearDTO[]): void {
        if (value !== undefined && value !== null) {
            const fishingGears = value.map(f => {
                const fishingGear: FishingGearDTO = (f.inspectedFishingGear ?? f.permittedFishingGear)!;

                let checkName: string | undefined = undefined;

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
                    DTO: f,
                    type: fishingGear.type,
                    count: fishingGear.count,
                    netEyeSize: fishingGear.netEyeSize,
                    marksNumbers: fishingGear.marksNumbers,
                    checkName: checkName,
                })
            });

            setTimeout(() => {
                this.allFishingGears = fishingGears;
                this.fishingGears = fishingGears
                    .filter(f => f.DTO.permittedFishingGear === null
                        || f.DTO.permittedFishingGear === undefined
                        || this.permitIds.includes(f.DTO.permittedFishingGear.permitId!));
            });
        }
        else {
            setTimeout(() => {
                this.fishingGears = [];
                this.allFishingGears = [];
            });
        }
    }

    public addEditEntry(fishingGear?: InspectedFishingGearTableModel, viewMode?: boolean): void {
        const readOnly: boolean = this.isDisabled || viewMode === true;

        let data: InspectedFishingGearTableParams | undefined;
        let title: string;

        if (fishingGear !== undefined && fishingGear !== null) {
            data = new InspectedFishingGearTableParams({
                model: fishingGear,
                readOnly: readOnly,
                isRegistered: fishingGear.DTO.permittedFishingGear !== null && fishingGear.DTO.permittedFishingGear !== undefined,
                isEdit: true,
                hasAttachedAppliances: this.hasAttachedAppliances,
            });

            if (readOnly) {
                title = this.translate.getValue('inspections.view-inspected-gear-dialog-title');
            }
            else {
                title = this.translate.getValue('inspections.edit-inspected-gear-dialog-title');
            }
        }
        else {
            data = new InspectedFishingGearTableParams({
                model: new InspectedFishingGearTableModel({
                    DTO: new InspectedFishingGearDTO({
                        checkInspectedMatchingRegisteredGear: InspectedFishingGearEnum.I
                    }),
                }),
                isEdit: false,
                isRegistered: false,
                hasAttachedAppliances: this.hasAttachedAppliances,
            });

            title = this.translate.getValue('inspections.add-inspected-gear-dialog-title');
        }

        const dialog = this.editEntryDialog.openWithTwoButtons({
            title: title,
            TCtor: EditInspectedFishingGearComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => closeFn()
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: !readOnly,
            viewMode: readOnly
        }, '800px');

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

    public deleteEntry(patrolVehicle: GridRow<InspectedFishingGearTableModel>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('inspections.inspected-gear-delete-dialog-title'),
            message: this.translate.getValue('inspections.inspected-gear-delete-message'),
            okBtnLabel: this.translate.getValue('inspections.inspected-gear-delete-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.datatable.softDelete(patrolVehicle);
                    this.fishingGears.splice(this.fishingGears.indexOf(patrolVehicle.data), 1);
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
        return this.allFishingGears.map(f => f.DTO);
    }

    private fishingGearsValidator(): ValidatorFn {
        return (): ValidationErrors | null => {
            if (this.fishingGears !== undefined && this.fishingGears !== null) {
                for (const fishingGear of this.fishingGears) {
                    if (fishingGear.DTO.checkInspectedMatchingRegisteredGear === null || fishingGear.DTO.checkInspectedMatchingRegisteredGear === undefined) {
                        return { 'fishingGearMustBeChecked': true };
                    }
                }
            }
            return null;
        };
    }
}
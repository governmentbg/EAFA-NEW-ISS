import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { InspectedFishingGearDTO } from '@app/models/generated/dtos/InspectedFishingGearDTO';
import { InspectedFishingGearTableParams } from '../../components/inspected-fishing-gears-table/models/inspected-fishing-gear-table-params';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { InspectionToggleTypesEnum } from '@app/enums/inspection-toggle-types.enum';
import { InspectionUtils } from '@app/shared/utils/inspection.utils';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { InspectionCheckModel } from '../../models/inspection-check.model';
import { InspectionCheckDTO } from '@app/models/generated/dtos/InspectionCheckDTO';
import { InspectedFishingGearEnum } from '@app/enums/inspected-fishing-gear.enum';
import { InspectedFishingGearTableModel } from '../../components/inspected-fishing-gears-table/models/inspected-fishing-gear-table.model';
import { FishingGearDTO } from '@app/models/generated/dtos/FishingGearDTO';
import { FishingGearMarkDTO } from '@app/models/generated/dtos/FishingGearMarkDTO';
import { FishingGearPingerDTO } from '@app/models/generated/dtos/FishingGearPingerDTO';

@Component({
    selector: 'edit-inspected-fishing-gear',
    templateUrl: './edit-inspected-fishing-gear.component.html',
})
export class EditInspectedFishingGearComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    protected model: InspectedFishingGearTableModel = new InspectedFishingGearTableModel({
        DTO: new InspectedFishingGearDTO()
    });

    public toggle: InspectionCheckModel;
    public appliancesToggle: InspectionCheckModel;
    public options: NomenclatureDTO<InspectionToggleTypesEnum>[];

    public isRegistered: boolean = false;
    public canEditInspectedGear: boolean = true;
    public hasAttachedAppliances: boolean = false;

    private readOnly: boolean = false;

    private readonly translate: FuseTranslationLoaderService;

    public constructor(translate: FuseTranslationLoaderService) {
        this.translate = translate;

        this.options = InspectionUtils.getToggleMatchesOptions(translate);

        this.toggle = new InspectionCheckModel({
            value: 0,
            isMandatory: true,
            displayName: translate.getValue('inspections.fishing-gear-registered'),
        });

        this.appliancesToggle = new InspectionCheckModel({
            value: 0,
            isMandatory: true,
            displayName: translate.getValue('inspections.fishing-gear-has-attached-appliances'),
        });

        this.buildForm();
    }

    public ngOnInit(): void {
        if (this.readOnly) {
            this.form.disable();
        }

        this.fillForm();
    }

    public setData(data: InspectedFishingGearTableParams, wrapperData: DialogWrapperData): void {
        if (data.model !== undefined && data.model !== null) {
            this.model = data.model;
        }

        this.readOnly = data.readOnly;
        this.isRegistered = data.isRegistered;
        this.hasAttachedAppliances = data.hasAttachedAppliances;

        if (!data.isRegistered) {
            this.form.get('optionsControl')!.disable();
        }

        if (!data.hasAttachedAppliances) {
            this.form.get('appliancesOptionsControl')!.disable();
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly) {
            dialogClose(this.model);
        }
        else {
            this.form.markAllAsTouched();
            if (this.form.valid) {
                this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);
                dialogClose(this.model);
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public onMarkSelected(mark: FishingGearMarkDTO): void {
        const value: FishingGearDTO = this.form.get('inspectedGearControl')!.value;

        if (value && !value.marks!.includes(mark)) {
            value.marks = [...value.marks!, mark];
        }
    }

    protected buildForm(): void {
        this.form = new FormGroup({
            permittedGearControl: new FormControl({ value: undefined, disabled: true }),
            inspectedGearControl: new FormControl(undefined),
            optionsControl: new FormControl(undefined, [Validators.required]),
            appliancesOptionsControl: new FormControl(undefined, [Validators.required]),
        });

        this.form.get('optionsControl')!.valueChanges.subscribe({
            next: (value: InspectionCheckDTO) => {
                if (value === null || value === undefined || value.checkValue !== InspectionToggleTypesEnum.N) {
                    this.canEditInspectedGear = false;
                }
                else {
                    this.canEditInspectedGear = true;
                    setTimeout(() => {
                        const gear = this.remap(this.model.DTO.inspectedFishingGear ?? this.model.DTO.permittedFishingGear)!;
                        gear.marks = [];
                        this.form.get('inspectedGearControl')!.setValue(gear);
                    }, 100);
                }
            }
        });
    }

    protected fillForm(): void {
        setTimeout(() => {
            this.form.get('permittedGearControl')!.setValue(this.remap(this.model.DTO.permittedFishingGear));
        }, 100);

        const checkValue = this.mapToCheckValue(this.model.DTO.checkInspectedMatchingRegisteredGear);

        if (checkValue !== null && checkValue !== undefined) {
            this.form.get('optionsControl')!.setValue(new InspectionCheckDTO({ checkValue: checkValue }));
        }
        else {
            this.form.get('optionsControl')!.setValue(null);
        }

        if (this.model.DTO.hasAttachedAppliances !== null && this.model.DTO.hasAttachedAppliances !== undefined) {
            this.form.get('appliancesOptionsControl')!.setValue(
                new InspectionCheckDTO({
                    checkValue: InspectionToggleTypesEnum.R
                })
            );
        }
        else if (this.model.DTO.hasAttachedAppliances) {
            this.form.get('appliancesOptionsControl')!.setValue(
                new InspectionCheckDTO({
                    checkValue: InspectionToggleTypesEnum.Y
                })
            );
        }
        else {
            this.form.get('appliancesOptionsControl')!.setValue(
                new InspectionCheckDTO({
                    checkValue: InspectionToggleTypesEnum.N
                })
            );
        }

        if (this.model.DTO.checkInspectedMatchingRegisteredGear === InspectedFishingGearEnum.N
            || this.model.DTO.checkInspectedMatchingRegisteredGear === InspectedFishingGearEnum.I) {
            this.canEditInspectedGear = true;
            setTimeout(() => {
                this.form.get('inspectedGearControl')!.setValue(this.remap(this.model.DTO.inspectedFishingGear ?? this.model.DTO.permittedFishingGear));
            }, 500);
        }
        else {
            this.canEditInspectedGear = false;
        }
    }

    protected fillModel(): void {
        this.model.DTO.inspectedFishingGear = this.canEditInspectedGear
            ? this.form.get('inspectedGearControl')!.value
            : undefined;
        this.model.DTO.checkInspectedMatchingRegisteredGear = this.mapToFishingGearEnum();

        const appliancesOptions: InspectionToggleTypesEnum = this.form.get('appliancesOptionsControl')!.value?.checkValue;

        if (appliancesOptions == null || appliancesOptions === InspectionToggleTypesEnum.R) {
            this.model.DTO.hasAttachedAppliances = undefined;
        }
        else if (appliancesOptions === InspectionToggleTypesEnum.Y) {
            this.model.DTO.hasAttachedAppliances = true;
        }
        else {
            this.model.DTO.hasAttachedAppliances = false;
        }

        if (this.model.DTO.inspectedFishingGear !== null && this.model.DTO.inspectedFishingGear !== undefined) {
            if (this.model.DTO.inspectedFishingGear.marks !== null && this.model.DTO.inspectedFishingGear.marks !== undefined) {
                this.model.DTO.inspectedFishingGear.marks = this.model.DTO.inspectedFishingGear.marks.filter(f => f.isActive);
            }

            if (this.model.DTO.inspectedFishingGear.pingers !== null && this.model.DTO.inspectedFishingGear.pingers !== undefined) {
                this.model.DTO.inspectedFishingGear.pingers = this.model.DTO.inspectedFishingGear.pingers.filter(f => f.isActive);
            }
        }

        const fishingGear: FishingGearDTO = (this.model.DTO.inspectedFishingGear ?? this.model.DTO.permittedFishingGear)!;

        this.model.type = fishingGear.type;
        this.model.count = fishingGear.count;
        this.model.netEyeSize = fishingGear.netEyeSize;
        this.model.marksNumbers = fishingGear.marksNumbers;

        switch (this.model.DTO.checkInspectedMatchingRegisteredGear) {
            case InspectedFishingGearEnum.I:
                this.model.checkName = this.translate.getValue('inspections.toggle-unregistered');
                break;
            case InspectedFishingGearEnum.N:
                this.model.checkName = this.translate.getValue('inspections.toggle-does-not-match');
                break;
            case InspectedFishingGearEnum.Y:
                this.model.checkName = this.translate.getValue('inspections.toggle-matches');
                break;
            case InspectedFishingGearEnum.R:
                this.model.checkName = this.translate.getValue('inspections.toggle-not-available');
                break;
        }
    }

    private mapToCheckValue(checkValue?: InspectedFishingGearEnum): InspectionToggleTypesEnum | undefined {
        if (checkValue === null || checkValue === undefined) {
            return undefined;
        }

        switch (checkValue) {
            case InspectedFishingGearEnum.Y:
                return InspectionToggleTypesEnum.Y;
            case InspectedFishingGearEnum.N:
                return InspectionToggleTypesEnum.N;
            case InspectedFishingGearEnum.R:
                return InspectionToggleTypesEnum.X;
            default:
                return undefined;
        }
    }

    private mapToFishingGearEnum(): InspectedFishingGearEnum | undefined {
        if (this.model.DTO.checkInspectedMatchingRegisteredGear === InspectedFishingGearEnum.I) {
            return InspectedFishingGearEnum.I;
        }

        const checkValue: InspectionCheckDTO = this.form.get('optionsControl')!.value;

        if (checkValue === null || checkValue === undefined
            || checkValue.checkValue === null || checkValue.checkValue === undefined) {
            return undefined;
        }

        switch (checkValue.checkValue) {
            case InspectionToggleTypesEnum.Y:
                return InspectedFishingGearEnum.Y;
            case InspectionToggleTypesEnum.N:
                return InspectedFishingGearEnum.N;
            case InspectionToggleTypesEnum.X:
                return InspectedFishingGearEnum.R;
            default:
                return undefined;
        }
    }

    private remap(dto: FishingGearDTO | undefined): FishingGearDTO | undefined {
        if (dto === null || dto === undefined) {
            return undefined;
        }

        return new FishingGearDTO({
            cordThickness: dto.cordThickness,
            count: dto.count,
            description: dto.description,
            hasPingers: dto.hasPingers,
            height: dto.height,
            hookCount: dto.hookCount,
            houseLength: dto.houseLength,
            houseWidth: dto.houseWidth,
            id: dto.id,
            isActive: dto.isActive,
            length: dto.length,
            marksNumbers: dto.marksNumbers,
            netEyeSize: dto.netEyeSize,
            towelLength: dto.towelLength,
            type: dto.type,
            typeId: dto.typeId,
            permitId: dto.permitId,
            marks: dto.marks?.map(f => new FishingGearMarkDTO({
                id: f.id,
                isActive: f.isActive,
                number: f.number,
                selectedStatus: f.selectedStatus,
                statusId: f.statusId
            })),
            pingers: dto.pingers?.map(f => new FishingGearPingerDTO({
                id: f.id,
                isActive: f.isActive,
                number: f.number,
                selectedStatus: f.selectedStatus,
                statusId: f.statusId
            }))
        });
    }
}
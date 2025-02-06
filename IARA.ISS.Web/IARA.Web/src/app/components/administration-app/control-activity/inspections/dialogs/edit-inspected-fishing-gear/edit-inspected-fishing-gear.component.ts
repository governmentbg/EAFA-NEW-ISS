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
import { FishingGearManipulationService } from '@app/components/common-app/commercial-fishing/components/fishing-gears/services/fishing-gear-manipulation.service';
import { PrefixInputDTO } from '@app/models/generated/dtos/PrefixInputDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';

@Component({
    selector: 'edit-inspected-fishing-gear',
    templateUrl: './edit-inspected-fishing-gear.component.html',
})
export class EditInspectedFishingGearComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    protected model: InspectedFishingGearTableModel = new InspectedFishingGearTableModel({
        gear: new InspectedFishingGearDTO()
    });

    public toggle: InspectionCheckModel;
    public appliancesToggle: InspectionCheckModel;
    public options: NomenclatureDTO<InspectionToggleTypesEnum>[];

    public isRegistered: boolean = false;
    public canEditInspectedGear: boolean = true;
    public hasAttachedAppliances: boolean = false;
    public isAddInspectedGear: boolean = false;
    public filterTypes: boolean = false;
    public pageCode: PageCodeEnum = PageCodeEnum.CommFishLicense;

    private readOnly: boolean = false;

    private inspectedMarks: FishingGearMarkDTO[] = [];

    private readonly translate: FuseTranslationLoaderService;
    private readonly gearManipulationService: FishingGearManipulationService;

    public constructor(translate: FuseTranslationLoaderService, gearManipulationService: FishingGearManipulationService) {
        this.translate = translate;
        this.gearManipulationService = gearManipulationService;

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
        this.isAddInspectedGear = !data.isEdit;
        this.hasAttachedAppliances = data.hasAttachedAppliances;
        this.filterTypes = data.filterTypes;

        if (data.pageCode !== undefined && data.pageCode !== null) {
            this.pageCode = data.pageCode;
        }

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
        this.gearManipulationService.markAdded.emit(mark);
        const inspectedGear: FishingGearDTO = this.form.get('inspectedGearControl')!.value;

        if (inspectedGear !== undefined && inspectedGear !== null) {
            if (inspectedGear.marks !== undefined && inspectedGear.marks !== null && inspectedGear.marks.length > 0) {
                inspectedGear.marks.push(mark);
            }
            else {
                inspectedGear.marks = [mark];
            }

            this.inspectedMarks = inspectedGear.marks;
        }

        this.form.get('inspectedGearControl')!.setValue(inspectedGear);
    }

    protected buildForm(): void {
        this.form = new FormGroup({
            permittedGearControl: new FormControl({ value: undefined, disabled: true }),
            inspectedGearControl: new FormControl(undefined),
            optionsControl: new FormControl(undefined, [Validators.required]),
            appliancesOptionsControl: new FormControl(undefined, [Validators.required])
        });

        this.form.get('optionsControl')!.valueChanges.subscribe({
            next: (value: InspectionCheckDTO) => {
                if (value === null || value === undefined || value.checkValue !== InspectionToggleTypesEnum.N) {
                    this.canEditInspectedGear = false;
                }
                else {
                    this.canEditInspectedGear = true;

                    setTimeout(() => {
                        let gear: FishingGearDTO = new FishingGearDTO();

                        if (this.model.gear.inspectedFishingGear !== null && this.model.gear.inspectedFishingGear !== undefined) {
                            this.isAddInspectedGear = false;
                            gear = this.remap(this.model.gear.inspectedFishingGear);
                            this.inspectedMarks = gear.marks ?? [];
                        }
                        else {
                            this.isAddInspectedGear = true;
                            gear = this.remap(this.model.gear.permittedFishingGear);
                            gear.id = undefined;
                        }

                        this.form.get('inspectedGearControl')!.setValue(gear);
                    }, 100);
                }

                this.form.get('permittedGearControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });

        this.form.get('inspectedGearControl')!.valueChanges.subscribe({
            next: (gear: FishingGearDTO | undefined) => {
                if (gear !== undefined && gear !== null) {
                    this.inspectedMarks = gear.marks ?? [];
                }
            }
        });
    }

    protected fillForm(): void {
        setTimeout(() => {
            const gear: FishingGearDTO = this.remap(this.model.gear.permittedFishingGear);
            this.form.get('permittedGearControl')!.setValue(gear);
        }, 500);

        const checkValue = this.mapToCheckValue(this.model.gear.checkInspectedMatchingRegisteredGear);
        if (checkValue !== null && checkValue !== undefined) {
            this.form.get('optionsControl')!.setValue(new InspectionCheckDTO({ checkValue: checkValue }));
        }
        else {
            this.form.get('optionsControl')!.setValue(null);
        }

        if (this.model.gear.hasAttachedAppliances !== null && this.model.gear.hasAttachedAppliances !== undefined) {
            if (this.model.gear.hasAttachedAppliances) {
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

        }
        else {
            this.form.get('appliancesOptionsControl')!.setValue(
                new InspectionCheckDTO({
                    checkValue: InspectionToggleTypesEnum.R
                })
            );
        }

        if (this.model.gear.checkInspectedMatchingRegisteredGear === InspectedFishingGearEnum.N
            || this.model.gear.checkInspectedMatchingRegisteredGear === InspectedFishingGearEnum.I) {
            this.canEditInspectedGear = true;

            setTimeout(() => {
                let gear: FishingGearDTO = new FishingGearDTO();

                if (this.model.gear.inspectedFishingGear !== null && this.model.gear.inspectedFishingGear !== undefined) {
                    gear = this.remap(this.model.gear.inspectedFishingGear);
                    this.inspectedMarks = gear.marks ?? [];
                }
                else if (this.model.gear.permittedFishingGear !== undefined && this.model.gear.permittedFishingGear !== null) {
                    gear = this.remap(this.model.gear.permittedFishingGear);
                    gear.id = undefined;
                }

                this.form.get('inspectedGearControl')!.setValue(gear);
            }, 500);
        }
        else {
            this.canEditInspectedGear = false;
        }
    }

    protected fillModel(): void {
        this.model.gear.inspectedFishingGear = this.canEditInspectedGear
            ? this.form.get('inspectedGearControl')!.value
            : undefined;

        this.model.gear.checkInspectedMatchingRegisteredGear = this.mapToFishingGearEnum();

        const appliancesOptions: InspectionToggleTypesEnum = this.form.get('appliancesOptionsControl')!.value?.checkValue;
        if (appliancesOptions == null || appliancesOptions === InspectionToggleTypesEnum.R) {
            this.model.gear.hasAttachedAppliances = undefined;
        }
        else if (appliancesOptions === InspectionToggleTypesEnum.Y) {
            this.model.gear.hasAttachedAppliances = true;
        }
        else {
            this.model.gear.hasAttachedAppliances = false;
        }

        if (this.model.gear.inspectedFishingGear !== null && this.model.gear.inspectedFishingGear !== undefined) {
            this.model.gear.inspectedFishingGear.marks = this.inspectedMarks;

            if (this.model.gear.inspectedFishingGear.pingers !== null && this.model.gear.inspectedFishingGear.pingers !== undefined) {
                const pingers: FishingGearPingerDTO[] = this.model.gear.inspectedFishingGear.pingers.filter(f => f.isActive) ?? [];

                if (pingers.length > 0) {
                    this.model.gear.inspectedFishingGear.pingers = pingers;
                    this.model.gear.inspectedFishingGear.hasPingers = true;
                }
            }
        }

        const fishingGear: FishingGearDTO = (this.model.gear.inspectedFishingGear ?? this.model.gear.permittedFishingGear)!;

        this.model.type = fishingGear.type;
        this.model.count = fishingGear.count;
        this.model.netEyeSize = fishingGear.netEyeSize;
        this.model.marksNumbers = fishingGear.marksNumbers;
        this.model.gear.isActive = true;

        switch (this.model.gear.checkInspectedMatchingRegisteredGear) {
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
        if (this.model.gear.checkInspectedMatchingRegisteredGear === InspectedFishingGearEnum.I) {
            return InspectedFishingGearEnum.I;
        }

        const checkValue: InspectionCheckDTO = this.form.get('optionsControl')!.value;

        if (checkValue === null || checkValue === undefined || checkValue.checkValue === null || checkValue.checkValue === undefined) {
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

    private remap(gear: FishingGearDTO | undefined): FishingGearDTO {
        if (gear === null || gear === undefined) {
            return new FishingGearDTO();
        }

        const result: FishingGearDTO = new FishingGearDTO({
            cordThickness: gear.cordThickness,
            count: gear.count,
            description: gear.description,
            hasPingers: gear.hasPingers ?? false,
            height: gear.height,
            hookCount: gear.hookCount,
            houseLength: gear.houseLength,
            houseWidth: gear.houseWidth,
            id: gear.id,
            isActive: gear.isActive,
            length: gear.length,
            marksNumbers: gear.marksNumbers,
            netEyeSize: gear.netEyeSize,
            towelLength: gear.towelLength,
            netNominalLength: gear.netNominalLength,
            netsInFleetCount: gear.netsInFleetCount,
            lineCount: gear.lineCount,
            trawlModel: gear.trawlModel,
            type: gear.type,
            typeId: gear.typeId,
            permitId: gear.permitId
        });

        if (gear.marks !== undefined && gear.marks !== null && gear.marks.length > 0) {
            result.marks = gear.marks.map(f => new FishingGearMarkDTO({
                id: f.id,
                isActive: f.isActive,
                createdOn: f.createdOn,
                fullNumber: new PrefixInputDTO({
                    prefix: f.fullNumber?.prefix,
                    inputValue: f.fullNumber?.inputValue
                }),
                selectedStatus: f.selectedStatus,
                statusId: f.statusId
            }));
        }

        if (gear.pingers !== undefined && gear.pingers !== null && gear.pingers.length > 0) {
            result.hasPingers = true;

            result.pingers = gear.pingers.map(f => new FishingGearPingerDTO({
                id: f.id,
                isActive: f.isActive,
                number: f.number,
                selectedStatus: f.selectedStatus,
                statusId: f.statusId,
                brand: f.brand,
                model: f.model
            }));
        }

        return result;
    }
}
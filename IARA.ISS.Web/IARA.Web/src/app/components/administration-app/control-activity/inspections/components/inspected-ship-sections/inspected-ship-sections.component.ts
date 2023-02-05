import { Component, ElementRef, Input, OnChanges, OnInit, Self, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { InspectionCheckModel } from '../../models/inspection-check.model';
import { InspectedShipSectionsModel } from '../../models/inspected-ship-sections.model';
import { ShipWithPersonnelModel } from '../../models/ship-with-personnel.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { InspectionToggleTypesEnum } from '@app/enums/inspection-toggle-types.enum';
import { InspectionUtils } from '@app/shared/utils/inspection.utils';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { InspectionCheckDTO } from '@app/models/generated/dtos/InspectionCheckDTO';
import { InspectionCheckTypesEnum } from '@app/enums/inspection-check-types.enum';
import { InspectionsService } from '@app/services/administration-app/inspections.service';
import { InspectionPermitLicenseDTO } from '@app/models/generated/dtos/InspectionPermitLicenseDTO';
import { InspectionShipLogBookDTO } from '@app/models/generated/dtos/InspectionShipLogBookDTO';
import { InspectedFishingGearDTO } from '@app/models/generated/dtos/InspectedFishingGearDTO';
import { FishingGearDTO } from '@app/models/generated/dtos/FishingGearDTO';
import { InspectionLogBookDTO } from '@app/models/generated/dtos/InspectionLogBookDTO';
import { InspectionPermitDTO } from '@app/models/generated/dtos/InspectionPermitDTO';
import { InspectionObservationCategoryEnum } from '@app/enums/inspection-observation-category.enum';
import { InspectionObservationTextDTO } from '@app/models/generated/dtos/InspectionObservationTextDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { VesselDuringInspectionDTO } from '@app/models/generated/dtos/VesselDuringInspectionDTO';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';

@Component({
    selector: 'inspected-ship-sections',
    templateUrl: './inspected-ship-sections.component.html'
})
export class InspectedShipSectionsComponent extends CustomFormControl<InspectedShipSectionsModel | undefined> implements OnInit, OnChanges {

    @Input()
    public toggles: InspectionCheckModel[] = [];

    @Input()
    public hasMap: boolean = true;

    @Input()
    public hasPort: boolean = true;

    @Input()
    public hasFishingGears: boolean = true;

    @Input()
    public hasUnloadedQuantity: boolean = false;

    @Input()
    public shipObservationCategory: InspectionObservationCategoryEnum = InspectionObservationCategoryEnum.ShipData;

    @Input()
    public checksObservationCategory: InspectionObservationCategoryEnum = InspectionObservationCategoryEnum.Check;

    @Input()
    public catchObservationCategory: InspectionObservationCategoryEnum = InspectionObservationCategoryEnum.Catch;

    @Input()
    public fishingGearObservationCategory: InspectionObservationCategoryEnum = InspectionObservationCategoryEnum.FishingGear;

    @Input()
    public shipLabel: string;

    @Input()
    public checksLabel: string;

    @Input()
    public catchLabel: string;

    @Input()
    public fishingGearLabel: string;

    @Input()
    public associations: NomenclatureDTO<number>[] = [];

    @Input()
    public ships: NomenclatureDTO<number>[] = [];

    @Input()
    public fishes: NomenclatureDTO<number>[] = [];

    @Input()
    public catchTypes: NomenclatureDTO<number>[] = [];

    @Input()
    public catchZones: NomenclatureDTO<number>[] = [];

    @Input()
    public fishSex: NomenclatureDTO<number>[] = [];

    @Input()
    public countries: NomenclatureDTO<number>[] = [];

    @Input()
    public ports: NomenclatureDTO<number>[] = [];

    @Input()
    public vesselTypes: NomenclatureDTO<number>[] = [];

    @Input()
    public turbotSizeGroups: NomenclatureDTO<number>[] = [];

    public shipToggles: InspectionCheckModel[] = [];
    public checkToggles: InspectionCheckModel[] = [];
    public catchToggles: InspectionCheckModel[] = [];
    public fishingGearToggles: InspectionCheckModel[] = [];

    public readonly boolOptions: NomenclatureDTO<InspectionToggleTypesEnum>[] = [];
    public readonly tripleOptions: NomenclatureDTO<InspectionToggleTypesEnum>[] = [];

    public readonly inspectionCheckTypesEnum: typeof InspectionCheckTypesEnum = InspectionCheckTypesEnum;

    public opMembership?: InspectionCheckModel;
    public preliminaryNotice?: InspectionCheckModel;

    public permitIds: number[] = [];

    private readonly service: InspectionsService;
    private readonly element: ElementRef<HTMLElement>;

    public constructor(@Self() ngControl: NgControl,
        @Self() validityChecker: ValidityCheckerDirective,
        @Self() element: ElementRef<HTMLElement>,
        service: InspectionsService,
        translate: FuseTranslationLoaderService
    ) {
        super(ngControl, true, validityChecker);

        this.service = service;
        this.element = element;

        this.shipLabel = translate.getValue('inspections.ship-data');
        this.checksLabel = translate.getValue('inspections.ship-inspection');
        this.catchLabel = translate.getValue('inspections.catch-inspection');
        this.fishingGearLabel = translate.getValue('inspections.fishing-gears-inspection');

        this.boolOptions = InspectionUtils.getToggleBoolOptions(translate);
        this.tripleOptions = InspectionUtils.getToggleTripleOptions(translate);

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.control.updateValueAndValidity();
            }
        });
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const toggles = changes['toggles'];
        const hasFishingGears = changes['hasFishingGears'];

        if (toggles !== null && toggles !== undefined) {
            this.shipToggles = this.toggles
                .filter(f => f.code!.startsWith('Ship-'));

            this.checkToggles = this.toggles
                .filter(f => !f.code!.startsWith('Catch-')
                    && !f.code!.startsWith('Ship-')
                    && !f.code!.startsWith('CheckObject-'));

            this.catchToggles = this.toggles
                .filter(f => f.code!.startsWith('Catch-'));

            this.fishingGearToggles = this.toggles
                .filter(f => f.code!.startsWith('FishingGear-'));

            this.opMembership = this.toggles
                .find(f => f.code === 'CheckObject-OPMembership');

            this.preliminaryNotice = this.toggles
                .find(f => f.code === 'CheckObject-PreliminaryNotice');
        }

        if (hasFishingGears !== null && hasFishingGears !== undefined) {
            if (this.hasFishingGears) {
                this.form.get('fishingGearsControl')!.enable();
            }
            else {
                this.form.get('fishingGearsControl')!.disable();
            }
        }
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(value: InspectedShipSectionsModel | undefined): void {
        if (value !== null && value !== undefined) {
            this.form.get('shipControl')!.setValue(new ShipWithPersonnelModel({
                ship: value.ship,
                personnel: value.personnel,
                toggles: value.checks,
                port: value.port,
                observationTexts: value.observationTexts,
            }));
            this.form.get('permitLicensesControl')!.setValue(value.permitLicenses);
            this.form.get('permitsControl')!.setValue(value.permits);
            this.form.get('togglesControl')!.setValue(value.checks);
            this.form.get('catchTogglesControl')!.setValue(value.checks);
            this.form.get('fishingGearTogglesControl')!.setValue(value.checks);
            this.form.get('catchesControl')!.setValue(value.catches);
            this.form.get('logBooksControl')!.setValue(value.logBooks);
            this.form.get('fishingGearsControl')!.setValue(value.fishingGears);

            this.permitIds = value.permitLicenses
                ?.filter(f => f.checkValue === InspectionToggleTypesEnum.Y || f.checkValue === InspectionToggleTypesEnum.N)
                .map(f => f.permitLicenseId!) ?? [];

            const checks = value.checks ?? [];

            const membership = checks.find(f => f.checkTypeId === this.opMembership!.value);
            const notice = checks.find(f => f.checkTypeId === this.preliminaryNotice!.value);

            if (membership !== null && membership !== undefined) {
                this.form.get('opMembershipControl')!.setValue(membership);
                this.form.get('opMembershipAssociationControl')!.setValue(
                    this.associations.find(f => f.value === Number(membership.number))
                );
            }

            if (notice !== null && notice !== undefined) {
                this.form.get('preliminaryNoticeControl')!.setValue(notice);
                this.form.get('preliminaryNoticeNumberControl')!.setValue(notice.number);
                this.form.get('preliminaryNoticePurposeControl')!.setValue(notice.description);
            }

            const checkObservation = value.observationTexts?.find(f => f.category === this.checksObservationCategory);
            const catchObservation = value.observationTexts?.find(f => f.category === this.catchObservationCategory);
            const fishingGearObservation = value.observationTexts?.find(f => f.category === this.fishingGearObservationCategory);

            if (checkObservation !== null && checkObservation !== undefined) {
                this.form.get('checkObservationControl')!.setValue(checkObservation.text);
            }

            if (catchObservation !== null && catchObservation !== undefined) {
                this.form.get('catchObservationControl')!.setValue(catchObservation.text);
            }

            if (fishingGearObservation !== null && fishingGearObservation !== undefined) {
                this.form.get('fishingGearObservationControl')!.setValue(fishingGearObservation.text);
            }
        }
        else {
            this.form.get('shipControl')!.setValue(null);
            this.form.get('togglesControl')!.setValue(null);
            this.form.get('opMembershipControl')!.setValue(null);
            this.form.get('opMembershipAssociationControl')!.setValue(null);
            this.form.get('preliminaryNoticeControl')!.setValue(null);
            this.form.get('preliminaryNoticeNumberControl')!.setValue(null);
            this.form.get('preliminaryNoticePurposeControl')!.setValue(null);
            this.form.get('catchTogglesControl')!.setValue(null);
            this.form.get('catchesControl')!.setValue(null);
            this.form.get('fishingGearsControl')!.setValue(null);
            this.form.get('logBooksControl')!.setValue(null);
            this.permitIds = [];
        }
    }

    public async onShipSelected(ship: VesselDuringInspectionDTO | undefined): Promise<void> {
        if (ship === null || ship === undefined) {
            return;
        }

        this.element.nativeElement.focus();

        const result = await forkJoin([
            this.service.getShipPermits(ship.shipId!),
            this.service.getShipPermitLicenses(ship.shipId!),
            this.service.getShipLogBooks(ship.shipId!),
            this.service.getShipFishingGears(ship.shipId!),
        ]).toPromise();

        const permits: InspectionPermitLicenseDTO[] = result[0];
        const permitLicenses: InspectionPermitLicenseDTO[] = result[1];
        const logBooks: InspectionShipLogBookDTO[] = result[2];
        const fishingGears: FishingGearDTO[] = result[3];

        this.form.get('permitsControl')!.setValue(permits.map(f => new InspectionPermitDTO({
            from: f.validFrom,
            to: f.validTo,
            permitNumber: f.permitNumber,
            permitLicenseId: f.id,
            typeId: f.typeId,
            typeName: f.typeName,
        })));

        this.form.get('permitLicensesControl')!.setValue(permitLicenses.map(f => new InspectionPermitDTO({
            from: f.validFrom,
            to: f.validTo,
            licenseNumber: f.licenseNumber,
            permitNumber: f.permitNumber,
            permitLicenseId: f.id,
            typeId: f.typeId,
            typeName: f.typeName,
        })));

        this.form.get('logBooksControl')!.setValue(logBooks.map(f => new InspectionLogBookDTO({
            endPage: f.endPage,
            from: f.issuedOn,
            logBookId: f.id,
            number: f.number,
            pages: f.pages,
            startPage: f.startPage,
        })));

        this.form.get('fishingGearsControl')!.setValue(fishingGears.map(f => new InspectedFishingGearDTO({
            permittedFishingGear: f,
        })));
    }

    public onPermitIdsChanged(permitIds: number[]): void {
        this.permitIds = permitIds;
    }

    protected buildForm(): AbstractControl {
        const form = new FormGroup({
            shipControl: new FormControl(undefined),
            togglesControl: new FormControl(undefined),
            opMembershipControl: new FormControl(undefined),
            opMembershipAssociationControl: new FormControl({ value: undefined, disabled: true }),
            preliminaryNoticeControl: new FormControl(undefined),
            preliminaryNoticeNumberControl: new FormControl({ value: undefined, disabled: true }),
            preliminaryNoticePurposeControl: new FormControl({ value: undefined, disabled: true }),
            permitLicensesControl: new FormControl(undefined),
            permitsControl: new FormControl(undefined),
            logBooksControl: new FormControl(undefined),
            catchesControl: new FormControl(undefined),
            catchTogglesControl: new FormControl(undefined),
            fishingGearsControl: new FormControl(undefined),
            fishingGearTogglesControl: new FormControl(undefined),
            checkObservationControl: new FormControl(undefined),
            catchObservationControl: new FormControl(undefined),
            fishingGearObservationControl: new FormControl(undefined),
        });

        form.get('opMembershipControl')!.valueChanges.subscribe({
            next: this.onOPMembershipChanged.bind(this)
        });

        form.get('preliminaryNoticeControl')!.valueChanges.subscribe({
            next: this.onPreliminaryNoticeChanged.bind(this)
        });

        return form;
    }

    protected getValue(): InspectedShipSectionsModel | undefined {
        const ship: ShipWithPersonnelModel = this.form.get('shipControl')!.value;

        if (ship === null || ship === undefined) {
            return undefined;
        }

        const opMembership: InspectionCheckDTO = this.form.get('opMembershipControl')!.value;
        const notice: InspectionCheckDTO = this.form.get('preliminaryNoticeControl')!.value;

        if (opMembership !== null && opMembership !== undefined) {
            opMembership.number = this.form.get('opMembershipAssociationControl')!.value?.value?.toString();
        }

        if (notice !== null && notice !== undefined) {
            notice.number = this.form.get('preliminaryNoticeNumberControl')!.value;
            notice.description = this.form.get('preliminaryNoticePurposeControl')!.value;
        }

        const checks: InspectionCheckDTO[] = [
            ...(ship.toggles ?? []),
            ...(this.form.get('togglesControl')!.value ?? []),
            ...(this.form.get('catchTogglesControl')!.value ?? []),
            ...(this.form.get('fishingGearTogglesControl')!.value ?? []),
            opMembership,
            notice,
        ].filter(f => f !== null && f !== undefined);

        return new InspectedShipSectionsModel({
            ship: ship.ship,
            personnel: ship.personnel,
            checks: checks,
            port: ship.port,
            permitLicenses: this.form.get('permitLicensesControl')!.value,
            permits: this.form.get('permitsControl')!.value,
            catches: this.form.get('catchesControl')!.value,
            fishingGears: this.form.get('fishingGearsControl')!.value,
            logBooks: this.form.get('logBooksControl')!.value,
            observationTexts: [
                new InspectionObservationTextDTO({
                    category: this.checksObservationCategory,
                    text: this.form.get('checkObservationControl')!.value
                }),
                new InspectionObservationTextDTO({
                    category: this.catchObservationCategory,
                    text: this.form.get('catchObservationControl')!.value
                }),
                new InspectionObservationTextDTO({
                    category: this.fishingGearObservationCategory,
                    text: this.form.get('fishingGearObservationControl')!.value
                }),
                ...(ship.observationTexts ?? [])
            ].filter(f => f !== null && f !== undefined && !CommonUtils.isNullOrWhiteSpace(f.text)),
        });
    }

    private onOPMembershipChanged(check: InspectionCheckDTO | undefined): void {
        if (check?.checkValue !== InspectionToggleTypesEnum.Y) {
            this.form.get('opMembershipAssociationControl')!.disable();
        }
        else if (!this.isDisabled) {
            this.form.get('opMembershipAssociationControl')!.enable();
        }
    }

    private onPreliminaryNoticeChanged(check: InspectionCheckDTO | undefined): void {
        if (check?.checkValue !== InspectionToggleTypesEnum.Y) {
            this.form.get('preliminaryNoticeNumberControl')!.disable();
            this.form.get('preliminaryNoticePurposeControl')!.disable();
        }
        else if (!this.isDisabled) {
            this.form.get('preliminaryNoticeNumberControl')!.enable();
            this.form.get('preliminaryNoticePurposeControl')!.enable();
        }
    }
}
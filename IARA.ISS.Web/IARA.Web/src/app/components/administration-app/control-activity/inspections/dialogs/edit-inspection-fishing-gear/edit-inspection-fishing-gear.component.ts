import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { InspectionsService } from '@app/services/administration-app/inspections.service';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { BaseInspectionsComponent } from '../base-inspection.component';
import { InspectionGeneralInfoModel } from '../../models/inspection-general-info-model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { InspectionTypesEnum } from '@app/enums/inspection-types.enum';
import { InspectionAdditionalInfoModel } from '../../models/inspection-additional-info.model';
import { InspectionObservationCategoryEnum } from '@app/enums/inspection-observation-category.enum';
import { InspectedPersonTypeEnum } from '@app/enums/inspected-person-type.enum';
import { InspectionCheckModel } from '../../models/inspection-check.model';
import { InspectionObservationTextDTO } from '@app/models/generated/dtos/InspectionObservationTextDTO';
import { InspectionCheckToolMarkDTO } from '@app/models/generated/dtos/InspectionCheckToolMarkDTO';
import { InspectionSubjectEnum } from '@app/enums/inspection-subject.enum';
import { FishingGearDTO } from '@app/models/generated/dtos/FishingGearDTO';
import { InspectedFishingGearDTO } from '@app/models/generated/dtos/InspectedFishingGearDTO';
import { VesselDuringInspectionDTO } from '@app/models/generated/dtos/VesselDuringInspectionDTO';
import { InspectionShipSubjectNomenclatureDTO } from '@app/models/generated/dtos/InspectionShipSubjectNomenclatureDTO';
import { InspectionSubjectPersonnelDTO } from '@app/models/generated/dtos/InspectionSubjectPersonnelDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';

enum InspectionPermitTypeEnum {
    Registered,
    Unregistered,
}

@Component({
    selector: 'edit-inspection-fishing-gear',
    templateUrl: './edit-inspection-fishing-gear.component.html',
})
export class EditInspectionFishingGearComponent extends BaseInspectionsComponent implements OnInit, IDialogComponent {
    protected model: InspectionCheckToolMarkDTO = new InspectionCheckToolMarkDTO();

    public toggles: InspectionCheckModel[] = [];

    public isShip: boolean = true;
    public otherRemarkReasonSelected: boolean = false;

    public readonly inspectedTypes: NomenclatureDTO<InspectionSubjectEnum>[] = [];
    public readonly permitTypeControls: NomenclatureDTO<InspectionPermitTypeEnum>[] = [];

    public selectedPermitIds: number[] = [];

    public institutions: NomenclatureDTO<number>[] = [];
    public owners: NomenclatureDTO<number>[] = [];
    public ships: NomenclatureDTO<number>[] = [];
    public dalyans: NomenclatureDTO<number>[] = [];
    public markReasons: NomenclatureDTO<number>[] = [];
    public remarkReasons: NomenclatureDTO<number>[] = [];
    public permits: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public vesselTypes: NomenclatureDTO<number>[] = [];
    public ports: NomenclatureDTO<number>[] = [];

    public readonly inspectedPersonTypeEnum: typeof InspectedPersonTypeEnum = InspectedPersonTypeEnum;
    public readonly inspectionPermitTypeEnum: typeof InspectionPermitTypeEnum = InspectionPermitTypeEnum;

    public constructor(
        service: InspectionsService,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        snackbar: MatSnackBar
    ) {
        super(service, translate, nomenclatures, snackbar);
        this.inspectionCode = InspectionTypesEnum.IGM;

        this.inspectedTypes = [
            new NomenclatureDTO({
                value: InspectionSubjectEnum.Ship,
                displayName: translate.getValue('inspections.fishing-ship'),
                isActive: true,
            }),
            new NomenclatureDTO({
                value: InspectionSubjectEnum.Poundnet,
                displayName: translate.getValue('inspections.dalyan'),
                isActive: true,
            }),
        ];

        this.permitTypeControls = [
            new NomenclatureDTO({
                value: InspectionPermitTypeEnum.Registered,
                displayName: translate.getValue('inspections.registered-permit'),
                isActive: true,
            }),
            new NomenclatureDTO({
                value: InspectionPermitTypeEnum.Unregistered,
                displayName: translate.getValue('inspections.unregistered-permit'),
                isActive: true,
            }),
        ];
    }

    public async ngOnInit(): Promise<void> {
        if (this.viewMode) {
            this.form.disable();
        }

        const nomenclatureTables = await forkJoin([
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Institutions, this.nomenclatures.getInstitutions.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.PoundNets, this.nomenclatures.getPoundNets.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.MarkReasons, this.nomenclatures.getMarkReasons.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.RemarkReasons, this.nomenclatures.getRemarkReasons.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Countries, this.nomenclatures.getCountries.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.VesselTypes, this.nomenclatures.getVesselTypes.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Ports, this.nomenclatures.getPorts.bind(this.nomenclatures), false
            ),
            this.service.getCheckTypesForInspection(InspectionTypesEnum.IGM),
        ]).toPromise();

        this.institutions = nomenclatureTables[0];
        this.ships = nomenclatureTables[1];
        this.dalyans = nomenclatureTables[2];
        this.markReasons = nomenclatureTables[3];
        this.remarkReasons = nomenclatureTables[4];
        this.countries = nomenclatureTables[5];
        this.vesselTypes = nomenclatureTables[6];
        this.ports = nomenclatureTables[7];

        this.toggles = nomenclatureTables[8].map(f => new InspectionCheckModel(f));

        this.form.get('inspectedTypeControl')!.setValue(this.inspectedTypes[0]);
        this.form.get('permitTypeControl')!.setValue(this.permitTypeControls[0]);

        if (this.id !== null && this.id !== undefined) {
            this.service.get(this.id, this.inspectionCode).subscribe({
                next: (dto: InspectionCheckToolMarkDTO) => {
                    this.model = dto;

                    this.fillForm();
                }
            });
        }
    }

    protected buildForm(): void {
        this.form = new FormGroup({
            generalInfoControl: new FormControl(undefined),
            inspectedTypeControl: new FormControl(undefined),
            shipControl: new FormControl(undefined),
            shipOwnerControl: new FormControl(undefined, [Validators.required]),
            portControl: new FormControl(undefined),
            dalyanControl: new FormControl({ value: undefined, disabled: true }, [Validators.required]),
            markReasonControl: new FormControl(undefined, [Validators.required]),
            remarkReasonControl: new FormControl(undefined),
            otherRemarkReasonControl: new FormControl(undefined),
            fishingGearsControl: new FormControl([]),
            togglesControl: new FormControl([]),
            additionalInfoControl: new FormControl(undefined),
            filesControl: new FormControl([]),
            ownerCommentControl: new FormControl(undefined, Validators.maxLength(4000)),
            permitTypeControl: new FormControl(undefined, Validators.required),
            permitControl: new FormControl(undefined, [Validators.required]),
            unregisteredPermitControl: new FormControl(undefined, [Validators.required, Validators.maxLength(50)]),
            unregisteredPermitYearControl: new FormControl(undefined, [Validators.required, TLValidators.number(1900, 2100, 0)]),
        });

        this.form.get('unregisteredPermitControl')!.disable();
        this.form.get('unregisteredPermitYearControl')!.disable();

        this.form.get('remarkReasonControl')!.valueChanges.subscribe({
            next: this.onRemarkReasonChanged.bind(this)
        });

        this.form.get('inspectedTypeControl')!.valueChanges.subscribe({
            next: this.onInspectedTypeChanged.bind(this)
        });

        this.form.get('dalyanControl')!.valueChanges.subscribe({
            next: this.onDalyanChanged.bind(this)
        });

        this.form.get('permitControl')!.valueChanges.subscribe({
            next: this.onPermitChanged.bind(this)
        });

        this.form.get('permitTypeControl')!.valueChanges.subscribe({
            next: this.onPermitTypeChanged.bind(this)
        });
    }

    protected async fillForm(): Promise<void> {
        if (this.id !== undefined && this.id !== null) {
            this.form.get('generalInfoControl')!.setValue(new InspectionGeneralInfoModel({
                reportNum: this.model.reportNum,
                startDate: this.model.startDate,
                endDate: this.model.endDate,
                inspectors: this.model.inspectors,
                byEmergencySignal: this.model.byEmergencySignal,
            }));

            this.form.get('filesControl')!.setValue(this.model.files);

            this.form.get('additionalInfoControl')!.setValue(new InspectionAdditionalInfoModel({
                actionsTaken: this.model.actionsTaken,
                administrativeViolation: this.model.administrativeViolation,
                inspectorComment: this.model.inspectorComment,
                violation: this.model.observationTexts?.find(f => f.category === InspectionObservationCategoryEnum.AdditionalInfo),
                violatedRegulations: this.model.violatedRegulations,
            }));

            this.form.get('togglesControl')!.setValue(this.model.checks);

            this.form.get('shipControl')!.setValue(this.model.inspectedShip, { emitEvent: false });

            this.form.get('portControl')!.setValue(this.model.port);

            const poundNet = this.dalyans.find(f => f.value === this.model.poundNetId);

            this.form.get('dalyanControl')!.setValue(poundNet, { emitEvent: false });

            this.form.get('fishingGearsControl')!.setValue(this.model.fishingGears);

            this.form.get('markReasonControl')!.setValue(this.markReasons.find(f => f.value === this.model.checkReasonId));

            this.form.get('remarkReasonControl')!.setValue(this.remarkReasons.find(f => f.value === this.model.recheckReasonId));

            this.form.get('otherRemarkReasonControl')!.setValue(this.model.otherRecheckReason);

            this.form.get('ownerCommentControl')!.setValue(this.model.ownerComment);

            if (this.model.permitId) {
                this.form.get('permitTypeControl')!.setValue(this.permitTypeControls[0]);
            }
            else {
                this.form.get('permitTypeControl')!.setValue(this.permitTypeControls[1]);
                this.form.get('unregisteredPermitControl')!.setValue(this.model.unregisteredPermitNumber);
                this.form.get('unregisteredPermitYearControl')!.setValue(this.model.unregisteredPermitYear);
            }

            if (this.model.inspectedShip !== null && this.model.inspectedShip !== undefined) {
                this.form.get('inspectedTypeControl')!.setValue(this.inspectedTypes[0]);
                await this.onShipChanged(this.model.inspectedShip);

                setTimeout(() => {
                    this.form.get('permitControl')!.setValue(this.permits.find(f => f.value === this.model.permitId), { emitEvent: false });
                    this.form.get('fishingGearsControl')!.setValue(this.model.fishingGears, { emitEvent: false });

                    if (this.form.get('permitControl')!.value) {
                        this.selectedPermitIds = [this.form.get('permitControl')!.value.value];
                    }
                }, 2000);
            } else if (poundNet !== null && poundNet !== undefined) {
                this.form.get('inspectedTypeControl')!.setValue(this.inspectedTypes[1]);
                await this.onDalyanChanged(poundNet);
                this.form.get('permitControl')!.setValue(this.permits.find(f => f.value === this.model.permitId), { emitEvent: false });
                this.form.get('fishingGearsControl')!.setValue(this.model.fishingGears, { emitEvent: false });
            } else {
                this.form.get('inspectedTypeControl')!.setValue(this.inspectedTypes[0]);
            }
        }
    }

    protected fillModel(): void {
        const generalInfo: InspectionGeneralInfoModel = this.form.get('generalInfoControl')!.value;
        const additionalInfo: InspectionAdditionalInfoModel = this.form.get('additionalInfoControl')!.value;

        const personnel: InspectionSubjectPersonnelDTO[] = [];

        if (this.isShip) {
            personnel.push(this.form.get('shipOwnerControl')!.value);
        }

        this.model = new InspectionCheckToolMarkDTO({
            id: this.model.id,
            startDate: generalInfo.startDate,
            endDate: generalInfo.endDate,
            inspectors: generalInfo.inspectors,
            reportNum: generalInfo.reportNum,
            files: this.form.get('filesControl')!.value,
            actionsTaken: additionalInfo?.actionsTaken,
            administrativeViolation: additionalInfo?.administrativeViolation === true,
            byEmergencySignal: generalInfo.byEmergencySignal,
            inspectionType: InspectionTypesEnum.IGM,
            inspectorComment: additionalInfo?.inspectorComment,
            violatedRegulations: additionalInfo?.violatedRegulations,
            isActive: true,
            port: this.form.get('portControl')!.value,
            checks: this.form.get('togglesControl')!.value,
            fishingGears: this.form.get('fishingGearsControl')!.value,
            checkReasonId: this.form.get('markReasonControl')!.value?.value,
            recheckReasonId: this.form.get('remarkReasonControl')!.value?.value,
            permitId: this.form.controls.permitTypeControl.value?.value === InspectionPermitTypeEnum.Registered
                ? this.form.get('permitControl')!.value?.value
                : undefined,
            poundNetId: this.form.get('dalyanControl')!.value?.value,
            inspectedShip: this.form.get('shipControl')!.value,
            otherRecheckReason: this.form.get('otherRemarkReasonControl')!.value,
            unregisteredPermitNumber: this.form.get('unregisteredPermitControl')!.value,
            unregisteredPermitYear: this.form.get('unregisteredPermitYearControl')!.value,
            ownerComment: this.form.get('ownerCommentControl')!.value,
            personnel: personnel,
            observationTexts: [
                additionalInfo?.violation,
            ].filter(f => f !== null && f !== undefined) as InspectionObservationTextDTO[],
        });
    }

    private onInspectedTypeChanged(value: NomenclatureDTO<InspectionSubjectEnum>): void {
        if (this.isSaving) {
            return;
        }

        if (value !== null && value !== undefined) {
            if (value.value === InspectionSubjectEnum.Ship) {
                this.isShip = true;
                this.form.get('dalyanControl')!.disable();
                this.form.get('shipControl')!.enable();
                this.form.get('shipOwnerControl')!.enable();
                this.form.get('portControl')!.enable();
            }
            else {
                this.isShip = false;
                this.form.get('dalyanControl')!.enable();
                this.form.get('shipControl')!.disable();
                this.form.get('shipOwnerControl')!.disable();
                this.form.get('portControl')!.disable();
            }
        }

        if (this.viewMode === true) {
            this.form.get('dalyanControl')!.disable();
            this.form.get('shipControl')!.disable();
            this.form.get('shipOwnerControl')!.disable();
            this.form.get('portControl')!.disable();
        }

        this.selectedPermitIds = [];
        this.permits = [];

        this.form.get('dalyanControl')!.setValue(undefined);
        this.form.get('shipControl')!.setValue(undefined);
        this.form.get('shipOwnerControl')!.setValue(undefined);
        this.form.get('permitControl')!.setValue(undefined);
        this.form.get('fishingGearsControl')!.setValue([]);
    }

    private onRemarkReasonChanged(value: NomenclatureDTO<number>): void {
        if (this.isSaving) {
            return;
        }

        if (value?.code === 'Other') {
            this.otherRemarkReasonSelected = true;
            this.form.get('otherRemarkReasonControl')!.enable();
        } else {
            this.otherRemarkReasonSelected = false;
            this.form.get('otherRemarkReasonControl')!.disable();
        }

        if (this.viewMode === true) {
            this.form.get('otherRemarkReasonControl')!.disable();
        }
    }

    private async onShipChanged(value: VesselDuringInspectionDTO): Promise<void> {
        if (this.isSaving) {
            return;
        }

        if (value?.shipId) {
            const permits = await this.service.getShipPermitLicenses(value.shipId).toPromise();

            if (permits !== null && permits !== undefined) {
                this.permits = permits.map(f => new NomenclatureDTO({
                    value: f.id,
                    displayName: f.licenseNumber + ' (' + f.typeName + ')',
                    code: f.typeName,
                    isActive: true,
                }));
            } else {
                this.permits = [];
            }

            this.service.getShipPersonnel(value.shipId!).subscribe({
                next: (personnel: InspectionShipSubjectNomenclatureDTO[]) => {
                    this.owners = personnel.filter(f => f.type === InspectedPersonTypeEnum.OwnerLegal || f.type === InspectedPersonTypeEnum.OwnerPers);

                    setTimeout(() => {
                        if (this.model.personnel !== null && this.model.personnel !== undefined && this.model.inspectedShip?.shipId === value.shipId) {
                            this.form.get('shipOwnerControl')!.setValue(
                                this.model.personnel.find(f => f.type === InspectedPersonTypeEnum.OwnerLegal || f.type === InspectedPersonTypeEnum.OwnerPers)
                            );
                        }
                        else {
                            this.form.get('shipOwnerControl')!.setValue(null);
                        }
                    });
                }
            });
        } else {
            this.permits = [];
            this.form.get('shipOwnerControl')!.setValue(null);
        }

        this.selectedPermitIds = [];
        this.form.get('fishingGearsControl')!.setValue([]);
        this.form.get('permitControl')!.setValue(null, { emitEvent: false });
    }

    private async onDalyanChanged(value: NomenclatureDTO<number>): Promise<void> {
        if (this.isSaving) {
            return;
        }

        if (value?.value !== null && value?.value !== undefined) {
            const permits = await this.service.getPoundNetPermitLicenses(value.value!).toPromise();

            if (permits !== null && permits !== undefined) {
                this.permits = permits.map(f => new NomenclatureDTO({
                    value: f.id,
                    displayName: f.licenseNumber + ' (' + f.typeName + ')',
                    code: f.typeName,
                    isActive: true,
                }));
            } else {
                this.permits = [];
            }
        } else {
            this.permits = [];
        }

        this.form.get('fishingGearsControl')!.setValue([]);
        this.form.get('permitControl')!.setValue(null);
    }

    private async onPermitChanged(value: NomenclatureDTO<number>): Promise<void> {
        if (this.isSaving) {
            return;
        }

        if (value !== null && value !== undefined) {
            let fishingGears: FishingGearDTO[] | undefined = undefined;

            if (this.isShip) {
                const shipId = this.form.get('shipControl')!.value?.shipId;
                if (shipId !== null && shipId !== undefined) {
                    fishingGears = await this.service.getShipFishingGears(shipId).toPromise();
                }
            } else {
                const dalyanId = this.form.get('dalyanControl')!.value?.value;
                if (dalyanId !== null && dalyanId !== undefined) {
                    fishingGears = await this.service.getPoundNetFishingGears(dalyanId).toPromise();
                }
            }

            if (fishingGears !== null && fishingGears !== undefined) {
                this.form.get('fishingGearsControl')!.setValue(
                    fishingGears.map(f => new InspectedFishingGearDTO({
                        permittedFishingGear: f
                    }))
                );
                this.selectedPermitIds = [value.value!];
            } else {
                this.form.get('fishingGearsControl')!.setValue([]);
                this.selectedPermitIds = [];
            }
        } else {
            this.form.get('fishingGearsControl')!.setValue([]);
            this.selectedPermitIds = [];
        }
    }

    private onPermitTypeChanged(type: NomenclatureDTO<InspectionPermitTypeEnum>): void {
        if (this.form.disabled) {
            return;
        }

        if (type?.value === InspectionPermitTypeEnum.Registered) {
            this.form.get('permitControl')!.enable();
            this.form.get('unregisteredPermitControl')!.disable();
            this.form.get('unregisteredPermitYearControl')!.disable();
        }
        else {
            this.form.get('permitControl')!.disable();
            this.form.get('unregisteredPermitControl')!.enable();
            this.form.get('unregisteredPermitYearControl')!.enable();
        }
    }
}
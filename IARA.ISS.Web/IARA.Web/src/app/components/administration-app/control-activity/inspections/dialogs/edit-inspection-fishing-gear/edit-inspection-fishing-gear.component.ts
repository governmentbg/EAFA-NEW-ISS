import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
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
import { InspectionCheckToolMarkDTO } from '@app/models/generated/dtos/InspectionCheckToolMarkDTO';
import { InspectionSubjectEnum } from '@app/enums/inspection-subject.enum';
import { FishingGearDTO } from '@app/models/generated/dtos/FishingGearDTO';
import { InspectedFishingGearDTO } from '@app/models/generated/dtos/InspectedFishingGearDTO';
import { VesselDuringInspectionDTO } from '@app/models/generated/dtos/VesselDuringInspectionDTO';
import { InspectionShipSubjectNomenclatureDTO } from '@app/models/generated/dtos/InspectionShipSubjectNomenclatureDTO';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FishingGearCheckReasonsEnum } from '@app/enums/fishing-gear-check-reasons.enum';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { ChooseFromOldPermitLicenseComponent } from './choose-from-old-permit-license/choose-from-old-permit-license.component';
import { ChooseFromOldPermitLicenseDialogParamsModel } from '../../models/choose-from-old-permit-license.model';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { InspectedFishingGearEnum } from '@app/enums/inspected-fishing-gear.enum';

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
    public showPermitLicenseBtn: boolean = false;
    public otherRemarkReasonSelected: boolean = false;

    public readonly inspectedTypes: NomenclatureDTO<InspectionSubjectEnum>[] = [];
    public readonly permitTypeControls: NomenclatureDTO<InspectionPermitTypeEnum>[] = [];

    public selectedPermitIds: number[] = [];

    public institutions: NomenclatureDTO<number>[] = [];
    public owners: NomenclatureDTO<number>[] = [];
    public ships: ShipNomenclatureDTO[] = [];
    public poundNets: NomenclatureDTO<number>[] = [];
    public markReasons: NomenclatureDTO<number>[] = [];
    public remarkReasons: NomenclatureDTO<number>[] = [];
    public permits: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public vesselTypes: NomenclatureDTO<number>[] = [];
    public ports: NomenclatureDTO<number>[] = [];

    public readonly gearCheckNew: string = FishingGearCheckReasonsEnum[FishingGearCheckReasonsEnum.New];
    public readonly gearCheckRecheck: string = FishingGearCheckReasonsEnum[FishingGearCheckReasonsEnum.Recheck];
    public readonly gearCheckInspection: string = FishingGearCheckReasonsEnum[FishingGearCheckReasonsEnum.Inspection];
    public readonly inspectedPersonTypeEnum: typeof InspectedPersonTypeEnum = InspectedPersonTypeEnum;
    public readonly inspectionPermitTypeEnum: typeof InspectionPermitTypeEnum = InspectionPermitTypeEnum;

    private shipGears: Map<number, FishingGearDTO[]> = new Map<number, FishingGearDTO[]>();
    private poundNetGears: Map<number, FishingGearDTO[]> = new Map<number, FishingGearDTO[]>();

    private chooseFromPermitLicenseDialog: TLMatDialog<ChooseFromOldPermitLicenseComponent>;

    public constructor(
        service: InspectionsService,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        confirmDialog: TLConfirmDialog,
        chooseFromPermitLicenseDialog: TLMatDialog<ChooseFromOldPermitLicenseComponent>,
        snackbar: MatSnackBar
    ) {
        super(service, translate, nomenclatures, confirmDialog, snackbar);
        this.inspectionCode = InspectionTypesEnum.IGM;
        this.chooseFromPermitLicenseDialog = chooseFromPermitLicenseDialog;

        this.inspectedTypes = [
            new NomenclatureDTO({
                value: InspectionSubjectEnum.Ship,
                displayName: translate.getValue('inspections.fishing-ship'),
                isActive: true
            }),
            new NomenclatureDTO({
                value: InspectionSubjectEnum.Poundnet,
                displayName: translate.getValue('inspections.dalyan'),
                isActive: true
            })
        ];

        this.permitTypeControls = [
            new NomenclatureDTO({
                value: InspectionPermitTypeEnum.Registered,
                displayName: translate.getValue('inspections.registered-permit'),
                isActive: true
            }),
            new NomenclatureDTO({
                value: InspectionPermitTypeEnum.Unregistered,
                displayName: translate.getValue('inspections.unregistered-permit'),
                isActive: true
            })
        ];
    }

    public async ngOnInit(): Promise<void> {
        if (this.viewMode) {
            this.form.disable();
        }

        const nomenclatureTables = await forkJoin([
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Institutions, this.nomenclatures.getInstitutions.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.PoundNets, this.nomenclatures.getPoundNets.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.MarkReasons, this.nomenclatures.getMarkReasons.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.RemarkReasons, this.nomenclatures.getRemarkReasons.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Countries, this.nomenclatures.getCountries.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.VesselTypes, this.nomenclatures.getVesselTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Ports, this.nomenclatures.getPorts.bind(this.nomenclatures), false),
            this.service.getCheckTypesForInspection(InspectionTypesEnum.IGM)
        ]).toPromise();

        this.institutions = nomenclatureTables[0];
        this.ships = nomenclatureTables[1];
        this.poundNets = nomenclatureTables[2];
        this.markReasons = nomenclatureTables[3];
        this.remarkReasons = nomenclatureTables[4];
        this.countries = nomenclatureTables[5];
        this.vesselTypes = nomenclatureTables[6];
        this.ports = nomenclatureTables[7];

        this.toggles = nomenclatureTables[8].map(x => new InspectionCheckModel(x));

        this.form.get('inspectedTypeControl')!.setValue(this.inspectedTypes[0]);
        this.form.get('permitTypeControl')!.setValue(this.permitTypeControls[0]);

        if (this.id !== null && this.id !== undefined) {
            this.service.get(this.id, this.inspectionCode).subscribe({
                next: (inspection: InspectionCheckToolMarkDTO) => {
                    this.model = inspection;
                    this.fillForm();
                }
            });
        }
    }

    public chooseOldPermitLicense(): void {
        const editDialogData: ChooseFromOldPermitLicenseDialogParamsModel = new ChooseFromOldPermitLicenseDialogParamsModel({
            permitLicenses: this.permits,
            isShip: this.isShip
        });

        this.chooseFromPermitLicenseDialog.open({
            title: this.translate.getValue('inspections.choose-old-permit-license-dialog-title'),
            TCtor: ChooseFromOldPermitLicenseComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => { closeFn(); }
            },
            componentData: editDialogData,
            translteService: this.translate,
            disableDialogClose: true,
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: 'inspections.save-choose-old-permit-license-btn'
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: 'common.cancel',
            },
            viewMode: false
        }, '900px').subscribe({
            next: (chosenPermitLicenseId: number | undefined) => {
                if (chosenPermitLicenseId !== null && chosenPermitLicenseId !== undefined) {
                    this.selectedPermitIds = [chosenPermitLicenseId];

                    this.service.getPermitLicenseFishingGears(chosenPermitLicenseId).subscribe({
                        next: (fishingGears: FishingGearDTO[]) => {
                            if (fishingGears !== null && fishingGears !== undefined) {
                                const inspectedFishingGears: InspectedFishingGearDTO[] = this.mapFishingGears(fishingGears);
                                this.form.get('fishingGearsControl')!.setValue(inspectedFishingGears);
                            }
                            else {
                                this.form.get('fishingGearsControl')!.setValue([]);
                            }
                        }
                    });
                }
            }
        });
    }

    protected buildForm(): void {
        this.form = new FormGroup({
            generalInfoControl: new FormControl(undefined),
            inspectedTypeControl: new FormControl(undefined),
            shipControl: new FormControl(undefined),
            shipOwnerControl: new FormControl(undefined),
            portControl: new FormControl(undefined),
            dalyanControl: new FormControl(undefined),
            markReasonControl: new FormControl(undefined, Validators.required),
            remarkReasonControl: new FormControl(undefined),
            otherRemarkReasonControl: new FormControl(undefined, Validators.maxLength(500)),
            fishingGearsControl: new FormControl([]),
            togglesControl: new FormControl([]),
            additionalInfoControl: new FormControl(undefined),
            filesControl: new FormControl([]),
            ownerCommentControl: new FormControl(undefined, Validators.maxLength(4000)),
            permitTypeControl: new FormControl(undefined, Validators.required),
            permitControl: new FormControl(undefined),
            unregisteredPermitControl: new FormControl(undefined, Validators.maxLength(50)),
            unregisteredPermitYearControl: new FormControl(undefined)
        });

        this.form.get('generalInfoControl')!.valueChanges.subscribe({
            next: () => {
                this.reportNumAlreadyExistsError = false;
            }
        });

        this.form.get('remarkReasonControl')!.valueChanges.subscribe({
            next: this.onRemarkReasonChanged.bind(this)
        });

        this.form.get('inspectedTypeControl')!.valueChanges.subscribe({
            next: this.onInspectedTypeChanged.bind(this)
        });

        this.form.get('dalyanControl')!.valueChanges.subscribe({
            next: (poundNet: NomenclatureDTO<number> | undefined) => {
                if (poundNet !== undefined && poundNet !== null) {
                    this.getPermitLicensesByPoundNet(poundNet);
                }
            }
        });

        this.form.get('permitControl')!.valueChanges.subscribe({
            next: (permit: NomenclatureDTO<number> | undefined) => {
                this.onPermitChanged(permit);
            }
        });

        this.form.get('permitTypeControl')!.valueChanges.subscribe({
            next: this.onPermitTypeChanged.bind(this)
        });

        this.form.get('markReasonControl')!.valueChanges.subscribe({
            next: (reason: NomenclatureDTO<number> | undefined) => {
                if (reason && typeof reason !== 'string') {
                    if (reason.code === this.gearCheckNew) {
                        this.form.get('permitTypeControl')!.setValue(this.permitTypeControls.find(x => x.value === InspectionPermitTypeEnum.Unregistered));
                        this.form.get('permitTypeControl')!.disable();
                        this.form.get('permitControl')!.setValue(undefined);
                    }
                    else if (reason.code === this.gearCheckInspection) {
                        this.form.get('permitTypeControl')!.setValue(this.permitTypeControls.find(x => x.value === InspectionPermitTypeEnum.Registered));
                        this.form.get('permitTypeControl')!.disable();
                        this.form.get('unregisteredPermitControl')!.setValue(undefined);
                        this.form.get('unregisteredPermitYearControl')!.setValue(undefined);
                    }
                    else if (reason.code === this.gearCheckRecheck) {
                        this.form.get('permitTypeControl')!.setValue(this.permitTypeControls.find(x => x.value === InspectionPermitTypeEnum.Registered));
                        this.form.get('permitTypeControl')!.enable();
                        this.form.get('unregisteredPermitControl')!.setValue(undefined);
                        this.form.get('unregisteredPermitYearControl')!.setValue(undefined);
                    }
                }
                else {
                    this.form.get('permitTypeControl')!.enable();
                    this.form.get('permitTypeControl')!.setValue(undefined);
                }
            }
        });
    }

    protected fillForm(): void {
        if (this.id !== undefined && this.id !== null) {
            this.form.get('generalInfoControl')!.setValue(new InspectionGeneralInfoModel({
                reportNum: this.model.reportNum,
                startDate: this.model.startDate,
                endDate: this.model.endDate,
                inspectors: this.model.inspectors,
                byEmergencySignal: this.model.byEmergencySignal
            }));

            this.form.get('additionalInfoControl')!.setValue(new InspectionAdditionalInfoModel({
                actionsTaken: this.model.actionsTaken,
                administrativeViolation: this.model.administrativeViolation,
                inspectorComment: this.model.inspectorComment,
                violation: this.model.observationTexts?.find(x => x.category === InspectionObservationCategoryEnum.AdditionalInfo),
                violatedRegulations: this.model.violatedRegulations
            }));

            this.form.get('togglesControl')!.setValue(this.model.checks);
            this.form.get('shipControl')!.setValue(this.model.inspectedShip);
            this.form.get('portControl')!.setValue(this.model.port);

            if (this.model.personnel !== undefined && this.model.personnel !== null) {
                this.form.get('shipOwnerControl')!.setValue(this.model.personnel.find(x => x.type === InspectedPersonTypeEnum.OwnerLegal || x.type === InspectedPersonTypeEnum.OwnerPers));
            }

            if (this.model.permitId) {
                this.form.get('permitTypeControl')!.setValue(this.permitTypeControls[0]);
            }
            else {
                this.form.get('permitTypeControl')!.setValue(this.permitTypeControls[1]);
                this.form.get('unregisteredPermitControl')!.setValue(this.model.unregisteredPermitNumber);

                if (this.model.unregisteredPermitYear !== undefined && this.model.unregisteredPermitYear !== null) {
                    this.form.get('unregisteredPermitYearControl')!.setValue(new Date(this.model.unregisteredPermitYear, 0, 1));
                }
            }

            const poundNet: NomenclatureDTO<number> | undefined = this.poundNets.find(x => x.value === this.model.poundNetId);
            if (poundNet !== null && poundNet !== undefined) {
                this.form.get('dalyanControl')!.setValue(poundNet);
                this.form.get('inspectedTypeControl')!.setValue(this.inspectedTypes[1]);
                this.getPermitLicensesByPoundNet(poundNet);
            }
            else if (this.model.inspectedShip !== null && this.model.inspectedShip !== undefined) {
                this.form.get('inspectedTypeControl')!.setValue(this.inspectedTypes[0]);
                this.getPermitLicensesByInspectedShip(this.model.inspectedShip);
            }
            else {
                this.form.get('inspectedTypeControl')!.setValue(this.inspectedTypes[0]);
            }

            this.form.get('remarkReasonControl')!.setValue(this.remarkReasons.find(x => x.value === this.model.recheckReasonId));
            this.form.get('markReasonControl')!.setValue(this.markReasons.find(x => x.value === this.model.checkReasonId));
            this.form.get('otherRemarkReasonControl')!.setValue(this.model.otherRecheckReason);
            this.form.get('ownerCommentControl')!.setValue(this.model.ownerComment);
            this.form.get('fishingGearsControl')!.setValue(this.model.fishingGears);
            this.form.get('filesControl')!.setValue(this.model.files);
        }
    }

    protected fillModel(): void {
        const generalInfo: InspectionGeneralInfoModel = this.form.get('generalInfoControl')!.value;
        const additionalInfo: InspectionAdditionalInfoModel | undefined = this.form.get('additionalInfoControl')!.value;

        this.model.inspectionType = InspectionTypesEnum.IGM;
        this.model.startDate = generalInfo.startDate;
        this.model.endDate = generalInfo.endDate;
        this.model.inspectors = generalInfo.inspectors;
        this.model.reportNum = generalInfo.reportNum;
        this.model.actionsTaken = additionalInfo?.actionsTaken;
        this.model.administrativeViolation = additionalInfo?.administrativeViolation === true;
        this.model.byEmergencySignal = generalInfo.byEmergencySignal;
        this.model.inspectorComment = additionalInfo?.inspectorComment;
        this.model.violatedRegulations = additionalInfo?.violatedRegulations;

        this.model.port = this.form.get('portControl')!.value;
        this.model.checks = this.form.get('togglesControl')!.value;
        this.model.fishingGears = this.form.get('fishingGearsControl')!.value;
        this.model.checkReasonId = this.form.get('markReasonControl')!.value?.value;
        this.model.recheckReasonId = this.form.get('remarkReasonControl')!.value?.value;
        this.model.permitId = this.form.controls.permitTypeControl.value?.value === InspectionPermitTypeEnum.Registered ? this.form.get('permitControl')!.value?.value : undefined;
        this.model.otherRecheckReason = this.form.get('otherRemarkReasonControl')!.value;
        this.model.unregisteredPermitNumber = this.form.get('unregisteredPermitControl')!.value;
        this.model.ownerComment = this.form.get('ownerCommentControl')!.value;
        this.model.files = this.form.get('filesControl')!.value;
        this.model.isActive = true;

        this.model.personnel = [];
        if (this.isShip) {
            this.model.personnel.push(this.form.get('shipOwnerControl')!.value);
        }

        this.model.observationTexts = [];
        if (additionalInfo?.violation !== null && additionalInfo?.violation !== undefined) {
            this.model.observationTexts.push(additionalInfo.violation);
        }

        const year: Date | undefined = this.form.get('unregisteredPermitYearControl')!.value;
        if (year !== undefined && year !== null) {
            this.model.unregisteredPermitYear = (year as Date).getFullYear();
        }
        else {
            this.model.unregisteredPermitYear = undefined;
        }

        const type: InspectionSubjectEnum = this.form.get('inspectedTypeControl')!.value?.value;
        if (type === InspectionSubjectEnum.Ship) {
            this.model.inspectedShip = this.form.get('shipControl')!.value;
            this.model.poundNetId = undefined;
        }
        else if (type === InspectionSubjectEnum.Poundnet) {
            this.model.poundNetId = this.form.get('dalyanControl')!.value?.value;
            this.model.inspectedShip = undefined;
        }
    }

    private onInspectedTypeChanged(value: NomenclatureDTO<InspectionSubjectEnum>): void {
        if (!this.isSaving) {
            if (value !== null && value !== undefined) {
                if (value.value === InspectionSubjectEnum.Ship) {
                    this.isShip = true;
                    this.form.get('dalyanControl')!.disable();
                    this.form.get('shipControl')!.enable();
                    this.form.get('shipOwnerControl')!.enable();
                    this.form.get('portControl')!.enable();
                    this.form.get('dalyanControl')!.setValue(undefined);

                    this.form.get('shipOwnerControl')!.setValidators(Validators.required);
                    this.form.get('shipOwnerControl')!.markAsPending();
                }
                else {
                    this.isShip = false;
                    this.form.get('dalyanControl')!.enable();
                    this.form.get('shipControl')!.disable();
                    this.form.get('shipOwnerControl')!.disable();
                    this.form.get('portControl')!.disable();

                    this.form.get('dalyanControl')!.setValidators(Validators.required);
                    this.form.get('dalyanControl')!.markAsPending();

                    this.form.get('shipControl')!.setValue(undefined);
                    this.form.get('shipOwnerControl')!.setValue(undefined);
                    this.form.get('permitControl')!.setValue(undefined);
                    this.form.get('portControl')!.setValue(undefined);
                    this.form.get('fishingGearsControl')!.setValue([]);
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
        }
    }

    private onRemarkReasonChanged(value: NomenclatureDTO<number>): void {
        if (!this.isSaving) {
            if (value?.code === 'Other') {
                this.otherRemarkReasonSelected = true;
                this.form.get('otherRemarkReasonControl')!.enable();
            }
            else {
                this.otherRemarkReasonSelected = false;
                this.form.get('otherRemarkReasonControl')!.disable();
            }

            if (this.viewMode === true) {
                this.form.get('otherRemarkReasonControl')!.disable();
            }
        }
    }

    private getPermitLicensesByInspectedShip(value: VesselDuringInspectionDTO): void {
        if (!this.isSaving) {
            this.selectedPermitIds = [];
            this.owners = [];
            this.form.get('fishingGearsControl')!.setValue([]);
            this.form.get('permitControl')!.setValue(null);
            this.form.get('shipOwnerControl')!.setValue(null);

            if (value !== undefined && value !== null) {
                if (value.shipId !== undefined && value.shipId !== null && value.isRegistered) {
                    this.service.getShipPermitLicenses(value.shipId, true).subscribe({
                        next: (permits: NomenclatureDTO<number>[]) => {
                            if (permits !== null && permits !== undefined) {
                                this.permits = permits;

                                this.form.get('permitControl')!.setValue(permits.find(x => x.value === this.model.permitId));
                                this.form.get('fishingGearsControl')!.setValue(this.model.fishingGears);

                                if (this.form.get('permitControl')!.value) {
                                    this.selectedPermitIds = [this.form.get('permitControl')!.value.value];
                                }
                            }
                            else {
                                this.permits = [];
                            }
                        }
                    });

                    this.service.getShipPersonnel(value.shipId).subscribe({
                        next: (personnel: InspectionShipSubjectNomenclatureDTO[]) => {
                            this.owners = personnel.filter(x => x.type === InspectedPersonTypeEnum.OwnerLegal || x.type === InspectedPersonTypeEnum.OwnerPers);

                            if (this.model.personnel !== null && this.model.personnel !== undefined && this.model.inspectedShip?.shipId === value.shipId) {
                                this.form.get('shipOwnerControl')!.setValue(this.model.personnel.find(x => x.type === InspectedPersonTypeEnum.OwnerLegal || x.type === InspectedPersonTypeEnum.OwnerPers));
                            }
                        }
                    });
                }
                else {
                    this.permits = [];

                    if (this.model.personnel !== null && this.model.personnel !== undefined && this.model.inspectedShip?.shipId === value.shipId) {
                        this.form.get('shipOwnerControl')!.setValue(this.model.personnel.find(x => x.type === InspectedPersonTypeEnum.OwnerLegal || x.type === InspectedPersonTypeEnum.OwnerPers));
                    }
                }
            }
        }
    }

    private getPermitLicensesByPoundNet(value: NomenclatureDTO<number>): void {
        if (!this.isSaving) {
            if (value?.value !== null && value?.value !== undefined) {
                this.service.getPoundNetPermitLicenses(value.value!).subscribe({
                    next: (permits: NomenclatureDTO<number>[]) => {
                        if (permits !== null && permits !== undefined) {
                            this.permits = permits;
                            this.selectedPermitIds = [value.value!];

                            this.form.get('permitControl')!.setValue(permits.find(x => x.value === this.model.permitId));
                            this.form.get('fishingGearsControl')!.setValue(this.model.fishingGears);

                            if (this.form.get('permitControl')!.value) {
                                this.selectedPermitIds = [this.form.get('permitControl')!.value.value];
                            }
                        }
                        else {
                            this.permits = [];
                            this.selectedPermitIds = [];
                        }
                    }
                });
            }
            else {
                this.permits = [];
                this.selectedPermitIds = [];
            }

            this.form.get('fishingGearsControl')!.setValue([]);
            this.form.get('permitControl')!.setValue(null);
        }
    }

    private onPermitChanged(value: NomenclatureDTO<number> | undefined): void {
        if (!this.isSaving) {
            if (value !== null && value !== undefined) {
                if (this.isShip) {
                    const shipId: number | undefined = this.form.get('shipControl')!.value.shipId;

                    if (shipId !== null && shipId !== undefined) {
                        const existingGears: FishingGearDTO[] | undefined = this.shipGears.get(shipId);

                        if (existingGears !== undefined) {
                            this.form.get('fishingGearsControl')!.setValue(
                                existingGears.map(x => new InspectedFishingGearDTO({
                                    permittedFishingGear: x
                                }))
                            );

                            this.selectedPermitIds = [value.value!];
                        }
                        else {
                            this.shipGears.set(shipId, []);

                            this.service.getShipFishingGears(shipId).subscribe({
                                next: (fishingGears: FishingGearDTO[]) => {
                                    this.shipGears.set(shipId, fishingGears ?? []);

                                    if (fishingGears !== null && fishingGears !== undefined) {
                                        this.form.get('fishingGearsControl')!.setValue(
                                            fishingGears.map(x => new InspectedFishingGearDTO({
                                                permittedFishingGear: x
                                            }))
                                        );

                                        this.selectedPermitIds = [value.value!];
                                    }
                                    else {
                                        this.form.get('fishingGearsControl')!.setValue([]);
                                    }
                                }
                            });
                        }
                    }
                }
                else {
                    const poundNetId: number | undefined = this.form.get('dalyanControl')!.value.value;

                    if (poundNetId !== null && poundNetId !== undefined) {
                        const existingGears: FishingGearDTO[] | undefined = this.poundNetGears.get(poundNetId);

                        if (existingGears !== undefined) {
                            this.form.get('fishingGearsControl')!.setValue(
                                existingGears.map(x => new InspectedFishingGearDTO({
                                    permittedFishingGear: x
                                }))
                            );

                            this.selectedPermitIds = [value.value!];
                        }
                        else {
                            this.poundNetGears.set(poundNetId, []);

                            this.service.getPoundNetFishingGears(poundNetId).subscribe({
                                next: (fishingGears: FishingGearDTO[]) => {
                                    this.poundNetGears.set(poundNetId, fishingGears ?? []);

                                    if (fishingGears !== null && fishingGears !== undefined) {
                                        this.form.get('fishingGearsControl')!.setValue(
                                            fishingGears.map(x => new InspectedFishingGearDTO({
                                                permittedFishingGear: x
                                            }))
                                        );

                                        this.selectedPermitIds = [value.value!];
                                    }
                                    else {
                                        this.form.get('fishingGearsControl')!.setValue([]);
                                    }
                                }
                            });
                        }
                    }
                }
            }
            else {
                this.form.get('fishingGearsControl')!.setValue([]);
                this.selectedPermitIds = [];
            }
        }
    }

    private onPermitTypeChanged(type: NomenclatureDTO<InspectionPermitTypeEnum>): void {
        if (!this.form.disabled) {
            this.showPermitLicenseBtn = false;

            if (type?.value === InspectionPermitTypeEnum.Registered) {
                this.form.get('permitControl')!.enable();
                this.form.get('permitControl')!.setValidators([Validators.required, this.noPermitLicensesValidator()]);
                this.form.get('permitControl')!.markAsPending();

                this.form.get('unregisteredPermitControl')!.setValue(undefined);
                this.form.get('unregisteredPermitYearControl')!.setValue(undefined);

                this.form.get('unregisteredPermitControl')!.disable();
                this.form.get('unregisteredPermitYearControl')!.disable();
            }
            else if (type?.value === InspectionPermitTypeEnum.Unregistered) {
                this.showPermitLicenseBtn = true;

                this.form.get('permitControl')!.setValue(undefined);
                this.form.get('permitControl')!.disable();

                this.form.get('unregisteredPermitControl')!.enable();
                this.form.get('unregisteredPermitYearControl')!.enable();

                this.form.get('unregisteredPermitControl')!.setValidators([Validators.required, Validators.maxLength(50)]);
                this.form.get('unregisteredPermitYearControl')!.setValidators(Validators.required);
                this.form.get('unregisteredPermitControl')!.markAsPending();
                this.form.get('unregisteredPermitYearControl')!.markAsPending();
            }
        }
    }

    private mapFishingGears(fishingGears: FishingGearDTO[]): InspectedFishingGearDTO[] {
        //Ако към нерегистрирано удостоверение се добавят уреди от старо УСР, те трябва да се запазят като инспектирани и с проверка от тип "Нерегистриран"
        const result: InspectedFishingGearDTO[] = fishingGears.map(x => new InspectedFishingGearDTO({
            inspectedFishingGear: new FishingGearDTO({
                typeId: x.typeId,
                type: x.type,
                netEyeSize: x.netEyeSize,
                cordThickness: x.cordThickness,
                description: x.description,
                count: x.count,
                length: x.length,
                height: x.height,
                hasPingers: x.hasPingers,
                hookCount: x.hookCount,
                houseLength: x.houseLength,
                houseWidth: x.houseWidth,
                lineCount: x.lineCount,
                netsInFleetCount: x.netsInFleetCount,
                netNominalLength: x.netNominalLength,
                towelLength: x.towelLength,
                trawlModel: x.trawlModel,
                marks: x.marks,
                marksNumbers: x.marksNumbers,
                pingers: x.pingers,
                isActive: x.isActive,
                id: undefined
            }),
            checkInspectedMatchingRegisteredGear: InspectedFishingGearEnum.I
        }));

        return result;
    }

    private noPermitLicensesValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            if (this.permits === undefined || this.permits === null || this.permits.length === 0) {
                return { 'novaluesindropdown': true };
            }

            return null;
        };
    }
}
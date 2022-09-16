import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { IFluxVmsRequestsService } from '@app/interfaces/administration-app/flux-vms-requests.interface';
import { FluxFlapRequestEditDTO } from '@app/models/generated/dtos/FluxFlapRequestEditDTO';
import { FluxVmsRequestsService } from '@app/services/administration-app/flux-vms-requests.service';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { FlapRequestPurposeCodes } from '@app/enums/flap-request-purpose-codes.enum';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { FishingGearNomenclatureDTO } from '@app/models/generated/dtos/FishingGearNomenclatureDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { FishNomenclatureDTO } from '@app/models/generated/dtos/FishNomenclatureDTO';
import { FluxFlapRequestTargetedQuotaDTO } from '@app/models/generated/dtos/FluxFlapRequestTargetedQuotaDTO';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { ShipsUtils } from '@app/shared/utils/ships.utils';
import { FluxFlapRequestShipDTO } from '@app/models/generated/dtos/FluxFlapRequestShipDTO';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { CommandTypes } from '@app/shared/components/data-table/enums/command-type.enum';

@Component({
    selector: 'edit-flux-flap-request',
    templateUrl: './edit-flux-flap-request.component.html'
})
export class EditFluxFlapRequestComponent implements IDialogComponent, OnInit, AfterViewInit {
    public form: FormGroup;
    public joinedShipControl: FormControl;
    public authorizedGearControl: FormControl;
    public targetedQuotasGroup: FormGroup;

    public viewMode: boolean = false;
    public editing: boolean = false;
    public isOutgoing: boolean = true;

    public authorizedGearsTouched: boolean = false;
    public targetedQuotaTouched: boolean = false;

    public agreementTypes: NomenclatureDTO<number>[] = [];
    public coastalParties: NomenclatureDTO<number>[] = [];
    public requestPurposes: NomenclatureDTO<number>[] = [];
    public fishingCategories: NomenclatureDTO<number>[] = [];
    public ships: ShipNomenclatureDTO[] = [];
    public gears: FishingGearNomenclatureDTO[] = [];
    public species: FishNomenclatureDTO[] = [];
    public quotaTypes: NomenclatureDTO<number>[] = [];

    public joinedShips: ShipNomenclatureDTO[] = [];
    public incomingJoinedShips: FluxFlapRequestShipDTO[] = [];
    public authorizedGears: FishingGearNomenclatureDTO[] = [];
    public targetedQuotas: FluxFlapRequestTargetedQuotaDTO[] = [];

    @ViewChild('targetedQuotasTable')
    private targetedQuotasTable!: TLDataTableComponent;

    private id: number | undefined;
    private model!: FluxFlapRequestEditDTO;

    private readonly service: IFluxVmsRequestsService;
    private readonly nomenclatures: CommonNomenclatures;

    public constructor(service: FluxVmsRequestsService, nomenclatures: CommonNomenclatures) {
        this.service = service;
        this.nomenclatures = nomenclatures;

        this.form = this.buildForm();
        this.joinedShipControl = new FormControl();
        this.authorizedGearControl = new FormControl();
        this.targetedQuotasGroup = this.buildTargetedQuotasGroup();

        this.registerValueChanges();
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures = await forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.FishingGear, this.nomenclatures.getFishingGear.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Fishes, this.nomenclatures.getFishTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.FlapAgreementTypes, this.service.getAgreementTypes.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.FlapCoastalParties, this.service.getCoastalParties.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.FlapRequestPurposes, this.service.getRequestPurposes.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.FlapFishingCategories, this.service.getFishingCategories.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.FlapQuotaType, this.service.getFlapQuotaTypes.bind(this.service), false)
        ).toPromise();

        this.ships = nomenclatures[0];
        this.gears = nomenclatures[1];
        this.species = nomenclatures[2];
        this.agreementTypes = nomenclatures[3];
        this.coastalParties = nomenclatures[4];
        this.requestPurposes = nomenclatures[5];
        this.fishingCategories = nomenclatures[6];
        this.quotaTypes = nomenclatures[7];

        if (this.id !== undefined && this.id !== null) {
            this.service.getFlapRequest(this.id).subscribe({
                next: (request: FluxFlapRequestEditDTO) => {
                    this.model = request;
                    this.isOutgoing = this.model.isOutgoing!;

                    if (this.isOutgoing) {
                        this.form.get('shipControl')!.setValidators(Validators.required);
                        this.form.get('shipControl')!.markAsPending({ emitEvent: false });
                    }

                    this.fillForm();
                }
            });
        }
        else {
            this.model = new FluxFlapRequestEditDTO();
        }
    }

    public ngAfterViewInit(): void {
        this.targetedQuotasTable?.recordChanged.subscribe({
            next: (event: RecordChangedEventArgs<FluxFlapRequestTargetedQuotaDTO>) => {
                this.targetedQuotaTouched = true;

                if (event.Command !== CommandTypes.Edit) {
                    this.form.updateValueAndValidity({ onlySelf: true });
                }
            }
        });

        this.form.setValidators([this.authorizedGearsValidator(), this.targetedQuotasValidator()]);
        this.form.updateValueAndValidity({ onlySelf: true });
    }

    public setData(data: DialogParamsModel | undefined, wrapperData: DialogWrapperData): void {
        this.id = data?.id;
        this.viewMode = data?.viewMode ?? false;
        this.editing = this.id !== undefined && this.id !== null;
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.viewMode) {
            dialogClose();
        }
        else {
            if (this.id === undefined || this.id === null) {
                this.markAllAsTouched();

                if (this.form.valid) {
                    this.fillModel();
                    CommonUtils.sanitizeModelStrings(this.model);

                    this.service.addFlapRequest(this.model).subscribe({
                        next: () => {
                            dialogClose(this.model);
                        }
                    });
                }
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public deleteJoinedShip(ship: ShipNomenclatureDTO): void {
        setTimeout(() => {
            this.joinedShips = this.joinedShips.filter(x => x.value !== ship.value);
        });
    }

    public deleteAuthorizedGear(gear: FishingGearNomenclatureDTO): void {
        setTimeout(() => {
            this.authorizedGears = this.authorizedGears.filter(x => x.value !== gear.value);
            this.authorizedGearsTouched = true;
            this.form.updateValueAndValidity({ onlySelf: true });
        });
    }

    private buildForm(): FormGroup {
        const group: FormGroup = new FormGroup({
            isOutgoingControl: new FormControl(undefined),
            agreementTypeControl: new FormControl(undefined, Validators.required),
            coastalPartyControl: new FormControl(undefined, Validators.required),
            requestPurposeControl: new FormControl(undefined, Validators.required),
            requestPurposeTextControl: new FormControl(undefined, Validators.maxLength(4000)),
            fishingCategoryControl: new FormControl(undefined, Validators.required),
            fishingMethodControl: new FormControl(undefined, [Validators.required, Validators.maxLength(4000)]),
            fishingAreaControl: new FormControl(undefined, [Validators.required, Validators.maxLength(4000)]),
            shipControl: new FormControl(undefined),
            shipIdentifierTypeControl: new FormControl(undefined),
            shipIdentifierControl: new FormControl(undefined),
            shipNameControl: new FormControl(undefined),
            isFirstApplicationControl: new FormControl(false),
            remarksControl: new FormControl(undefined, Validators.maxLength(4000)),
            localSeamenCountControl: new FormControl(undefined, TLValidators.number(0, undefined, 0)),
            acpSeamenCountControl: new FormControl(undefined, TLValidators.number(0, undefined, 0)),
            authorizationDateControl: new FormControl(undefined, Validators.required)
        });

        group.get('isOutgoingControl')!.disable({ emitEvent: false });

        return group;
    }

    private buildTargetedQuotasGroup(): FormGroup {
        return new FormGroup({
            flapQuotaTypeCodeControl: new FormControl(undefined, Validators.required),
            speciesCodeControl: new FormControl(undefined, Validators.required),
            tonnageControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)])
        });
    }

    private registerValueChanges(): void {
        this.form.get('requestPurposeControl')!.valueChanges.subscribe({
            next: (purpose: NomenclatureDTO<number> | string | undefined) => {
                if (purpose !== undefined && purpose !== null && typeof purpose !== 'string' && purpose.code === FlapRequestPurposeCodes[FlapRequestPurposeCodes.TER]) {
                    this.form.get('requestPurposeTextControl')!.setValidators([Validators.required, Validators.maxLength(4000)]);
                }
                else {
                    this.form.get('requestPurposeTextControl')!.setValidators([Validators.maxLength(4000)]);
                }

                this.form.get('requestPurposeTextControl')!.markAsPending({ emitEvent: false });
                this.form.get('requestPurposeTextControl')!.updateValueAndValidity({ emitEvent: false });

                if (this.viewMode) {
                    this.form.get('requestPurposeTextControl')!.disable({ emitEvent: false });
                }
            }
        });

        // Joined ships
        this.joinedShipControl.valueChanges.subscribe({
            next: (ship: ShipNomenclatureDTO | string | undefined) => {
                if (ship !== undefined && ship !== null && typeof ship !== 'string') {
                    if (!this.joinedShips.includes(ship)) {
                        this.joinedShips.push(ship);

                        setTimeout(() => {
                            this.joinedShips = this.joinedShips.slice();
                        });
                    }

                    this.ships = this.ships.slice();
                    this.joinedShipControl.setValue(undefined);
                }
            }
        });

        // Authorized gear
        this.authorizedGearControl.valueChanges.subscribe({
            next: (gear: FishingGearNomenclatureDTO | string | undefined) => {
                if (gear !== undefined && gear !== null && typeof gear !== 'string') {
                    if (!this.authorizedGears.includes(gear)) {
                        this.authorizedGears.push(gear);

                        setTimeout(() => {
                            this.authorizedGears = this.authorizedGears.slice();
                            this.authorizedGearsTouched = true;
                            this.form.updateValueAndValidity({ onlySelf: false });
                        });
                    }

                    this.gears = this.gears.slice();
                    this.authorizedGearControl.setValue(undefined);
                }
            }
        });
    }

    private fillForm(): void {
        this.form.get('isOutgoingControl')!.setValue(this.model.isOutgoing);
        this.form.get('agreementTypeControl')!.setValue(this.agreementTypes.find(x => x.code === this.model.agreementTypeCode));
        this.form.get('coastalPartyControl')!.setValue(this.coastalParties.find(x => x.code === this.model.coastalPartyCode));
        this.form.get('requestPurposeControl')!.setValue(this.requestPurposes.find(x => x.code === this.model.requestPurposeCode));
        this.form.get('requestPurposeTextControl')!.setValue(this.model.requestPurposeText);
        this.form.get('fishingCategoryControl')!.setValue(this.fishingCategories.find(x => x.code === this.model.fishingCategoryCode));
        this.form.get('fishingMethodControl')!.setValue(this.model.fishingMethod);
        this.form.get('fishingAreaControl')!.setValue(this.model.fishingArea);
        this.form.get('isFirstApplicationControl')!.setValue(this.model.isFirstApplication ?? false);
        this.form.get('remarksControl')!.setValue(this.model.remarks);
        this.form.get('localSeamenCountControl')!.setValue(this.model.localSeamenCount);
        this.form.get('acpSeamenCountControl')!.setValue(this.model.acpSeamenCount);
        this.form.get('authorizationDateControl')!.setValue(new DateRangeData({ start: this.model.authorizationStartDate, end: this.model.authorizationEndDate }));

        if (this.isOutgoing) {
            this.form.get('shipControl')!.setValue(ShipsUtils.get(this.ships, this.model.ship!.shipId!));
        }
        else {
            this.form.get('shipIdentifierTypeControl')!.setValue(this.model.ship!.shipIdentifierType);
            this.form.get('shipIdentifierControl')!.setValue(this.model.ship!.shipIdentifier);
            this.form.get('shipNameControl')!.setValue(this.model.ship!.shipName);
        }

        setTimeout(() => {
            if (this.isOutgoing) {
                this.joinedShips = (this.model.joinedShips ?? []).map((ship: FluxFlapRequestShipDTO) => ShipsUtils.get(this.ships, ship.shipId!));
            }
            else {
                this.incomingJoinedShips = this.model.joinedShips ?? [];
            }

            this.authorizedGears = this.gears.filter(x => (this.model.authorizedFishingGearCodes ?? []).includes(x.code!));
            this.targetedQuotas = this.model.targetedQuotas ?? [];
        });

        if (this.viewMode) {
            this.form.disable();
            this.joinedShipControl.disable();
            this.authorizedGearControl.disable();
        }
    }

    private fillModel(): void {
        this.model.agreementTypeCode = this.form.get('agreementTypeControl')!.value.code;
        this.model.coastalPartyCode = this.form.get('coastalPartyControl')!.value.code;
        this.model.requestPurposeCode = this.form.get('requestPurposeControl')!.value.code;
        this.model.requestPurposeText = this.form.get('requestPurposeTextControl')!.value;
        this.model.fishingCategoryCode = this.form.get('fishingCategoryControl')!.value.code;
        this.model.fishingMethod = this.form.get('fishingMethodControl')!.value;
        this.model.fishingArea = this.form.get('fishingAreaControl')!.value;
        this.model.isFirstApplication = this.form.get('isFirstApplicationControl')!.value ?? false;
        this.model.remarks = this.form.get('remarksControl')!.value;
        this.model.localSeamenCount = this.form.get('localSeamenCountControl')!.value;
        this.model.acpSeamenCount = this.form.get('acpSeamenCountControl')!.value;
        this.model.authorizationStartDate = (this.form.get('authorizationDateControl')!.value as DateRangeData).start;
        this.model.authorizationEndDate = (this.form.get('authorizationDateControl')!.value as DateRangeData).end;
        this.model.ship = new FluxFlapRequestShipDTO({
            shipId: this.form.get('shipControl')!.value.value
        });

        this.model.joinedShips = this.joinedShips.map(x => new FluxFlapRequestShipDTO({ shipId: x.value! }));
        this.model.authorizedFishingGearCodes = this.authorizedGears.map(x => x.code!);

        this.model.targetedQuotas = this.targetedQuotasTable.rows.map(x => new FluxFlapRequestTargetedQuotaDTO({
            flapQuotaTypeCode: x.flapQuotaTypeCode,
            speciesCode: x.speciesCode,
            tonnage: x.tonnage
        }));
    }

    private markAllAsTouched(): void {
        this.form.markAllAsTouched();

        this.authorizedGearsTouched = true;
        this.targetedQuotaTouched = true;
    }

    private authorizedGearsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.authorizedGears.length === 0) {
                return { 'atLeastOneAuthorizedGearNeeded': true };
            }
            return null;
        };
    }

    private targetedQuotasValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.targetedQuotasTable !== undefined && this.targetedQuotasTable !== null) {
                if (this.targetedQuotasTable.rows.length === 0) {
                    return { 'atLeastOneTargetedQuotaNeeded': true };
                }
            }
            return null;
        };
    }
}
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { Moment } from 'moment';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { OriginDeclarationDialogParamsModel } from '../../models/origin-declaration-dialog-params.model';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { OriginDeclarationFishDTO } from '@app/models/generated/dtos/OriginDeclarationFishDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { FishNomenclatureDTO } from '@app/models/generated/dtos/FishNomenclatureDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { FishPresentationCodesEnum } from '@app/enums/fish-presentation-codes.enum';
import { FishCatchStateCodesEnum } from '@app/enums/fish-catch-state-codes.enum';
import { DEFAULT_CATCH_STATE_CODE, DEFAULT_PRESENTATION_CODE, DEFAULT_PRESERVATION_CODE } from '../ship-log-book/edit-ship-log-book-page.component';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { ShipsUtils } from '@app/shared/utils/ships.utils';
import { FishQuotaDTO } from '@app/models/generated/dtos/FishQuotaDTO';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { FishPreservationCodesEnum } from '@app/enums/fish-preservation-codes.enum';


@Component({
    selector: 'edit-origin-declaration',
    templateUrl: './edit-origin-declaration.component.html'
})
export class EditOriginDeclarationComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;
    private viewMode!: boolean;
    public model!: OriginDeclarationFishDTO;
    public service!: ICatchesAndSalesService;

    public aquaticOrganismTypes: FishNomenclatureDTO[] = [];
    public catchStates: NomenclatureDTO<number>[] = [];
    public catchPresentations: NomenclatureDTO<number>[] = [];
    public catchPreservations: NomenclatureDTO<number>[] = [];
    public allPorts: NomenclatureDTO<number>[] = [];
    public ports: NomenclatureDTO<number>[] = [];
    public ships: ShipNomenclatureDTO[] = [];

    public isAllCatchMarkedAsTransboarded: boolean = false;

    public getControlErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelText.bind(this);

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private translationService: FuseTranslationLoaderService;
    private commonNomenclaturesService: CommonNomenclatures;

    public constructor(
        translate: FuseTranslationLoaderService,
        commonNomenclaturesService: CommonNomenclatures
    ) {
        this.translationService = translate;
        this.commonNomenclaturesService = commonNomenclaturesService;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures: (NomenclatureDTO<number> | FishNomenclatureDTO | ShipNomenclatureDTO)[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.Fishes, this.commonNomenclaturesService.getFishTypes.bind(this.commonNomenclaturesService), false),
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.CatchStates, this.service.getCatchStates.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.CatchPresentations, this.commonNomenclaturesService.getCatchPresentations.bind(this.commonNomenclaturesService), false),
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.CatchPreservations, this.commonNomenclaturesService.getCatchPreservations.bind(this.commonNomenclaturesService), false),
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.Ports, this.commonNomenclaturesService.getPorts.bind(this.commonNomenclaturesService), false),
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.Ships, this.commonNomenclaturesService.getShips.bind(this.commonNomenclaturesService), false)
        ).toPromise();

        this.aquaticOrganismTypes = nomenclatures[0] as FishNomenclatureDTO[];
        this.catchStates = nomenclatures[1] as NomenclatureDTO<number>[];
        this.catchPresentations = nomenclatures[2] as NomenclatureDTO<number>[];
        this.catchPreservations = nomenclatures[3] as NomenclatureDTO<number>[];

        const portsNomenclature = nomenclatures[4] as NomenclatureDTO<number>[];
        this.allPorts = this.deepCopyPorts(portsNomenclature);
        this.ports = this.allPorts.slice();

        this.ships = nomenclatures[5] as ShipNomenclatureDTO[];

        this.fillForm();
    }

    public setData(data: OriginDeclarationDialogParamsModel, buttons: DialogWrapperData): void {
        this.viewMode = data.viewMode;
        this.service = data.service;
        this.isAllCatchMarkedAsTransboarded = data.isAllCatchTransboarded;

        if (data.model === null || data.model === undefined) {
            this.model = new OriginDeclarationFishDTO({ fromPreviousTrip: false, isActive: true });
        }
        else {
            if (this.viewMode === true) {
                this.form.disable();
            }

            this.model = data.model;
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.updateValueAndValidity({ emitEvent: false });
        this.form.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);
            dialogClose(this.model);
        }
        else {
            this.model.isValid = false;
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public getControlErrorLabelText(controlName: string, errorValue: unknown, errorCode: string): TLError | undefined {
        if (controlName === 'transboardDateTimeControl') {
            if (errorCode === 'matDatetimePickerParse') {
                const message: string = `${this.translationService.getValue('validation.pattern')}: ${this.translationService.getValue('common.date-time-control-format-hint')}`;
                return new TLError({ text: message });
            }
            else if (errorCode === 'required') {
                const message: string = `${this.translationService.getValue('validation.required')} (${this.translationService.getValue('catches-and-sales.with-pattern')}: ${this.translationService.getValue('common.date-time-control-format-hint')})`;
                return new TLError({ text: message });
            }
        }

        return undefined;
    }

    private fillForm(): void {
        if (this.model.fishId !== null && this.model.fishId !== undefined) {
            const aquaticOrganism: FishNomenclatureDTO = this.aquaticOrganismTypes.find(x => x.value === this.model.fishId)!;
            this.form.get('aquaticOrganismTypeControl')!.setValue(aquaticOrganism);
        }

        if (this.model.catchFishStateId !== null && this.model.catchFishStateId !== undefined) {
            const state: NomenclatureDTO<number> = this.catchStates.find(x => x.value === this.model.catchFishStateId)!;
            this.form.get('catchStateControl')!.setValue(state);
        }
        else {
            const defaultCatchState: NomenclatureDTO<number> | undefined = this.catchStates.find(x => x.code === FishCatchStateCodesEnum[DEFAULT_CATCH_STATE_CODE] && x.isActive);
            if (defaultCatchState !== null && defaultCatchState !== undefined) {
                this.form.get('catchStateControl')!.setValue(defaultCatchState);
            }
        }

        if (this.model.catchFishPresentationId !== null && this.model.catchFishPresentationId !== undefined) {
            const presentation: NomenclatureDTO<number> = this.catchPresentations.find(x => x.value === this.model.catchFishPresentationId)!;
            this.form.get('catchPresentationControl')!.setValue(presentation);
        }
        else {
            const defaultPresentation: NomenclatureDTO<number> | undefined = this.catchPresentations.find(x => x.code === FishPresentationCodesEnum[DEFAULT_PRESENTATION_CODE] && x.isActive);
            if (defaultPresentation !== null && defaultPresentation !== undefined) {
                this.form.get('catchPresentationControl')!.setValue(defaultPresentation);
            }
        }

        if (this.model.catchFishPreservationId !== null && this.model.catchFishPreservationId !== undefined) {
            const preservation: NomenclatureDTO<number> = this.catchPreservations.find(x => x.value === this.model.catchFishPreservationId)!;
            this.form.get('catchPreservationControl')!.setValue(preservation);
        }
        else {
            const defaultPreservation: NomenclatureDTO<number> | undefined = this.catchPreservations.find(x => x.code === FishPreservationCodesEnum[DEFAULT_PRESERVATION_CODE] && x.isActive);
            if (defaultPreservation !== null && defaultPreservation !== undefined) {
                this.form.get('catchPreservationControl')!.setValue(defaultPreservation);
            }
        }

        this.form.get('isProcessedOnBoardControl')!.setValue(this.model.isProcessedOnBoard ?? false);
        this.form.get('quantityKgControl')!.setValue(this.model.quantityKg);
        this.form.get('unloadedProcessedQuantityKgControl')!.setValue(this.model.unloadedProcessedQuantityKg);

        this.form.get('transboardDateTimeControl')!.setValue(this.model.transboradDateTime);

        if (this.model.transboardTargetPortId !== null && this.model.transboardTargetPortId !== undefined) {
            const transboardTargetPort: NomenclatureDTO<number> = this.ports.find(x => x.value === this.model.transboardTargetPortId)!;
            this.form.get('transboardTargetPortControl')!.setValue(transboardTargetPort);
        }

        //this.form.get('transbordNationalityControl') // TODO is this field needed ???
        if (this.model.transboardShipId !== null && this.model.transboardShipId !== undefined) {
            const transboardShip: NomenclatureDTO<number> = ShipsUtils.get(this.ships, this.model.transboardShipId);
            this.form.get('transboardingShipControl')!.setValue(transboardShip);
        }

        if (this.model.transboradDateTime !== null && this.model.transboradDateTime !== undefined) {
            this.form.get('isTransboardedControl')!.setValue(true);
        }

        if (this.isAllCatchMarkedAsTransboarded) { // Щом целият улов е трансбордиран, трябва да се попълнят задължителни данни за трансбордирането на рибата
            this.form.get('isTransboardedControl')!.setValue(true);
        }

        this.form.get('commentsControl')!.setValue(this.model.comments);
    }

    private buildForm(): void {
        this.form = new FormGroup({
            aquaticOrganismTypeControl: new FormControl(undefined, Validators.required),
            catchStateControl: new FormControl(undefined, Validators.required),
            catchPresentationControl: new FormControl(undefined, Validators.required),
            catchPreservationControl: new FormControl(undefined, Validators.required),

            isProcessedOnBoardControl: new FormControl(false),
            quantityKgControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),
            unloadedProcessedQuantityKgControl: new FormControl(undefined, TLValidators.number(0)),

            isTransboardedControl: new FormControl(),
            transboardDateTimeControl: new FormControl(),
            transboardTargetPortControl: new FormControl(),
            //transboardNationalityControl: new FormControl(),
            transboardingShipControl: new FormControl(),

            commentsControl: new FormControl(undefined, Validators.maxLength(4000))
        });

        this.form.get('isTransboardedControl')!.valueChanges.subscribe({
            next: (value: boolean | undefined) => {
                if (value) {
                    this.setTransboardingFieldsValidators();
                }
                else {
                    this.clearTransboardingFieldsValidators();
                }

                if (this.viewMode) {
                    this.form.disable();
                }
            }
        });

        this.form.get('transboardDateTimeControl')!.valueChanges.subscribe({
            next: (date: Moment | undefined) => {
                if (date !== undefined && date !== null) {
                    const fish: FishNomenclatureDTO | undefined = this.form.get('aquaticOrganismTypeControl')!.value;

                    if (fish && fish.quotas && fish.quotas.length !== 0) {
                        const quotas: FishQuotaDTO[] = fish.quotas
                            .filter(x => x.periodFrom!.getTime() <= date.toDate().getTime() && x.periodTo!.getTime() > date.toDate().getTime());

                        if (quotas.length === 1) {
                            const quotaPortIds: number[] = (quotas[0].permittedPorts ?? []).map(x => x.portId!);
                            this.ports = this.allPorts.filter(x => quotaPortIds.includes(x.value!));
                        }
                        else {
                            this.ports = this.allPorts.slice();
                        }
                    }
                    else {
                        this.ports = this.allPorts.slice();
                    }
                }
                else {
                    this.ports = this.allPorts.slice();
                }
            }
        });
    }

    private fillModel(): void {
        const state: NomenclatureDTO<number> | undefined = this.form.get('catchStateControl')!.value;
        const presentation: NomenclatureDTO<number> | undefined = this.form.get('catchPresentationControl')!.value;
        const preservation: NomenclatureDTO<number> | undefined = this.form.get('catchPreservationControl')!.value;
        const transboardingShip: NomenclatureDTO<number> | undefined = this.form.get('transboardingShipControl')!.value;
        const transboardingPort: NomenclatureDTO<number> | undefined = this.form.get('transboardTargetPortControl')!.value;

        this.model.catchFishStateId = state?.value;
        this.model.catchFishPresentationId = presentation?.value;
        this.model.catchFishPreservationId = preservation?.value;
        this.model.isProcessedOnBoard = this.form.get('isProcessedOnBoardControl')!.value ?? false;
        this.model.quantityKg = this.form.get('quantityKgControl')!.value;
        this.model.unloadedProcessedQuantityKg = this.form.get('unloadedProcessedQuantityKgControl')!.value;

        if (this.form.get('isTransboardedControl')!.value) {
            this.model.transboradDateTime = this.form.get('transboardDateTimeControl')!.value;
            // this.model.transboardNationalityControl
            this.model.transboardShipId = transboardingShip?.value;
            this.model.transboardTargetPortId = transboardingPort?.value;
        }

        this.model.comments = this.form.get('commentsControl')!.value;

        this.model.isValid = true;
    }

    private setTransboardingFieldsValidators(): void {
        this.form.get('transboardDateTimeControl')!.setValidators(Validators.required);
        this.form.get('transboardTargetPortControl')!.setValidators(Validators.required);
        // this.form.get('transboardNationalityControl')!.setValidators(Validators.required);
        this.form.get('transboardingShipControl')!.setValidators(Validators.required);

        this.form.get('transboardDateTimeControl')!.markAsPending();
        this.form.get('transboardTargetPortControl')!.markAsPending();
        // this.form.get('transboardNationalityControl')!.markAsPending();
        this.form.get('transboardingShipControl')!.markAsPending();
    }

    private clearTransboardingFieldsValidators(): void {
        this.form.get('transboardDateTimeControl')!.setValidators(null);
        this.form.get('transboardTargetPortControl')!.setValidators(null);
        // this.form.get('transboardNationalityControl')!.setValidators(null);
        this.form.get('transboardingShipControl')!.setValidators(null);

        this.form.get('transboardDateTimeControl')!.reset();
        this.form.get('transboardTargetPortControl')!.reset();
        // this.form.get('transboardNationalityControl')!.reset();
        this.form.get('transboardingShipControl')!.reset();

        this.form.get('transboardDateTimeControl')!.updateValueAndValidity();
        this.form.get('transboardTargetPortControl')!.updateValueAndValidity();
        // this.form.get('transboardNationalityControl')!.updateValueAndValidity();
        this.form.get('transboardingShipControl')!.updateValueAndValidity();
    }


    // helpers

    private deepCopyPorts(ports: NomenclatureDTO<number>[]): NomenclatureDTO<number>[] {
        if (ports !== null && ports !== undefined) {
            const copiedPorts: NomenclatureDTO<number>[] = [];

            for (const port of ports) {
                const stringified: string = JSON.stringify(port);
                const newPort: NomenclatureDTO<number> = new NomenclatureDTO<number>(JSON.parse(stringified));

                copiedPorts.push(newPort);
            }

            return copiedPorts;
        }
        else {
            return [];
        }
    }
}
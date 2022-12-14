import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';

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
import { DEFAULT_CATCH_STATE_CODE, DEFAULT_PRESENTATION_CODE } from '../ship-log-book/edit-ship-log-book-page.component';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { ShipsUtils } from '@app/shared/utils/ships.utils';


@Component({
    selector: 'edit-origin-declaration',
    templateUrl: './edit-origin-declaration.component.html'
})
export class EditOriginDeclarationComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;
    public viewMode!: boolean;
    public model!: OriginDeclarationFishDTO;
    public service!: ICatchesAndSalesService;

    public aquaticOrganismTypes: FishNomenclatureDTO[] = [];
    public catchStates: NomenclatureDTO<number>[] = [];
    public catchPresentations: NomenclatureDTO<number>[] = [];
    public allPorts: NomenclatureDTO<number>[] = [];
    public ports: NomenclatureDTO<number>[] = [];
    public ships: ShipNomenclatureDTO[] = [];

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
                NomenclatureTypes.Ports, this.commonNomenclaturesService.getPorts.bind(this.commonNomenclaturesService), false),
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.Ships, this.commonNomenclaturesService.getShips.bind(this.commonNomenclaturesService), false)
        ).toPromise();

        this.aquaticOrganismTypes = nomenclatures[0] as FishNomenclatureDTO[];
        this.catchStates = nomenclatures[1] as NomenclatureDTO<number>[];
        this.catchPresentations = nomenclatures[2] as NomenclatureDTO<number>[];

        const portsNomenclature = nomenclatures[3] as NomenclatureDTO<number>[];
        this.allPorts = this.deepCopyPorts(portsNomenclature);
        this.ports = this.allPorts.slice();

        this.ships = nomenclatures[4] as ShipNomenclatureDTO[];

        this.fillForm();
    }

    public setData(data: OriginDeclarationDialogParamsModel, buttons: DialogWrapperData): void {
        this.viewMode = data.viewMode;
        this.service = data.service;

        if (data.model === null || data.model === undefined) {
            this.model = new OriginDeclarationFishDTO({ isActive: true });
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

            if (aquaticOrganism.quotaId !== null
                && aquaticOrganism.quotaId !== undefined
                && aquaticOrganism.quotaSpiciesPermittedPortIds !== null
                && aquaticOrganism.quotaSpiciesPermittedPortIds !== undefined
            ) { // the fish is part of a quota so the ports must be filtered
                this.ports = this.allPorts.filter(x => aquaticOrganism.quotaSpiciesPermittedPortIds!.some(y => y.portId === x.value));

                for (const port of this.ports) {
                    port.isActive = aquaticOrganism.quotaSpiciesPermittedPortIds.find(x => x.portId === port.value)!.isActive;
                }
            }
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

        this.form.get('commentsControl')!.setValue(this.model.comments);
    }

    private buildForm(): void {
        this.form = new FormGroup({
            aquaticOrganismTypeControl: new FormControl(undefined, Validators.required),
            catchStateControl: new FormControl(undefined, Validators.required),
            catchPresentationControl: new FormControl(undefined, Validators.required),

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
    }

    private fillModel(): void {
        const state: NomenclatureDTO<number> | undefined = this.form.get('catchStateControl')!.value;
        const presentation: NomenclatureDTO<number> | undefined = this.form.get('catchPresentationControl')!.value;
        const transboardingShip: NomenclatureDTO<number> | undefined = this.form.get('transboardingShipControl')!.value;
        const transboardingPort: NomenclatureDTO<number> | undefined = this.form.get('transboardTargetPortControl')!.value;

        this.model.catchFishStateId = state?.value;
        this.model.catchFishStateName = state?.displayName;
        this.model.catchFishPresentationId = presentation?.value;
        this.model.catchFishPresentationName = presentation?.displayName;
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
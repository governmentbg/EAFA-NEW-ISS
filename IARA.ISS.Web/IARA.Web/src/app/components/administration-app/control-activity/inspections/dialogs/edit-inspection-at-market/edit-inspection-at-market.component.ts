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
import { InspectionTypesEnum } from '@app/enums/inspection-types.enum';
import { InspectionAdditionalInfoModel } from '../../models/inspection-additional-info.model';
import { InspectionObservationCategoryEnum } from '@app/enums/inspection-observation-category.enum';
import { InspectionFirstSaleDTO } from '@app/models/generated/dtos/InspectionFirstSaleDTO';
import { InspectedBuyerNomenclatureDTO } from '@app/models/generated/dtos/InspectedBuyerNomenclatureDTO';
import { InspectedPersonTypeEnum } from '@app/enums/inspected-person-type.enum';
import { InspectionCheckModel } from '../../models/inspection-check.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { InspectionObservationTextDTO } from '@app/models/generated/dtos/InspectionObservationTextDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { InspectionUtils } from '@app/shared/utils/inspection.utils';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';

@Component({
    selector: 'edit-inspection-at-market',
    templateUrl: './edit-inspection-at-market.component.html',
})
export class EditInspectionAtMarketComponent extends BaseInspectionsComponent implements OnInit, IDialogComponent {
    protected model: InspectionFirstSaleDTO = new InspectionFirstSaleDTO();

    public institutions: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public fishes: NomenclatureDTO<number>[] = [];
    public catchTypes: NomenclatureDTO<number>[] = [];
    public catchZones: NomenclatureDTO<number>[] = [];
    public presentations: NomenclatureDTO<number>[] = [];
    public buyers: InspectedBuyerNomenclatureDTO[] = [];

    public catchToggles: InspectionCheckModel[] = [];

    public hasImporter: boolean = false;

    public readonly inspectedPersonTypeEnum: typeof InspectedPersonTypeEnum = InspectedPersonTypeEnum;

    public constructor(
        service: InspectionsService,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        confirmDialog: TLConfirmDialog,
        snackbar: MatSnackBar
    ) {
        super(service, translate, nomenclatures, confirmDialog, snackbar);
        this.inspectionCode = InspectionTypesEnum.IFS;
    }

    public async ngOnInit(): Promise<void> {
        if (this.viewMode) {
            this.form.disable();
        }

        const nomenclatureTables = await forkJoin([
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Institutions, this.nomenclatures.getInstitutions.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Countries, this.nomenclatures.getCountries.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Fishes, this.nomenclatures.getFishTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.CatchTypes, this.nomenclatures.getCatchInspectionTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.CatchZones, this.nomenclatures.getCatchZones.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.CatchPresentations, this.nomenclatures.getCatchPresentations.bind(this.nomenclatures), false),
            this.service.getBuyers(),
            this.service.getCheckTypesForInspection(InspectionTypesEnum.IFS),
        ]).toPromise();

        this.institutions = nomenclatureTables[0];
        this.countries = nomenclatureTables[1];
        this.fishes = nomenclatureTables[2];
        this.catchTypes = nomenclatureTables[3];
        this.catchZones = nomenclatureTables[4];
        this.presentations = nomenclatureTables[5];
        this.buyers = nomenclatureTables[6];

        this.catchToggles = nomenclatureTables[7].map(f => new InspectionCheckModel(f));

        if (this.id !== null && this.id !== undefined) {
            this.service.get(this.id, this.inspectionCode).subscribe({
                next: (inspection: InspectionFirstSaleDTO) => {
                    this.model = inspection;
                    this.fillForm();
                }
            });
        }
    }

    public onBuyerSelected(buyer: InspectedBuyerNomenclatureDTO): void {
        if (buyer !== null && buyer !== undefined && buyer instanceof NomenclatureDTO) {
            if (buyer.hasUtility === true) {
                this.form.get('marketNameControl')!.setValue(buyer.utilityName);
            }
        }
        else {
            this.form.get('marketNameControl')!.setValue(undefined);
        }
    }

    protected buildForm(): void {
        this.form = new FormGroup({
            generalInfoControl: new FormControl(undefined),
            marketNameControl: new FormControl(undefined, [Validators.required, Validators.maxLength(500)]),
            addressControl: new FormControl(undefined, [Validators.required, Validators.maxLength(500)]),
            buyerControl: new FormControl(undefined),
            representativeControl: new FormControl(undefined),
            catchTogglesControl: new FormControl([]),
            hasImporterControl: new FormControl(false),
            importerControl: new FormControl(undefined),
            catchesControl: new FormControl([]),
            catchViolationControl: new FormControl(undefined),
            representativeCommentControl: new FormControl(undefined, Validators.max(4000)),
            additionalInfoControl: new FormControl(undefined),
            filesControl: new FormControl([])
        }, InspectionUtils.atLeastOneCatchValidator());

        this.form.get('generalInfoControl')!.valueChanges.subscribe({
            next: () => {
                this.reportNumAlreadyExistsError = false;
            }
        });

        this.form.get('hasImporterControl')!.valueChanges.subscribe({
            next: (value: boolean) => {
                this.hasImporter = value;

                if (value && this.viewMode === false) {
                    this.form.get('importerControl')!.enable();
                }
                else {
                    this.form.get('importerControl')!.disable();
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
                byEmergencySignal: this.model.byEmergencySignal,
            }));

            this.form.get('additionalInfoControl')!.setValue(new InspectionAdditionalInfoModel({
                actionsTaken: this.model.actionsTaken,
                administrativeViolation: this.model.administrativeViolation,
                inspectorComment: this.model.inspectorComment,
                violation: this.model.observationTexts?.find(x => x.category === InspectionObservationCategoryEnum.AdditionalInfo),
                violatedRegulations: this.model.violatedRegulations,
            }));

            this.form.get('representativeCommentControl')!.setValue(this.model.representativeComment);
            this.form.get('catchTogglesControl')!.setValue(this.model.checks);
            this.form.get('catchesControl')!.setValue(this.model.inspectionLogBookPages);
            this.form.get('marketNameControl')!.setValue(this.model.subjectName);
            this.form.get('addressControl')!.setValue(this.model.subjectAddress);
            this.form.get('filesControl')!.setValue(this.model.files);
            this.form.get('catchViolationControl')!.setValue(this.model.observationTexts?.find(x => x.category === InspectionObservationCategoryEnum.Catch)?.text);

            if (this.model.personnel !== null && this.model.personnel !== undefined) {
                this.form.get('buyerControl')!.setValue(this.model.personnel.find(x => x.type === InspectedPersonTypeEnum.RegBuyer));
                this.form.get('representativeControl')!.setValue(this.model.personnel.find(x => x.type === InspectedPersonTypeEnum.ReprsPers));

                const importer = this.model.personnel.find(x => x.type === InspectedPersonTypeEnum.Importer);
                this.hasImporter = importer !== null && importer !== undefined;

                this.form.get('hasImporterControl')!.setValue(this.hasImporter);

                if (this.hasImporter) {
                    this.form.get('importerControl')!.setValue(importer);
                }
            }
        }
    }

    protected fillModel(): void {
        const generalInfo: InspectionGeneralInfoModel = this.form.get('generalInfoControl')!.value;
        const additionalInfo: InspectionAdditionalInfoModel = this.form.get('additionalInfoControl')!.value;
        const catchViolation: string = this.form.get('catchViolationControl')!.value;

        this.model.isActive = true;
        this.model.inspectionType = InspectionTypesEnum.IFS;
        this.model.startDate = generalInfo?.startDate;
        this.model.endDate = generalInfo?.endDate;
        this.model.inspectors = generalInfo?.inspectors;
        this.model.reportNum = generalInfo?.reportNum;
        this.model.byEmergencySignal = generalInfo?.byEmergencySignal;
        this.model.actionsTaken = additionalInfo?.actionsTaken;
        this.model.administrativeViolation = additionalInfo?.administrativeViolation === true;
        this.model.inspectorComment = additionalInfo?.inspectorComment;
        this.model.violatedRegulations = additionalInfo?.violatedRegulations;

        this.model.files = this.form.get('filesControl')!.value;
        this.model.representativeComment = this.form.get('representativeCommentControl')!.value;
        this.model.inspectionLogBookPages = this.form.get('catchesControl')!.value;
        this.model.checks = this.form.get('catchTogglesControl')!.value;
        this.model.subjectName = this.form.get('marketNameControl')!.value;
        this.model.subjectAddress = this.form.get('addressControl')!.value;

        this.model.observationTexts = [
            additionalInfo?.violation,
            !CommonUtils.isNullOrWhiteSpace(catchViolation)
                ? new InspectionObservationTextDTO({
                    category: InspectionObservationCategoryEnum.Catch,
                    text: catchViolation
                }) : undefined
        ].filter(x => x !== null && x !== undefined) as InspectionObservationTextDTO[];

        this.model.personnel = [
            this.form.get('buyerControl')!.value,
            this.form.get('representativeControl')!.value,
            this.hasImporter ? this.form.get('importerControl')!.value : null,
        ].filter(x => x !== null && x !== undefined);
    }
}
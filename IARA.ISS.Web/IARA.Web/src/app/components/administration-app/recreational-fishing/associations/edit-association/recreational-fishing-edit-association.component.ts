import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RecreationalFishingAssociationEditDTO } from '@app/models/generated/dtos/RecreationalFishingAssociationEditDTO';
import { RecreationalFishingAssociationService } from '@app/services/administration-app/recreational-fishing-association.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { RecreationalFishingAnnulAssociationComponent } from '../annul-association/recreational-fishing-annul-association.component';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { AssociationAnnulmentResult } from '../models/association-annulment-result.model';
import { AssociationEditDialogParams } from '../models/association-edit-dialog-params.model';
import { EikUtils } from '@app/shared/utils/eik.utils';
import { IRecreationalFishingAssociationService } from '@app/interfaces/common-app/recreational-fishing-association.interface';
import { LegalFullDataDTO } from '@app/models/generated/dtos/LegalFullDataDTO';

@Component({
    selector: 'recreational-fishing-edit-association',
    templateUrl: './recreational-fishing-edit-association.component.html'
})
export class RecreationalFishingEditAssociationComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;
    public service: IRecreationalFishingAssociationService;
    public isEditing: boolean = false;

    public territoryUnits!: NomenclatureDTO<number>[];

    public readonly pageCode: PageCodeEnum = PageCodeEnum.Assocs;

    private model!: RecreationalFishingAssociationEditDTO;

    private nomenclatures: CommonNomenclatures;
    private translate: FuseTranslationLoaderService;
    private annulDialog: TLMatDialog<RecreationalFishingAnnulAssociationComponent>;
    private id!: number | undefined;
    private isAdding: boolean = false;
    private readOnly: boolean = false;

    public constructor(
        service: RecreationalFishingAssociationService,
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        annulDialog: TLMatDialog<RecreationalFishingAnnulAssociationComponent>
    ) {
        this.service = service;
        this.nomenclatures = nomenclatures;
        this.translate = translate;
        this.annulDialog = annulDialog;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        this.territoryUnits = await NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false
        ).toPromise();

        if (this.id === undefined) {
            this.model = new RecreationalFishingAssociationEditDTO();
        }
        else {
            if (this.isAdding === true) {
                this.service.getLegalForAssociation(this.id).subscribe({
                    next: (association: RecreationalFishingAssociationEditDTO) => {
                        this.model = association;
                        this.fillForm();
                    }
                });
            }
            else {
                this.service.getAssociation(this.id).subscribe({
                    next: (association: RecreationalFishingAssociationEditDTO) => {
                        this.model = association;
                        this.fillForm();

                        this.isEditing = this.model.legal?.eik !== undefined && EikUtils.isEikValid(this.model.legal.eik);
                    }
                });
            }
        }
    }

    public saveBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (action.id === 'save') {
            if (this.readOnly) {
                dialogClose();
            }

            this.form.markAllAsTouched();
            if (this.form.valid) {
                this.addOrEdit(dialogClose);
            }
        }
    }

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (action.id === 'cancel') {
            dialogClose();
        }
    }


    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        if (this.form.valid || this.form.disabled) {
            if (action.id === 'annul') {
                this.openAnnulDialog(dialogClose, true);
            }
            else if (action.id === 'activate') {
                this.openAnnulDialog(dialogClose, false);
            }
        }
    }

    public setData(data: AssociationEditDialogParams | undefined, buttons: DialogWrapperData): void {
        this.id = data?.id;
        this.isAdding = data?.adding ?? false;
        this.readOnly = data?.readonly ?? false;
    }

    public downloadedLegalData(legal: LegalFullDataDTO): void {
        this.form.get('legalRegixDataControl')!.setValue(legal.legal);
        this.form.get('legalAddressesControl')!.setValue(legal.addresses);
    }

    private buildForm(): void {
        this.form = new FormGroup({
            territoryUnitControl: new FormControl(null, Validators.required),
            legalRegixDataControl: new FormControl(),
            legalAddressesControl: new FormControl(),
            filesControl: new FormControl()
        });
    }

    private fillForm(): void {
        this.form.get('territoryUnitControl')!.setValue(this.territoryUnits.find(x => x.value === this.model.territoryUnitId));
        this.form.get('legalRegixDataControl')!.setValue(this.model.legal);
        this.form.get('legalAddressesControl')!.setValue(this.model.legalAddresses);
        this.form.get('filesControl')!.setValue(this.model.files);

        if (this.readOnly) {
            this.form.disable();
        }

        if (this.isAdding) {
            this.form.get('legalRegixDataControl')!.disable();
            this.form.get('legalAddressesControl')!.disable();
        }
    }

    private fillModel(): void {
        this.model.territoryUnitId = this.form.get('territoryUnitControl')!.value.value;
        this.model.files = this.form.get('filesControl')!.value;
        this.model.isAdding = this.isAdding;

        if (this.isAdding) {
            this.model.legalId = this.id;
        }
        else {
            this.model.legal = this.form.get('legalRegixDataControl')!.value;
            this.model.legalAddresses = this.form.get('legalAddressesControl')!.value;
        }

        CommonUtils.sanitizeModelStrings(this.model);
    }

    private addOrEdit(dialogClose: DialogCloseCallback): void {
        this.fillModel();

        if (this.id !== undefined && !this.isAdding) {
            this.service.editAssociation(this.model).subscribe({
                next: () => {
                    dialogClose(this.model);
                }
            });
        }
        else {
            this.service.addAssociation(this.model).subscribe({
                next: (id: number) => {
                    this.model.id = id;
                    dialogClose(this.model);
                }
            });
        }
    }

    private openAnnulDialog(dialogClose: DialogCloseCallback, annuling: boolean): void {
        const title: string = annuling
            ? this.translate.getValue('recreational-fishing.annul-association-dialog-title')
            : this.translate.getValue('recreational-fishing.activate-association-dialog-title');

        const dialog = this.annulDialog.openWithTwoButtons({
            title: title,
            TCtor: RecreationalFishingAnnulAssociationComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeAnnulDialogBtnClicked.bind(this)
            },
            translteService: this.translate,
            componentData: new AssociationAnnulmentResult(!annuling, this.model.cancellationDate!, this.model.cancellationReason!)
        }, '1200px');

        dialog.subscribe((result: AssociationAnnulmentResult | undefined) => {
            if (result !== undefined && result !== null) {
                this.model.isCanceled = result.canceled;
                this.model.cancellationDate = result.canceled ? result.date : undefined;
                this.model.cancellationReason = result.canceled ? result.reason : undefined;

                this.addOrEdit(dialogClose);
            }
        });
    }

    private closeAnnulDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}
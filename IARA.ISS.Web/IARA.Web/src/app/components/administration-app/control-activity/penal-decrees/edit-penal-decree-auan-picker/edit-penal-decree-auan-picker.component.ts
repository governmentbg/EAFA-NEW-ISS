import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { Observable } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IPenalDecreesService } from '@app/interfaces/administration-app/penal-decrees.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PenalDecreeEditDTO } from '@app/models/generated/dtos/PenalDecreeEditDTO';
import { PenalDecreesService } from '@app/services/administration-app/penal-decrees.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { EditPenalDecreeComponent } from '../edit-penal-decree/edit-penal-decree.component';
import { EditPenalDecreeDialogParams } from '../models/edit-penal-decree-params.model';
import { PenalDecreeTypeEnum } from '@app/enums/penal-decree-type.enum';
import { EditDecreeAgreementComponent } from '../edit-decree-agreement/edit-decree-agreement.component';
import { EditDecreeWarningComponent } from '../edit-decree-warning/edit-decree-warning.component';

@Component({
    selector: 'edit-penal-decree-auan-picker',
    templateUrl: './edit-penal-decree-auan-picker.component.html'
})
export class EditPenalDecreeAuanPickerComponent implements OnInit, IDialogComponent {
    public auanControl: FormControl;
    public typeControl: FormControl;

    public auans: NomenclatureDTO<number>[] = [];
    public types: NomenclatureDTO<number>[] = [];

    public type!: PenalDecreeTypeEnum;

    private service: IPenalDecreesService;
    private translate: FuseTranslationLoaderService;
    private penalDecreeDialog: TLMatDialog<EditPenalDecreeComponent>;
    private agreementDialog: TLMatDialog<EditDecreeAgreementComponent>;
    private warningDialog: TLMatDialog<EditDecreeWarningComponent>;

    public constructor(
        service: PenalDecreesService,
        translate: FuseTranslationLoaderService,
        penalDecreeDialog: TLMatDialog<EditPenalDecreeComponent>,
        agreementDialog: TLMatDialog<EditDecreeAgreementComponent>,
        warningDialog: TLMatDialog<EditDecreeWarningComponent>
    ) {
        this.service = service;
        this.translate = translate;
        this.penalDecreeDialog = penalDecreeDialog;
        this.agreementDialog = agreementDialog;
        this.warningDialog = warningDialog;

        this.typeControl = new FormControl(null, Validators.required);
        this.auanControl = new FormControl(null, Validators.required);
    }

    public ngOnInit(): void {
        this.service.getAllAuans().subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.auans = result;
            }
        });

        this.service.getPenalDecreeTypes().subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.types = result;
            }
        })
    }

    public setData(data: unknown, wrapperData: DialogWrapperData): void {
        // nothing to do
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.auanControl.markAllAsTouched();
        this.typeControl.markAllAsTouched();

        if (this.auanControl.valid && this.typeControl.valid) {
            const rightButtons: IActionInfo[] = [];
            const type: NomenclatureDTO<number> = this.typeControl.value;
            this.type = PenalDecreeTypeEnum[type.code as keyof typeof PenalDecreeTypeEnum];

            switch (this.type) {
                case PenalDecreeTypeEnum.PenalDecree: {
                    this.openEditPenalDecreeDialog().subscribe({
                        next: (decree: PenalDecreeEditDTO | undefined) => {
                            dialogClose(decree);
                        }
                    });
                } break;
                case PenalDecreeTypeEnum.Agreement: {
                    this.openEditAgreementDialog().subscribe({
                        next: (decree: PenalDecreeEditDTO | undefined) => {
                            dialogClose(decree);
                        }
                    });
                } break;
                case PenalDecreeTypeEnum.Warning: {
                    this.openEditWarningDialog().subscribe({
                        next: (decree: PenalDecreeEditDTO | undefined) => {
                            dialogClose(decree);
                        }
                    });
                } break;
                default: {
                    dialogClose();
                } break;
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private openEditPenalDecreeDialog(): Observable<PenalDecreeEditDTO | undefined> {
        const dialog = this.penalDecreeDialog.openWithTwoButtons({
            title: this.translate.getValue('penal-decrees.add-penal-decree-dialog-title'),
            TCtor: EditPenalDecreeComponent,
            headerAuditButton: undefined,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: new EditPenalDecreeDialogParams({
                auanId: this.auanControl.value!.value,
                typeId: this.typeControl.value!.value,
                isReadonly: false
            }),
            rightSideActionsCollection: [{
                id: 'print',
                color: 'accent',
                translateValue: 'penal-decrees.save-print'
            }],
            translteService: this.translate,
            disableDialogClose: true
        }, '1400px');

        return dialog;
    }

    private openEditAgreementDialog(): Observable<PenalDecreeEditDTO | undefined> {
        const dialog = this.agreementDialog.openWithTwoButtons({
            title: this.translate.getValue('penal-decrees.add-agreement-dialog-title'),
            TCtor: EditDecreeAgreementComponent,
            headerAuditButton: undefined,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: new EditPenalDecreeDialogParams({
                auanId: this.auanControl.value!.value,
                typeId: this.typeControl.value!.value,
                isReadonly: false
            }),
            rightSideActionsCollection: [{
                id: 'print',
                color: 'accent',
                translateValue: 'penal-decrees.save-print'
            }],
            translteService: this.translate,
            disableDialogClose: true
        }, '1400px');

        return dialog;
    }

    private openEditWarningDialog(): Observable<PenalDecreeEditDTO | undefined> {
        const dialog = this.warningDialog.openWithTwoButtons({
            title: this.translate.getValue('penal-decrees.add-warning-dialog-title'),
            TCtor: EditDecreeWarningComponent,
            headerAuditButton: undefined,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: new EditPenalDecreeDialogParams({
                auanId: this.auanControl.value!.value,
                typeId: this.typeControl.value!.value,
                isReadonly: false
            }),
            rightSideActionsCollection: [{
                id: 'print',
                color: 'accent',
                translateValue: 'penal-decrees.save-print'
            }],
            translteService: this.translate,
            disableDialogClose: true
        }, '1400px');

        return dialog;
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}
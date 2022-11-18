import { Component, OnInit } from '@angular/core';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { AbstractControl, FormControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { EditPenalPointsDialogParams } from '../models/edit-penal-points-dialog-params.model';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { Observable } from 'rxjs';
import { IPenalPointsService } from '@app/interfaces/administration-app/penal-points.interface';
import { PenalPointsService } from '@app/services/administration-app/penal-points.service';
import { PenalPointsEditDTO } from '@app/models/generated/dtos/PenalPointsEditDTO';
import { EditPenalPointsComponent } from '../edit-penal-points/edit-penal-points.component';
import { PointsTypeEnum } from '@app/enums/points-type.enum';

@Component({
    selector: 'edit-penal-points-decree-picker',
    templateUrl: './edit-penal-points-decree-picker.component.html'
})
export class EditPenalPointsDecreePickerComponent implements OnInit, IDialogComponent {
    public penalDecreeControl: FormControl;
    public typeControl: FormControl;

    public penalDecrees: NomenclatureDTO<number>[] = [];
    public types: NomenclatureDTO<PointsTypeEnum>[] = [];

    public type!: PointsTypeEnum;

    private service!: IPenalPointsService;
    private translate: FuseTranslationLoaderService;
    private editDialog: TLMatDialog<EditPenalPointsComponent>;

    public constructor(
        service: PenalPointsService,
        translate: FuseTranslationLoaderService,
        editDialog: TLMatDialog<EditPenalPointsComponent>
    ) {
        this.service = service;
        this.translate = translate;
        this.editDialog = editDialog;

        this.penalDecreeControl = new FormControl(null, [Validators.required, this.noPenalDecreesValidator()]);
        this.typeControl = new FormControl(null, Validators.required);

        this.types = [
            new NomenclatureDTO<PointsTypeEnum>({
                value: PointsTypeEnum.PermitOwner,
                displayName: this.translate.getValue('penal-points.points-type-permit-owner'),
                isActive: true
            }),
            new NomenclatureDTO<PointsTypeEnum>({
                value: PointsTypeEnum.QualifiedFisher,
                displayName: this.translate.getValue('penal-points.points-type-qualified-fisher'),
                isActive: true
            })
        ];
    }

    public ngOnInit(): void {
        this.service.getAllPenalDecrees().subscribe({
            next: (decrees: NomenclatureDTO<number>[]) => {
                this.penalDecrees = decrees;
            }
        });
    }

    public setData(data: unknown, wrapperData: DialogWrapperData): void {
        //nothing to do
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.penalDecreeControl.markAllAsTouched();
        this.typeControl.markAllAsTouched();

        if (this.penalDecreeControl.valid && this.typeControl.valid) {
            this.type = this.typeControl!.value?.value;

            this.openEditAwardedPointsDialog().subscribe({
                next: (points: PenalPointsEditDTO | undefined) => {
                    dialogClose(points);
                }
            });
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private openEditAwardedPointsDialog(): Observable<PenalPointsEditDTO | undefined> {
        const dialog = this.editDialog.openWithTwoButtons({
            title: this.translate.getValue('penal-points.add-penal-points-dialog-title'),
            TCtor: EditPenalPointsComponent,
            headerAuditButton: undefined,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: new EditPenalPointsDialogParams({
                penalDecreeId: this.penalDecreeControl.value!.value,
                type: this.type,
                isReadonly: false
            }),
            translteService: this.translate,
            disableDialogClose: true
        }, '1400px');

        return dialog;
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private noPenalDecreesValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.penalDecrees.length === 0) {
                return { 'noPenalDecrees': true };
            }
            return null;
        };
    }
}
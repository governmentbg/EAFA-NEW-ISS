import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CrossChecksService } from '@app/services/administration-app/cross-checks.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CrossCheckResultDTO } from '@app/models/generated/dtos/CrossCheckResultDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { DialogAssignedUserParamsModel } from '@app/models/common/dialog-assigned-user-params.model';

@Component({
    selector: 'cross-check-results-assigned-user',
    templateUrl: './cross-check-results-assigned-user.component.html'
})
export class CrossCheckResultsAssignedUserComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    public assignedUserIds: NomenclatureDTO<number>[];

    private readonly service: CrossChecksService;
    private readonly commonNomenclaturesService: CommonNomenclatures;

    private crossCheckResultId: number | undefined;
    private assingedUserId: number | undefined;
    private model!: CrossCheckResultDTO;

    public constructor(
        service: CrossChecksService,
        commonNomenclaturesService: CommonNomenclatures
    ) {
        this.service = service;
        this.commonNomenclaturesService = commonNomenclaturesService;

        this.assignedUserIds = [];

        this.buildform();
    }

    public async ngOnInit(): Promise<void> {
        this.model = new CrossCheckResultDTO();
        this.assignedUserIds = await this.getUserNames();
        this.fillForm();
    }

    public setData(data: DialogAssignedUserParamsModel, buttons: DialogWrapperData): void {
        this.crossCheckResultId = data?.id;
        this.assingedUserId = data?.userId;
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);
            this.service.assignCrossCheckResult(this.crossCheckResultId!, this.model.assignedUserId!).subscribe({
                next: () => {
                    dialogClose(this.model);
                }
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private buildform(): void {
        this.form = new FormGroup({
            assignedUserControl: new FormControl(null, Validators.required)
        });
    }

    private fillForm(): void {
        this.form.get('assignedUserControl')!.setValue(this.assignedUserIds.find(x => x.value === this.assingedUserId));
    }

    private fillModel(): void {
        this.model.assignedUserId = this.form.get('assignedUserControl')!.value.value;
    }

    private getUserNames(): Promise<NomenclatureDTO<number>[]> {
        return this.commonNomenclaturesService.getUserNames().toPromise();
    }
}
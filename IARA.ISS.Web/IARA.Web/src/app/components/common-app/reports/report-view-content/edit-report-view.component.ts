import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { IReportService } from '@app/interfaces/administration-app/report.interface';
import { BaseDialogParamsModel } from '@app/models/common/base-dialog-params.model';
import { ReportGroupDTO } from '@app/models/generated/dtos/ReportGroupDTO';
import { ReportAdministrationService } from '@app/services/administration-app/report-administration.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { ReportTypesEnum } from '@app/enums/reports-type.enum';

@Component({
    selector: 'edit-report-view',
    templateUrl: './edit-report-view.component.html'
})
export class EditReportViewComponent implements OnInit, IDialogComponent {
    public groupPropertiesGroup: FormGroup;
    public groupTypes: string[];

    private readonly reportService: IReportService;

    private model!: ReportGroupDTO;
    private isAddDialog: boolean;

    public constructor(reportService: ReportAdministrationService) {
        this.reportService = reportService;

        this.groupTypes = [
            ReportTypesEnum[ReportTypesEnum.SQL],
            ReportTypesEnum[ReportTypesEnum.JasperPDF],
            ReportTypesEnum[ReportTypesEnum.JasperWord]
        ];

        this.groupPropertiesGroup = new FormGroup({
            nameControl: new FormControl('', Validators.compose(
                [Validators.required, Validators.maxLength(500)]
            )),
            descriptionControl: new FormControl('', Validators.compose(
                [Validators.required, Validators.maxLength(4000)]
            )),
            groupTypeControl: new FormControl('', Validators.required)
        });

        this.isAddDialog = false;
    }

    public ngOnInit(): void {
        if (!CommonUtils.isNullOrUndefined(this.model.id)) {
            this.reportService.getGroup(this.model.id!).subscribe({
                next: (result: ReportGroupDTO) => {
                    this.fillForm(result);
                }
            });
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.groupPropertiesGroup.markAllAsTouched();
        if (this.groupPropertiesGroup.valid) {
            this.model = this.fillModel(this.groupPropertiesGroup);
            this.model = CommonUtils.sanitizeModelStrings(this.model);
            if (!this.isAddDialog) {
                this.reportService.editGroup(this.model).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    }
                });
            }
            else {
                this.reportService.addGroup(this.model).subscribe({
                    next: (result: number) => {
                        this.model.id = result;
                        dialogClose(this.model);
                    }
                });
            }
        }
    }

    public setData(data: BaseDialogParamsModel | undefined, wrapperData: DialogWrapperData): void {
        const groupId: number | undefined = data?.id;

        //edit
        if (groupId !== undefined) {
            this.model = new ReportGroupDTO({
                id: groupId
            });
        }
        else {
            this.isAddDialog = true;
            this.model = new ReportGroupDTO();
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private fillModel(formGroup: FormGroup): ReportGroupDTO {
        this.model.name = formGroup.controls.nameControl.value;
        this.model.description = formGroup.controls.descriptionControl.value;
        this.model.groupType = formGroup.controls.groupTypeControl.value;

        return this.model;
    }

    private fillForm(inputModel: ReportGroupDTO): void {
        this.groupPropertiesGroup.controls.nameControl.setValue(inputModel.name);
        this.groupPropertiesGroup.controls.descriptionControl.setValue(inputModel.description);
        this.groupPropertiesGroup.controls.groupTypeControl.setValue(inputModel.groupType);
    }
}
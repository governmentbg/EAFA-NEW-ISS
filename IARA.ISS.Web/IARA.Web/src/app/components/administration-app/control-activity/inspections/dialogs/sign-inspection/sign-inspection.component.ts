import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';

import { PageCodeEnum } from '@app/enums/page-code.enum';
import { InspectionDTO } from '@app/models/generated/dtos/InspectionDTO';
import { InspectionsService } from '@app/services/administration-app/inspections.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';

@Component({
    selector: 'sign-inspection',
    templateUrl: './sign-inspection.component.html',
})
export class SignInspectionComponent implements IDialogComponent {
    public readonly pageCode: PageCodeEnum = PageCodeEnum.SignInspections;

    public filesControl: FormControl;

    public inspection!: InspectionDTO;

    public readonly service: InspectionsService;

    constructor(service: InspectionsService) {
        this.service = service;

        this.filesControl = new FormControl([]);
    }

    public setData(data: InspectionDTO, wrapperData: DialogWrapperData): void {
        this.inspection = data;
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.filesControl.markAsTouched();

        if (this.filesControl.valid) {
            this.service.sign(this.inspection.id!, this.filesControl.value).subscribe(() => {
                dialogClose(this.inspection);
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }
}
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { MobileDeviceDTO } from '@app/models/generated/dtos/MobileDeviceDTO';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { EditAccessDialogParams } from '../../../components/models/edit-access-dialog-params';
import { InternalUserManagementService } from '@app/services/administration-app/user-management/internal-user-management.service';

@Component({
    selector: 'edit-access-dialog',
    templateUrl: './edit-access-dialog.component.html',
})
export class EditAccessDialogComponent implements OnInit, IDialogComponent {
    public userId!: number;
    public userFullName!: string;
    public matCardTitleLabel!: string;

    public translationService: FuseTranslationLoaderService;
    public userDevices: FormControl = new FormControl();

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    private service: InternalUserManagementService;

    public constructor(translationService: FuseTranslationLoaderService,
        service: InternalUserManagementService) {
        this.translationService = translationService;
        this.service = service;
    }

    public ngOnInit(): void {
        if (this.userId !== undefined) {
            this.getUserMobileDevices();
        }
    }

    public setData(data: EditAccessDialogParams, buttons: DialogWrapperData): void {
        if (data !== undefined) {
            this.userId = data.id;
            this.userFullName = data.userFullName;
            this.matCardTitleLabel = data.matCardTitleLabel;
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.service.addOrEditMobileDevices(this.userId, this.userDevices.value).subscribe(result => {
            dialogClose(result);
        });
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private getUserMobileDevices(): void {
        setTimeout(() => {
            this.service.getUserMobileDevices(this.userId).subscribe((result: MobileDeviceDTO[]) => {
                if (result) {
                    this.userDevices.setValue(result);
                }
            });
        });
    }
}
import { Component, OnInit } from '@angular/core';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { FormControl, FormGroup } from '@angular/forms';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { LogBookPageFilesDialogParamsModel } from '../../models/log-book-page-files-dialog-params.model';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { LogBookPageFilesDTO } from '@app/models/generated/dtos/LogBookPageFilesDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { FileInfoDTO } from '@app/models/generated/dtos/FileInfoDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';

@Component({
    selector: 'log-book-page-files',
    templateUrl: './log-book-page-files.component.html',
})
export class LogBookPageFilesComponent implements OnInit, IDialogComponent {
    public pageCode!: PageCodeEnum;
    public service!: ICatchesAndSalesService

    public form: FormGroup;

    private id!: number;
    private logBookType!: LogBookTypesEnum;
    private model!: LogBookPageFilesDTO;
    private readonly translationService: FuseTranslationLoaderService;

    public constructor(translationService: FuseTranslationLoaderService) {
        this.translationService = translationService;
        this.model = new LogBookPageFilesDTO();

        this.form = new FormGroup({
            filesControl: new FormControl()
        });
    }

    public ngOnInit(): void {
        if (this.id !== undefined && this.id !== null) {
            this.service.getLogBookPageFiles(this.id, this.logBookType!).subscribe({
                next: (files: FileInfoDTO[]) => {
                    setTimeout(() => {
                        this.form.get('filesControl')!.setValue(files);
                    });
                }
            });
        }
    }

    public setData(data: LogBookPageFilesDialogParamsModel, wrapperData: DialogWrapperData): void {
        this.id = data.logBookPageId;
        this.logBookType = data.logBookType;
        this.pageCode = data.pageCode;
        this.service = data.service as ICatchesAndSalesService;
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAsTouched();

        if (this.form.valid) {
            this.fillModel();
            this.model = CommonUtils.sanitizeModelStrings(this.model);

            this.service.editLogBookPageFiles(this.model).subscribe({
                next: () => {
                    dialogClose(this.model);
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

    private fillModel() {
        this.model.logBookPageId = this.id;
        this.model.logBookType = this.logBookType;
        this.model.files = this.form.get('filesControl')!.value;
    }
}
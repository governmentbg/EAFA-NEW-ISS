import { AfterViewInit, Component } from "@angular/core";
import { FuseTranslationLoaderService } from "@fuse/services/translation-loader.service";
import { IActionInfo } from "@app/shared/components/dialog-wrapper/interfaces/action-info.interface";
import { DialogCloseCallback, IDialogComponent } from "@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface";
import { DialogWrapperData } from "@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model";
import { SystemLogDialogParams } from "./models/system-log-dialog-params.model";
import { FormControl, FormGroup } from "@angular/forms";
import { SystemLogDTO } from "@app/models/generated/dtos/SystemLogDTO";


@Component({
    selector: 'view-system-log',
    templateUrl: './view-system-log.component.html'
})
export class ViewSystemLogComponent implements IDialogComponent, AfterViewInit {
    public formGroup: FormGroup;
    public oldValue: string;
    public newValue: string;

    private translationService: FuseTranslationLoaderService;
    private systemLog!: SystemLogDTO;

    public constructor(translationService: FuseTranslationLoaderService) {
        this.translationService = translationService;
        this.oldValue = this.translationService.getValue('system-log.dialog-no-value');
        this.newValue = this.translationService.getValue('system-log.dialog-no-value');
        this.formGroup = new FormGroup({
            uniqueNumberControl: new FormControl(),
            dateTimeControl: new FormControl(),
            actionTypeControl: new FormControl(),
            applicationControl: new FormControl(),
            moduleControl: new FormControl(),

            actionControl: new FormControl(),
            objectControl: new FormControl(),
            userControl: new FormControl(),
            ipControl: new FormControl(),
            browserControl: new FormControl()
        });
    }

    public ngAfterViewInit(): void {
        this.formGroup.disable();
        this.fillForm();

        this.translateLabels();
    }

    public setData(data: SystemLogDialogParams, buttons: DialogWrapperData): void {
        this.systemLog = data.systemLog;
        if (data.systemLogView.oldValue !== undefined && data.systemLogView.oldValue !== null) {
            try {
                this.oldValue = JSON.stringify(JSON.parse(data.systemLogView.oldValue), null, 10);
            }
            catch {
                this.oldValue = data.systemLogView.oldValue;
            }
        }
        if (data.systemLogView.newValue !== undefined && data.systemLogView.newValue !== null) {
            try {
                this.newValue = JSON.stringify(JSON.parse(data.systemLogView.newValue), null, 10);
            }
            catch {
                this.newValue = data.systemLogView.newValue;
            }
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private fillForm(): void {
        this.formGroup.controls.uniqueNumberControl.setValue(this.systemLog.id);
        this.formGroup.controls.dateTimeControl.setValue(this.systemLog.logDate);
        this.formGroup.controls.actionTypeControl.setValue(this.systemLog.actionType);
        this.formGroup.controls.applicationControl.setValue(this.systemLog.application);
        this.formGroup.controls.moduleControl.setValue(this.systemLog.module);
        this.formGroup.controls.actionControl.setValue(this.systemLog.action);
        this.formGroup.controls.objectControl.setValue(this.systemLog.tableName);
        this.formGroup.controls.userControl.setValue(this.systemLog.username);
        this.formGroup.controls.ipControl.setValue(this.systemLog.ipAddress);
        this.formGroup.controls.browserControl.setValue(this.systemLog.browserInfo);
    }

    private translateLabels(): void {
        const interval: number = setInterval(() => {
            const component: HTMLElement | null = document.getElementById('text-diff-component');

            if (component !== null) {
                const sideBySide: HTMLElement = document.getElementById('side-by-side')!;
                const lineByLine: HTMLElement = document.getElementById('line-by-line')!;
                sideBySide.innerText = this.translationService.getValue('system-log.side-by-side');
                lineByLine.innerText = this.translationService.getValue('system-log.line-by-line');

                const checkbox: HTMLElement = component.getElementsByClassName('td-checkbox-container')!.item(0) as HTMLElement;
                
                const openParenIdx: number = checkbox.childNodes.item(0).textContent!.indexOf('(');
                const closeParenIdx: number = checkbox.childNodes.item(0).textContent!.indexOf(')');
                const count: string = checkbox.childNodes.item(0).textContent!.substring(openParenIdx + 1, closeParenIdx);

                checkbox.childNodes.item(0).textContent = `${this.translationService.getValue('system-log.only-show-lines-with-differences')} (${count})`;

                clearInterval(interval);
            }
        });
    }
}
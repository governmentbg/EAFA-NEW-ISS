import { AbstractControl, FormGroup } from '@angular/forms';
import { type } from 'os';
import { Observable } from 'rxjs';
import { IApplicationRegister } from '../../interfaces/common-app/application-register.interface';
import { FileInfoDTO } from '@app/models/generated/dtos/FileInfoDTO';
import { IActionInfo } from '../components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback } from '../components/dialog-wrapper/interfaces/dialog-content.interface';

export class ApplicationUtils {

    /**
     * Before calling the method, one should fill the model property with data and sanitize it if needed
     * @param action
     * @param dialogClose
     * @param applicationId
     * @param model
     * @param readOnly
     * @param viewMode
     * @param editPermitForm
     * @param saveFn
     */
    public static applicationDialogButtonClicked(data: ApplicationDialogData): boolean {
        if (!data.readOnly && !data.viewMode) {
            if (data.action.id === 'save-draft-content') {
                // save draft in Applications table
                if (((data.model.isDraft === undefined || data.model.isDraft === null) && (data.model.id === undefined || data.model.id === null)) || data.model.isDraft === true) {
                    data.model.applicationId = data.applicationId!;
                    data.action.buttonData.callbackFn(data.applicationId!, data.model, data.dialogClose);
                    return true;
                }
                // (if form is valid) update Application data in the register table with RecordType == "Application" without manual status change
                else {
                    data.editForm.markAllAsTouched();
                    if (data.onMarkAsTouched) {
                        data.onMarkAsTouched();
                    }

                    if (data.editForm.valid) {
                        data.saveFn(data.dialogClose, true).subscribe();
                        return true;
                    }
                }
            }
            else {
                data.editForm.markAllAsTouched();
                if (data.onMarkAsTouched) {
                    data.onMarkAsTouched();
                }

                if (data.editForm.valid) {
                    switch (data.action.id) {
                        case 'more-corrections-needed': {
                            data.action.buttonData.callbackFn(data.model, data.dialogClose);
                            return true;
                        }
                        case 'no-corrections-needed': {
                            data.action.buttonData.callbackFn(data.model.applicationId, data.dialogClose);
                            return true;
                        }
                        case 'save-and-start-regix-check': {
                            data.action.buttonData.callbackFn(data.model, data.dialogClose);
                            return true;
                        }
                        case 'confirm-data-irregularity': {
                            data.action.buttonData.callbackFn(data.model.applicationId, data.dialogClose);
                            return true;
                        }
                        case 'confirm-data-regularity': {
                            data.action.buttonData.callbackFn(data.model.applicationId, data.dialogClose);
                            return true;
                        }
                        case 'save-and-download-for-sign': {
                            data.saveFn(data.dialogClose, false).subscribe((isCompleted: boolean) => {
                                if (isCompleted) {
                                    data.action.buttonData.callbackFn(data.model.applicationId);
                                }
                            });
                            return true;
                        }
                    }
                }
                else {
                    switch (data.action.id) {
                        case 'save-and-start-regix-check': {
                            data.action.buttonData.callbackFn(data.model, data.dialogClose);
                            return true;
                        }
                        case 'more-corrections-needed': {
                            data.action.buttonData.callbackFn(data.model, data.dialogClose);
                            return true;
                        }
                        case 'no-corrections-needed': {
                            data.action.buttonData.callbackFn(data.model.applicationId, data.dialogClose);
                            return true;
                        }
                    }
                }
            }
        }
        else if (data.readOnly && !data.viewMode) {
            switch (data.action.id) {
                case 'confirm-data-irregularity': {
                    data.action.buttonData.callbackFn(data.model.applicationId, data.dialogClose);
                    return true;
                }
                case 'confirm-data-regularity': {
                    data.action.buttonData.callbackFn(data.model.applicationId, data.dialogClose);
                    return true;
                }
            }
        }
        return false;
    }

    public static enableOrDisableRegixCheckButtons(form: AbstractControl, actions: IActionInfo[] | undefined): void {
        if (actions !== undefined && actions.length > 0) {
            const interval: number = setInterval(() => {
                if (form.dirty) {
                    form.markAsPristine();
                }
                else {
                    form.valueChanges.subscribe({
                        next: () => {
                            const regixButton: IActionInfo | undefined = actions?.find(x => x.id === 'save-and-start-regix-check');

                            if (regixButton !== undefined) {
                                if (regixButton.disabled === true) {
                                    const correctionsNeededButton: IActionInfo = actions.find(x => x.id === 'more-corrections-needed')!;
                                    const noCorrectionsNeededButton: IActionInfo = actions.find(x => x.id === 'no-corrections-needed')!;

                                    regixButton.disabled = false;
                                    correctionsNeededButton.disabled = true;
                                    noCorrectionsNeededButton.disabled = true;
                                }
                            }
                        }
                    });
                    clearInterval(interval);
                }
            });
        }
    }

    public static spliceFilesFromModel<T>(model: T): FileInfoDTO[] {
        const files: FileInfoDTO[] = [];
        this.spliceFilesFromModelHelper(model, files);
        return files;
    }

    private static spliceFilesFromModelHelper<T>(model: T, files: FileInfoDTO[]): void {
        for (const key in model) {
            const property = model[key];

            if (property !== null && property !== undefined) {
                if (model[key] instanceof FileInfoDTO) {
                    files.push(new FileInfoDTO(model[key]));
                    (model[key] as FileInfoDTO | undefined) = undefined;
                }
                else if (Array.isArray(property)) {
                    if (property.length > 0) {
                        if (property[0] instanceof FileInfoDTO) {
                            for (const element of property) {
                                files.push(element);
                            }
                            (model[key] as unknown as FileInfoDTO[]) = [];
                        }
                        else if (typeof property[0] === 'object' && !(property[0] instanceof Date)) {
                            for (const element of property) {
                                this.spliceFilesFromModelHelper(element, files);
                            }
                        }
                    }
                }
                else if (typeof property === 'object' && !(property instanceof Date)) {
                    this.spliceFilesFromModelHelper(property, files);
                }
            }
        }
    }
}

export type SaveFunc = (dialogClose: DialogCloseCallback, fromSaveAsDraft: boolean) => Observable<boolean>;

export class ApplicationDialogData {
    public action!: IActionInfo;
    public dialogClose!: DialogCloseCallback;
    public applicationId!: number;
    public model!: IApplicationRegister;
    public editForm!: FormGroup;
    public readOnly: boolean = false;
    public viewMode: boolean = false;
    public saveFn!: SaveFunc;
    public onMarkAsTouched?: () => void;

    public constructor(obj?: Partial<ApplicationDialogData>) {
        Object.assign(this, obj);
    }
}
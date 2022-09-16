import { Component, Input, ViewChild, ElementRef, Self, OnInit, DoCheck, OnChanges, SimpleChanges, Output, EventEmitter } from '@angular/core';
import { FormControl, ControlValueAccessor, Validator, AbstractControl, ValidationErrors, NgControl, Validators } from '@angular/forms';
import { MatSnackBarConfig } from '@angular/material/snack-bar';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { FileInfoDTO } from '@app/models/generated/dtos/FileInfoDTO';

export interface IFileSelectedEventArgs {
    previousFile: FileInfoDTO | undefined;
    newFile: FileInfoDTO | undefined;
}

@Component({
    selector: 'tl-file-uploader',
    templateUrl: './file-upload.component.html',
    styleUrls: ['./file-upload.component.scss']
})
export class TLFileUploadComponent implements OnInit, OnChanges, DoCheck, ControlValueAccessor, Validator {
    @Input()
    public label: string = '';

    @Input()
    public acceptedFormats = '';

    @Input()
    public isFileRequired: boolean = false;

    /** Shown in title of the upload input */
    @Input()
    public disabledBtnTitleText: string = '';

    @Output()
    public fileSelected: EventEmitter<IFileSelectedEventArgs> = new EventEmitter<IFileSelectedEventArgs>();

    public file: FileInfoDTO | undefined;
    public fileControl: FormControl = new FormControl('');
    public enabledBtnTitle: string = '';
    public isDisabled: boolean = false;

    @ViewChild('fileUpload')
    private uploadFileInput!: ElementRef;

    private onChanged: (value: FileInfoDTO) => void = (value: FileInfoDTO) => { return; };
    private onTouched: (value: FileInfoDTO) => void = (value: FileInfoDTO) => { return; };

    private ngControl: NgControl;
    private errorSnackbarConfig: MatSnackBarConfig;
    private translationLoader: FuseTranslationLoaderService;

    public constructor(@Self() ngControl: NgControl, translationLoader: FuseTranslationLoaderService) {
        this.ngControl = ngControl;
        this.ngControl.valueAccessor = this;

        this.translationLoader = translationLoader;

        this.errorSnackbarConfig = new MatSnackBarConfig();
        this.errorSnackbarConfig.panelClass = ['snack-bar-error-color'];
        this.errorSnackbarConfig.duration = 6000; // ms

        this.enabledBtnTitle = this.translationLoader.getValue('common.choose-file');
    }

    public ngOnInit(): void {
        if (this.ngControl.control) {
            this.ngControl.control.validator = this.validate.bind(this);
        }
    }

    public ngDoCheck(): void {
        if (this.ngControl?.control?.touched) {
            this.fileControl.markAsTouched();
        }
    }

    public ngOnChanges(changes: SimpleChanges): void {
        this.isFileRequired = changes.isFileRequired.currentValue;

        if (this.isFileRequired) {
            this.fileControl.setValidators(Validators.required);
        }
        else {
            this.fileControl.clearValidators();
        }
        this.fileControl.updateValueAndValidity({ emitEvent: false });
    }

    public writeValue(obj: FileInfoDTO): void {
        this.file = obj;
        this.fileControl.setValue(this.file?.name ?? '');

        this.onChanged(this.file);
    }

    public registerOnChange(fn: (value: FileInfoDTO) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: FileInfoDTO) => void): void {
        this.onTouched = fn;
    }

    public setDisabledState(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
        if (this.isDisabled) {
            this.fileControl.disable();
        }
        else {
            this.fileControl.enable();
        }
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        if (this.isFileRequired) {
            if (!this.file?.file || !this.file?.id && (this.file.id && this.file.deleted)) {
                return { required: true };
            }
        }
        return this.fileControl.errors;
    }

    public onFileSelected(files: FileList): void {
        const prevFile: FileInfoDTO | undefined = this.file;
        const file: File = files[0];

        this.file = new FileInfoDTO({
            file: file,
            fileTypeId: this.file?.fileTypeId,
            size: file.size,
            contentType: file.type,
            name: file.name,
            uploadedOn: new Date()
        });

        this.fileControl.setValue(this.file?.name ?? '');

        this.onChanged(this.file);

        this.fileSelected.emit({
            previousFile: prevFile,
            newFile: this.file
        });
    }

    public onUploadBtnClick(): void {
        this.uploadFileInput.nativeElement.click();
    }
}
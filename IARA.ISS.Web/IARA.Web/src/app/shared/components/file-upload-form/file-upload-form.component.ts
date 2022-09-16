import { Component, DoCheck, EventEmitter, Input, OnInit, Output, Self } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormControl, FormGroup, NgControl, ValidationErrors, Validator, ValidatorFn, Validators } from '@angular/forms';

import { FileInfoDTO } from '@app/models/generated/dtos/FileInfoDTO';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';
import { IFileSelectedEventArgs } from '../file-upload/file-upload.component';

@Component({
    selector: 'file-uploader-form',
    templateUrl: './file-upload-form.component.html'
})
export class FileUploadFormComponent implements OnInit, DoCheck, ControlValueAccessor, Validator {
    @Input()
    public fileTypesCollection!: PermittedFileTypeDTO[];

    @Input()
    public isTypeReadonly: boolean = false;

    @Input()
    public acceptedFormats: string = '';

    @Output()
    public fileSelected: EventEmitter<IFileSelectedEventArgs> = new EventEmitter<IFileSelectedEventArgs>();

    public fileUploadForm: FormGroup;
    public isRequired: boolean = false;

    private ngControl: NgControl;
    private onChanged: (value: FileInfoDTO) => void = (value: FileInfoDTO) => { return; };
    private onTouched: (value: FileInfoDTO) => void = (value: FileInfoDTO) => { return; };

    public constructor(@Self() ngControl: NgControl) {
        this.ngControl = ngControl;
        this.ngControl.valueAccessor = this;

        this.fileUploadForm = new FormGroup({
            type: new FormControl(),
            description: new FormControl('', Validators.maxLength(4000)),
            file: new FormControl()
        });
    }

    public ngOnInit(): void {
        if (this.ngControl.control) {
            this.ngControl.control.validator = this.validate.bind(this);
        }

        this.fileUploadForm.valueChanges.subscribe({
            next: () => {
                const file: FileInfoDTO = this.fileUploadForm.get('file')!.value;
                file.fileTypeId = this.fileUploadForm.get('type')!.value?.value;
                file.description = this.fileUploadForm.get('description')!.value;

                this.onChanged(file);
            }
        });
    }

    public ngDoCheck(): void {
        if (this.ngControl?.control?.touched) {
            this.fileUploadForm.markAllAsTouched();
        }
    }

    public writeValue(value: FileInfoDTO): void {
        if (value !== null && value !== undefined) {
            const fileType: PermittedFileTypeDTO | undefined = this.fileTypesCollection.find(x => x.value === value.fileTypeId);

            if (fileType !== undefined) {
                this.isRequired = fileType.isRequired!;

                this.fileUploadForm.get('type')!.setValue(fileType);
            }

            this.fileUploadForm.get('description')!.setValue(value.description);
            this.fileUploadForm.get('file')!.setValue(value);

            if (this.isRequired) {
                this.fileUploadForm.get('type')!.setValidators(Validators.required);
                this.fileUploadForm.get('file')!.setValidators(this.requiredFile(this.isRequired));
            }
            else {
                this.fileUploadForm.get('type')!.clearValidators();
                this.fileUploadForm.get('file')!.clearValidators();
            }

            this.fileUploadForm.get('type')!.markAsPending({ emitEvent: false });
            this.fileUploadForm.get('file')!.markAsPending({ emitEvent: false });

            this.onChanged(value);
        }
    }

    public registerOnChange(fn: (value: FileInfoDTO) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: FileInfoDTO) => void): void {
        this.onTouched = fn;
    }

    public setDisabledState(isDisabled: boolean): void {
        if (isDisabled) {
            this.fileUploadForm.disable();
        }
        else {
            this.fileUploadForm.enable();
        }
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        const errors: ValidationErrors = {};
        Object.keys(this.fileUploadForm.controls).forEach((key: string) => {
            const controlErrors: ValidationErrors | null = this.fileUploadForm.controls[key].errors;
            if (controlErrors !== null) {
                errors[key] = controlErrors;
            }
        });
        return Object.keys(errors).length > 0 ? errors : null;
    }

    public onFileSelected(event: IFileSelectedEventArgs): void {
        this.fileSelected.emit(event);
    }

    private requiredFile(required: boolean): ValidatorFn {
        return (absControl: AbstractControl): ValidationErrors | null => {
            const control: FormControl = absControl as FormControl;
            const file: FileInfoDTO = control.value;
            if (required) {
                if (!file.file && !file.id || (file.id && file.deleted)) {
                    return { required: true };
                }
            }
            return null;
        };
    }
}

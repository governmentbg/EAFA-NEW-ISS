import { AfterViewInit, Component, DoCheck, Input, OnChanges, OnDestroy, OnInit, Self, SimpleChange, SimpleChanges } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormArray, FormControl, NgControl, ValidationErrors, Validator, ValidatorFn } from '@angular/forms';
import { Observable, Subject, Subscription } from 'rxjs';

import { FileTypeEnum } from '@app/enums/file-types.enum';
import { FileInfoDTO } from '@app/models/generated/dtos/FileInfoDTO';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { IFileSelectedEventArgs } from '../file-upload/file-upload.component';

@Component({
    selector: 'file-uploader-form-array',
    templateUrl: './file-upload-form-array.component.html'
})
export class FileUploadFormArrayComponent implements OnInit, AfterViewInit, DoCheck, OnChanges, OnDestroy, ControlValueAccessor, Validator {
    @Input()
    public pageCode!: PageCodeEnum;

    @Input()
    public downloadFileMethod!: (fileId: number, fileName: string) => Observable<boolean>;

    @Input()
    public hideAddButton: boolean = false;

    @Input()
    public fileTypeFilterFn?: (options: PermittedFileTypeDTO[]) => PermittedFileTypeDTO[];

    @Input()
    public refreshFileTypes: Subject<void> = new Subject<void>();

    public form: FormArray;

    public deletableStatusMap: Map<FormControl, boolean> = new Map<FormControl, boolean>();
    public readonlyStatusMap: Map<FormControl, boolean> = new Map<FormControl, boolean>();

    public isDisabled: boolean = false;

    public fileTypes: PermittedFileTypeDTO[] = [];

    private ngControl: NgControl;
    private onChanged: (values: FileInfoDTO[]) => void = (values: FileInfoDTO[]) => { return; };
    private onTouched: (values: FileInfoDTO[]) => void = (values: FileInfoDTO[]) => { return; };

    private nomenclatures: CommonNomenclatures;

    private permittedFileTypes: PermittedFileTypeDTO[] = [];
    private allFileTypes: PermittedFileTypeDTO[] = [];
    private deletedFiles: FileInfoDTO[] = [];
    private value: FileInfoDTO[] = [];

    private refreshFileTypesSub: Subscription | undefined;

    private readonly loader: FormControlDataLoader;
    private readonly filesSubject: Subject<FileInfoDTO[]> = new Subject<FileInfoDTO[]>();

    private readonly OTHER_FILE_TYPE_CODE: string = FileTypeEnum[FileTypeEnum.OTHER];

    public constructor(@Self() ngControl: NgControl, nomenclatures: CommonNomenclatures) {
        this.ngControl = ngControl;
        this.ngControl.valueAccessor = this;

        this.nomenclatures = nomenclatures;

        this.form = new FormArray([], this.requiredFiles());
        this.loader = new FormControlDataLoader(this.getPermittedFileTypes.bind(this));

        this.filesSubject.subscribe({
            next: (fileInfos: FileInfoDTO[]) => {
                this.loader.load(() => {
                    this.filterFileTypes();
                    this.fillForm(fileInfos);
                });
            }
        });
    }

    public ngOnInit(): void {
        if (this.ngControl.control) {
            this.ngControl.control.validator = this.validate.bind(this);
        }

        this.loader.load();
        this.filesSubject.next(this.value);
    }

    public ngAfterViewInit(): void {
        this.form.valueChanges.subscribe({
            next: (files: FileInfoDTO[]) => {
                this.applyOnChanged();
            }
        });
    }

    public ngDoCheck(): void {
        if (this.ngControl?.control?.touched) {
            this.form.markAllAsTouched();
        }
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const pageCode: SimpleChange | undefined = changes['pageCode'];
        const refreshFileTypes: SimpleChange | undefined = changes['refreshFileTypes'];

        if (pageCode && !pageCode.isFirstChange()) {
            this.filesSubject.next(this.value);
        }

        if (refreshFileTypes) {
            this.refreshFileTypesSub?.unsubscribe();
            this.refreshFileTypesSub = this.refreshFileTypes.subscribe({
                next: () => {
                    this.filesSubject.next(this.value);
                }
            });
        }
    }

    public ngOnDestroy(): void {
        this.refreshFileTypesSub?.unsubscribe();
    }

    public writeValue(fileInfos: FileInfoDTO[]): void {
        this.value = fileInfos;
        this.filesSubject.next(fileInfos);
    }

    public registerOnChange(fn: (values: FileInfoDTO[]) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: FileInfoDTO[]) => void): void {
        this.onTouched = fn;
    }

    public setDisabledState(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
        if (this.isDisabled) {
            this.form.disable();
        }
        else {
            this.form.enable();
        }
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        return this.form.errors;
    }

    public addFileUploadControl(): void {
        const fileInfo: FileInfoDTO = new FileInfoDTO({
            fileTypeId: this.permittedFileTypes.find(x => x.code === this.OTHER_FILE_TYPE_CODE && x.isActive)!.value
        });

        const control: FormControl = new FormControl(fileInfo);
        this.form.push(control);

        this.fillDeletableStatusMap();
        this.applyOnChanged();
    }

    public onDownloadButtonClicked(control: FormControl): void {
        if (control.value) {
            const value: FileInfoDTO = control.value as FileInfoDTO;
            if (value.id && value.name) {
                this.downloadFileMethod(value.id, value.name).subscribe();
            }
        }
    }

    public onClearButtonClicked(control: FormControl, index: number): void {
        const file: FileInfoDTO = control.value;
        file.deleted = true;

        if (file.id !== undefined && file.id !== null) {
            this.deletedFiles.push(file);
        }

        if (this.deletableStatusMap.get(control)) {
            this.form.removeAt(index);
        }
        else {
            control.setValue(new FileInfoDTO({
                id: file.id,
                fileTypeId: file.fileTypeId,
                deleted: true
            }));
            control.markAsTouched();
        }

        this.fillDeletableStatusMap();
        this.applyOnChanged();
    }

    public onFileSelected(event: IFileSelectedEventArgs): void {
        if (event.previousFile) {
            if (event.previousFile.id !== undefined && event.previousFile.id !== null) {
                event.previousFile.deleted = true;
                this.deletedFiles.push(event.previousFile);
            }
        }

        this.applyOnChanged();
    }

    private fillForm(fileInfos: FileInfoDTO[]): void {
        this.reset();

        if (fileInfos) {
            this.fillControls(fileInfos);
        }
        this.setDisabledState(this.isDisabled);
    }

    private reset(): void {
        this.form.clear();
        this.buildFormArray();
        this.deletedFiles = [];
    }

    private applyOnChanged(): void {
        const files: FileInfoDTO[] = (this.form.value as FileInfoDTO[]).filter((file: FileInfoDTO) => {
            return (file.file !== undefined && file.file !== null) || (file.id !== undefined && file.id !== null) && !file.deleted;
        });

        files.push(...this.deletedFiles);

        this.onChanged(files);
    }

    private buildFormArray(): void {
        if (this.permittedFileTypes.length > 0) {
            let files: PermittedFileTypeDTO[] = this.permittedFileTypes.filter(x => x.code !== this.OTHER_FILE_TYPE_CODE && x.isActive);
            const activePermittedFileTypes: PermittedFileTypeDTO[] = this.permittedFileTypes.filter(x => x.isActive);
            if (activePermittedFileTypes.length === 1 && activePermittedFileTypes[0].code === this.OTHER_FILE_TYPE_CODE) {
                files = [activePermittedFileTypes[0]];
            }

            for (const fileType of files) {
                const fileInfo: FileInfoDTO = new FileInfoDTO({
                    fileTypeId: fileType.value
                });

                const control: FormControl = new FormControl(fileInfo);
                this.form.push(control);

                this.readonlyStatusMap.set(control, true);
            }

            this.fillDeletableStatusMap();
            this.applyOnChanged();
        }
    }

    private fillControls(fileInfos: FileInfoDTO[]): void {
        const filledControls: Set<AbstractControl> = new Set<AbstractControl>();

        for (const fileInfo of fileInfos) {
            let fileControl: AbstractControl | undefined;

            for (const control of this.form.controls) {
                if (filledControls.has(control)) {
                    continue;
                }
                if (control?.value) {
                    const file: FileInfoDTO = control.value as FileInfoDTO;
                    if (file.fileTypeId === fileInfo.fileTypeId) {
                        filledControls.add(control);
                        fileControl = control;
                        break;
                    }
                }
            }

            if (fileControl) {
                fileControl.setValue(fileInfo);
            }
            else if (this.permittedFileTypes.some(x => x.value === fileInfo.fileTypeId)) {
                const newControl: FormControl = new FormControl(fileInfo);
                this.form.push(newControl);
                filledControls.add(newControl);

                this.readonlyStatusMap.set(newControl, true);
            }
        }

        this.fillDeletableStatusMap();
        this.applyOnChanged();
    }

    private fillDeletableStatusMap(): void {
        const controls: FormControl[] = this.form.controls as FormControl[];
        const typeIdsCount: Map<number, number> = new Map<number, number>();

        for (const control of controls) {
            const fileTypeId: number = (control.value as FileInfoDTO).fileTypeId!;

            const count: number | undefined = typeIdsCount.get(fileTypeId);
            typeIdsCount.set(fileTypeId, count !== undefined ? count + 1 : 1);
        }

        for (const control of controls) {
            const fileTypeId: number = (control.value as FileInfoDTO).fileTypeId!;

            const count: number | undefined = typeIdsCount.get(fileTypeId);
            this.deletableStatusMap.set(control, count !== undefined ? count > 1 : true);
        }
    }

    private requiredFiles(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const array: FormArray = control as FormArray;

            const errors: ValidationErrors = {};
            for (const control of array.controls) {
                const file: FileInfoDTO = control.value;
                const fileType: PermittedFileTypeDTO | undefined = this.permittedFileTypes.find(x => x.value === file.fileTypeId);

                if (fileType !== undefined) {
                    if (fileType.isRequired) {
                        if (!file.file && !file.id || (file.id && file.deleted)) {
                            errors['required'] = true;
                        }
                    }
                }
                else {
                    if ((file.file !== undefined && file.file !== null) || (file.id !== undefined && file.id !== null)) {
                        errors['required'] = true;
                    }
                }
            }
            return Object.keys(errors).length > 0 ? errors : null;
        };
    }

    private getPermittedFileTypes(): Subscription {
        return NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.PermittedFileTypes, this.nomenclatures.getPermittedFileTypes.bind(this.nomenclatures), false
        ).subscribe({
            next: (types: PermittedFileTypeDTO[]) => {
                this.allFileTypes = types;
                this.loader.complete();
            }
        });
    }

    private filterFileTypes(): void {
        this.permittedFileTypes = this.allFileTypes.filter(x => x.pageCode === this.pageCode);

        if (this.fileTypeFilterFn) {
            this.permittedFileTypes = this.fileTypeFilterFn(this.permittedFileTypes);
        }
    }
}
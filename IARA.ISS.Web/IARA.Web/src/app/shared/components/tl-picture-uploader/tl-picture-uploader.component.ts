import { Component, DoCheck, Input, OnChanges, OnDestroy, OnInit, Self, SimpleChange, SimpleChanges } from '@angular/core';
import { AbstractControl, ControlValueAccessor, NgControl, ValidationErrors, Validator } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { FileTypeEnum } from '@app/enums/file-types.enum';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { FileInfoDTO } from '@app/models/generated/dtos/FileInfoDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureStore } from '../../utils/nomenclatures.store';

import { TLPictureUploaderOptions } from './tl-picture-uploader-options.model';

export type TLPictureRequestMethod = (...args: unknown[]) => Observable<string>;

@Component({
    selector: 'tl-picture-uploader',
    templateUrl: './tl-picture-uploader.component.html',
    styleUrls: ['./tl-picture-uploader.component.scss']
})
export class TLPictureUploaderComponent implements OnInit, OnChanges, DoCheck, OnDestroy, ControlValueAccessor, Validator {
    @Input()
    public options!: TLPictureUploaderOptions;

    @Input()
    public requestMethod?: TLPictureRequestMethod;

    @Input()
    public label: string | undefined;

    @Input()
    public showTicketRegixData: boolean = false;

    public inputId: string;

    public photo!: FileInfoDTO;
    public image!: string;

    public addImage!: string;
    public editImage!: string;
    public deleteImage!: string;

    public isDisabled: boolean = false;
    public isTouched: boolean = false;
    public showRequiredError: boolean = false;
    public showWrongFormatError: boolean = false;

    private fileTypes: NomenclatureDTO<number>[] = [];

    private ngControl: NgControl;
    private translate: FuseTranslationLoaderService;
    private nomenclatures: CommonNomenclatures;
    private onChange: (photo: FileInfoDTO | string | null) => void = (photo: FileInfoDTO | string | null) => { return; };
    private onTouched: (photo: FileInfoDTO | string | null) => void = (photo: FileInfoDTO | string | null) => { return; };

    private requestMethodSubscription: Subscription | undefined;
    private hasImage: boolean = false;

    private static cachedImages: Map<number, string> = new Map<number, string>();
    private static instanceNo: number = 0;

    public constructor(@Self() ngControl: NgControl, translate: FuseTranslationLoaderService, nomenclatures: CommonNomenclatures) {
        this.ngControl = ngControl;
        this.translate = translate;
        this.nomenclatures = nomenclatures;

        this.inputId = `file-input-${TLPictureUploaderComponent.instanceNo++}`;

        this.ngControl.valueAccessor = this;
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.FileTypes, this.nomenclatures.getFileTypes.bind(this.nomenclatures), false
        ).subscribe({
            next: (types: NomenclatureDTO<number>[]) => {
                this.fileTypes = types;
            }
        });

        if (this.ngControl.control) {
            this.ngControl.control.validator = this.validate.bind(this);
        }

        this.update();
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const options: SimpleChange = changes['options'];
        const request: SimpleChange = changes['requestMethod'];

        if (options !== undefined && options !== null) {
            this.initOptions();
        }

        if (request !== undefined && request !== null) {
            this.writeValue(this.photo ?? this.image);
        }
    }

    public ngDoCheck(): void {
        if (this.ngControl?.control?.touched && !this.showTicketRegixData) {
            this.ngControl.control.updateValueAndValidity();
            this.isTouched = true;
        }
    }

    public ngOnDestroy(): void {
        this.requestMethodSubscription?.unsubscribe();
    }

    public writeValue(photo: FileInfoDTO | string | null): void {
        if (photo !== null && photo !== undefined) {
            if (typeof photo === 'string') {
                this.image = photo.startsWith('url(') && photo.endsWith(')') ? photo : this.wrapUrl(photo);
                this.hasImage = true;
                this.ngControl.control?.updateValueAndValidity();
            }
            else {
                this.photo = new FileInfoDTO(photo);
                if (this.photo.file) {
                    this.preview(this.photo.file);
                }
                else {
                    if (this.photo.id && !this.photo.deleted) {
                        const img: string | undefined = TLPictureUploaderComponent.cachedImages.get(this.photo.id);
                        if (img !== undefined) {
                            this.image = img;
                            this.hasImage = true;
                            this.ngControl.control?.updateValueAndValidity();
                        }
                        else if (this.requestMethod) {
                            this.requestMethodSubscription?.unsubscribe();
                            this.requestMethodSubscription = this.requestMethod().subscribe({
                                next: (photo: string) => {
                                    if (photo && photo.length > 0) {
                                        this.image = photo;
                                        this.hasImage = true;
                                        this.ngControl.control?.updateValueAndValidity();

                                        TLPictureUploaderComponent.cachedImages.set(this.photo.id!, this.image);
                                    }
                                }
                            });
                        }
                    }
                }
            }
        }
        else {
            this.photo = new FileInfoDTO();
            this.image = this.wrapUrl(this.getOptions().defaultPictureUrl);
        }
    }

    public registerOnChange(fn: (photo: FileInfoDTO | string | null) => void): void {
        this.onChange = fn;
    }

    public registerOnTouched(fn: (photo: FileInfoDTO | string | null) => void): void {
        this.onTouched = fn;
    }

    public setDisabledState(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
    }

    public validate(control: AbstractControl | null): ValidationErrors | null {
        if (this.getOptions().validations.isRequired === true) {
            if (this.photo) {
                if (this.photo.deleted || (!this.hasImage && !this.photo.file)) {
                    this.showRequiredError = true;
                    return { required: true };
                }
                else {
                    this.showRequiredError = false;
                }
            }
            else if (this.image) {
                this.showRequiredError = false;
            }
            else {
                this.showRequiredError = true;
                return { required: true };
            }
        }

        if (this.photo) {
            if (this.photo.file && !this.getOptions().isAllowedType(this.photo.file.type)) {
                this.showWrongFormatError = true;
                return { wrongformat: true };
            }
            else {
                this.showWrongFormatError = false;
            }
        }
        return null;
    }

    public onFileSelected(files: FileList): void {
        const file: File | null = files.item(0);

        if (file !== undefined && file !== null) {
            this.photo.id = undefined;
            this.photo.fileTypeId = this.fileTypes.find(x => x.code === FileTypeEnum[FileTypeEnum.PHOTO])!.value;
            this.photo.file = file;
            this.photo.size = file.size;
            this.photo.name = file.name;
            this.photo.contentType = file.type;
            this.photo.uploadedOn = new Date();
            this.photo.deleted = false;

            this.onChange(this.photo);
            this.onTouched(this.photo);
            this.preview(this.photo.file);
        }
    }

    public onFileRemoved(): void {
        if (this.photo && (this.photo.file || this.photo.id)) {
            this.photo.file = undefined;
            this.photo.deleted = true;

            this.onChange(this.photo);
            this.onTouched(this.photo);
            this.image = this.wrapUrl(this.getOptions().defaultPictureUrl);
        }
        else if (this.image) {
            this.onChange(null);
            this.onTouched(null);
            this.image = this.wrapUrl(this.getOptions().defaultPictureUrl);
        }
    }

    private preview(file: File): void {
        this.initOptions();

        if (this.getOptions().isAllowedType(file.type)) {
            const reader: FileReader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = (event: ProgressEvent<FileReader>) => {
                if (event.target?.result) {
                    const url: string = event.target?.result as string;
                    this.image = this.wrapUrl(url);
                }
            };
        }
        else {
            this.image = this.wrapUrl(this.getOptions().defaultPictureUrl);
        }
    }

    private update(): void {
        if (!this.photo?.file && !this.hasImage) {
            this.image = this.wrapUrl(this.getOptions().defaultPictureUrl);
        }
        this.addImage = this.wrapUrl(this.getOptions().buttons?.add?.imageUrl);
        this.editImage = this.wrapUrl(this.getOptions().buttons?.edit?.imageUrl);
        this.deleteImage = this.wrapUrl(this.getOptions().buttons?.delete?.imageUrl);
    }

    private wrapUrl(url: string | undefined): string {
        url = url ?? this.getOptions().defaultPictureUrl;
        return `url(${url})`;
    }

    private initOptions(): void {
        if (this.options === undefined || this.options === null) {
            this.options = new TLPictureUploaderOptions();
        }
        else {
            this.options = new TLPictureUploaderOptions(this.options);
        }

        this.options.patch(this.translate);
        this.update();
    }

    private getOptions(): TLPictureUploaderOptions {
        if (this.options === undefined || this.options === null) {
            this.options = new TLPictureUploaderOptions();
            this.options.patch(this.translate);
        }
        return this.options;
    }
}

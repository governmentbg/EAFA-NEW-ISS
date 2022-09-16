import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { ImageResize } from 'quill-image-resize-module-plus-remastered/src/ImageResize';
import Quill from 'quill';

import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { NewsManagementService } from '@app/services/administration-app/news-management.service';
import { NewsManagementEditDTO } from '@app/models/generated/dtos/NewsManagementEditDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NewsManagementDialogParams } from './models/news-management-dialog-params.model';
import { TLPictureRequestMethod } from '@app/shared/components/tl-picture-uploader/tl-picture-uploader.component';
import { ImageFormat } from './utils/image-format.util';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';

// initialization of basic quill settings (modules, fonts and image parameters)
Quill.register(ImageFormat, true);
Quill.register('modules/imageResize', ImageResize); // Register quill 

const font = Quill.import('formats/font');
font.whitelist = ['mirza', 'aref', 'sans-serif', 'monospace', 'serif'];
Quill.register(font, true);

const QUILL_MODULES = {
    toolbar: [
        ['bold', 'italic', 'underline', 'strike'],
        ['blockquote', 'code-block'],
        [{ 'list': 'ordered' }, { 'list': 'bullet' }],
        [{ 'script': 'sub' }, { 'script': 'super' }],
        [{ 'indent': '-1' }, { 'indent': '+1' }],

        [{ 'size': ['small', false, 'large', 'huge'] }],
        [{ 'header': [1, 2, 3, 4, 5, 6, false] }],

        [{ 'color': [] }, { 'background': [] }],
        [{ 'font': [] }],
        [{ 'align': [] }],

        ['clean'],
        ['link', 'video', 'image']
    ],
    imageResize: true
};

const PREVIEW_QUILL_MODULES = {
    toolbar: [],
    imageResize: true
};

@Component({
    selector: 'edit-news-management',
    templateUrl: './edit-news-management.component.html'
})
export class EditNewsManagementComponent implements IDialogComponent, OnInit {
    public readonly defaultImageUrl: string = '../../../assets/images/misc/photo-component-picture-default.jpg';
    public readonly pageCode: PageCodeEnum = PageCodeEnum.NewsManagement;
    public readonly service: NewsManagementService;

    public quillModules = QUILL_MODULES;
    public form!: FormGroup;

    public districts!: NomenclatureDTO<number>[];
    public photoRequestMethod?: TLPictureRequestMethod;

    public isContentRequired: boolean = true;
    public isSummaryRequired: boolean = true;

    private id: number | undefined;
    private model!: NewsManagementEditDTO;
    private viewMode: boolean = false;

    private readonly nomenclatures: CommonNomenclatures;

    public constructor(
        service: NewsManagementService,
        nomenclatures: CommonNomenclatures
    ) {
        this.service = service;
        this.nomenclatures = nomenclatures;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        this.districts = await NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Districts, this.nomenclatures.getDistricts.bind(this.nomenclatures), false
        ).toPromise();

        if (this.id !== undefined && this.id !== null) {
            this.service.get(this.id).subscribe({
                next: (result: NewsManagementEditDTO) => {
                    this.model = result;
                    this.photoRequestMethod = this.service.getMainImage.bind(this.service, this.model.id!);

                    this.fillForm();
                }
            });
        }
        else {
            this.model = new NewsManagementEditDTO();
        }
    }

    public setData(data: NewsManagementDialogParams | undefined, buttons: DialogWrapperData): void {
        if (data !== undefined) {
            this.id = data.id;
            this.viewMode = data.viewMode;

            if (this.viewMode) {
                this.form.disable();
                this.quillModules = PREVIEW_QUILL_MODULES;
            }
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();

        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            if (this.id !== null && this.id !== undefined) {
                this.service.edit(this.model).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    }
                });
            }
            else {
                this.service.add(this.model).subscribe({
                    next: (result: number) => {
                        this.model.id = result;
                        dialogClose(this.model);
                    }
                });
            }
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public contentChanged(quillData: Record<string, unknown>): void {
        const quillText: string | unknown = quillData['text'];

        if (typeof quillText === 'string') {
            const summary: string = quillText.substr(0, 1000);
            this.form.get('summaryControl')!.setValue(summary);
        }
    }

    private buildForm(): void {
        this.form = new FormGroup({
            titleControl: new FormControl(null, [Validators.maxLength(1000), Validators.required]),
            contentControl: new FormControl(null, Validators.required),
            summaryControl: new FormControl(null, [Validators.maxLength(1000), Validators.required]),
            filesControl: new FormControl(),
            mainImageControl: new FormControl(),
            hasNoDistrictControl: new FormControl(),
            districtsControl: new FormControl(),
            dateFromControl: new FormControl(),
            dateToControl: new FormControl()
        });

        this.form.get('hasNoDistrictControl')!.valueChanges.subscribe({
            next: (value: boolean) => {
                if (value) {
                    this.form.get('districtsControl')!.clearValidators();
                    this.form.get('districtsControl')!.disable();
                }
                else {
                    this.form.get('districtsControl')!.setValidators(Validators.required);
                    this.form.get('districtsControl')!.enable();
                }
                this.form.get('districtsControl')!.markAsPending();
            }
        });

        this.form.get('hasNoDistrictControl')!.setValue(false);
    }

    private fillForm(): void {
        this.form.get('mainImageControl')!.setValue(this.model.mainImage)
        this.form.get('titleControl')!.setValue(this.model.title);
        this.form.get('contentControl')!.setValue(this.model.content);
        this.form.get('summaryControl')!.setValue(this.model.summary);
        this.form.get('dateFromControl')!.setValue(this.model.publishStart);
        this.form.get('dateToControl')!.setValue(this.model.publishEnd);
        this.form.get('summaryControl')!.setValue(this.model.summary);
        this.form.get('hasNoDistrictControl')!.setValue(!this.model.isDistrictLimited)
        this.form.get('districtsControl')!.setValue(this.districts.filter(x => (this.model.districtsIds ?? []).includes(x.value!)));
        this.form.get('filesControl')!.setValue(this.model.files);
    }

    private fillModel(): void {
        this.model.title = this.form.get('titleControl')!.value;
        this.model.content = this.form.get('contentControl')!.value;
        this.model.summary = this.form.get('summaryControl')!.value;
        this.model.files = this.form.get('filesControl')!.value;
        this.model.mainImage = this.form.get('mainImageControl')!.value;
        this.model.publishStart = this.form.get('dateFromControl')!.value;
        this.model.publishEnd = this.form.get('dateToControl')!.value;
        this.model.isDistrictLimited = !this.form.get('hasNoDistrictControl')!.value;
        this.model.districtsIds = [];

        if (this.model.isDistrictLimited) {
            for (const district of this.form.get('districtsControl')!.value ?? []) {
                this.model.districtsIds.push(district.value);
            }
        }
    }
}
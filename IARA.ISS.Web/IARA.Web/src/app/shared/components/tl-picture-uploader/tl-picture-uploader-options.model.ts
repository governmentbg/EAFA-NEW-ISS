import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';

const DEFAULT_PHOTO_URL = '../../../../assets/images/misc/photo-component-default.png';
const ADD_IMG_URL = '../../../../assets/images/misc/photo-component-add.png';
const EDIT_IMG_URL = '../../../../assets/images/misc/photo-component-edit.png';
const DELETE_IMG_URL = '../../../../assets/images/misc/photo-component-delete.png';

export class TLPictureUploaderOptions {
    public size!: {
        width?: string,
        height?: string
    };

    public defaultPictureUrl!: string;

    public buttons!: {
        add?: {
            imageUrl?: string,
            tooltip?: string
        },
        edit?: {
            imageUrl?: string,
            tooltip?: string
        },
        delete?: {
            imageUrl?: string,
            tooltip?: string
        }
    };

    public validations!: {
        allowedFormats?: string,
        allowedFormatsMessage?: string,
        isRequired?: boolean,
        isRequiredMessage?: string
    };

    public borderRadius!: string;

    public constructor(options?: Partial<TLPictureUploaderOptions>) {
        Object.assign(this, options);
    }

    public patch(translate: FuseTranslationLoaderService): void {
        if (this.size === null || this.size === undefined) {
            this.size = {
                width: '200px',
                height: '200px'
            };
        }
        else {
            if (this.size?.width === null || this.size?.width === undefined) {
                this.size.width = '200px';
            }
            if (this.size?.height === null || this.size?.height === undefined) {
                this.size.height = '200px';
            }
        }

        if (this.defaultPictureUrl === null || this.defaultPictureUrl === undefined) {
            this.defaultPictureUrl = DEFAULT_PHOTO_URL;
        }

        if (this.buttons === null || this.buttons === undefined) {
            this.buttons = {
                add: {
                    imageUrl: ADD_IMG_URL,
                    tooltip: translate.getValue('photo-component.add-photo')
                },
                edit: {
                    imageUrl: EDIT_IMG_URL,
                    tooltip: translate.getValue('photo-component.edit-photo')
                },
                delete: {
                    imageUrl: DELETE_IMG_URL,
                    tooltip: translate.getValue('photo-component.delete-photo')
                }
            };
        }
        else {
            if (this.buttons.add === null || this.buttons.add === undefined) {
                this.buttons.add = {
                    imageUrl: ADD_IMG_URL,
                    tooltip: translate.getValue('photo-component.add-photo')
                };
            }
            else {
                if (this.buttons.add.imageUrl === null || this.buttons.add.imageUrl === undefined) {
                    this.buttons.add.imageUrl = ADD_IMG_URL;
                }
                if (this.buttons.add.tooltip === null || this.buttons.add.tooltip === undefined) {
                    this.buttons.add.tooltip = translate.getValue('photo-component.add-photo');
                }
            }

            if (this.buttons.edit === null || this.buttons.edit === undefined) {
                this.buttons.edit = {
                    imageUrl: EDIT_IMG_URL,
                    tooltip: translate.getValue('photo-component.edit-photo')
                };
            }
            else {
                if (this.buttons.edit.imageUrl === null || this.buttons.edit.imageUrl === undefined) {
                    this.buttons.edit.imageUrl = EDIT_IMG_URL;
                }
                if (this.buttons.edit.tooltip === null || this.buttons.edit.tooltip === undefined) {
                    this.buttons.edit.tooltip = translate.getValue('photo-component.edit-photo');
                }
            }

            if (this.buttons.delete === null || this.buttons.delete === undefined) {
                this.buttons.delete = {
                    imageUrl: DELETE_IMG_URL,
                    tooltip: translate.getValue('photo-component.delete-photo')
                };
            }
            else {
                if (this.buttons.delete.imageUrl === null || this.buttons.delete.imageUrl === undefined) {
                    this.buttons.delete.imageUrl = DELETE_IMG_URL;
                }
                if (this.buttons.delete.tooltip === null || this.buttons.delete.tooltip === undefined) {
                    this.buttons.delete.tooltip = translate.getValue('photo-component.delete-photo');
                }
            }
        }

        if (this.validations === null || this.validations === undefined) {
            this.validations = {
                isRequired: false,
                isRequiredMessage: translate.getValue('photo-component.photo-is-required'),
                allowedFormatsMessage: translate.getValue('photo-component.photo-format-is-not-allowed')
            };
        }
        else {
            if (this.validations.isRequired === null || this.validations.isRequired === undefined) {
                this.validations.isRequired = false;
            }
            if (this.validations.isRequiredMessage === null || this.validations.isRequiredMessage === undefined) {
                this.validations.isRequiredMessage = translate.getValue('photo-component.photo-is-required');
            }
            if (this.validations.allowedFormatsMessage === null || this.validations.allowedFormatsMessage === undefined) {
                this.validations.allowedFormatsMessage = translate.getValue('photo-component.photo-format-is-not-allowed');
            }
        }

        if (this.borderRadius === null || this.borderRadius === undefined) {
            this.borderRadius = '90%';
        }
    }

    public isAllowedType(type: string): boolean {
        if (this.validations.allowedFormats) {
            return this.validations.allowedFormats.includes(type);
        }
        return type.startsWith('image/');
    }
}
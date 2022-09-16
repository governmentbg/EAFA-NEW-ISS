import Quill from "quill";

// image format for retrieving custom attributes
const BaseImageFormat = Quill.import('formats/image');
const ATTRIBUTES = ['src', 'alt', 'height', 'width', 'style'];

export class ImageFormat extends BaseImageFormat {

    public static formats(domNode: any): any {
        return ATTRIBUTES.reduce(function (formats, attribute) {
            if (domNode.hasAttribute(attribute)) {
                (formats as any)[attribute] = domNode.getAttribute(attribute);
            }
            return formats;
        }, {});
    }

    public format(name: string, value: any): void {
        if (ATTRIBUTES.indexOf(name) > -1) {
            if (value) {
                this.domNode.setAttribute(name, value);
            } else {
                this.domNode.removeAttribute(name);
            }
        } else {
            super.format(name, value);
        }
    }
}
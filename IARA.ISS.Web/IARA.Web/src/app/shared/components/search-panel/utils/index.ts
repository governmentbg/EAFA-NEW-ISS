import { isArray, isObject, isString } from 'lodash';

export class CommonUtils {
    public static isNullOrEmpty(obj: any | string | []): boolean {
        if (obj === undefined || obj === null) {
            return true;
        }

        if (isString(obj) && (obj as string).length === 0) {
            return true;
        }

        if (isObject(obj)) {
            let isEmpty: boolean = true;
            const object = obj as any;
            if (object instanceof Date) {
                isEmpty = false;
            }
            else {
                for (const value of Object.values(object)) {
                    if (!this.isNullOrEmpty(value)) {
                        isEmpty = false;
                        break;
                    }
                }
            }

            if (isEmpty === true) {
                return true;
            }
        }

        if (isArray(obj) && (obj as []).length === 0) {
            return true;
        }

        return false;
    }
}
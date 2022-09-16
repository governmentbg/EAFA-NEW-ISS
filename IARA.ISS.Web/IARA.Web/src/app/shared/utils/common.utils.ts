import { AbstractControl, ValidatorFn } from '@angular/forms';
import { isArray, isObject, isString } from 'util';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { TLError } from '../components/input-controls/models/tl-error.model';
import { DateUtils } from './date.utils';

export class CommonUtils {

    public static readonly IC_ICON_SIZE: number = 20;

    public static readonly COUNTRIES_BG: string = 'BGR';
    public static readonly INSTITUTIONS_IARA: string = 'ИАРА';

    public static readonly MENU_ICONS_MAP: Map<string, string> = new Map([
        ['fishing_vessels', 'fa-ship'],
        ['commercial_fishing', 'fa-fish'],
        ['fishing_quotas', 'fa-weight'],
        ['poundnets', 'fa-hashtag'],
        ['administrative_services', 'fa-concierge-bell'],
        ['application_processing', 'fa-hourglass-half'],
        ['statistical_forms', 'fa-chart-line'],
        ['aqua_culture_farms', 'fa-tractor'],
        ['qualified_fishers', 'fa-id-badge'],
        ['recreational_fishing', 'fa-vest'],
        ['buyers_and_sales_centers', 'fa-handshake'],
        ['catches_and_sales', 'fa-money-bill-alt'],
        ['scientific_fishing', 'fa-flask'],
        ['control_activity', 'fa-users-cog'],
        ['documents', 'fa-archive'],
        ['reports', 'fa-th-list'],
        ['cross_checks', 'fa-check-double'],
        ['statement_findings', 'fa-file-alt'],
        ['personal_data_legal_entities_and_persons_reports', 'group'],
        ['administration', 'fa-cogs'],
        ['fishing-capacity', 'fa-tachometer-alt']
    ]);

    public static getValueOrDefault<T>(value: unknown): unknown {
        if (value === '') {
            return null;
        } else {
            return value;
        }
    }

    public static toBoolean(value: string): boolean {
        value = value.toLowerCase();
        return value == 'true' ? true : false;
    }

    public static sanitizeModelStrings<T>(model: any, trim: boolean = true): T {
        const keys = Object.keys(model);

        for (const property of keys) {
            if (model[property] === '') {
                model[property] = null;
            }
            else if (trim && typeof model[property] === 'string') {
                model[property] = (model[property] as string)?.trim();
            }
            else if (model[property] instanceof Date) {
                const date: Date = model[property] as Date;
                if (date.getTime() < DateUtils.MIN_DATE.getTime()) {
                    model[property] = null;
                }
                else if (date.getTime() > DateUtils.MAX_DATE.getTime()) {
                    model[property] = null;
                }
            }
        }

        return model as T;
    }



    public static toColumnDataType(originalType: string | undefined): string {
        switch (originalType) {
            case 'date': return 'date';
            case 'boolean': return 'boolean';
            default: return 'string';
        }
    }

    /**
     * Returns whether the parameter passed is null, undefined or equal to empty string or empty array
     * @param value - any object
     */
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

    /**
     * Returns whether the parameter is null or undefined
     * @param value can be any object
     */
    public static isNullOrUndefined(value: unknown): boolean {
        if (value === undefined || value === null) {
            return true;
        }
        return false;
    }

    /**
     * Checks whether `num` parameter is null/undefined, empty or NaN.
     * @param num Parameter that is checked whether its a null or NaN.
     */
    public static isNumberNullOrNaN(num: number): boolean {
        return CommonUtils.isNullOrEmpty(num) || isNaN(num);
    }

    /**
     * Indicates whether a specified string is null, undefined, empty, or consists only of white-space characters.
     * @param str - any string
     */
    public static isNullOrWhiteSpace(str: string | undefined | null): boolean {
        return str === undefined
            || str === null
            || str.trim().length === 0;
    }

    public static applyFilterToTable(filterValue: string, items: any[]): any[] {
        const value = filterValue.toString().toLowerCase().trim();
        const keys = Object.keys(items[0]);

        return items.filter(item => {
            for (const key of keys) {
                if ((item[key] && item[key].toString().toLowerCase().indexOf(value) !== -1) || value === undefined || value === null) {
                    return true;
                }
            }
            return false;
        });
    }

    static zeroPad = (num: any, places: number) => String(num).padStart(places, '0');

    public static hasControlRequiredValidator(validator: ValidatorFn | null | undefined): boolean {
        let hasRequiredValidator = false;
        if (validator) {
            const validation = validator({} as AbstractControl);
            if (validation?.required) {
                hasRequiredValidator = true;
            }
            else {
                hasRequiredValidator = false;
            }
        }
        return hasRequiredValidator;
    }

    public static getFormControlName(control: AbstractControl): string {
        const parent = control.parent;
        let controlName: string = '';
        if (parent !== null && parent !== undefined) {
            for (const name of Object.keys(parent.controls)) {
                if (control === parent.get(name)) {
                    controlName = name;
                }
            }
        }

        return controlName;
    }

    public static sortArrayByProperty<T>(items: T[], property: string, dir: 'asc' | 'desc'): T[] {
        return items.sort((lhs: T, rhs: T) => {
            const left: T[keyof T] = lhs[property as keyof T];
            const right: T[keyof T] = rhs[property as keyof T];

            if (typeof left === 'number' && typeof right === 'number') {
                return dir === 'asc' ? left - right : right - left;
            }
            if (typeof left === 'string' && typeof right === 'string') {
                return dir === 'asc' ? left.localeCompare(right) : right.localeCompare(left);
            }
            if (typeof left === 'object' && typeof right === 'object') {
                if (left instanceof Date && right instanceof Date) {
                    return dir === 'asc' ? left.getTime() - right.getTime() : right.getTime() - left.getTime();
                }
            }
            return 0;
        });
    }

    public static getControlErrorLabelTextForRegixExpectedValueValidator(controlName: string, errorValue: unknown, errorCode: string): TLError | undefined {
        switch (errorCode) {
            case 'expectedValueNotMatching': {
                let expectedValue: string = '';
                if (errorValue instanceof NomenclatureDTO) {
                    expectedValue = errorValue.displayName as string;
                }
                else if (typeof errorValue === 'string' || typeof errorValue === 'number' || typeof errorValue === 'boolean') {
                    expectedValue = errorValue.toString();
                }
                else if (errorValue instanceof Date) {
                    expectedValue = DateUtils.ToDisplayDateString(errorValue as Date);
                }

                return new TLError({ text: `RegiX: ${expectedValue}`, type: 'warn' });
            };
            default: return undefined;
        }
    }

    public static objectsEqual(obj1: unknown, obj2: unknown): boolean {
        let areEqual: boolean = false;

        const obj1Json: string = JSON.stringify(obj1);
        const obj2Json: string = JSON.stringify(obj2);

        if (obj1Json === obj2Json) {
            areEqual = true;
        }

        return areEqual;
    }

    public static convertKeysToCamelCase(obj: Record<string, unknown> | unknown): unknown | null | undefined {
        if (obj === null || obj === undefined) {
            return obj;
        }
        else if (Array.isArray(obj)) {
            return obj.map(CommonUtils.convertKeysToCamelCase);
        }
        if (typeof obj !== 'object') {
            return obj;
        }
        else {
            const record: Record<string, unknown> = obj as Record<string, unknown>;

            return Array.from(CommonUtils.getProperties(record)).reduce((prev: Record<string, unknown>, current: string) => {
                const newKey: string = `${current[0].toLowerCase()}${current.slice(1)}`;

                if (typeof record[current] === 'object') {
                    const value: unknown = record[current];
                    if (value instanceof Number || value instanceof String || value instanceof Boolean || value instanceof Date) {
                        prev[newKey] = value;
                    }
                    else {
                        prev[newKey] = CommonUtils.convertKeysToCamelCase(record[current]);
                    }
                }
                else {
                    prev[newKey] = record[current];
                }
                return prev;
            }, {});
        }
    }

    public static getProperties(obj: any): Set<string> {
        const lowercase = (str: string) => {
            return `${str[0].toLowerCase()}${str.slice(1)}`;
        };

        const result: Set<string> = new Set<string>();
        for (const property in obj) {
            if (property[0] !== '_') {
                result.add(lowercase(property));
            }
        }

        const prototype = Object.getPrototypeOf(obj);
        if (prototype !== null && prototype !== undefined) {
            const descriptors = Object.getOwnPropertyDescriptors(prototype);
            for (const descriptor in descriptors) {
                if (typeof obj[descriptor] !== 'function' && descriptor[0] !== '_') {
                    result.add(lowercase(descriptor));
                }
            }
        }
        return result;
    }

    public static groupByKey(array: Array<any>, key: string): Record<string, unknown> {
        return array
            .reduce((hash, obj) => {
                if (obj[key] === undefined) return hash;
                return Object.assign(hash, { [obj[key]]: ((hash[obj[key]] || []) as any).concat(obj) })
            }, {});
    }

    public static groupBy = <T, K extends keyof any>(list: T[], getKey: (item: T) => K): Record<K, T[]> =>
        list.reduce((previous, currentItem) => {
            const group = getKey(currentItem);
            if (!previous[group]) previous[group] = [];
            previous[group].push(currentItem);
            return previous;
        }, {} as Record<K, T[]>);

    public static getFileAsBase64(file: File): Promise<string> {
        type Resolve = (result: string) => void;
        type Reject = (error: ProgressEvent<FileReader>) => void;

        return new Promise((resolve: Resolve, reject: Reject) => {
            const reader: FileReader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = (event: ProgressEvent<FileReader>) => { resolve(event.target!.result as string); };
            reader.onerror = (error: ProgressEvent<FileReader>) => { reject(error); };
        });
    }
}
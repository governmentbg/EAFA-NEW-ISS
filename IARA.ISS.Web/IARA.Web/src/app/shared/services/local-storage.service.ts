import { Injectable } from '@angular/core';
import { CommonUtils } from '../utils/common.utils';

@Injectable({
    providedIn: 'root'
})
export class LocalStorageService {
    get Length(): number {
        return localStorage.length;
    }

    public addOrUpdate(key: string, value: string): void {
        localStorage.setItem(key, value);
    }

    public get(key: string): string {
        const value: string | null = localStorage.getItem(key);
        if (CommonUtils.isNullOrUndefined(value)) {
            return '';
        }
        else {
            return value as string;
        }
    }

    public removeItem(key: string): void {
        localStorage.removeItem(key);
    }

    public clear(): void {
        localStorage.clear();
    }

    public hasItem(key: string): boolean {
        const item = localStorage.getItem(key);
        if (CommonUtils.isNullOrEmpty(item)) {
            return false;
        }
        else {
            return true;
        }
    }
}
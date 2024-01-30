import { StorageTypes } from '../enums/storage-types.enum';
import { CommonUtils } from '../utils/common.utils';

export class StorageService {
    private storage?: Storage;
    private cookieStorage?: CookieStorage;
    private type: StorageTypes;

    protected constructor(type: StorageTypes) {
        this.type = type;

        switch (type) {
            case StorageTypes.Local:
                this.storage = localStorage;
                break;
            case StorageTypes.Session:
                this.storage = sessionStorage;
                break;
            case StorageTypes.Cookie:
                this.cookieStorage = window.cookieStore;
                break;
            default:
                throw new Error('Not supported storage type');
        }
    }

    public static getStorage(type: StorageTypes) {
        return new StorageService(type);
    }

    public get length(): Promise<number> | number {
        if (this.storage != undefined) {
            return this.storage.length;
        } else if (this.cookieStorage != undefined) {
            return this.cookieStorage.getAll().then(x => x.length);
        } else {
            throw new Error("Not supported method of storage type: 'Cookie'");
        }
    }

    public addOrUpdate(key: string, value: string): void {
        if (this.type != StorageTypes.Cookie && this.storage != undefined) {
            this.storage.setItem(key, value);
        } else {
            this.cookieStorage?.set(key, value);
        }
    }

    public get(key: string): Promise<string | null> | string | null {
        if (this.storage != undefined) {
            const value: string | null = this.storage.getItem(key);
            if (CommonUtils.isNullOrUndefined(value)) {
                return null;
            }
            else {
                return value as string;
            }
        } else if (this.cookieStorage != undefined) {
            return this.cookieStorage.get(key).then(result => {
                if (result == undefined || result == null) {
                    return null;
                } else {
                    return result.value as string;
                }
            });
        } else {
            throw new Error("No storage available");
        }
    }

    public async removeItem(key: string): Promise<boolean> {
        if (await this.hasItem(key)) {

            if (this.storage != undefined) {
                this.storage.removeItem(key);
            } else if (this.cookieStorage != undefined) {
                this.cookieStorage.delete(key);
            } else {
                throw new Error('No storage available');
            }
            return true;
        } else {
            return false;
        }
    }

    public async clear(): Promise<void> {
        if (this.type != StorageTypes.Cookie && this.storage != undefined) {
            this.storage.clear();
        } else {
            await this.cookieStorage?.getAll().then(async (result) => {
                for (const item of result) {
                    await this.cookieStorage?.delete(item.name);
                }
            });
        }
    }

    public hasItem(key: string): Promise<boolean> | boolean {
        if (this.storage != undefined) {
            const item = this.storage.getItem(key);
            if (CommonUtils.isNullOrEmpty(item)) {
                return false;
            }
            else {
                return true;
            }
        } else if (this.cookieStorage != undefined) {
            return this.cookieStorage.get(key).then(result => {
                if (result != undefined) {
                    return true;
                } else {
                    return false;
                }
            });
        } else {
            throw new Error('No storage available');
        }
    }
}

declare global {
    interface Window {
        cookieStore: CookieStorage;
    }
    interface Document {
        cookieStore: CookieStorage;
    }

    interface CookieStorage {
        get(name: string): Promise<Cookie>;
        getAll(): Promise<Cookie[]>;
        delete(name: string): Promise<void>;
        set(name: string, value: string): Promise<void>;
    }

    interface Cookie {
        domain: string;
        expires: number;
        name: string;
        path: string;
        sameSite: string;
        secure: boolean;
        value: string
    }
}
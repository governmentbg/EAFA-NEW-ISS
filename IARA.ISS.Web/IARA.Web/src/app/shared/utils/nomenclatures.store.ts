import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, of, Subject } from 'rxjs';

import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

export class DataLoadParams {
    public loadSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    public loadStarted: boolean = false;

    public constructor(params?: Partial<DataLoadParams>) {
        Object.assign(this, params);
    }
}

export class NomenclatureStore {
    public static get instance(): NomenclatureStore {
        if (NomenclatureStore._instance === null || NomenclatureStore._instance === undefined) {
            this._instance = new NomenclatureStore();
        }

        return this._instance;
    }

    private static _instance: NomenclatureStore;

    private nomenclatures: Map<NomenclatureTypes, NomenclatureDTO<unknown>[]>;
    private loadParams: Map<NomenclatureTypes, DataLoadParams>;

    private constructor() {
        this.nomenclatures = new Map<NomenclatureTypes, NomenclatureDTO<unknown>[]>();
        this.loadParams = new Map<NomenclatureTypes, DataLoadParams>();
    }

    public getNomenclature<T>(type: NomenclatureTypes, requestService: () => Observable<NomenclatureDTO<T>[]>, hasEmptyItem: boolean = true): Observable<NomenclatureDTO<T>[]> {
        if (this.nomenclatures.has(type)) {
            const values: NomenclatureDTO<T>[] = this.nomenclatures.get(type) as NomenclatureDTO<T>[];
            return of(values);
        }
        else {
            let params: DataLoadParams | undefined = this.loadParams.get(type);
            if (params === undefined) {
                params = new DataLoadParams();
                this.loadParams.set(type, params);
            }

            const subject: Subject<NomenclatureDTO<T>[]> = new Subject<NomenclatureDTO<T>[]>();

            if (!params.loadSubject.value) {
                if (params.loadStarted) {
                    // todo unsubscribe
                    params.loadSubject.subscribe({
                        next: (yes: boolean) => {
                            if (yes) {
                                const values: NomenclatureDTO<T>[] = this.nomenclatures.get(type) as NomenclatureDTO<T>[];
                                subject.next(values);
                                subject.complete();
                            }
                        }
                    });
                }
                else {
                    params.loadStarted = true;
                    requestService().subscribe({
                        next: (result: NomenclatureDTO<T>[]) => {
                            if (hasEmptyItem) {
                                result.unshift(this.createEmptyElement<T>());
                            }

                            this.nomenclatures.set(type, result);

                            params!.loadSubject.next(true);
                            subject.next(result);
                            subject.complete();
                        },
                        error: (error: HttpErrorResponse) => {
                            subject.error(error);
                        }
                    });
                }
            }
            else {
                const values: NomenclatureDTO<T>[] = this.nomenclatures.get(type) as NomenclatureDTO<T>[];
                subject.next(values);
                subject.complete();
            }

            return subject;
        }
    }

    public getNomenclatureItem<T>(type: NomenclatureTypes, value: T): NomenclatureDTO<T> | undefined {
        if (this.nomenclatures.has(type)) {
            const values: NomenclatureDTO<T>[] = this.nomenclatures.get(type) as NomenclatureDTO<T>[];
            const item: NomenclatureDTO<T> | undefined = values.find(x => x.value === value);

            return item;
        }
        else {
            throw new Error(`Nomeclature ${type} not loaded.`);
        }
    }

    public refreshNomenclature<T>(type: NomenclatureTypes, requestService: () => Observable<NomenclatureDTO<T>[]>): Observable<NomenclatureDTO<T>[]> {
        this.clearNomenclature(type);
        return this.getNomenclature(type, requestService);
    }

    public clearNomenclature<T>(type: NomenclatureTypes): void {
        if (this.nomenclatures.has(type)) {
            this.nomenclatures.delete(type);
        }

        if (this.loadParams.has(type)) {
            this.loadParams.delete(type);
        }
    }

    public static getValue<T>(selectedItem: NomenclatureDTO<T>): T | undefined {
        if (selectedItem === undefined || selectedItem === null) {
            return undefined;
        }
        else {
            return selectedItem.value;
        }
    }

    public static isNomenclature<T>(obj: T): boolean {
        if (obj !== null && obj !== undefined && typeof obj === 'object') {
            return 'value' in obj && 'displayName' in obj;
        }
        return false;
    }

    private createEmptyElement<T>(): NomenclatureDTO<T> {
        const value = new NomenclatureDTO<T>({
            value: undefined,
            displayName: '',
            isActive: true
        });
        return value;
    }
}
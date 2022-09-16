import { Subscription } from 'rxjs';
import { DataLoadParams } from './nomenclatures.store';

export type DataLoaderMethod = () => Subscription;
export type DataLoaderCallback = () => void;

export class FormControlDataLoader {
    private loadMethod: DataLoaderMethod;
    private params: DataLoadParams;

    public constructor(loadMethod: DataLoaderMethod) {
        this.loadMethod = loadMethod;
        this.params = new DataLoadParams();
    }

    public load(callback?: DataLoaderCallback): void {
        // данните вече са заредени
        if (this.isDataLoaded()) {
            if (callback) {
                callback();
            }
        }
        // данните не са заредени
        else {
            // започнала е заявка за зареждане 
            if (this.isLoadStarted()) {
                this.params.loadSubject.subscribe({
                    next: (yes: boolean) => {
                        if (yes && callback) {
                            callback();
                        }
                    }
                });
            }
            // не е започнала заявка за зареждане
            else {
                this.params.loadStarted = true;

                this.params.loadSubject.subscribe({
                    next: (yes: boolean) => {
                        if (yes && callback) {
                            callback();
                        }
                    }
                });

                this.loadMethod();
            }
        }
    }

    public isDataLoaded(): boolean {
        return this.params.loadSubject.value;
    }

    public isLoadStarted(): boolean {
        return this.params.loadStarted;
    }

    // извиква се при клиента, щом се заредят данните
    public complete(): void {
        this.params.loadSubject.next(true);
    }
}
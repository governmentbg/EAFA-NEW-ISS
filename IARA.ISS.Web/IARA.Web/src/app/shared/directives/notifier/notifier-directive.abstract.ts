import { Subject } from 'rxjs';
import { INotifierComponent } from './notifier-component.interface';

export abstract class AbstractNotifierDirective {
    public notifierComponent: INotifierComponent | null = null;

    public onNotify: Subject<void> = new Subject<void>();
    public onStart: Subject<void> = new Subject<void>();
    public onStop: Subject<void> = new Subject<void>();

    public get notifying(): boolean {
        return this.isNotifying;
    }

    private isNotifying: boolean = false;

    public start(): void {
        this.isNotifying = true;
        this.onStart.next();
    }

    public stop(): void {
        this.isNotifying = false;
        this.onStop.next();
    }

    public abstract notify(): void;
}
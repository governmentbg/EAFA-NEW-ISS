import { Subject } from 'rxjs';

export class Notifier {
    public onNotify!: Subject<void>;
    public onStart: Subject<void> = new Subject<void>();
    public onStop: Subject<void> = new Subject<void>();

    public start(): void {
        this.onStart.next();
    }

    public stop(): void {
        this.onStop.next();
    }
}
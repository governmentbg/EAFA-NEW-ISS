import { AfterContentInit, ContentChildren, Directive, Input, OnDestroy, OnInit, QueryList } from '@angular/core';
import { combineLatest, Subject, Subscription } from 'rxjs';
import { AbstractNotifierDirective } from './notifier-directive.abstract';
import { Notifier } from './notifier.class';
import { NotifierDirective } from './notifier.directive';

@Directive({
    selector: '[notifierGroup]'
})
export class NotifierGroupDirective extends AbstractNotifierDirective implements OnInit, AfterContentInit, OnDestroy {
    @Input('notifierGroup')
    public notifier!: Notifier;

    @ContentChildren(NotifierDirective, { descendants: true })
    private notifiers: QueryList<NotifierDirective> | undefined;

    private subjects: Subject<void>[] | undefined;
    private notifySubscription: Subscription | undefined;
    private subscriptions: Subscription[] = [];

    public constructor() {
        super();
    }

    public ngAfterContentInit(): void {
        this.notifiers?.changes.subscribe({
            next: (test) => {
                if (this.notifying) {
                    this.stop();
                    this.start();
                }
            }
        });
    }

    public ngOnInit(): void {
        this.notifier.onNotify = this.onNotify;

        this.subscriptions.push(this.notifier.onStart.subscribe({
            next: () => {
                this.start();
            }
        }));

        this.subscriptions.push(this.notifier.onStop.subscribe({
            next: () => {
                this.stop();
            }
        }));
    }

    public ngOnDestroy(): void {
        if (this.notifySubscription) {
            this.notifySubscription.unsubscribe();
        }

        for (const sub of this.subscriptions) {
            sub.unsubscribe();
        }
    }

    public start(): void {
        super.start();
        
        if (this.notifiers) {
            for (const notifier of this.notifiers.toArray()) {
                notifier.start();
            }
        }

        this.notify();
        this.onStart.next();
    }

    public stop(): void {
        super.stop();

        if (this.notifiers) {
            for (const notifier of this.notifiers.toArray()) {
                notifier.stop();
            }
        }
        this.onStop.next();
    }

    public notify(): void {
        if (this.notifying && this.notifiers) {
            this.subjects = this.getSubjects();
            
            if (this.subjects.length === 0) {
                setTimeout(() => {
                    this.onNotify.next();
                });
            }
            else {
                this.notifySubscription?.unsubscribe();
                this.notifySubscription = combineLatest(this.subjects).subscribe({
                    next: (results: void[]) => {
                        if (results.length === this.notifiers!.length) {
                            this.onNotify.next();
                        }
                    }
                });
            }
        }
    }

    private getSubjects(): Subject<void>[] {
        const result: Subject<void>[] = [];
        if (this.notifiers) {
            for (const notifier of this.notifiers.toArray()) {
                result.push(notifier.onNotify);
            }
        }
        return result;
    }
}
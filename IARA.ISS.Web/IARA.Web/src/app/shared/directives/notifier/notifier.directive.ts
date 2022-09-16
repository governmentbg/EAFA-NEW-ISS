import { Directive, Input, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { AbstractNotifierDirective } from './notifier-directive.abstract';
import { Notifier } from './notifier.class';

@Directive({
    selector: '[notifier]'
})
export class NotifierDirective extends AbstractNotifierDirective implements OnInit, OnDestroy {
    @Input('notifier')
    public notifier: Notifier | undefined;

    private subscriptions: Subscription[] = [];

    public constructor() {
        super();
    }

    public ngOnInit(): void {
        if (this.notifier) {
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
    }

    public ngOnDestroy(): void {
        for (const sub of this.subscriptions) {
            sub.unsubscribe();
        }
    }

    public notify(): void {
        if (this.notifying) {
            this.onNotify.next();
        }
    }
}
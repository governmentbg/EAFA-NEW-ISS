import { NgControl } from '@angular/forms';
import { INotifierComponent } from '../directives/notifier/notifier-component.interface';
import { AbstractNotifierDirective } from '../directives/notifier/notifier-directive.abstract';
import { Notifier } from '../directives/notifier/notifier.class';
import { ValidityChecker } from '../directives/validity-checker/validity-checker.abstract';
import { CustomFormControl } from './custom-form-control';

export class BindNotifierOptions {
    public onNotify: boolean = true;
    public onStart: boolean = true;
    public onStop: boolean = true;

    public constructor(options?: Partial<BindNotifierOptions>) {
        Object.assign(this, options);
    }
}

export abstract class NotifyingCustomFormControl<ValueType> extends CustomFormControl<ValueType> implements INotifierComponent {
    protected readonly notifier: AbstractNotifierDirective | null;

    public constructor(
        ngControl: NgControl,
        registerValueChanges: boolean = true,
        validityChecker: ValidityChecker | undefined = undefined,
        notifier: AbstractNotifierDirective | undefined = undefined
    ) {
        super(ngControl, registerValueChanges, validityChecker);

        this.notifier = notifier ?? null;
    }

    public notify(): void {
        if (this.notifier) {
            setTimeout(() => {
                this.notifier!.notify();
            });
        }
    }

    protected initNotifyingCustomFormControl(notifier?: Notifier, onNotifyCallback?: () => void): void {
        this.initCustomFormControl();
        this.initNotifier(notifier, onNotifyCallback);
    }

    private initNotifier(notifier: Notifier | undefined, onNotifyCallback: (() => void) | undefined): void {
        if (this.notifier) {
            this.notifier.notifierComponent = this;

            if (notifier) {
                this.notifier.onStart.subscribe({
                    next: () => {
                        notifier.start();
                    }
                });

                this.notifier.onStop.subscribe({
                    next: () => {
                        notifier.stop();
                    }
                });

                if (onNotifyCallback) {
                    const interval: number = setInterval(() => {
                        while (notifier!.onNotify === undefined || notifier!.onNotify === null);

                        notifier!.onNotify.subscribe({
                            next: () => {
                                onNotifyCallback!();
                            }
                        });

                        clearInterval(interval);
                    });
                }
            }
        }
    }
}
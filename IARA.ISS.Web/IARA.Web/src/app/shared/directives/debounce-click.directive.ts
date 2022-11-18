import { Directive, EventEmitter, HostListener, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { throttleTime } from 'rxjs/operators';

@Directive({
    selector: '[tlDebounceClick]'
})
export class TLDebounceClickDirective implements OnInit, OnDestroy {
    @Input()
    public debounceTime: number = 500;

    @Output()
    public tlDebounceClick: EventEmitter<Event> = new EventEmitter<Event>();

    private clicksSub: Subject<Event> = new Subject<Event>();
    private subscription: Subscription | undefined;

    @HostListener('click', ['$event'])
    private clickEvent(event: PointerEvent) {
        event.preventDefault();
        event.stopPropagation();
        this.clicksSub.next(event);
    }

    public ngOnInit(): void {
        this.subscription = this.clicksSub.pipe(
            throttleTime(this.debounceTime, undefined, { leading: true, trailing: true })
        ).subscribe({
            next: (e: Event) => {
                this.tlDebounceClick.emit(e);
            }
        });
    }

    public ngOnDestroy(): void {
        if (this.subscription !== null && this.subscription !== undefined) {
            this.subscription.unsubscribe();
        }
    }
}
import { Directive, ElementRef, Host, Input, OnDestroy, Optional, Self } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { BehaviorSubject, forkJoin, Subscription } from 'rxjs';

import { ValidityChecker } from './validity-checker.abstract';
import { TLCardComponent } from '../../components/tl-card/tl-card.component';
import { TLExpansionPanelComponent } from '../../components/tl-expansion-panel/tl-expansion-panel.component';

@Directive({
    selector: `[validityChecker]`
})
export class ValidityCheckerDirective extends ValidityChecker implements OnDestroy {
    @Input('validityChecker')
    public form?: FormGroup;

    @Input()
    public validityCheckerExtraCondition: boolean = true;

    private hostElement: ElementRef;
    private cardComponent: TLCardComponent | undefined;
    private expansionPanelComponent: TLExpansionPanelComponent | undefined;

    private get element(): HTMLElement {
        return this.hostElement.nativeElement as HTMLElement;
    }

    private static expansionPanelSubjects: BehaviorSubject<boolean>[] = [];
    private static expansionPanelSubscriptions: Subscription[] = [];

    private get subjects(): BehaviorSubject<boolean>[] {
        return ValidityCheckerDirective.expansionPanelSubjects;
    }

    private set subjects(subs: BehaviorSubject<boolean>[]) {
        ValidityCheckerDirective.expansionPanelSubjects = subs;
    }

    private get subscriptions(): Subscription[] {
        return ValidityCheckerDirective.expansionPanelSubscriptions;
    }

    private set subscriptions(subs: Subscription[]) {
        ValidityCheckerDirective.expansionPanelSubscriptions = subs;
    }

    private static refs: ValidityCheckerDirective[] = [];

    public constructor(
        hostElement: ElementRef,
        @Host() @Self() @Optional() cardComponent: TLCardComponent,
        @Host() @Self() @Optional() expansionPanelComponent: TLExpansionPanelComponent
    ) {
        super();

        this.hostElement = hostElement;
        this.cardComponent = cardComponent;
        this.expansionPanelComponent = expansionPanelComponent;

        ValidityCheckerDirective.refs.push(this);
    }

    public ngOnDestroy(): void {
        ValidityCheckerDirective.refs.pop();

        if (ValidityCheckerDirective.refs.length === 0) {
            for (const sub of this.subscriptions) {
                sub.unsubscribe();
            }
            this.subscriptions = [];

            for (const sub of this.subjects) {
                sub.complete();
            }
            this.subjects = [];
        }
    }

    public validate(): void {
        if (this.validityChecker) {
            this.validityChecker.check();
        }
        else {
            if (this.valid()) {
                this.setValidTitle();
                this.collapse();
            }
            else {
                this.setInvalidTitle();
                this.expand();
            }
        }
    }

    public valid(): boolean {
        if (this.validityChecker) {
            return this.validityChecker.isValid();
        }

        if (this.validityCheckerExtraCondition === false) {
            return false;
        }

        if (this.form) {
            const formControlNames: string[] = this.getFormControlNames();
            for (const formControl of formControlNames) {
                if (this.form.controls[formControl].invalid) {
                    return false;
                }
            }
        }
        return true;
    }

    public setInvalidTitle(): void {
        if (this.isCard()) {
            this.cardComponent!.hasError = true;
        }
        else if (this.isExpansionPanel()) {
            this.expansionPanelComponent!.hasError = true;
        }
    }

    public setValidTitle(): void {
        if (this.isCard()) {
            this.cardComponent!.hasError = false;
        }
        else if (this.isExpansionPanel()) {
            this.expansionPanelComponent!.hasError = false;
        }
    }

    public expand(): boolean {
        if (this.isExpansionPanel()) {
            if (this.expansionPanelComponent!.expanded === false) {
                const subject = new BehaviorSubject<boolean>(false);
                this.subscriptions.push(this.expansionPanelComponent!.afterExpand.subscribe({
                    next: () => {
                        subject.next(true);
                        subject.complete();
                    }
                }));

                this.subjects.push(subject);

                setTimeout(() => {
                    this.expansionPanelComponent!.expanded = true;
                });
            }
            return true;
        }
        return false;
    }

    public collapse(): boolean {
        if (this.isExpansionPanel()) {
            if (this.expansionPanelComponent!.expanded === true) {
                const subject = new BehaviorSubject<boolean>(false);
                this.subscriptions.push(this.expansionPanelComponent!.afterCollapse.subscribe({
                    next: () => {
                        subject.next(true);
                        subject.complete();
                    }
                }));

                this.subjects.push(subject);

                setTimeout(() => {
                    this.expansionPanelComponent!.expanded = false;
                });
            }
            return true;
        }
        return false;
    }

    public scrollIntoView(): void {
        if (this.subjects.length > 0) {
            this.subscriptions.push(forkJoin(this.subjects).subscribe({
                next: (values: boolean[]) => {
                    setTimeout(() => {
                        this.element.scrollIntoView({ behavior: 'smooth', block: 'start' });

                        for (const sub of this.subscriptions) {
                            sub.unsubscribe();
                        }
                        this.subjects = [];
                        this.subscriptions = [];
                    });
                }
            }));
        }
        else {
            setTimeout(() => {
                this.element.scrollIntoView({ behavior: 'smooth', block: 'start' });
            });
        }
    }

    private getFormControlNames(): string[] {
        const result: string[] = [];

        this.element.querySelectorAll('[formControlName]').forEach((element: Element) => {
            const name: string = element.getAttribute('formControlName')!;

            if (this.isFormControlInGroup(name)) {
                result.push(name);
            }
        });

        return result;
    }

    private isFormControlInGroup(formControl: string): boolean {
        return Object.keys((this.form as FormGroup).controls).includes(formControl);
    }

    private isCard(): boolean {
        return this.cardComponent !== undefined && this.cardComponent !== null;
    }

    private isExpansionPanel(): boolean {
        return this.expansionPanelComponent !== undefined && this.expansionPanelComponent !== null;
    }

    private isCustom(): boolean {
        return !this.isCard() && !this.isExpansionPanel();
    }
}
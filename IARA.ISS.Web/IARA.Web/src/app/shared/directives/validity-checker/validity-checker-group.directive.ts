import { ContentChildren, Directive, QueryList } from '@angular/core';

import { ValidityChecker } from './validity-checker.abstract';
import { ValidityCheckerDirective } from './validity-checker.directive';

@Directive({
    selector: '[validityCheckerGroup]'
})
export class ValidityCheckerGroupDirective extends ValidityChecker {
    @ContentChildren(ValidityCheckerDirective, { descendants: true })
    private checkers!: QueryList<ValidityCheckerDirective>;

    public constructor() {
        super();
    }

    public validate(scrollIntoView: boolean = true): void {
        const checkers: ValidityCheckerDirective[] = this.checkers.toArray();

        if (!this.valid()) {
            for (const checker of checkers) {
                checker.validate();
            }

            if (scrollIntoView) {
                const invalidIdx: number = checkers.findIndex(x => !x.valid());
                if (invalidIdx !== -1) {
                    checkers[invalidIdx].scrollIntoView();
                }
            }
        }
        else {
            for (const checker of checkers) {
                checker.setValidTitle();
            }
        }
    }

    public valid(): boolean {
        const checkers: ValidityCheckerDirective[] = this.checkers.toArray();

        for (const checker of checkers) {
            if (!checker.valid()) {
                return false;
            }
        }
        return true;
    }

    public setInvalidTitle(): void {
        const checkers: ValidityCheckerDirective[] = this.checkers.toArray();

        for (const checker of checkers) {
            checker.setInvalidTitle();
        }
    }

    public setValidTitle(): void {
        const checkers: ValidityCheckerDirective[] = this.checkers.toArray();

        for (const checker of checkers) {
            checker.setValidTitle();
        }
    }

    public expand(): boolean {
        const checkers: ValidityCheckerDirective[] = this.checkers.toArray();

        let result: boolean = false;
        for (const checker of checkers) {
            result ||= checker.expand();
        }
        return result;
    }

    public collapse(): boolean {
        const checkers: ValidityCheckerDirective[] = this.checkers.toArray();

        let result: boolean = false;
        for (const checker of checkers) {
            result ||= checker.collapse();
        }
        return result;
    }

    public scrollIntoView(): void {
        const checkers: ValidityCheckerDirective[] = this.checkers.toArray();

        if (checkers.length > 0) {
            checkers[0].scrollIntoView();
        }
    }
}
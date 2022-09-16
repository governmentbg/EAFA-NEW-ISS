import { Component, Inject, ViewChild } from '@angular/core';
import { IValidityCheckerComponent } from '../directives/validity-checker/validity-checker-component.interface';
import { ValidityCheckerGroupDirective } from '../directives/validity-checker/validity-checker-group.directive';
import { ValidityChecker } from '../directives/validity-checker/validity-checker.abstract';

@Component({ template: '' })
export class ValidityCheckerComponent implements IValidityCheckerComponent {
    @ViewChild(ValidityCheckerGroupDirective)
    protected readonly validityCheckerGroup: ValidityCheckerGroupDirective | undefined;

    protected readonly validityChecker: ValidityChecker | null;

    public constructor(@Inject(ValidityChecker) validityChecker: ValidityChecker | undefined) {
        this.validityChecker = validityChecker ?? null;
    }

    public check(): void {
        this.validityCheckerGroup!.validate(false);
    }

    public isValid(): boolean {
        return this.validityCheckerGroup!.valid();
    }

    protected initValidityChecker(): void {
        if (this.validityChecker) {
            this.validityChecker.validityChecker = this;
        }
    }
}
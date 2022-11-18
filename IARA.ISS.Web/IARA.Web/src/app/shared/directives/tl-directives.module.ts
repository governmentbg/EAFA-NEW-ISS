import { NgModule } from "@angular/core";
import { TLDebounceClickDirective } from './debounce-click.directive';
import { MatVerticalStepperScrollerDirective } from './mat-vertical-stepper-scroller.directive';
import { NotifierGroupDirective } from './notifier/notifier-group.directive';
import { NotifierDirective } from './notifier/notifier.directive';
import { TLResizableDirective } from './resizable.directive';
import { ValidityCheckerGroupDirective } from './validity-checker/validity-checker-group.directive';
import { ValidityCheckerDirective } from './validity-checker/validity-checker.directive';

@NgModule({
    declarations: [
        TLDebounceClickDirective,
        MatVerticalStepperScrollerDirective,
        TLResizableDirective,
        ValidityCheckerDirective,
        ValidityCheckerGroupDirective,
        NotifierDirective,
        NotifierGroupDirective
    ],
    exports: [
        TLDebounceClickDirective,
        MatVerticalStepperScrollerDirective,
        TLResizableDirective,
        ValidityCheckerDirective,
        ValidityCheckerGroupDirective,
        NotifierDirective,
        NotifierGroupDirective
    ],
    imports: []
})
export class TLDirectivesModule {

}
import { IValidityCheckerComponent } from './validity-checker-component.interface';

export abstract class ValidityChecker {
    public validityChecker: IValidityCheckerComponent | null = null;

    public abstract validate(scrollIntoView: boolean): void;
    public abstract valid(): boolean;
    public abstract setInvalidTitle(): void;
    public abstract setValidTitle(): void;
    public abstract expand(): boolean;
    public abstract collapse(): boolean;
    public abstract scrollIntoView(): void;
}
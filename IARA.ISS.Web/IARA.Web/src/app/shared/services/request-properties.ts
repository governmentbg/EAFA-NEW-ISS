export type MatSnackBarHorizontalPosition = 'start' | 'center' | 'end' | 'left' | 'right';
export type MatSnackBarVerticalPosition = 'top' | 'bottom';

export class RequestProperties {


    public static get DEFAULT(): RequestProperties {
        return new RequestProperties();
    }

    public static get NO_SPINNER(): RequestProperties {
        return new RequestProperties({
            showException: true,
            rethrowException: false,
            showProgressSpinner: false
        });
    }

    public successDuration?: number = 6000;
    public successColorClass?: string = 'snack-bar-success-color';
    public showException?: boolean = true;
    public rethrowException?: boolean = false;
    public showProgressSpinner?: boolean = true;
    public showExceptionColorClassErr?: string = 'snack-bar-error-color';
    public showExceptionDurationErr?: number = 6000;
    public showExceptionColorClassSucc?: string = 'snack-bar-success-color';
    public showExceptionDurationSucc?: number = 3000;
    public snackbarHorizontalPosition?: MatSnackBarHorizontalPosition = 'center';
    public snackbarVerticalPosition?: MatSnackBarVerticalPosition = 'bottom';
    public asFormData?: boolean = false;
    public asText?: boolean = false;

    public constructor(props?: Partial<RequestProperties>) {
        this.showException = true;
        this.rethrowException = false;
        this.showProgressSpinner = true;
        this.showExceptionColorClassErr = 'snack-bar-error-color';
        this.showExceptionDurationErr = 6000;
        this.showExceptionColorClassSucc = 'snack-bar-success-color';
        this.showExceptionDurationSucc = 3000;
        this.snackbarHorizontalPosition = 'center';
        this.snackbarVerticalPosition = 'bottom';
        this.asFormData = false;
        this.asText = false;

        if (props != undefined) {
            for (const key of Object.keys(props)) {
                (this as any)[key] = (props as any)[key];
            }
        }
    }
}
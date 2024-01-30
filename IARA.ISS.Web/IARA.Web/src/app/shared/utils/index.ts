import { HttpErrorResponse } from '@angular/common/http';
import { ErrorModel, ErrorType } from '@app/models/common/exception.model';
import { TLSnackbar } from '../components/snackbar/tl.snackbar';

export class TLUtils {

    public static isNullOrUndefined(value: any): boolean {
        return value === null || value === undefined;
    }

    public static handleResponseError(errorResponse: HttpErrorResponse,
        snackbar: TLSnackbar,
        handleServerError: (error: ErrorModel) => void): void {

        const error: ErrorModel | undefined = errorResponse.error as ErrorModel;
        if (error !== null && error !== undefined) {
            if (error.type === ErrorType.Unhandled) {
                snackbar.errorResource('common.an-error-occurred-during-action');
            }
            else {
                handleServerError(error);
            }
        }
        else {
            snackbar.errorResource('common.an-error-occurred-in-the-app');
        }
    }
}
import { Inject, Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { ErrorModel, ErrorType } from '@app/models/common/exception.model';
import { TRANSLATE_SERVICE_TOKEN } from '@app/shared/di/shared-di.tokens';
import { ITranslationService } from '@app/shared/interfaces/translate-service.interface';
import { RequestProperties } from '@app/shared/services/request-properties';
import { ErrorSnackbarComponent } from './error-snackbar/error-snackbar.component';
import { WarningSnackbarComponent } from './warning-snackbar/warning-snackbar.component';

@Injectable({
    providedIn: 'root'
})
export class TLSnackbar {
    private readonly snackbar: MatSnackBar;
    private readonly translateService: ITranslationService;

    public constructor(snackbar: MatSnackBar, @Inject(TRANSLATE_SERVICE_TOKEN) translateService: ITranslationService) {
        this.snackbar = snackbar;
        this.translateService = translateService;
    }

    public success(successMessage: string, duration?: number, panelClass?: string, config?: MatSnackBarConfig): void {

        if (config == undefined) {
            config = new MatSnackBarConfig();
            const properties = RequestProperties.DEFAULT;
            config.horizontalPosition = properties.snackbarHorizontalPosition;
            config.verticalPosition = properties.snackbarVerticalPosition;
            config.duration = duration ?? properties.successDuration;
            config.panelClass = [(panelClass ?? properties.successColorClass!), 'text-white'];
        }
        else {
            if (typeof config.panelClass === 'string') {
                config.panelClass = [config.panelClass, 'text-white'];
            }
            else if (config.panelClass) {
                config.panelClass.push('text-white');
            }
            else {
                config.panelClass = ['text-white'];
            }
        }

        this.snackbar.open(successMessage, undefined, config);
    }

    public successResource(resource: string, duration?: number, panelClass?: string): void {
        const message = this.translateService.getValue(resource);
        this.success(message, duration, panelClass);
    }

    public error(errorMessage: string, duration?: number, panelClass?: string): void {
        const error = new ErrorModel({
            messages: [errorMessage],
            type: ErrorType.Unhandled
        });

        const properties = RequestProperties.DEFAULT;

        properties.showExceptionDurationErr = duration ?? properties.showExceptionDurationErr;
        properties.showExceptionColorClassErr = panelClass ?? properties.showExceptionColorClassErr;

        return this.errorModel(error, properties);
    }

    public errorModel(error: ErrorModel, properties?: RequestProperties): void {

        if (properties == undefined) {
            properties = RequestProperties.DEFAULT;
        }

        const panelClass = ['text-white'];

        if (properties.showExceptionColorClassErr) {
            panelClass.push(properties.showExceptionColorClassErr);
        }

        this.snackbar.openFromComponent(ErrorSnackbarComponent, {
            data: error,
            duration: properties.showExceptionDurationErr,
            panelClass: panelClass,
        });
    }

    public errorResource(resource: string, duration?: number, panelClass?: string): void {
        const message = this.translateService.getValue(resource);
        return this.error(message, duration, panelClass);
    }

    //warning snackbar

    public warning(warningMessage: string, duration?: number, panelClass?: string): void {
        const properties = RequestProperties.DEFAULT;

        properties.showExceptionDurationErr = duration ?? properties.showExceptionDurationErr;
        properties.showExceptionColorClassErr = panelClass ?? properties.showExceptionColorClassErr;

        return this.warningModel(warningMessage, properties);
    }

    public warningModel(warningMessage: string, properties?: RequestProperties): void {

        if (properties == undefined) {
            properties = RequestProperties.DEFAULT;
        }

        const panelClass = ['text-white'];

        if (properties.showExceptionColorClassErr) {
            panelClass.push(properties.showExceptionColorClassErr);
        }

        this.snackbar.openFromComponent(WarningSnackbarComponent, {
            data: warningMessage,
            duration: properties.showExceptionDurationErr,
            panelClass: panelClass,
        });
    }

    public warningResource(resource: string, duration?: number, panelClass?: string): void {
        const message = this.translateService.getValue(resource);
        return this.warning(message, duration, panelClass);
    }
}
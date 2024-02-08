import { InjectionToken } from '@angular/core';
import { IEnvironmentConfig } from '@env/environment.interface';
import { IRequestService } from '../interfaces/request-service.interface';
import { ITranslationService } from '../interfaces/translate-service.interface';

export const TRANSLATE_SERVICE_TOKEN = new InjectionToken<ITranslationService>('ITranslationService');
export const ENVIRONMENT_CONFIG_TOKEN = new InjectionToken<IEnvironmentConfig>('IEnvironmentConfig');
export const REQUEST_SERVICE_TOKEN = new InjectionToken<IRequestService>('IRequestService');
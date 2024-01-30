import { InjectionToken } from '@angular/core';
import { IPermissionsService } from '../interfaces/permissions-service.interface';
import { SecurityConfig } from '../interfaces/security-config.interface';
import { ISecurityService } from '../interfaces/security-service.interface';
import { ITFSecurityService } from '../interfaces/tf-security-service.interface';
import { IUserService } from '../interfaces/user-service.interface';

export const SECURITY_SERVICE_TOKEN = new InjectionToken<ISecurityService>('ISecurityService');
export const PERMISSIONS_SERVICE_TOKEN = new InjectionToken<IPermissionsService>('IPermissionsService');
export const SECURITY_CONFIG_TOKEN = new InjectionToken<SecurityConfig>('ISecurityConfig');
export const USER_SERVICE_TOKEN = new InjectionToken<IUserService>('IUserService');
export const TF_SECURITY_SERVICE_TOKEN = new InjectionToken<ITFSecurityService>('ITFSecurityService');
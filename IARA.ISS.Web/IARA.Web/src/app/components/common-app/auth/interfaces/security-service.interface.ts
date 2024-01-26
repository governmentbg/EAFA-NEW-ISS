import { Observable } from "rxjs";
import { AuthCredentials } from "../models/auth/auth-credentials.model";
import { LoginResult } from '../models/auth/login-result.model';
import { TFAuthenticationModel } from '../models/auth/tf-authentication.model';
import { User } from "../models/auth/user.model";

export interface IGenericSecurityService<TIdentifier, TUser extends User<TIdentifier>> extends ISecurityService {
    getUser(): Observable<TUser>;
}

export interface ISecurityService {
    isAuthenticatedEvent: Observable<boolean>;
    token: string | undefined;
    isTempToken: boolean;
    impersonationToken: string | undefined;
    authorize(): void;
    isAuthenticated(): Promise<boolean>;
    login(credentials: AuthCredentials): Promise<LoginResult | undefined>;
    logout(): Observable<void>;
    clearToken(): Promise<boolean>;
    getUserRedirectPath(): Promise<string> | string;
    readTokenMeta(jwtToken: string, shouldEmit?: boolean): Promise<LoginResult | undefined>;
}

export interface ITwoFactorAuthenticationService {
    getTFAuthenticationData(): Observable<TFAuthenticationModel>;
    sendSMSNonce(): Observable<void>;
    sendEmailNonce(): Observable<void>;
    validateNonce(nonce: string): Observable<boolean>;
    verifyPin(pin: string): Observable<boolean>;
}

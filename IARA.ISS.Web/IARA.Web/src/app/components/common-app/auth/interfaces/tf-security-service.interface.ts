import { Observable } from 'rxjs';

export interface ITFSecurityService {
    sendEmailNonce(): Observable<void>;
    sendSMSNonce(): Observable<void>;
    validateNonce(value: string): Observable<boolean>;
    verifyPin(value: string): Observable<boolean>;

}
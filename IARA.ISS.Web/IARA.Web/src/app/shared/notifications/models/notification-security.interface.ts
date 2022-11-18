import { BehaviorSubject } from 'rxjs';

export interface INotificationSecurity {
    getToken(): string;
    isAuthenticated(): BehaviorSubject<boolean>;
    isAuthenticatedEvent: BehaviorSubject<boolean>;
}
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpEvent, HttpErrorResponse, HttpHandler, HttpRequest, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ControlContainer } from '@angular/forms';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
            return next.handle(req).pipe(
                catchError(error => {
                    if ( error instanceof HttpErrorResponse) {
                        if ( error.status === 401) {
                            return throwError(error.statusText);
                        }
                        const applicationError =  error.headers.get('Application-error');
                        if ( applicationError) {
                            console.log('ENTROU');
                            return throwError(applicationError);
                        }
                        const serverError = error.error.errors;
                        let modalStateErrors = '';
                        if ( serverError && typeof serverError === 'object') {
                            for (const key in serverError) {
                                if (serverError[key]) {
                                    modalStateErrors += serverError[key] + '\n';
                                }
                            }
                        }
                        return throwError(modalStateErrors || error.statusText || 'Server Error');
                    }
                })
            );
        }
}



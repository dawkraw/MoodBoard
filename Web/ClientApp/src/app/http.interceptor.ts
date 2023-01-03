import {
  HTTP_INTERCEPTORS,
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpStatusCode
} from "@angular/common/http";
import {Injectable} from "@angular/core";
import {Observable, throwError} from "rxjs";
import {StorageService} from "./services/storage.service";
import {catchError, map} from "rxjs/operators";

@Injectable()
export class HttpRequestsInterceptor implements HttpInterceptor {
  constructor(private storageService: StorageService) {
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    req = req.clone({
      withCredentials: true
    });
    return next.handle(req).pipe(
      map((ev: HttpEvent<any>) => ev),
      catchError((errorResponse: HttpErrorResponse) => {
        if (errorResponse.status === HttpStatusCode.Unauthorized) {
          this.storageService.clearUser();
          window.location.reload();
        }
        return throwError(errorResponse);
      })
    )
  }
}

export const httpInterceptorProviders = [
  {provide: HTTP_INTERCEPTORS, useClass: HttpRequestsInterceptor, multi: true}
];

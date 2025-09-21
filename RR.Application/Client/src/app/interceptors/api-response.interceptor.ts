import { Inject, Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpResponse, HttpErrorResponse, HttpEvent } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { ApiResponse } from '../domain/dto/ApiResponse';

@Injectable()
@Injectable()
export class ApiResponseHttp implements HttpInterceptor {

  constructor(
    @Inject('BASE_URL_API') private baseUrlApi: string,
  ) { }

  public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (req.url.startsWith(this.baseUrlApi)) {
      return next.handle(req)
        .pipe(map((event) => {
          if (event instanceof HttpResponse) {
            try {
              var response: ApiResponse<any> = event.body;

              if (response?.Success ?? false)
                event = event.clone({ body: response.Data })
            }
            catch { }
          }

          return event;
        }))
    }
    else {
      return next.handle(req);
    }
  }
}

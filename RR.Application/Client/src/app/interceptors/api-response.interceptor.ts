import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { ApiResponse } from '../domain/dto/ApiResponse';

@Injectable()
export class ApiResponseInterceptor implements HttpInterceptor {

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<any> {
    return next.handle(req).pipe(
      map((event) => {
        if (event instanceof HttpResponse) {
          const body = event.body;

          if (body && typeof body === 'object' && 'success' in body && 'data' in body) {
            const apiResponse = body as ApiResponse<any>;

            if (apiResponse.success) {
              return event.clone({ body: apiResponse.data });
            } else {
              throw new Error(apiResponse.message || 'Erro na requisição');
            }
          }

          // Se não for uma ApiResponse, retorna o body original
          return event;
        }

        return event;
      }),
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'Erro desconhecido';

        if (error.error && typeof error.error === 'object') {
          const apiError = error.error as ApiResponse<any>;

          if (apiError.message) {
            errorMessage = apiError.message;
          } else if (apiError.errors && apiError.errors.length > 0) {
            errorMessage = apiError.errors.join('; ');
          }
        } else if (error.message) {
          errorMessage = error.message;
        }

        const newError = new HttpErrorResponse({
          error: {
            message: errorMessage,
            originalError: error.error
          },
          headers: error.headers,
          status: error.status,
          statusText: error.statusText
        });

        return throwError(() => newError);
      })
    );
  }
}

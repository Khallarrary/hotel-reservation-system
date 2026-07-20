import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Auth } from '../services/auth';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(Auth);
  const router = inject(Router);
  const token = authService.obterToken();


  if (token != null) {

    const clone = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    }
  )

      return next(clone).pipe(
        catchError(erro => {
          if (erro.status === 401) {
            authService.logout()
            router.navigate(['/login']);
          }
          return throwError(() => erro);
        }
      )
    )
  }
return next(req);
};

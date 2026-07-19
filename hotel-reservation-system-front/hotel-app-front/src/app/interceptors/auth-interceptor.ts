import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Auth } from '../services/auth';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = inject(Auth).obterToken();

  if(token != null){
    const clone = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`}
  })

    return next(clone)
    
  }
  
  return next(req);  
};

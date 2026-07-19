import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { Auth } from '../services/auth';
import { Router } from '@angular/router';


export const authGuard: CanActivateFn = (route, state) => {
  const estaLogado = inject(Auth).estaLogado()
  
  if (estaLogado){
  return true 
} else {
  return inject(Router).createUrlTree(['/login'])
}
};

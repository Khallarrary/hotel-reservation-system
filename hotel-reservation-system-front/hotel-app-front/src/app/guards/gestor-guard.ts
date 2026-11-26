import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Auth } from '../services/auth';

export const gestorGuard: CanActivateFn = () =>
  inject(Auth).ehGestor() || inject(Router).createUrlTree(['/reservas']);

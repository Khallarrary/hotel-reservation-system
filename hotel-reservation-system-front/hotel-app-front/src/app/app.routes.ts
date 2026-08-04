import { Routes } from '@angular/router';
import { ReservasComponent } from './pages/reservas/reservas';
import { ReservasLista } from './pages/reservas-lista/reservas-lista';
import { ReservaCaixa } from './pages/reserva-caixa/reserva-caixa';
import { Login } from './pages/login/login'
import { authGuard } from '../app/guards/auth-guard';
import { Usuarios } from '../app/pages/usuarios/usuarios';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'reservas', component: ReservasComponent, canActivate: [authGuard] },
  { path: 'reservas-lista', component: ReservasLista, canActivate: [authGuard] },
  { path: 'reservas/:id/caixa', component: ReservaCaixa, canActivate: [authGuard] },
  { path: 'login', component: Login },
  { path: 'usuarios', component: Usuarios, canActivate: [authGuard] }
]
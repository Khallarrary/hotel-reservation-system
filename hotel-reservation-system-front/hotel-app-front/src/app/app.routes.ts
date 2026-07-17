import { Routes } from '@angular/router';
import { ReservasComponent } from './pages/reservas/reservas';
import { ReservasLista } from './pages/reservas-lista/reservas-lista';
import { ReservaCaixa } from './pages/reserva-caixa/reserva-caixa';
import { Login } from './pages/login/login'

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'reservas', component: ReservasComponent },
  { path: 'reservas-lista', component: ReservasLista },
  { path: 'reservas/:id/caixa', component: ReservaCaixa },
  { path: 'login', component: Login }
]
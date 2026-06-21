import { Routes } from '@angular/router';
import { ReservasComponent } from './pages/reservas/reservas';
import { ReservasLista } from './pages/reservas-lista/reservas-lista';
import { ReservaCaixa } from './pages/reserva-caixa/reserva-caixa';

export const routes: Routes = [
  { path: '', component: ReservasComponent },
  { path: 'reservas-lista', component: ReservasLista },
  { path: 'reservas/:id/caixa', component: ReservaCaixa }
]
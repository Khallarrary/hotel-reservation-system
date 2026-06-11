import { Routes } from '@angular/router';
import { ReservasComponent } from './pages/reservas/reservas';
import { ReservasLista } from './pages/reservas-lista/reservas-lista';

export const routes: Routes = [
  { path: '', component: ReservasComponent },
  { path: 'reservas-lista', component: ReservasLista }
]
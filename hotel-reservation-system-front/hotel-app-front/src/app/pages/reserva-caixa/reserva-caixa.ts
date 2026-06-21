import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { CaixaResumo, CaixaService } from '../../services/caixa'



@Component({
  selector: 'app-reserva-caixa',
  imports: [],
  templateUrl: './reserva-caixa.html',
  styleUrl: './reserva-caixa.css',
})
export class ReservaCaixa {

  reservaId: number = 0;
  caixa: CaixaResumo | null = null

  constructor(private route: ActivatedRoute, private caixaService: CaixaService){}

  ngOnInit(): void {
    this.reservaId = Number(this.route.snapshot.paramMap.get('id'));

    this.caixaService.obterResumo(this.reservaId).subscribe({
      next: (resumo) => {
        this.caixa = resumo;
      },
      error: (err) => {
        console.log(err);
      }
    })
  }
    

}

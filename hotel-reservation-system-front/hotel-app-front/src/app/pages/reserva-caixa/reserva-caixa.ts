import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core';
import { Observable } from 'rxjs';
import { CaixaResumo, CaixaService } from '../../services/caixa'




@Component({
  selector: 'app-reserva-caixa',
  imports: [ CommonModule],
  templateUrl: './reserva-caixa.html',
  styleUrl: './reserva-caixa.css',
})
export class ReservaCaixa {

  reservaId: number = 0;
  caixa: CaixaResumo | null = null

  constructor(private route: ActivatedRoute, private caixaService: CaixaService, private cdr: ChangeDetectorRef){}

  ngOnInit(): void {
    this.reservaId = Number(this.route.snapshot.paramMap.get('id'));

    this.caixaService.obterResumo(this.reservaId).subscribe({
      next: (resumo) => {
        console.log('RESUMO CAIXA:', resumo);
        this.caixa = resumo;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.log(err);
      }
    })
  }
    

}

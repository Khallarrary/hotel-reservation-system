import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CaixaResumo, CaixaService, LancarCredito, LancarDebito } from '../../services/caixa'
import { Location } from '@angular/common';


@Component({
  selector: 'app-reserva-caixa',
  imports: [ CommonModule, FormsModule],
  templateUrl: './reserva-caixa.html',
  styleUrl: './reserva-caixa.css',
})
export class ReservaCaixa {

  reservaId: number = 0;
  caixa: CaixaResumo | null = null

  novoMovimento = {tipo: 'Credito', descricao: '', valor: 0, formaPagamento: 0}

  constructor(private route: ActivatedRoute, private caixaService: CaixaService, private cdr: ChangeDetectorRef, private location: Location){}

  ngOnInit(): void {
    this.reservaId = Number(this.route.snapshot.paramMap.get('id'));

    this.carregarResumo();
  }

  carregarResumo(): void{
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

  limparFormulario(): void{
    this.novoMovimento = {tipo: 'Credito', descricao: '', valor: 0, formaPagamento: 0}
  }

  voltar(): void {
  this.location.back();
  }

  salvarMovimento(): void {
    console.log(this.novoMovimento)

    const credito: LancarCredito = {
      valor: this.novoMovimento.valor,
      descricao: this.novoMovimento.descricao,
      formaPagamento: this.novoMovimento.formaPagamento
    };

    const debito: LancarDebito = {
      valor: this.novoMovimento.valor,
      descricao: this.novoMovimento.descricao,
    };

    if(this.novoMovimento.tipo === 'Credito'){
      this.caixaService.lancarCredito(this.reservaId, credito).subscribe({
        next: () => {
          this.carregarResumo();
          this.limparFormulario();
          console.log('credito lançado')
          },
          error: (err) => {
            console.log(err)
            
          }
      })
    } else if(this.novoMovimento.tipo === 'Debito'){
      this.caixaService.lancarDebito(this.reservaId, debito).subscribe({
        next: () => {
          this.carregarResumo();
          this.limparFormulario();
          console.log('debito lançado')
          },
          error: (err) => {
            console.log(err)
            
          }
      })
    }
  }
    

}

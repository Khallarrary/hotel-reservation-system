import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Auth } from '../../services/auth';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  email: string = ''
  senha: string = ''
  mensagemErro: string = ''
  processando: boolean = false

  

  constructor(private authService: Auth, private router: Router, private cdr: ChangeDetectorRef) {}

  entrar(): void {
    if (this.processando) {
      return;
    }

    this.mensagemErro = ''

    
    if(this.email.trim() == ''){
      this.mensagemErro = 'E-mail é obrigatorio';
      return;
    }

    if(this.senha == ''){
      this.mensagemErro = 'Senha é obrigatorio';
      return;
    }

    const dados = {
      email: this.email,
      senha: this.senha
    }

    this.processando = true;

    this.authService.login(dados).subscribe({
      next:(resposta) => {
        this.processando = false;

        if(resposta != null){
          this.authService.salvarSessao(resposta);
          this.router.navigate(['/reservas']);
        }
      },
      error: (err) =>{
        this.processando = false;
        this.mensagemErro = err.status === 0
          ? 'Não foi possível conectar ao sistema. Tente novamente.'
          : err.error?.message || 'Não foi possível entrar. Tente novamente.';
        this.cdr.detectChanges();
      }
    })
  }
}

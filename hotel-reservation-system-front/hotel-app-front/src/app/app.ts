import { Component, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { Auth } from './services/auth';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.css'
})

export class AppComponent {
  menuAberto = false;

  constructor(
    private router: Router,
    private authService: Auth
  ) {}

  estaNaTelaDeLogin(): boolean {
    return this.router.url.startsWith('/login');
  }

  obterNomeUsuario(): string {
    return this.authService.obterNome() || 'Usuario';
  }

  obterPerfilUsuario(): string {
    return this.authService.obterPerfil() || '';
  }

  ehGestor(): boolean {
    return this.authService.ehGestor();
  }

  alternarMenu(): void {
    this.menuAberto = !this.menuAberto;
  }

  abrirCadastroQuarto(): void {
    this.menuAberto = false;
    this.router.navigate(['/reservas'], {
      queryParams: { acao: 'novo-quarto' }
    });
  }

  abrirGerenciamentoUsuarios(): void {
    this.menuAberto = false;
    this.router.navigate(['/usuarios'], {
    });
  }

  sair(): void {
    this.authService.logout();
    this.menuAberto = false;
    this.router.navigate(['/login']);
  }

  @HostListener('document:click')
  fecharMenu(): void {
    this.menuAberto = false;
  }
}

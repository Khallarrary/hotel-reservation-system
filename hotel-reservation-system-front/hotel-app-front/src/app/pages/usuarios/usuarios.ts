import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsuarioService, Usuario } from '../../services/usuario';
import { ChangeDetectorRef } from '@angular/core';


@Component({
  selector: 'app-usuarios',
  imports: [CommonModule],
  templateUrl: './usuarios.html',
  styleUrl: './usuarios.css',
})

export class Usuarios implements OnInit
{ 
  usuarios: Usuario[] = []
  carregando: boolean = false
  mensagemSucesso: string = "";
  mensagemErro: string = ""

  constructor(private usuarioService: UsuarioService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.carregarUsuarios();
  }

  mostrarErro(texto: string) {
  this.mensagemErro = texto;
  this.mensagemSucesso = '';

  setTimeout(() => {
    this.mensagemErro = '';
  }, 3000);
}

  carregarUsuarios(): void{

    this.carregando = true;
    this.mensagemErro = '';

    this.usuarioService.listar().subscribe({
        next: (resposta) => {
          console.log(resposta)
          this.usuarios = resposta;
          this.carregando = false;
          this.cdr.detectChanges();          
        },
        error: (err) => {
          this.mostrarErro("Erro ao carregar usuarios")
          this.carregando = false;
        }
      })
  }
}

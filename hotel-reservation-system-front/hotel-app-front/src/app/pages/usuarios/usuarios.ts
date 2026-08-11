import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsuarioService, Usuario, CriarUsuario } from '../../services/usuario';
import { ChangeDetectorRef } from '@angular/core';
import { FormsModule} from '@angular/forms';


@Component({
  selector: 'app-usuarios',
  imports: [CommonModule, FormsModule],
  templateUrl: './usuarios.html',
  styleUrl: './usuarios.css',
})

export class Usuarios implements OnInit
{ 
  usuarios: Usuario[] = []
  carregando: boolean = false
  mensagemSucesso: string = "";
  mensagemErro: string = ""
  usuarioEmEdicao: Usuario | null = null;

  formularioEmEdicao = {
    nome: '',
    email: '',
    perfil: ''
  }

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
          this.mostrarErro(err.error?.message || "Erro ao carregar usuarios")
          this.cdr.detectChanges(); 
          this.carregando = false;
        }
      })
  }

  exibirFormulario: boolean = false;

  alterarFormulario(): void{
    this.exibirFormulario = !this.exibirFormulario;
  }

  mostrarSucesso(texto: string) {
  this.mensagemSucesso = texto;
  this.mensagemErro = '';

  setTimeout(() => {
    this.mensagemSucesso = '';
  }, 3000);
}

  novoUsuario: CriarUsuario = {
    nome: '',
    email: '',
    senha: '',
    perfil: 'Operador'
  }

  criarUsuario(): void{

    if(!this.novoUsuario.nome.trim()){
      this.mostrarErro('Nome é obrigatório.');
      return;
    }

    if(!this.novoUsuario.email.trim()){
      this.mostrarErro('E-mail é obrigatório.');
      return;
    }

    if(!this.novoUsuario.senha.trim()){
      this.mostrarErro('Senha é obrigatório.');
      return;
    }

   this.usuarioService.criar(this.novoUsuario).subscribe({
    next: () =>{
      this.carregarUsuarios();
      this.exibirFormulario = false;
      this.mostrarSucesso("Usuaio criado com suceso!")
      this.novoUsuario = {nome: '', email: '', senha: '', perfil: 'Operador'}
    }, 
    error: (err) => {
      this.mostrarErro(err.error?.message || 'Erro ao criar usuario.');
    }
   })
    
  }

  alterarAtivacao(usuario: Usuario): void{
    const novoEstado = !usuario.ativo;

    this.usuarioService.alterarAtivacao(usuario.id, novoEstado).subscribe({
      next: () =>{
        this.carregarUsuarios();
        this.mostrarSucesso("Ativação alterada com suceso!");
      },
      error: (err) => {
        this.mostrarErro(err.error?.message || 'Erro ao atualizar usuario.');
        this.cdr.detectChanges(); 
      }
    })
  }

  iniciarEdicao(usuario: Usuario): void{
    this.usuarioEmEdicao = usuario;

    this.formularioEmEdicao = {
      nome: usuario.nome,
      email: usuario.email,
      perfil: usuario.perfil
    }
  }

  salvarEdicao(): void{
    if(this.usuarioEmEdicao == null){
      return
    }

    this.usuarioService.alterarUsuario(this.usuarioEmEdicao.id, this.formularioEmEdicao).subscribe({
      next: () => {
        this.carregarUsuarios();
        this.cancelarEdicao();
        this.mostrarSucesso("Usuario alterado com sucesso!");
        this.cdr.detectChanges(); 
      },
      error: (err) => {
        this.mostrarErro(err.error?.message || 'Erro ao atualizar usuario.');
        this.cdr.detectChanges(); 
      }
    })

  }

  cancelarEdicao(): void{
    this.usuarioEmEdicao = null;

    this.formularioEmEdicao = {
      nome: '',
      email: '',
      perfil: ''
    }
  }


}

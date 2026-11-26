import {
  Component, DestroyRef, ElementRef, Injector, OnInit,
  afterNextRender, computed, inject, signal, viewChild
} from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Observable, finalize, forkJoin, map } from 'rxjs';
import { CategoriaQuarto, CategoriaQuartoService, DadosCategoriaQuarto } from '../../services/categoria-quarto';
import { QuartoGestao, QuartoService } from '../../services/quarto';

@Component({
  selector: 'app-gestao-acomodacoes',
  imports: [FormsModule, RouterLink],
  templateUrl: './gestao-acomodacoes.html',
  styleUrl: './gestao-acomodacoes.css',
})
export class GestaoAcomodacoes implements OnInit {
  private readonly categoriasService = inject(CategoriaQuartoService);
  private readonly quartosService = inject(QuartoService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly injector = inject(Injector);
  private readonly primeiroCampo = viewChild<ElementRef<HTMLInputElement>>('primeiroCampo');

  readonly aba = toSignal(
    inject(ActivatedRoute).queryParamMap.pipe(
      map(params => params.get('aba') === 'categorias' ? 'categorias' : 'quartos')
    ),
    { initialValue: 'quartos' }
  );

  readonly quartos = signal<QuartoGestao[]>([]);
  readonly categorias = signal<CategoriaQuarto[]>([]);
  readonly categoriasPorId = computed(() => new Map(this.categorias().map(c => [c.id, c])));
  readonly temCategoriaAtiva = computed(() => this.categorias().some(c => c.ativa));
  readonly quartosPorCategoria = computed(() => {
    const totais = new Map<number, number>();
    for (const { categoriaQuartoId } of this.quartos()) {
      totais.set(categoriaQuartoId, (totais.get(categoriaQuartoId) ?? 0) + 1);
    }
    return totais;
  });

  readonly carregando = signal(false);
  readonly salvando = signal(false);
  readonly erroLista = signal(false);
  readonly erroFormulario = signal('');
  readonly sucesso = signal('');

  formularioQuarto: {
    id: number | null;
    categoriaOriginalId: number | null;
    numero: string;
    categoriaQuartoId: number | null;
  } | null = null;
  formularioCategoria: { id: number | null; dados: DadosCategoriaQuarto } | null = null;

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.carregando.set(true);
    this.erroLista.set(false);
    forkJoin({
      quartos: this.quartosService.listarParaGestao(),
      categorias: this.categoriasService.listar()
    }).pipe(
      takeUntilDestroyed(this.destroyRef),
      finalize(() => this.carregando.set(false))
    ).subscribe({
      next: ({ quartos, categorias }) => {
        this.quartos.set(quartos);
        this.categorias.set(categorias);
      },
      error: () => this.erroLista.set(true)
    });
  }

  abrirQuarto(quarto?: QuartoGestao): void {
    this.erroFormulario.set('');
    this.sucesso.set('');
    this.formularioQuarto = {
      id: quarto?.id ?? null,
      categoriaOriginalId: quarto?.categoriaQuartoId ?? null,
      numero: quarto?.numero ?? '',
      categoriaQuartoId: quarto?.categoriaQuartoId ?? null
    };
    this.focarFormulario();
  }

  abrirCategoria(categoria?: CategoriaQuarto): void {
    this.erroFormulario.set('');
    this.sucesso.set('');
    this.formularioCategoria = {
      id: categoria?.id ?? null,
      dados: {
        nome: categoria?.nome ?? '',
        descricao: categoria?.descricao ?? '',
        capacidadeFisica: categoria?.capacidadeFisica ?? 1,
        limiteComercial: categoria?.limiteComercial ?? 1
      }
    };
    this.focarFormulario();
  }

  fecharFormulario(): void {
    if (this.aba() === 'quartos') {
      this.formularioQuarto = null;
    } else {
      this.formularioCategoria = null;
    }
    this.erroFormulario.set('');
  }

  salvarQuarto(): void {
    const formulario = this.formularioQuarto;
    if (!formulario || this.salvando()) return;

    const numero = formulario.numero.trim();
    const categoriaQuartoId = formulario.categoriaQuartoId;
    if (!numero || categoriaQuartoId === null) {
      this.erroFormulario.set('Informe o número e a categoria do quarto.');
      return;
    }

    const dados = { numero, categoriaQuartoId };
    const requisicao = formulario.id === null
      ? this.quartosService.criar(dados)
      : this.quartosService.atualizar(formulario.id, dados);

    this.salvar(requisicao, 'Quarto salvo.', () => this.formularioQuarto = null);
  }

  salvarCategoria(): void {
    const formulario = this.formularioCategoria;
    if (!formulario || this.salvando()) return;

    const { capacidadeFisica, limiteComercial } = formulario.dados;
    const nome = formulario.dados.nome.trim();
    if (!nome || !Number.isInteger(capacidadeFisica) || !Number.isInteger(limiteComercial) ||
        limiteComercial < 1 || limiteComercial > capacidadeFisica) {
      this.erroFormulario.set('Informe um nome e um limite comercial entre 1 e a capacidade física.');
      return;
    }

    const dados = {
      nome,
      descricao: formulario.dados.descricao?.trim() || null,
      capacidadeFisica,
      limiteComercial
    };
    const requisicao = formulario.id === null
      ? this.categoriasService.criar(dados)
      : this.categoriasService.atualizar(formulario.id, dados);

    this.salvar(requisicao, 'Categoria salva.', () => this.formularioCategoria = null);
  }

  private focarFormulario(): void {
    afterNextRender(() => this.primeiroCampo()?.nativeElement.focus(), { injector: this.injector });
  }

  private salvar(requisicao: Observable<void>, mensagem: string, fechar: () => void): void {
    this.salvando.set(true);
    this.erroFormulario.set('');
    this.sucesso.set('');
    requisicao.pipe(
      takeUntilDestroyed(this.destroyRef),
      finalize(() => this.salvando.set(false))
    ).subscribe({
      next: () => {
        fechar();
        this.sucesso.set(mensagem);
        this.carregar();
      },
      error: erro => this.erroFormulario.set(erro.error?.message || 'Não foi possível salvar.')
    });
  }
}

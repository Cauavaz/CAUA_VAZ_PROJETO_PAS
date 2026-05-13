import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { TituloService } from '../../../titulos/services/titulo.service';
import { TituloListagem } from '../../../titulos/models/titulo.model';
import { CurrencyBrlPipe } from '../../../../shared/pipes/currency-brl.pipe';

interface FaixaAtraso {
  label: string;
  quantidade: number;
  cor: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, CurrencyBrlPipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private readonly tituloService = inject(TituloService);
  private readonly toastr = inject(ToastrService);

  readonly loading = signal(false);
  readonly titulos = signal<TituloListagem[]>([]);

  readonly totalTitulos = computed(() => this.titulos().length);

  readonly totalDevedores = computed(() => {
    const cpfs = new Set(this.titulos().map(t => t.cpfDevedor));
    return cpfs.size;
  });

  readonly valorOriginalTotal = computed(() =>
    this.titulos().reduce((acc, t) => acc + t.valorOriginal, 0)
  );

  readonly valorAtualizadoTotal = computed(() =>
    this.titulos().reduce((acc, t) => acc + t.valorAtualizado, 0)
  );

  readonly acrescimoTotal = computed(() =>
    this.valorAtualizadoTotal() - this.valorOriginalTotal()
  );

  readonly percentualAcrescimo = computed(() => {
    const original = this.valorOriginalTotal();
    if (original <= 0) return 0;
    return (this.acrescimoTotal() / original) * 100;
  });

  readonly mediaDiasAtraso = computed(() => {
    const lista = this.titulos();
    if (lista.length === 0) return 0;
    const total = lista.reduce((acc, t) => acc + t.diasAtraso, 0);
    return Math.round(total / lista.length);
  });

  readonly titulosEmAtraso = computed(() =>
    this.titulos().filter(t => t.diasAtraso > 0).length
  );

  readonly totalParcelas = computed(() =>
    this.titulos().reduce((acc, t) => acc + t.quantidadeParcelas, 0)
  );

  readonly topDevedores = computed(() => {
    const agrupado = new Map<string, { nome: string; cpf: string; valor: number; titulos: number }>();
    for (const t of this.titulos()) {
      const atual = agrupado.get(t.cpfDevedor);
      if (atual) {
        atual.valor += t.valorAtualizado;
        atual.titulos += 1;
      } else {
        agrupado.set(t.cpfDevedor, {
          nome: t.nomeDevedor,
          cpf: t.cpfDevedor,
          valor: t.valorAtualizado,
          titulos: 1
        });
      }
    }
    return Array.from(agrupado.values())
      .sort((a, b) => b.valor - a.valor)
      .slice(0, 5);
  });

  readonly faixasAtraso = computed<FaixaAtraso[]>(() => {
    const faixas: FaixaAtraso[] = [
      { label: 'Em dia', quantidade: 0, cor: '#10b981' },
      { label: '1 a 30 dias', quantidade: 0, cor: '#facc15' },
      { label: '31 a 90 dias', quantidade: 0, cor: '#f59e0b' },
      { label: '91 a 180 dias', quantidade: 0, cor: '#fb7185' },
      { label: 'Acima de 180', quantidade: 0, cor: '#dc2626' }
    ];

    for (const t of this.titulos()) {
      const d = t.diasAtraso;
      if (d <= 0) faixas[0].quantidade++;
      else if (d <= 30) faixas[1].quantidade++;
      else if (d <= 90) faixas[2].quantidade++;
      else if (d <= 180) faixas[3].quantidade++;
      else faixas[4].quantidade++;
    }
    return faixas;
  });

  readonly maiorFaixa = computed(() =>
    Math.max(1, ...this.faixasAtraso().map(f => f.quantidade))
  );

  readonly ultimosTitulos = computed(() => this.titulos().slice(0, 5));

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.loading.set(true);
    this.tituloService.listar().subscribe({
      next: titulos => {
        this.titulos.set(titulos);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.toastr.error(
          'Verifique se a API está em execução.',
          'Não foi possível carregar a dashboard'
        );
      }
    });
  }

  larguraBarra(quantidade: number): number {
    return (quantidade / this.maiorFaixa()) * 100;
  }
}

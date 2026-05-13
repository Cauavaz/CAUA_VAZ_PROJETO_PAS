import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { TituloService } from '../../services/titulo.service';
import { TituloListagem } from '../../models/titulo.model';
import { CurrencyBrlPipe } from '../../../../shared/pipes/currency-brl.pipe';
import { CpfPipe } from '../../../../shared/pipes/cpf.pipe';

@Component({
  selector: 'app-titulo-list',
  standalone: true,
  imports: [CommonModule, RouterLink, CurrencyBrlPipe, CpfPipe],
  templateUrl: './titulo-list.component.html',
  styleUrl: './titulo-list.component.scss'
})
export class TituloListComponent implements OnInit {
  private readonly tituloService = inject(TituloService);
  private readonly toastr = inject(ToastrService);

  readonly titulos = signal<TituloListagem[]>([]);
  readonly loading = signal(false);
  readonly carregamentoFalhou = signal(false);

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.loading.set(true);
    this.carregamentoFalhou.set(false);
    this.tituloService.listar().subscribe({
      next: titulos => {
        this.titulos.set(titulos);
        this.loading.set(false);
      },
      error: () => {
        this.carregamentoFalhou.set(true);
        this.loading.set(false);
        this.toastr.error(
          'Verifique se a API está em execução.',
          'Não foi possível carregar os títulos'
        );
      }
    });
  }
}

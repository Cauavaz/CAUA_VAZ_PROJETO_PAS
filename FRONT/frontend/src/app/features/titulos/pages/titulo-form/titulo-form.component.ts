import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { TituloService } from '../../services/titulo.service';
import { CriarTituloRequest, TituloListagem } from '../../models/titulo.model';
import { ParcelaRequest } from '../../models/parcela.model';
import { cpfValidator } from '../../../../shared/validators/cpf.validator';
import { CurrencyBrlPipe } from '../../../../shared/pipes/currency-brl.pipe';

interface ParcelaFormControls {
  numero: FormControl<number | null>;
  dataVencimento: FormControl<string | null>;
  valor: FormControl<number | null>;
}

interface TituloFormControls {
  numeroTitulo: FormControl<string | null>;
  nomeDevedor: FormControl<string | null>;
  cpfDevedor: FormControl<string | null>;
  percentualJuros: FormControl<number | null>;
  percentualMulta: FormControl<number | null>;
  parcelas: FormArray<FormGroup<ParcelaFormControls>>;
}

@Component({
  selector: 'app-titulo-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CurrencyBrlPipe],
  templateUrl: './titulo-form.component.html',
  styleUrl: './titulo-form.component.scss'
})
export class TituloFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly tituloService = inject(TituloService);
  private readonly router = inject(Router);
  private readonly toastr = inject(ToastrService);

  form!: FormGroup<TituloFormControls>;

  readonly saving = signal(false);

  readonly parcelasTotal = signal(0);
  readonly parcelasCount = signal(1);

  readonly resumoValorFormatado = computed(() => this.parcelasTotal());

  ngOnInit(): void {
    this.form = this.fb.group<TituloFormControls>({
      numeroTitulo: this.fb.control('', { validators: [Validators.required, Validators.maxLength(50)] }),
      nomeDevedor: this.fb.control('', { validators: [Validators.required, Validators.maxLength(200)] }),
      cpfDevedor: this.fb.control('', { validators: [Validators.required, cpfValidator()] }),
      percentualJuros: this.fb.control<number | null>(0, {
        validators: [Validators.required, Validators.min(0)]
      }),
      percentualMulta: this.fb.control<number | null>(0, {
        validators: [Validators.required, Validators.min(0)]
      }),
      parcelas: this.fb.array<FormGroup<ParcelaFormControls>>(
        [this.criarParcelaGroup(1)],
        Validators.required
      )
    });

    this.atualizarResumo();
    this.form.controls.parcelas.valueChanges.subscribe(() => this.atualizarResumo());
  }

  get parcelas(): FormArray<FormGroup<ParcelaFormControls>> {
    return this.form.controls.parcelas;
  }

  adicionarParcela(): void {
    const proximo = this.parcelas.length + 1;
    this.parcelas.push(this.criarParcelaGroup(proximo));
  }

  removerParcela(index: number): void {
    if (this.parcelas.length <= 1) {
      return;
    }
    this.parcelas.removeAt(index);
  }

  onCpfInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const digits = input.value.replace(/\D/g, '').slice(0, 11);
    let formatted = digits;
    if (digits.length > 9) {
      formatted = `${digits.slice(0, 3)}.${digits.slice(3, 6)}.${digits.slice(6, 9)}-${digits.slice(9)}`;
    } else if (digits.length > 6) {
      formatted = `${digits.slice(0, 3)}.${digits.slice(3, 6)}.${digits.slice(6)}`;
    } else if (digits.length > 3) {
      formatted = `${digits.slice(0, 3)}.${digits.slice(3)}`;
    }
    input.value = formatted;
    this.form.controls.cpfDevedor.setValue(formatted);
    this.form.controls.cpfDevedor.markAsDirty();
  }

  hasError(controlPath: string, error: string): boolean {
    const control = this.form.get(controlPath);
    return !!control && control.touched && control.hasError(error);
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      const erros = this.coletarErros();
      const mensagem = erros.length
        ? erros.join('<br>')
        : 'Verifique os campos destacados antes de continuar.';
      this.toastr.warning(mensagem, 'Formulário incompleto', {
        enableHtml: true,
        timeOut: 6000
      });
      return;
    }

    const raw = this.form.getRawValue();
    const request: CriarTituloRequest = {
      numeroTitulo: raw.numeroTitulo!.trim(),
      nomeDevedor: raw.nomeDevedor!.trim(),
      cpfDevedor: raw.cpfDevedor!,
      percentualJuros: this.parseNumero(raw.percentualJuros),
      percentualMulta: this.parseNumero(raw.percentualMulta),
      parcelas: raw.parcelas.map<ParcelaRequest>(p => ({
        numero: this.parseNumero(p.numero),
        dataVencimento: p.dataVencimento!,
        valor: this.parseNumero(p.valor)
      }))
    };

    this.saving.set(true);
    this.tituloService.criar(request).subscribe({
      next: (titulo: TituloListagem) => {
        this.saving.set(false);
        this.toastr.success(
          `Título ${titulo.numeroTitulo} cadastrado com sucesso.`,
          'Tudo certo!'
        );
        this.router.navigate(['/titulos']);
      },
      error: (err: HttpErrorResponse) => {
        this.saving.set(false);
        this.toastr.error(this.extrairMensagemErro(err), 'Não foi possível salvar');
      }
    });
  }

  cancelar(): void {
    this.router.navigate(['/titulos']);
  }

  private criarParcelaGroup(numero: number): FormGroup<ParcelaFormControls> {
    return this.fb.group<ParcelaFormControls>({
      numero: this.fb.control<number | null>(numero, {
        validators: [Validators.required, Validators.min(1)]
      }),
      dataVencimento: this.fb.control<string | null>('', { validators: [Validators.required] }),
      valor: this.fb.control<number | null>(null, {
        validators: [Validators.required, Validators.min(0.01)]
      })
    });
  }

  private atualizarResumo(): void {
    const total = this.parcelas.controls.reduce((acc, group) => {
      const valor = this.parseNumero(group.controls.valor.value);
      return acc + (isFinite(valor) ? valor : 0);
    }, 0);
    this.parcelasTotal.set(total);
    this.parcelasCount.set(this.parcelas.length);
  }

  private coletarErros(): string[] {
    const mensagens: string[] = [];
    const c = this.form.controls;

    if (c.numeroTitulo.invalid) {
      mensagens.push('Informe o número do título.');
    }
    if (c.nomeDevedor.invalid) {
      mensagens.push('Informe o nome do devedor.');
    }
    if (c.cpfDevedor.invalid) {
      if (c.cpfDevedor.hasError('required')) {
        mensagens.push('Informe o CPF do devedor.');
      } else if (c.cpfDevedor.hasError('cpfInvalid')) {
        mensagens.push('CPF do devedor é inválido.');
      }
    }
    if (c.percentualJuros.invalid) {
      mensagens.push('Informe um percentual de juros válido.');
    }
    if (c.percentualMulta.invalid) {
      mensagens.push('Informe um percentual de multa válido.');
    }

    this.parcelas.controls.forEach((parcela, index) => {
      const numero = index + 1;
      const numeroCtrl = parcela.controls.numero;
      const dataCtrl = parcela.controls.dataVencimento;
      const valorCtrl = parcela.controls.valor;

      if (numeroCtrl.invalid) {
        mensagens.push(`Parcela ${numero}: informe um número válido.`);
      }
      if (dataCtrl.invalid) {
        mensagens.push(`Parcela ${numero}: informe a data de vencimento.`);
      }
      if (valorCtrl.invalid) {
        if (valorCtrl.hasError('required')) {
          mensagens.push(`Parcela ${numero}: informe o valor.`);
        } else if (valorCtrl.hasError('min')) {
          mensagens.push(`Parcela ${numero}: o valor deve ser maior que zero.`);
        }
      }
    });

    return mensagens;
  }

  private parseNumero(valor: number | string | null | undefined): number {
    if (valor === null || valor === undefined || valor === '') {
      return 0;
    }
    if (typeof valor === 'number') {
      return valor;
    }
    return Number(String(valor).replace(',', '.'));
  }

  private extrairMensagemErro(err: HttpErrorResponse): string {
    if (err.error?.message) {
      return err.error.message;
    }
    if (err.error?.errors) {
      const first = Object.values(err.error.errors).flat()[0];
      if (typeof first === 'string') {
        return first;
      }
    }
    return 'Não foi possível salvar o título. Verifique os dados e tente novamente.';
  }
}

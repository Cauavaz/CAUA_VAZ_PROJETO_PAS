import { ParcelaRequest } from './parcela.model';

export interface CriarTituloRequest {
  numeroTitulo: string;
  nomeDevedor: string;
  cpfDevedor: string;
  percentualJuros: number;
  percentualMulta: number;
  parcelas: ParcelaRequest[];
}

export interface TituloListagem {
  id: string;
  numeroTitulo: string;
  nomeDevedor: string;
  cpfDevedor: string;
  quantidadeParcelas: number;
  valorOriginal: number;
  diasAtraso: number;
  valorAtualizado: number;
  dataAtualizacao: string;
}

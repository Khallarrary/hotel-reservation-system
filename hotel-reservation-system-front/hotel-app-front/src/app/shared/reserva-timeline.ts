import { Reserva } from '../services/reserva';

const MILISSEGUNDOS_POR_DIA = 1000 * 60 * 60 * 24;

export function gerarDiasTimeline(inicio: Date, totalDias: number): Date[] {
  const dias: Date[] = [];

  for (let i = 0; i < totalDias; i++) {
    const dia = new Date(inicio);
    dia.setDate(inicio.getDate() + i);
    dias.push(dia);
  }

  return dias;
}

export function reservaEstaNaTimeline(reserva: Reserva, dias: Date[]): boolean {
  const inicioTimeline = dias[0];
  const fimTimeline = new Date(inicioTimeline);
  fimTimeline.setDate(inicioTimeline.getDate() + dias.length);
  const checkIn = new Date(reserva.checkIn);
  const checkOut = new Date(reserva.checkOut);

  return inicioTimeline < checkOut && fimTimeline > checkIn;
}

export function calcularDuracaoNaTimeline(reserva: Reserva, dias: Date[]): number {
  const checkIn = new Date(reserva.checkIn);
  const checkOut = new Date(reserva.checkOut);
  const inicioTimeline = dias[0];
  const fimTimeline = new Date(inicioTimeline);
  fimTimeline.setDate(inicioTimeline.getDate() + dias.length);

  const inicioVisual = checkIn > inicioTimeline ? checkIn : inicioTimeline;
  const fimVisual = checkOut < fimTimeline ? checkOut : fimTimeline;
  let duracao = (fimVisual.getTime() - inicioVisual.getTime()) / MILISSEGUNDOS_POR_DIA;

  if (checkIn < inicioTimeline) {
    duracao += 0.5;
  }

  return duracao;
}

export function calcularOffsetNaTimeline(reserva: Reserva, dias: Date[]): number {
  const inicioTimeline = dias[0];
  const checkIn = new Date(reserva.checkIn);

  if (checkIn < inicioTimeline) {
    return -0.5;
  }

  return (checkIn.getTime() - inicioTimeline.getTime()) / MILISSEGUNDOS_POR_DIA;
}

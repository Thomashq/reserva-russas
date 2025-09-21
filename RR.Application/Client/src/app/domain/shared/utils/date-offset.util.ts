export type DateTimeOffsetString = string;

const pad = (n: number, w = 2) => String(n).padStart(w, '0');

function format(date: Date): DateTimeOffsetString {
  const d = new Date(date);
  const yyyy = d.getFullYear();
  const MM = pad(d.getMonth() + 1);
  const dd = pad(d.getDate());
  const HH = pad(d.getHours());
  const mm = pad(d.getMinutes());
  const ss = pad(d.getSeconds());

  const offsetMin = -d.getTimezoneOffset();
  const sign = offsetMin >= 0 ? '+' : '-';
  const abs = Math.abs(offsetMin);
  const oh = pad(Math.floor(abs / 60));
  const om = pad(abs % 60);

  return `${yyyy}-${MM}-${dd}T${HH}:${mm}:${ss}${sign}${oh}:${om}`;
}

function fromDateAndTime(date: Date, hhmm: string): DateTimeOffsetString {
  const [H, M] = (hhmm ?? '').split(':').map(Number);
  const d = new Date(date);
  d.setHours(H || 0, M || 0, 0, 0);
  return format(d);
}

function startOfDay(d: Date): Date {
  const r = new Date(d);
  r.setHours(0, 0, 0, 0);
  return r;
}

function endOfDay(d: Date): Date {
  const r = new Date(d);
  r.setHours(23, 59, 59, 999);
  return r;
}

function startOfToday(): Date {
  return startOfDay(new Date());
}

function addDays(d: Date, n: number): Date {
  const r = new Date(d);
  r.setDate(r.getDate() + n);
  return r;
}

// Janela semanal: hoje (00:00) até +7 dias (23:59:59)
function weekWindow(from: Date = startOfToday()): { start: DateTimeOffsetString; end: DateTimeOffsetString } {
  const start = startOfDay(from);
  const end = endOfDay(addDays(start, 7));
  return { start: format(start), end: format(end) };
}

export const DateOffset = {
  format,
  fromDateAndTime,
  startOfDay,
  endOfDay,
  startOfToday,
  addDays,
  weekWindow,
};

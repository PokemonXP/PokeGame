export function randomInteger(min: number, max: number): number {
  const minCeil: number = Math.ceil(min);
  const maxFloor: number = Math.floor(max);
  return Math.floor(Math.random() * (maxFloor - minCeil + 1)) + minCeil;
}

export function roll(input: string): number {
  const match: RegExpMatchArray | null = input
    .trim()
    .toLowerCase()
    .match(/^(\d+)d(\d+)$/);
  if (!match) {
    return 0;
  }

  const dice: number = parseInt(match[1], 10);
  const die: number = parseInt(match[2], 10);

  if (dice < 1 || die < 1) {
    return 0;
  }

  let total: number = 0;
  for (let i = 0; i < dice; i++) {
    total += 1 + Math.floor(Math.random() * die);
  }
  return total;
}

// TODO(fpion): unit tests

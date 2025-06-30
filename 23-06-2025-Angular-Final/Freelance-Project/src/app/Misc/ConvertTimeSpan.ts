export class ConvertTimeSpan {
  public static toCSharpTimeSpan(input: string | number): string {
    let totalMinutes = 0;
    if (typeof input === 'number') {
      totalMinutes = input;
    } else if (typeof input === 'string') {
      const trimmed = input.trim();
      // Parse '3d 4h 5m'
      const regex = /(?:(\d+)d)?\s*(?:(\d+)h)?\s*(?:(\d+)m)?/i;
      const match = trimmed.match(regex);
      if (match && (match[1] || match[2] || match[3])) {
        const days = parseInt(match[1] || '0', 10);
        const hours = parseInt(match[2] || '0', 10);
        const minutes = parseInt(match[3] || '0', 10);
        totalMinutes = days * 24 * 60 + hours * 60 + minutes;
      } else if (/^P/.test(trimmed)) {
        // ISO 8601 duration e.g. P2DT3H4M
        const iso = trimmed.match(/P(?:(\d+)D)?(?:T(?:(\d+)H)?(?:(\d+)M)?)?/i);
        if (iso) {
          const days = parseInt(iso[1] || '0', 10);
          const hours = parseInt(iso[2] || '0', 10);
          const minutes = parseInt(iso[3] || '0', 10);
          totalMinutes = days * 24 * 60 + hours * 60 + minutes;
        }
      } else if (!isNaN(Number(trimmed))) {
        totalMinutes = Number(trimmed);
      }
    }
    const days = Math.floor(totalMinutes / (24 * 60));
    const hours = Math.floor((totalMinutes % (24 * 60)) / 60);
    const minutes = totalMinutes % 60;
    // Always output as dd:hh:mm:00
    return `${days.toString().padStart(2, '0')}:${hours
      .toString()
      .padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:00`;
  }
}
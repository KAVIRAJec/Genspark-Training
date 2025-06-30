import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'durationFormat',
  standalone: true
})
export class DurationFormatPipe implements PipeTransform {
  transform(value: string | number | null | undefined): string {
    if (!value) return '';
    let totalMinutes = 0;
    if (typeof value === 'number') {
      totalMinutes = value;
    } else if (typeof value === 'string') {
      // Try to parse as ISO 8601 duration (e.g. 'P2DT3H4M') or as minutes
      const isoMatch = value.match(/^P(?:(\d+)D)?(?:T(?:(\d+)H)?(?:(\d+)M)?)?$/);
      if (isoMatch) {
        const days = parseInt(isoMatch[1] || '0', 10);
        const hours = parseInt(isoMatch[2] || '0', 10);
        const minutes = parseInt(isoMatch[3] || '0', 10);
        totalMinutes = days * 24 * 60 + hours * 60 + minutes;
      } else if (!isNaN(Number(value))) {
        totalMinutes = Number(value);
      } else {
        // Try to parse '3d 4h 5m' style
        const regex = /(?:(\d+)d)?\s*(?:(\d+)h)?\s*(?:(\d+)m)?/i;
        const match = value.match(regex);
        if (match) {
          const days = parseInt(match[1] || '0', 10);
          const hours = parseInt(match[2] || '0', 10);
          const minutes = parseInt(match[3] || '0', 10);
          totalMinutes = days * 24 * 60 + hours * 60 + minutes;
        } else {
          return value;
        }
      }
    }
    const days = Math.floor(totalMinutes / (24 * 60));
    const hours = Math.floor((totalMinutes % (24 * 60)) / 60);
    const minutes = totalMinutes % 60;
    let result = '';
    if (days) result += `${days}d `;
    if (hours) result += `${hours}h `;
    if (minutes || (!days && !hours)) result += `${minutes}m`;
    return result.trim();
  }
}

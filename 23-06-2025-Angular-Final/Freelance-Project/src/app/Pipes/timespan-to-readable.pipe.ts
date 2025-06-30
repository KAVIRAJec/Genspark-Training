import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timespanToReadable',
  standalone: true
})
export class TimespanToReadablePipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    if (!value || typeof value !== 'string') return value || '';
    let days = 0, hours = 0, minutes = 0, seconds = 0;

    // Format: '5.00:00:00' => (d.hh:mm:ss)
    const dotMatch = value.match(/^(\d+)\.(\d{2}):(\d{2}):(\d{2})$/);
    if (dotMatch) {
      days = parseInt(dotMatch[1], 10);
      hours = parseInt(dotMatch[2], 10);
      minutes = parseInt(dotMatch[3], 10);
      seconds = parseInt(dotMatch[4], 10);
    } else if (/^\d{2}:\d{2}:\d{2}:\d{2}$/.test(value)) {
      // Format: 'dd:hh:mm:ss'
      const parts = value.split(':').map(Number);
      days = parts[0];
      hours = parts[1];
      minutes = parts[2];
      seconds = parts[3];
    } else if (/^\d{2}:\d{2}:\d{2}$/.test(value)) {
      // Format: 'hh:mm:ss'
      const parts = value.split(':').map(Number);
      hours = parts[0];
      minutes = parts[1];
      seconds = parts[2];
    } else {
      return value;
    }

    let result = '';
    if (days) result += `${days}d `;
    if (hours) result += `${hours}h `;
    if (minutes) result += `${minutes}m`;

    // If all are zero, return '0m'
    if (!days && !hours && !minutes) result = '0m';

    return result.trim();
  }
}

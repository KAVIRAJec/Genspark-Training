import { TimespanToReadablePipe } from './timespan-to-readable.pipe';

describe('TimespanToReadablePipe', () => {
  let pipe: TimespanToReadablePipe;
  beforeEach(() => { pipe = new TimespanToReadablePipe(); });

  it('should return input for null/undefined/empty', () => {
    expect(pipe.transform(null)).toBe('');
    expect(pipe.transform(undefined as any)).toBe('');
    expect(pipe.transform('')).toBe('');
  });

  it('should parse d.hh:mm:ss format', () => {
    expect(pipe.transform('5.12:30:45')).toBe('5d 12h 30m');
    expect(pipe.transform('0.01:02:03')).toBe('1h 2m');
  });

  it('should parse dd:hh:mm:ss format', () => {
    expect(pipe.transform('02:03:04:05')).toBe('2d 3h 4m');
    expect(pipe.transform('00:00:00:00')).toBe('0m');
  });

  it('should parse hh:mm:ss format', () => {
    expect(pipe.transform('12:34:56')).toBe('12h 34m');
    expect(pipe.transform('00:00:00')).toBe('0m');
  });

  it('should return input for unrecognized string', () => {
    expect(pipe.transform('nonsense')).toBe('nonsense');
  });
});

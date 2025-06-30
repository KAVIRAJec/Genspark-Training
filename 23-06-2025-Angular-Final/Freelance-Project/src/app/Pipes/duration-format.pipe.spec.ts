import { DurationFormatPipe } from './duration-format.pipe';

describe('DurationFormatPipe', () => {
  let pipe: DurationFormatPipe;
  beforeEach(() => { pipe = new DurationFormatPipe(); });

  it('should return empty string for null/undefined/empty', () => {
    expect(pipe.transform(null)).toBe('');
    expect(pipe.transform(undefined)).toBe('');
    expect(pipe.transform('')).toBe('');
  });

  it('should format number of minutes', () => {
    expect(pipe.transform(5)).toBe('5m');
    expect(pipe.transform(65)).toBe('1h 5m');
    expect(pipe.transform(1440)).toBe('1d');
    expect(pipe.transform(1505)).toBe('1d 1h 5m');
  });

  it('should parse ISO 8601 duration', () => {
    expect(pipe.transform('P2DT3H4M')).toBe('2d 3h 4m');
    expect(pipe.transform('PT4H')).toBe('4h');
    expect(pipe.transform('PT30M')).toBe('30m');
    expect(pipe.transform('P1DT')).toBe('1d');
  });

  it('should parse minutes as string', () => {
    expect(pipe.transform('90')).toBe('1h 30m');
  });

  it('should parse 3d 4h 5m style', () => {
    expect(pipe.transform('3d 4h 5m')).toBe('3d 4h 5m');
    expect(pipe.transform('2d')).toBe('2d');
    expect(pipe.transform('7h')).toBe('7h');
    expect(pipe.transform('15m')).toBe('15m');
  });
});

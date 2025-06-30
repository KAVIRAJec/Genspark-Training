import { TestBed } from '@angular/core/testing';
import { ImageUploadService } from '../ImageUpload.service';
import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';

describe('ImageUploadService', () => {
  let service: ImageUploadService;
  let httpSpy: jasmine.SpyObj<HttpClient>;

  beforeEach(() => {
    httpSpy = jasmine.createSpyObj('HttpClient', ['post', 'delete']);
    TestBed.configureTestingModule({
      providers: [
        ImageUploadService,
        { provide: HttpClient, useValue: httpSpy }
      ]
    });
    service = TestBed.inject(ImageUploadService);
  });

  it('should upload image', () => {
    httpSpy.post.and.returnValue(of({}));
    service.UploadImage({} as any).subscribe();
    expect(httpSpy.post).toHaveBeenCalled();
  });

  it('should delete image', () => {
    httpSpy.delete.and.returnValue(of({}));
    service.DeleteImage('url').subscribe();
    expect(httpSpy.delete).toHaveBeenCalled();
  });
});

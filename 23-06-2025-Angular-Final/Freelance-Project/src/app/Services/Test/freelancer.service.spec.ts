import { TestBed } from '@angular/core/testing';
import { FreelancerService } from '../freelancer.service';
import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';

describe('FreelancerService', () => {
  let service: FreelancerService;
  let httpSpy: jasmine.SpyObj<HttpClient>;

  beforeEach(() => {
    httpSpy = jasmine.createSpyObj('HttpClient', ['get', 'post', 'put', 'delete']);
    TestBed.configureTestingModule({
      providers: [
        FreelancerService,
        { provide: HttpClient, useValue: httpSpy }
      ]
    });
    service = TestBed.inject(FreelancerService);
  });

  it('should get all freelancers', () => {
    httpSpy.get.and.returnValue(of({}));
    service.getAllFreelancers().subscribe();
    expect(httpSpy.get).toHaveBeenCalled();
  });

  it('should get freelancer by id', () => {
    httpSpy.get.and.returnValue(of({}));
    service.getFreelancerById('id').subscribe();
    expect(httpSpy.get).toHaveBeenCalled();
  });

  it('should create freelancer', () => {
    httpSpy.post.and.returnValue(of({}));
    service.createFreelancer({} as any).subscribe();
    expect(httpSpy.post).toHaveBeenCalled();
  });

  it('should update freelancer', () => {
    httpSpy.put.and.returnValue(of({}));
    service.updateFreelancer('id', {} as any).subscribe();
    expect(httpSpy.put).toHaveBeenCalled();
  });

  it('should delete freelancer', () => {
    httpSpy.delete.and.returnValue(of({}));
    service.deleteFreelancer('id').subscribe();
    expect(httpSpy.delete).toHaveBeenCalled();
  });
});
